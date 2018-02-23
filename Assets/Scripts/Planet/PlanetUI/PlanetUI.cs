using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
    public class PlanetUI : MonoSingleton<PlanetUI>
    {
        // 自由视角,半自由.., 总览模式, 建筑模式
        public enum WatchState
        {
            FREE,HALF_FREE,LOOK,ENTER
        }
        public WatchState watchState = WatchState.FREE;
        [Header("绑定MenuManager")]
        public MenuManager planetUI;
        [Header("进入星球范围时显示的HUD")]
        public GameObject lookPlanetHUDPrefab;
        [Header("修建控制中心")]
        public GameObject buildControlCenterHUDPrefab;
        public GameObject dropingControlCenterPrefab;
        [Range(0f,1f)]
        public float centerFlySpeed = 0.01f;



        Planet currentPlanet = null;
        public Planet CurrentPlanet
        {
            get { return currentPlanet;}
            private set{ }
        }
        Transform currentShip = null;// 如果一艘飞船进入了星球, 就绑定这个飞船

        string currentBuildingName = "NULL";
        public string CurrentBuildingName
        {
            get { return currentBuildingName;}
            private set{ }
        }
        BuildingBase currentBuilding = null;
        public BuildingBase CurrentBuilding
        {
            get { return currentBuilding;}
            private set{ }
        }
        [SerializeField]
        int lookLandIndex = 0;
        public int LookLandIndex
        {
            get 
            {
                if(currentPlanet != null)
                    return currentPlanet.GetValidIndex(lookLandIndex);
                return lookLandIndex;
            }
            set
           {
                if(currentPlanet != null)
                    lookLandIndex = currentPlanet.GetValidIndex(value);
                else lookLandIndex = 0;
            }
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener(this,GameEvent.ENTER_PLANET_AREA, OnSomethingEnterArea);
            EventManager.Instance.AddListener(this,GameEvent.EXIT_PLANET_AREA, OnSomethingExitArea);
        }
        void OnDisable()
        {
            EventManager.Instance.RemoveObjectEvent(this,GameEvent.ENTER_PLANET_AREA);
            EventManager.Instance.RemoveObjectEvent(this,GameEvent.EXIT_PLANET_AREA);
        }

        public void OnSomethingEnterArea(GameEvent eventType, Component sender, object param = null)
        {
            Debug.Assert(eventType == GameEvent.ENTER_PLANET_AREA);

            currentPlanet = (Planet)param;
            Debug.Log(currentPlanet.gameObject.name);
            currentShip = sender.transform;
            watchState = WatchState.HALF_FREE;

            Instantiate<GameObject>(lookPlanetHUDPrefab,sender.transform);
        }
        public void OnSomethingExitArea(GameEvent eventType, Component sender, object param = null)
        {
            Debug.Assert(eventType == GameEvent.EXIT_PLANET_AREA);

            if (watchState == WatchState.LOOK)
                ExitLooking(sender.transform);
            else if (watchState == WatchState.ENTER)
            {
                ExitPlanet();
                ExitLooking(sender.transform);
            }

            watchState = WatchState.FREE;
            currentShip = null;
            currentPlanet = null;
        }

        /// <summary>
        /// 根据已有绑定, 进入总览模式
        /// </summary>
        public void LookPlanet() 
        {
            watchState = WatchState.LOOK;

            CameraController.Instance.LookAt(currentPlanet.transform);
            CameraController.Instance.SetSize(currentPlanet.Radius*3);
            planetUI.gameObject.SetActive(true);
            // 总览星球事件!
            EventManager.Instance.PostEvent(GameEvent.LOOK_PLANET, CurrentPlanet);
            if(currentShip!=null)
                CheckColony();
    	}
        /// <summary>
        /// 退出总览模式, 并不解除各种绑定
        /// </summary>
        public void ExitLooking()
        {
            ExitLooking(currentShip);
        }
        public void ExitLooking(Transform objectToFollow)
        {
            watchState = WatchState.HALF_FREE;
            Instantiate<GameObject>(lookPlanetHUDPrefab,currentShip);

            CameraController.Instance.LookAt(objectToFollow);
            CameraController.Instance.SetRotation(Vector3.zero);
            CameraController.Instance.SetSize(CameraController.Instance.DefaultFreeSize);
            planetUI.CloseManager();

        }

        public void EnterPlanet()
        {
            watchState = WatchState.ENTER;
            CameraController.Instance.SetSize(currentPlanet.Radius);
            LookLandIndex = 0;
            LookBuilding(LookLandIndex);

            EventManager.Instance.PostEvent(GameEvent.ENTER_PLANET, CurrentPlanet);
        }
        public void ExitPlanet()
        {
            watchState = WatchState.LOOK;
            currentBuildingName = "NULL";
            currentBuilding = null;

            CameraController.Instance.SetSize(currentPlanet.Radius*3);
            CameraController.Instance.LookAt(currentPlanet.transform);
            CameraController.Instance.SetRotation(Vector3.zero);

            EventManager.Instance.PostEvent(GameEvent.EXIT_PLANET, CurrentPlanet);
        }

        public void LeftBuilding()
        {
            LookLandIndex--;
            LookBuilding(LookLandIndex);
        }
        public void RightBuilding()
        {
            LookLandIndex++;
            LookBuilding(LookLandIndex);
        }
        public void LookBuilding(int index)
        {
            var pos = currentPlanet.GetIndexPosition(index);
            var rot = currentPlanet.GetIndexRotation(index);
            CameraController.Instance.SetPosition(pos);
            CameraController.Instance.SetRotation(rot);
            currentBuildingName = currentPlanet.colony.GetBuildingAtIndex(index, out currentBuilding);

            EventManager.Instance.PostEvent(GameEvent.LOOK_BUILDING, currentBuilding, currentBuildingName);
            
            Debug.Log(index + " : " + currentBuildingName);
        }


        // 在总览星球时, 才可以查看能否建筑Colony
        void CheckColony()
        {
            if (currentPlanet.colony != null)
                return;
            PoolManager.Instance.GetCacheGameObejct(buildControlCenterHUDPrefab).transform.SetParent(currentShip);
        }
        /// <summary>
        /// 由其他地方计算时间, 这里直接负责修建特技
        /// </summary>
        public void DropControllCenter(Vector3 pos)
        {
            GameObject go = Instantiate<GameObject>(dropingControlCenterPrefab,pos,Quaternion.identity);
            StartCoroutine(DropControllCenter(go.transform,  currentPlanet));
        }
        IEnumerator DropControllCenter(Transform startPos, Planet thatPlanet)
        {
            float minDis = float.MaxValue;
            Vector3 targetPos = startPos.position;
            int nearestIndex = 0;
            for (int i = 0; i < thatPlanet.AreaCount; i++)
            {
                Vector3 landPos = thatPlanet.GetIndexPosition(i);
                float dis = Vector3.Distance(landPos, startPos.position);
                if (dis < minDis)
                {
                    minDis = dis;
                    targetPos = landPos;
                    nearestIndex = i;
                }
            }
            Debug.Log("nearestIndex" + nearestIndex);
            while (Vector3.Distance(startPos.position,targetPos)>0.1f)
            {
                startPos.position = Vector3.Lerp(startPos.position, targetPos, centerFlySpeed);
                yield return null;
            }
            thatPlanet.CreateColony();
            BuildingBase building = Instantiate<GameObject>(ControlCenter.GetBuildingPrefab(BuildingType.CONTROL_CENTER)).GetComponent<BuildingBase>();
            Build(building ,nearestIndex,thatPlanet);
            EventManager.Instance.PostEvent(GameEvent.COLONY_SET_UP, thatPlanet);
            Destroy(startPos.gameObject);
        }

        /// <summary>
        /// 需传入一个GameObject的Component
        /// </summary>
        public void Build(BuildingBase building, int index = int.MaxValue, Planet planetToBuild = null)
        {
            bool isCurrent = false;
            if (planetToBuild == null)
                planetToBuild = currentPlanet;
                
            if (index == int.MaxValue)
                index = LookLandIndex;
            if(index == LookLandIndex && planetToBuild.Equals(currentPlanet))
                isCurrent = true;

            // 将建筑建造到殖民地
            planetToBuild.colony.SetBuildingAtIndex(index,building);

            if (isCurrent)
            {
                Debug.Log("It Look !!!!!!!!!!!!!!!!");
                currentBuilding = building;
                LookBuilding(index);
            }
        }

        /// <summary>
        /// 建筑新的建筑, TEST
        /// </summary>
        public void New()
        {
            //BuildingBase building = Instantiate<GameObject>(ShipFactory.GetBuildingPrefab(BuildingType.SHIP_FACTORY)).GetComponent<BuildingBase>();
            //Build(building ,LookLandIndex,currentPlanet);

            BuildingBase building = Instantiate<GameObject>(ResourceCollector.GetBuildingPrefab(BuildingType.RESOURCE_COLLECTOR)).GetComponent<BuildingBase>();
            Build(building ,LookLandIndex,currentPlanet);
        }
        /// <summary>
        /// 默认打开当前Building
        /// </summary>
        public void Open()
        {
            BuildingUI.Instance.Open(currentBuilding);
        }
        public void Upgrade()
        {
            BuildingUI.Instance.Upgrade(currentBuilding);
        }
        public void Recycle()
        {
            BuildingUI.Instance.Recycle(currentBuilding);
        }
    }
}