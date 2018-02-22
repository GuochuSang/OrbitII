using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Universe 
{
    public class LeftBuildingWindow : MonoBehaviour 
    {
        public Button newBuilding;
        public Button open;
        public Button upgrade;
        public Button recycle;

        void OnEnable()
        {
            Manager.EventManager.Instance.AddListener(this,Manager.GameEvent.LOOK_BUILDING, OnLookingBuilding);
        }
        void OnDisable()
        {
            Manager.EventManager.Instance.RemoveObjectEvent(this,Manager.GameEvent.LOOK_BUILDING);
        }
        void OnLookingBuilding(Manager.GameEvent eventType, Component sender, object param = null)
        {
            Debug.Assert(eventType == Manager.GameEvent.LOOK_BUILDING);
            if (sender == null)
            {
                NoBuilding();
                return;
            }
            ExistsBuilding();
        }

        void NoBuilding()
        {
            newBuilding.gameObject.SetActive(true);
            open.gameObject.SetActive(false);
            upgrade.gameObject.SetActive(false);
            recycle.gameObject.SetActive(false);
        }
        void ExistsBuilding()
        {
            newBuilding.gameObject.SetActive(false);
            open.gameObject.SetActive(true);
            upgrade.gameObject.SetActive(true);
            recycle.gameObject.SetActive(true);
        }
    }
}