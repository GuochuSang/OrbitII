using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
    public class CheckEnterable : MonoBehaviour 
    {
        bool isOpen = false;
        void Awake()
        {
            Manager.EventManager.Instance.AddListener(this,GameEvent.ENTER_BUILDING_AREA,OnEnterControlCenterArea);
            Manager.EventManager.Instance.AddListener(this,GameEvent.EXIT_BUILDING_AREA,OnLeaveControlCenterArea);
            Manager.EventManager.Instance.AddListener(this,GameEvent.EXIT_PLANET,OnExitPlanet);
        }
        void OnEnable()
        {
            if(!isOpen)
                this.gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            Manager.EventManager.Instance.RemoveObjectEvent(this,GameEvent.ENTER_BUILDING_AREA);
            Manager.EventManager.Instance.RemoveObjectEvent(this,GameEvent.EXIT_BUILDING_AREA);
            Manager.EventManager.Instance.RemoveObjectEvent(this,GameEvent.EXIT_PLANET);
        }
        public void OnEnterControlCenterArea(GameEvent eventType, Component sender, object param = null)
        {
            isOpen = true;
            if (PlanetUI.Instance.watchState == PlanetUI.WatchState.LOOK)
            {
                this.gameObject.SetActive(true);
            }
        }
        public void OnExitPlanet(GameEvent eventType, Component sender, object param = null)
        {
            if (isOpen)
                this.gameObject.SetActive(true);
        }
        public void OnLeaveControlCenterArea(GameEvent eventType, Component sender, object param = null)
        {
            isOpen = false;
            this.gameObject.SetActive(false);
        }
    }
}