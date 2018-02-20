using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe
{   
    [System.Serializable]
    public class PlanetResourceData : SaveData
    {
        // 将Element转换为可以储存的ElementID
        public Dictionary<ElementID, float[]> elementInLands;
        public float prevUpdateTime;
    }

    // 储存和管理星球的资源
    public class PlanetResource : MonoBehaviour, ISaveable
    {
        public const float UPDATE_PERIOD = 1f;
        public const int MAX_UPDATE_TIME_RELOAD = 30;
        float prevUpdateTime;

        Planet momPlanet;
        ID id;
        // 初始化完毕开始更新
        bool initialized = false;
        // 星球的每个地方都有自己的资源
        // 他们会慢慢地将自己的资源传递给邻近的地块
        // 所有的资源有一个上限, 低于上限时会慢慢增长.
        // 生物资源比较特殊单独讨论
        // elementInLands : 每一种资源在每个地区的分布
        public Dictionary<Element, float[]> elementInLands;

        #region 初始化方法
        public void Init(Planet momPlanet)
        {
            this.momPlanet = momPlanet;
            this.id = new ID();
            this.id.idx = momPlanet.id.idx;
            this.id.idy = momPlanet.id.idy;
            this.id.className = "PlanetResource";
            this.id.sceneName = "Universe";
            this.id.Init();
            InitPlanetResources();
            initialized = true;
            StartUpdate(true);
        }
        public void Init(ID id,Planet momPlanet)
        {
            this.momPlanet = momPlanet;
            this.id = id;
            this.id.Init();
            SaveManager.Instance.Load(this, this.id);
            initialized = true;
            StartUpdate(false);
        }
        #endregion

        void OnEnable()
        {
            if (initialized)
                StartUpdate(false);
        }
        void OnDisable()
        {
            if (initialized)
                StopUpdate();
        }

        public void StartUpdate(bool firstStart)
        {
            if (firstStart)
                prevUpdateTime = UniverseTime.Instance.Time;
            else
            {
                float deltaTime = UniverseTime.Instance.Time - prevUpdateTime;
                int collectTimes = Mathf.Clamp((int)(deltaTime / UPDATE_PERIOD),0,MAX_UPDATE_TIME_RELOAD);
                for (int i = 0; i < collectTimes; i++)
                {
                    UpdateOnce();
                }
            }
            StartCoroutine(UpdatePerPeriod());
        }
        public void StopUpdate()
        {
            StopCoroutine(UpdatePerPeriod());
        }
        IEnumerator UpdatePerPeriod()
        {
            while (true)
            {
                yield return new WaitForSeconds(UPDATE_PERIOD);
                UpdateOnce();
                prevUpdateTime = UniverseTime.Instance.Time;
            }
        }

        #region 资源管理
        /// <summary>
        /// 初始化星球资源(初次生成) , 所有普通,生物元素, 1~2种稀有元素
        /// </summary>
        public void InitPlanetResources()
        {
            InitAddElements();
            InitElementsAmount();
        }
        // 向星球添加将会存在于这个星球的元素
        void InitAddElements()
        {
            // 初始化 elementInLands
            elementInLands = new Dictionary<Element, float[]>();
            // 遍历elementType
            for (int i = 0; i < 3; i++)
            {
                // 稀有元素
                if ((ElementType)i == ElementType.SP)
                {
                    int count = Resource.GetElementCount(ElementType.SP);
                    if (count > 1)
                    {
                        List<int> indexs = new List<int>();
                        indexs.Add(Random.Range(0, count));
                        if (Random.value > 0.5f)
                        {
                            int index2 = Random.Range(0, count);
                            if (index2 == indexs[0])
                            {
                                index2++;
                                if (index2 == count)
                                    index2 = 0;
                            }
                            indexs.Add(index2);
                        }
                        // 最后添加一种或两种稀有元素
                        for (int index = 0; index < indexs.Count; index++)
                        {
                            Element ele;
                            Resource.GetElement(ElementType.SP, (ElementIndex)indexs[index], out ele);
                            elementInLands.Add(ele, new float[momPlanet.AreaCount]);
                        }
                    }
                    else
                    {
                        Element ele;
                        Resource.GetElement(ElementType.SP, (ElementIndex)0, out ele);
                        elementInLands.Add(ele, new float[momPlanet.AreaCount]);
                    }
                }
                //普通元素
                else
                {
                    int eleCount = Resource.GetElementCount((ElementType)i);
                    // 遍历elementIndex
                    for (int j = 0; j < eleCount; j++)
                    {
                        Element ele;
                        Resource.GetElement((ElementType)i, (ElementIndex)j, out ele);
                        elementInLands.Add(ele, new float[momPlanet.AreaCount]);
                    }
                }
            }
        }
        // 给星球的每个Land加上一定数量(初始为最大数量)各种元素
        void InitElementsAmount()
        {
            // 对每块地, 添加各种元素
            for (int i = 0; i < momPlanet.AreaCount; i++)
            {
                Planet.LandType landType;
                momPlanet.GetIndexLandType(i, out landType);
                foreach (var kv in elementInLands)
                {
                    kv.Value[i] = kv.Key.growInfos[landType].maxAmount;
                }
            }
        }

        /// <summary>
        /// 每个单位时间(可长可短)更新星球上的所有资源
        /// </summary>
        public void UpdateOnce()
        {
            Debug.Log("资源更新!");
            foreach (var kv in elementInLands)
            {
                Element curElement = kv.Key;
                float[] curAmounts = kv.Value;
                for (int i = 0; i < momPlanet.AreaCount; i++)
                {
                    Planet.LandType curLand;
                    momPlanet.GetIndexLandType(i,out curLand);
                    int leftIndex = momPlanet.GetValidIndex(i-1);
                    int rightIndex = momPlanet.GetValidIndex(i+1);
                    curAmounts[i] = curElement.GrowedAmount(curLand,curAmounts[i],ref curAmounts[leftIndex],ref  curAmounts[rightIndex]);
                }
            }
        }
        #endregion

        #region ISaveable
        public ID Save()
        {
            SaveManager.Instance.Save(this, id);
            return id;
        }
        public SaveData toSaveData()
        {
            PlanetResourceData data = new PlanetResourceData();
            data.elementInLands = new Dictionary<ElementID, float[]>();
            data.prevUpdateTime = this.prevUpdateTime;
            foreach (var kv in this.elementInLands)
            {
                data.elementInLands.Add(kv.Key.GetID(), kv.Value);
            }
            return data;
        }
        public void fromSaveData(SaveData saveData)
        {
            PlanetResourceData data = (PlanetResourceData)saveData;
            this.elementInLands = new Dictionary<Element, float[]>();
            this.prevUpdateTime = data.prevUpdateTime;
            foreach (var kv in data.elementInLands)
            {
                this.elementInLands.Add(kv.Key.GetElement(), kv.Value);
            }
           
        }
        #endregion
    }
}