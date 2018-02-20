using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    public enum BuildingType
    {
        NULL,
        CONTROL_CENTER,
        SHIP_FACTORY,
        RESOURCE_COLLECTOR,
        TECH_LAB
    }

    public abstract class BuildingBase : MonoBehaviour, ISaveable
    {
        public ID id = null;
        protected BuildingType type = BuildingType.NULL;
        public Colony momColony;
        protected int index;// 建筑在星球的下标
        public BuildingType Type
        {
            get{ return type; }
            private set{ }
        }

        // Planet/Colony/type.ToString()
        static Dictionary<BuildingType,GameObject> buildingPrefabs;
        public static GameObject GetBuildingPrefab(BuildingType buildingType)
        {
            if (buildingPrefabs == null)
                buildingPrefabs = new Dictionary<BuildingType, GameObject>();
            if (!buildingPrefabs.ContainsKey(buildingType))
            {
                GameObject go = Resources.Load<GameObject>("Planet/Colony/" + buildingType.ToString());
                buildingPrefabs.Add(buildingType, go);
            }
            Debug.Assert(buildingPrefabs[buildingType] != null, "找不到建筑预制体 : " + buildingType.ToString());
            return buildingPrefabs[buildingType];
        }

        /// <summary>
        /// 初始化ID
        /// </summary>
        public void InitID()
        {
            id.idx = (int)(transform.position.x*10);
            id.idy = (int)(transform.position.y*10);
            id.className = GetName();
            id.sceneName = "Planet";
            id.Init();
        }
        /// <summary>
        /// 绑定所在殖民地
        /// </summary>
        public void SetMomColony(Colony momColony,int index)
        {
            this.momColony = momColony;
            this.index = index;
            StartWork();
        }
        /// <summary>
        /// 初始化殖民地和index之后正式开始运行建筑
        /// </summary>
        protected abstract void StartWork();
        /// <summary>
        /// 返回建筑子类名称
        /// </summary>
        public abstract string GetName();
    	#region ISaveable
        public abstract SaveData toSaveData();
        public abstract void fromSaveData(SaveData saveData);
        public abstract ID Save();
        #endregion

    }
}