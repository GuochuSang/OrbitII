using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe
{
    /// <summary>
    /// 检测周围是否有星球, 然后检测是否靠近控制中心
    /// </summary>
    public class CheckPlanet : MonoBehaviour 
    {
        public float checkRadius = 5f;
    	[SerializeField]
        int planetLayerMask = 1;
        [SerializeField]
        int buildingLayerMask = 1;
        public string planetTag = "Planet";
        public string controlCenterTag = "ControlCenter";
        Planet currentPlanet = null;
        ControlCenter currentControlCenter = null;
    	
        void Awake()
        {
            planetLayerMask = LayerMask.NameToLayer("Planet");
            buildingLayerMask = LayerMask.NameToLayer("Building");
        }
        void FixedUpdate () 
        {
            if (currentPlanet == null)
                TryEnterPlanet();
            else
                TryEnterControlCenter();
            
    	}
        void TryEnterPlanet()
        {
            RaycastHit2D hit;
            if (CheckIn(planetLayerMask, planetTag, out hit))
                EnterPlanet(hit.collider.GetComponent<Planet>());
        }
        void TryEnterControlCenter()
        {
            RaycastHit2D hit;
            if (!CheckIn(planetLayerMask, planetTag, out hit))
            {
                ExitPlanet();
                return;
            }
            if (CheckIn(buildingLayerMask, controlCenterTag, out hit))
            {
                if (currentControlCenter == null)
                    EnterControlCenter((ControlCenter)hit.collider.GetComponent<BuildingBase>());
            }
            else if(currentControlCenter != null)
            {
                ExitControlCenter();
            }
        }
        void EnterPlanet(Planet pl)
        {
            currentPlanet = pl;
            EventManager.Instance.PostEvent(GameEvent.ENTER_PLANET_AREA, this, currentPlanet);
        }
        void ExitPlanet()
        {
            EventManager.Instance.PostEvent(GameEvent.EXIT_PLANET_AREA, this, currentPlanet);
            currentPlanet = null;
        }
        void EnterControlCenter(ControlCenter center)
        {
            currentControlCenter = center;
            EventManager.Instance.PostEvent(GameEvent.ENTER_BUILDING_AREA,this,center);
            Debug.Log("靠近了控制中心");
        }
        void ExitControlCenter()
        {
            currentControlCenter = null;
            EventManager.Instance.PostEvent(GameEvent.EXIT_BUILDING_AREA,this);
            Debug.Log("离开了控制中心");
        }
        bool CheckIn(int layerMask,string tag,out RaycastHit2D hit)
        {
            hit = RayCast(layerMask);
            if (hit.collider != null && hit.collider.tag.Equals(tag))
            {
                Debug.Log("Checked Ship in "+tag);
                return true;
            }
            return false;
        }
        RaycastHit2D RayCast(int layerMask)
        {
            return Physics2D.CircleCast(this.transform.position, checkRadius, Vector2.up, 0f,1<<layerMask);
        }
    }
}