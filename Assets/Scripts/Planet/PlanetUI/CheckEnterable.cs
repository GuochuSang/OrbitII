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
        }
        public void OnEnterControlCenterArea(GameEvent eventType, Component sender, object param = null)
        {
            isOpen = true;
            this.gameObject.SetActive(true);
        }
        public void OnLeaveControlCenterArea(GameEvent eventType, Component sender, object param = null)
        {
            isOpen = false;
            this.gameObject.SetActive(false);
        }
    }
}