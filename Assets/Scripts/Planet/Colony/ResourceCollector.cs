using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe 
{
    [System.Serializable]
    public class ResourceCollectorData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            GameObject go = GameObject.Instantiate(BuildingBase.GetBuildingPrefab(BuildingType.RESOURCE_COLLECTOR));
            ResourceCollector cc = go.GetComponent<ResourceCollector>();
            id.Init();
            cc.id = id;
            SaveManager.Instance.Load(cc, id);
            return (T)(System.Object)cc;
        }
        public float prevUpdateTime;
        public float collectPeriod;
        public float currentCollectHeight;
    }

    public class ResourceCollector : BuildingBase
    {
        public const string BUILDING_NAME = "ResourceCollector";
        public override string GetName()
        {
            return BUILDING_NAME;
        }
        public const float FIXED_MIN_COLLECT = 0.5f;// 每次挖矿"数量值"固定最小值
        public const float COLLECT_RATIO = 0.06f;// 每次采集6%
        // 重新加载星球的最大挖矿次数(防止爆炸)
        public const int MAX_COLLECT_TIMES_RELOAD = 20;

        // 是否可以开始工作, 在成功加载之后才startWork
        bool startWork = false;

        #region 需要储存的数据
        // 上次挖矿时间
        float prevUpdateTime = 0;
        // 挖矿周期
        public float collectPeriod = 3f;
        // 挖矿高度
        [Range(0f,1f)]
        public float currentCollectHeight = 1f;
        // 每种元素最大挖掘数量/次
        public float maxCollectAmountPerElement = 3f;
        #endregion

        /// <summary>
        /// 收集一次
        /// 遍历所有元素, 对于每一个元素 : 挖矿所得 = 高度值*数量值
        /// 高度值 = abs(当前高度-最佳高度)/最大高度-高度差限制 // (高度差超过限制挖不到矿)
        /// 数量值 = (当前该元素量/元素最大量+固定基数) // 固定基数用于保证在元素很少时挖完矿(比如有10点矿, 数量值本来是0.1, 挖了和没挖有毛区别, 所以加一个基数..)
        /// </summary>
        void CollectOnce()
        {
            Container containerToAdd = momColony.container;
            var resourcesToCollect = momColony.momPlanet.resources.elementInLands;
            Planet.LandType currentType;
            momColony.momPlanet.GetIndexLandType(index, out currentType);
            foreach (var kv in resourcesToCollect)
            {
                // 数据准备
                float heightLocation = kv.Key.growInfos[currentType].heightLocation;
                float locationRange = kv.Key.growInfos[currentType].locationRange;
                float currentElementValue = kv.Value[index];
                float maxAmount = kv.Key.growInfos[currentType].maxAmount;
                // 计算采集量
                float heightValue = Mathf.Clamp01(1f-Mathf.Abs(currentCollectHeight-heightLocation)/locationRange);
                float amountValue = currentElementValue * COLLECT_RATIO + FIXED_MIN_COLLECT;
                float collectAmount = heightValue*amountValue;
                collectAmount = Mathf.Clamp(collectAmount, 0, kv.Value[index]);
                collectAmount = Mathf.Clamp(collectAmount, 0, maxCollectAmountPerElement);
                // 添加到殖民地背包
                float unableToAddAmount = containerToAdd.AddItem(kv.Key.ToString(), collectAmount);
                if (unableToAddAmount>0.01f)
                    Debug.Log("你的殖民地背包满了,无法继续挖矿...");
                collectAmount -= unableToAddAmount;
                // 星球资源扣除采集的元素
                kv.Value[index] -= collectAmount;
            }
        }

        #region 生命周期相关
        void Awake () 
        {
            type = BuildingType.RESOURCE_COLLECTOR;
            Debug.Log("新建了资源采集中心!!");
        }
        void OnEnable()
        {
            if (startWork)
            {
                StartCollect();
            }
        } 
        void OnDisable()
        {
            if (startWork)
            {
                StopCollect();
            }
        }
        // 每个周期收集一次
        IEnumerator CollectPerPeriod()
        {
            while (true)
            {
                yield return new WaitForSeconds(collectPeriod);
                CollectOnce();
                prevUpdateTime = UniverseTime.Instance.Time;
            }
        }
        // 开始工作, 并将期间没有做的工作补充(是否第一次开始工作)
        protected override void StartWork()
        {
            // 如果还没有开始工作, 说明没有加载数据, 他是新建的挖矿机
            if(!startWork)
                StartCollect(true);
        }
        void StartCollect(bool firstStart = false)
        {
            startWork = true;
            if (firstStart)
            {
                prevUpdateTime = UniverseTime.Instance.Time;
            }
            else
            {
                float deltaTime = UniverseTime.Instance.Time - prevUpdateTime;
                int collectTimes = Mathf.Clamp((int)(deltaTime / collectPeriod),0,MAX_COLLECT_TIMES_RELOAD);
                for (int i = 0; i < collectTimes; i++)
                {
                    CollectOnce();
                }
            }
            StartCoroutine(CollectPerPeriod());
        }
        void StopCollect()
        {
            StopCoroutine(CollectPerPeriod());
        }
        #endregion

        #region ISaveable
        public override SaveData toSaveData()
        {
            ResourceCollectorData data = new ResourceCollectorData();
            data.collectPeriod = this.collectPeriod;
            data.currentCollectHeight = this.currentCollectHeight;
            data.prevUpdateTime = this.prevUpdateTime;
            return data;
        }
        public override void fromSaveData(SaveData saveData)
        {
            ResourceCollectorData data = (ResourceCollectorData)saveData;
            this.collectPeriod = data.collectPeriod;
            this.currentCollectHeight = data.currentCollectHeight;
            this.prevUpdateTime = data.prevUpdateTime;
            // 加载完毕之后开始工作
            StartCollect();
        }
        public override ID Save()
        {
            SaveManager.Instance.Save(this, id);
            return id;
        }
        #endregion
    }
}