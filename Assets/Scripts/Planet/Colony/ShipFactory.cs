using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    [System.Serializable]
    public class ShipFactorySaveData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            GameObject go = GameObject.Instantiate(BuildingBase.GetBuildingPrefab(BuildingType.SHIP_FACTORY));
            ShipFactory cc = go.GetComponent<ShipFactory>();
            id.Init();
            cc.id = id;
            SaveManager.Instance.Load(cc, id);
            return (T)(System.Object)cc;
        }

    }

    public class ShipFactory : BuildingBase
    {
        public const string buildingName = "ShipFactory";
        public override string GetName()
        {
            return buildingName;
        }
    	void Awake () 
        {
            type = BuildingType.SHIP_FACTORY;
            Debug.Log("新建了飞船工厂!!");
    	}
        protected override void StartWork()
        {

        }
        #region ISaveable
        public override SaveData toSaveData()
        {
            ShipFactorySaveData data = new ShipFactorySaveData();
            return data;
        }
        public override void fromSaveData(SaveData saveData)
        {

        }
        public override ID Save()
        {
            SaveManager.Instance.Save(this, id);
            return id;
        }
        #endregion
    	
    }
}