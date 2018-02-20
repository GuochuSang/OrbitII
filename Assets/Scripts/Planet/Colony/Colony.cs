/// <summary>
/// 殖民地需要在游戏开始时全部加载, 并且同时存在
/// 但是星球是走到哪, 加载到哪
/// 
/// 所有殖民地用协程+循环队列慢慢依次更新, 如果所在星球在活跃Chunk内, 星球可以快速刷新殖民地 
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    [System.Serializable]
    public class ColonyData : SaveData
    {
        public Dictionary<int,ID> buildingIndexID;
        public ID controlCenterID;
        public ID containerID;
    }

    public class Colony : MonoBehaviour, ISaveable
    {
        public ID id;
        public Planet momPlanet;

        #region 需要储存的数据
        // 储存所有建筑<int, buildingbase>, 包括controllCenter
        Dictionary<int,BuildingBase> buildings;
        // 单独储存ControllCenter
        BuildingBase controllCenter = null;
        public Container container;
        #endregion

        /// <summary>
        /// 新建空colony
        /// </summary>
        public void Init(Planet mom)
        {
            id = new ID();
            momPlanet = mom;
            id.idx = mom.id.idx;
            id.idy = mom.id.idy;
            id.className = "Colony";
            id.sceneName = "Planet";
            Debug.Log(id.ToString());
            id.Init();
            container = new Container(id.ToString(), 100);
        }
        public void BindPlanet(Planet mom)
        {
            momPlanet = mom;
        }

        #region ISaveable
        public SaveData toSaveData()
        {
            ColonyData data = new ColonyData();
            Debug.Log("储存殖民地====!!");
            Dictionary<int,ID> indexID = new Dictionary<int, ID>();
            foreach(var kv in buildings)
            {
                indexID.Add(kv.Key, kv.Value.Save());
            }
            data.buildingIndexID = indexID;
            data.controlCenterID = controllCenter.id;
            data.containerID = this.container.Save();
            Debug.Log("殖民地储存完毕====!!");
            return data;
        }
        public void fromSaveData(SaveData saveData)
        {
            ColonyData data = (ColonyData)saveData;
            Debug.Log("加载殖民地====!!");
            this.container = new Container(data.containerID);
            if (data.buildingIndexID != null)
            {
                Debug.Log("data.buildingIndexID : "+data.buildingIndexID.Count);
                foreach (var kv in data.buildingIndexID)
                {
                    SaveData originData = SaveManager.Instance.GetSaveData(kv.Value);
                    if (originData == null)
                        continue;
                    CreatableSaveData buildingData = (CreatableSaveData)originData;
                    BuildingBase building = buildingData.Create<BuildingBase>(kv.Value);
                    SetBuildingAtIndex(kv.Key, building);
                    if (kv.Value.Equals(data.controlCenterID))
                        controllCenter = building;
                }
            }
            Debug.Log("殖民地加载完毕====!!");
        }
        public ID Save()
        {
            Debug.Log("Hey Save Colony");
            SaveManager.Instance.Save(this, id);
            return id;
        }
        #endregion

        /// <summary>
        /// 在指定下标绑定建筑
        /// </summary>
        public void SetBuildingAtIndex(int index,BuildingBase building, float heightRatio = 1.16f)
        {
            index = momPlanet.GetValidIndex(index);
            // 将建筑建造到星球
            momPlanet.SetGameObjectAtIndex(building.gameObject, index, heightRatio);
            // 位置确定后生成buildingID
            if (building.id == null || (building.id.className + "").Equals(""))
            {
                building.InitID();
            }
            // 建筑绑定殖民地
            building.SetMomColony(this,index);
                
            // 第一个建筑必须是控制中心
            if (buildings == null || buildings.Count == 0)
            {
                Debug.Assert(building.Type==BuildingType.CONTROL_CENTER,"第一个建筑应该是控制中心!");
                controllCenter = building;
                buildings = new Dictionary<int, BuildingBase>();
                buildings.Add(index,building);
                return;
            }
            buildings.Add(index,building);
        }
        /// <summary>
        /// 获得指定下标位置的建筑名及建筑
        /// </summary>
        public string GetBuildingAtIndex(int index, out BuildingBase buildingToGet)
        {
            buildingToGet = null;
            index = momPlanet.GetValidIndex(index);
            if(buildings != null)
                buildings.TryGetValue(index, out buildingToGet);
            if (buildingToGet == null)
            {
                Planet.LandType t;
                return momPlanet.GetIndexLandType(index,out t);
            }
            return buildingToGet.Type.ToString();
        }
    }
}