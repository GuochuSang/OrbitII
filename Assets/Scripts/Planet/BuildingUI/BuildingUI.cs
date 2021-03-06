﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
    /// <summary>
    /// 用于打开各种Building的UI面板
    /// </summary>
    public class BuildingUI : MonoSingleton<BuildingUI>
    {
        public GameObject resourceCollectUI;

        public void Open(BuildingBase building)
        {
            Debug.Log("Open"+building.Type);
            if (building.Type == BuildingType.CONTROL_CENTER)
                ShowContainer.Instance.Show(building.momColony.container);
            else if (building.Type == BuildingType.RESOURCE_COLLECTOR)
                resourceCollectUI.SetActive(true);
            else if (building.Type == BuildingType.SHIP_FACTORY)
                EventManager.Instance.PostEvent(GameEvent.ENTER_SHIP_FACTORY, this,building);
        }
        public void Upgrade(BuildingBase building)
        {
            Debug.Log("Upgrade"+building.Type);
        }
        public void Recycle(BuildingBase building)
        {
            Debug.Log("Recycle"+building.Type);
        }

        public void CloseUI()
        {
            resourceCollectUI.SetActive(false);
        }
    }
}