using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    [System.Serializable]
    public class TechLabData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            GameObject go = GameObject.Instantiate(BuildingBase.GetBuildingPrefab(BuildingType.TECH_LAB));
            TechLab cc = go.GetComponent<TechLab>();
            id.Init();
            cc.id = id;
            SaveManager.Instance.Load(cc, id);
            return (T)(System.Object)cc;
        }

    }

    public class TechLab : BuildingBase
    {
        public const string buildingName = "TechLab";
        public override string GetName()
        {
            return buildingName;
        }
    	void Awake () 
        {
            type = BuildingType.SHIP_FACTORY;
            Debug.Log("新建了科技实验室!!");
    	}
        protected override void StartWork()
        {

        }
        #region ISaveable
        public override SaveData toSaveData()
        {
            TechLabData data = new TechLabData();
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