using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    [System.Serializable]
    public class ControlCenterSaveData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            Debug.Log("控制中心从存档创建!!");
            GameObject go = GameObject.Instantiate(BuildingBase.GetBuildingPrefab(BuildingType.CONTROL_CENTER));
            ControlCenter cc = go.GetComponent<ControlCenter>();
            id.Init();
            cc.id = id;
            SaveManager.Instance.Load(cc, id);
            return (T)(System.Object)cc;
        }

    }

    public class ControlCenter : BuildingBase
    {
        public const string buildingName = "ControlCenter";
        public override string GetName()
        {
            return buildingName;
        }
    	void Awake () 
        {
            gameObject.tag = "ControlCenter";
            type = BuildingType.CONTROL_CENTER;
            Debug.Log("新建了控制中心!");

    	}

        protected override void StartWork()
        {
            
        }
        #region ISaveable
        public override SaveData toSaveData()
        {
            ControlCenterSaveData data = new ControlCenterSaveData();
            return data;
        }
        public override void fromSaveData(SaveData saveData)
        {
            
        }
        public override ID Save()
        {
            Debug.Log("控制中心存档!");
            SaveManager.Instance.Save(this, id);
            return id;
        }
        #endregion
    }
}