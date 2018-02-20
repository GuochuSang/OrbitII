using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System;
using Manager;
namespace Universe 
{
    [System.Serializable]
    public class PlanetSaveData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            id.Init();
            GameObject go = new GameObject("Planet");
            go.transform.SetParent(PlanetGenerate.generateRoot);
            Planet pl = go.AddComponent<Planet>();
            pl.GeneratePlanet(id);
            Debug.Log("这个星球是读取存档生成的 : "+ go.transform.position);
            return (T)(System.Object)(pl);
        }
        public Vector3Data position;
        public ID colonyID;
        public ID resourcesID;
        public int areaCount;
        public float radius;
        public Planet.LandType[] lands;
    }

    public class Planet : MonoBehaviour,IChunkObject,ISaveable  
    {
        #region Const
        //public enum LandType{SOIL, GRASS, SAND, SEA, SNOW}
        [System.Serializable]
        public enum LandType{GRASS, SAND, SEA}
        public const int LAND_COUNT = 3;
        // 请按照上方顺序绘制和添加贴图
        // 原始半径为1, 周长2*PI, 可以容纳5个土地, 每个土地的长度如下
        const float AREA_LENGTH = Mathf.PI*2f/5f;
        // 原始半径占17个unit
        const float ORIGIN_RADIUS = 17f;
        #endregion

        #region Static Resources to load
        // 暂时不考虑性能
        public static Sprite[] landSprites;
        public static Sprite[,] landTranslationSprite;
        public static Shader landShader;
        public static Shader perfectPixelShader;
        public static GameObject seaPrefab;// 海洋放在最底层, 有动画
        public static GameObject landPrefab;
        public static GameObject cloudPrefab;
        public static Sprite treeSprite;
        public static Sprite coreSprite;
        #endregion

        #region 子物体的几个根节点
        Transform landRoot;// 放置经过shader裁剪的土地
        Transform buildingRoot;// 放置星球建筑 
        Transform othersRoot; // 海洋, 云, 树木等..
        Transform translationRoot; //地形过渡
        #endregion

        // Planet Id 由其所在的Chunk来储存和读取
        public ID id;
        // 将生成的静态部分储存在这里, 交给RawImage显示
        public Texture2D targetLand = null;

        #region 需要存储的数据
        public Colony colony = null;
        public PlanetResource resources = null;
        // 有多少个区域
        int areaCount;
        // 星球半径(unit size)
        float radius;
        // 土地按顺序的类型
        [SerializeField]
        LandType[] lands;
        #endregion

        public int AreaCount
        {
            get{ return areaCount; }
            private set{ }
        }
        public float Radius
        {
            get{ return radius; }
            private set{ }
        }
            

        #region 生命周期 Awake|OnDestroy
        void Awake()
        {
            if (landSprites == null || landSprites.Length == 0)
                LoadResources();
            GenerateRoot();
        }
        void OnDestroy()
        {
            id.OnDestroy();
        }
        #endregion

        #region IChunkObject
        public Vector2 GetPosition()
        {
            return (Vector2)this.transform.position;
        }
        public ID Save()
        {
            Manager.SaveManager.Instance.Save(this, this.id);
            return this.id;
        }


        ///  <summary>
        /// 当物体进入可以刷新的范围, 确保重复调用不会出错!
        /// </summary>
        public void EnterUpdate()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 当物体退出可以刷新的范围, 确保重复调用不会出错!
        /// </summary>
        public void ExitUpdate()
        {
            gameObject.SetActive(false);
            SaveManager.Instance.Save(this, this.id);
        }
        #endregion
        #region ISaveable
        public SaveData toSaveData()
        {
            Debug.Log("对"+id.ToString()+"进行存档");
            PlanetSaveData data = new PlanetSaveData();
            data.areaCount = this.areaCount;
            data.radius = this.radius;
            data.lands = this.lands;
            data.resourcesID = this.resources.Save();
            if (this.colony != null)
                data.colonyID = this.colony.Save();
            else
                data.colonyID = null;
            data.position = new Vector3Data().SaveData(this.transform.position);
            return data;
        }
        public void fromSaveData(SaveData saveData)
        {
            PlanetSaveData data = (PlanetSaveData)saveData;

            this.areaCount = data.areaCount;
            this.radius = data.radius;
            this.transform.localScale = new Vector3(radius/ORIGIN_RADIUS,radius/ORIGIN_RADIUS,1);
            this.lands = data.lands;
            this.transform.position = data.position.GetData();
            // 加载资源
            this.resources = gameObject.AddComponent<PlanetResource>();
            this.resources.Init(data.resourcesID, this);
            GenerateOutLook();

            if (data.colonyID != null)
            {
                data.colonyID.Init();
                GameObject go = new GameObject("Colony");
                this.colony = go.AddComponent<Colony>();
                this.colony.id = data.colonyID;
                this.colony.BindPlanet(this);
                this.colony.transform.SetParent(transform);
                SaveManager.Instance.Load(this.colony, data.colonyID);
            }
            else
                this.colony = null;
        }
        #endregion

        static void LoadResources()
        {
            landSprites = new Sprite[LAND_COUNT];
            landTranslationSprite = new Sprite[LAND_COUNT, LAND_COUNT];
            for (int i = 0; i < LAND_COUNT; i++)
                landSprites[i] = Resources.Load<Sprite>("Planet/PlanetLand/"+((LandType)i).ToString());
            
            for (int i = 0; i < LAND_COUNT-1; i++)
                for (int j = i + 1; j < LAND_COUNT; j++)
                    landTranslationSprite[i, j] = 
                        Resources.Load<Sprite>("Planet/PlanetLand/"+((LandType)i).ToString()+"_"+((LandType)j).ToString());
            for (int i = 0; i < LAND_COUNT; i++)
                for (int j = 0; j < LAND_COUNT; j++)
                {
                    if (j > i)
                        landTranslationSprite[j, i] = landTranslationSprite[i, j];
                }
            landShader = Resources.Load<Shader>("Planet/MaskRotateShader");
            perfectPixelShader = Resources.Load<Shader>("Planet/PixelRotate");
            seaPrefab = Resources.Load<GameObject>("Planet/Sea");
            landPrefab = Resources.Load<GameObject>("Planet/Land");
            cloudPrefab = Resources.Load<GameObject>("Planet/Shaders/CloudMask");
            treeSprite = Resources.Load<Sprite>("Planet/PlanetLand/Tree");
            coreSprite = Resources.Load<Sprite>("Planet/PlanetLand/CORE");
        }

        #region 初始化
        /// <summary>
        /// 生成全新的星球 
        /// </summary>
        public void GeneratePlanet(float perlinScale)
        {
            // 生成ID
            InitID();
            // 生成星球(数据)
            InitPlanet(perlinScale);
            // 生成星球(外貌)
            GenerateOutLook();
        }
        /// <summary>
        /// 根据ID生成储存的星球
        /// </summary>
        public void GeneratePlanet(ID id)
        {
            Debug.Log("加载星球 "+id.ToString());
            this.id = id;
            SaveManager.Instance.Load(this, id);
        }

        void InitID()
        {
            id = new ID();
            id.idx = (int)transform.position.x;
            id.idy = (int)transform.position.y;
            id.className = "Planet";
            id.sceneName = "Universe";
            id.Init();
        }
        // 初始化数据
        void InitPlanet(float perlinScale)
        {
            int seed = (int)(perlinScale * 100000);
            Random.InitState(seed);// 注意注意!!! 在一个星球生成时的Random, 取决于传入的perlinScale
            InitSize();
            InitArea();
            InitResource();
        }
        /// <summary>
        /// 在数据已存在(新建/读取存档)时生成星球外貌
        /// </summary>
        void GenerateOutLook()
        {
            GenerateTrigger();
            GenerateSea(); // 2
            GenerateStaticArea();
            GenerateTrees(); // 3
            GenerateCloud(); // 0
        }

        // 设置星球大小
        void InitSize()
        {
            float zoomScale = Mathf.Pow(Random.value + 1f, 2);// [0,4]
            radius = zoomScale*ORIGIN_RADIUS;
            transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
        }
        // 设置星球区域数量, 设置区域类型
        void InitArea()
        {
            areaCount = (int)(radius/ORIGIN_RADIUS*2*Mathf.PI / AREA_LENGTH);
            lands = new LandType[areaCount];
            string[] landTypes = Enum.GetNames(typeof(LandType));
            for(int i=0;i<areaCount;i++)
            {
                int landIndex = ((int)(Random.value*100))%landTypes.Length;
                lands[i] = (LandType)Enum.Parse(typeof(LandType), landTypes[landIndex]);
            }
        }
        // 设置星球资源
        void InitResource()
        {
            this.resources = gameObject.AddComponent<PlanetResource>();
            this.resources.Init(this);
        }

        // 生成各个根节点
        void GenerateRoot()
        {
            landRoot = new GameObject("LandRoot").transform;
            landRoot.SetParent(this.transform);
            buildingRoot = new GameObject("BuildingRoot").transform;
            buildingRoot.SetParent(this.transform);
            othersRoot = new GameObject("OthersRoot").transform;
            othersRoot.SetParent(this.transform);
            translationRoot = new GameObject("TranslationRoot").transform;
            translationRoot.SetParent(othersRoot);
        }

        void GenerateTrigger()
        {
            var collid = gameObject.AddComponent<CircleCollider2D>();
            collid.radius = 2*ORIGIN_RADIUS;// 2倍于localScale
            collid.isTrigger = true;
        }
        void GenerateSea()
        {
            // 先生成海洋, 放在最底层
            Instantiate(seaPrefab,this.transform);
        }
        // 生成静态的部分 : 土地,及土地的连接
        void GenerateStaticArea()
        {
            // 生成基础地形
            for (int landTypeIndex = 0; landTypeIndex < LAND_COUNT; landTypeIndex++)
            {
                if ((LandType)landTypeIndex == LandType.SEA)
                    continue;

                float[] landIndexToShow = new float[areaCount];
                for (int j = 0; j < areaCount; j++)
                    if (lands[j] == (LandType)landTypeIndex)
                        landIndexToShow[j] = 1f;

                Texture2D temp = PlanetGenerateHelper.Instance.CutArea(landSprites[landTypeIndex].texture, areaCount, landIndexToShow);
                targetLand = PlanetGenerateHelper.Instance.Merge(targetLand, temp);
            }

            // 生成过渡区
            // 每个地区占多少角度
            float areaAngle = 360f / (float)areaCount;
            for (int index = 0; index < areaCount - 1; index++)
            {
                int firstIndex = (int)lands[index];
                int secondIndex = (int)lands[index + 1];
                if (firstIndex == secondIndex)
                    continue;
                Sprite transSprite = landTranslationSprite[firstIndex, secondIndex];
                Texture2D temp;
                if (firstIndex > secondIndex)
                {
                    temp = PlanetGenerateHelper.Instance.PixelRotate(transSprite.texture, 360 - (index + 1) * areaAngle, true);
                    targetLand = PlanetGenerateHelper.Instance.Merge(temp, targetLand);
                }
                else
                {
                    temp = PlanetGenerateHelper.Instance.PixelRotate(transSprite.texture, (index + 1) * areaAngle, false);
                    targetLand = PlanetGenerateHelper.Instance.Merge(temp, targetLand);
                }
            }
            // 处理最后一个!
            int first = (int)lands[areaCount - 1];
            int second = (int)lands[0];
            if (first != second)
            {
                Sprite tSprite = landTranslationSprite[first, second];
                bool isFlip = false;
                if (first > second)
                    isFlip = true;
                Texture2D temp;
                temp = PlanetGenerateHelper.Instance.PixelRotate(tSprite.texture, 0, isFlip);
                targetLand = PlanetGenerateHelper.Instance.Merge(temp, targetLand);
            }
            // 生成核心
            targetLand = PlanetGenerateHelper.Instance.Merge(coreSprite.texture, targetLand);

            // 显示在屏幕上
            targetLand.filterMode = FilterMode.Point;
            GameObject land = Instantiate<GameObject>(landPrefab,this.transform);
            land.GetComponent<RawImage>().texture = targetLand;
        }
      
        void GenerateTrees()
        {
            float areaAngle = 360f / (float)areaCount;
            for (int t = 0; t < areaCount; t++)
            {
                if (lands[t] == LandType.GRASS)
                {
                    float a = Random.value;
                    if (a < 0.4f)
                        continue;
                    GameObject go = CreateGameObject("Tree", othersRoot);
                    go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -(t + 0.5f) * areaAngle));
                    AddSpriteRenderer(go, treeSprite, "Planet", 3);
                }
            }
        }
        void GenerateCloud()
        {
            GameObject go = Instantiate<GameObject>(cloudPrefab, this.transform);
            go.transform.localScale *= Random.Range(1.1f,1.4f);
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.value * 233));
        }

        // 抽象一些功能
        // 创建一个GameObject, 大小随this.gameObject的Scale, 绑定到parentTransform
        GameObject CreateGameObject(string name, Transform parentTransform)
        {
            GameObject go = new GameObject(name);
            go.transform.position = this.transform.position;
            go.transform.localScale = this.transform.localScale;
            go.transform.SetParent(parentTransform);
            return go;
        }
        SpriteRenderer AddSpriteRenderer(GameObject go,Sprite sprite, string sortinglayerName, int sortingOrder)
        {
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = sortinglayerName;
            sr.sortingOrder = sortingOrder;
            return sr;
        }
        #endregion

        #region 功能
        /// <summary>
        /// 创建船新的殖民地版本, 张家辉和你一起殖民!
        /// </summary>
        public void CreateColony()
        {
            Debug.Log("创建新殖民地!");
            if (colony != null)
                return;
            GameObject go = new GameObject("Colony");
            go.transform.SetParent(this.transform);
            colony = go.AddComponent<Colony>();
            colony.Init(this);
        }
        public void DeleteColony()
        {
            if (colony == null)
                return;
            Destroy(colony.gameObject);
            colony = null;
        }

        /// <summary>
        /// 将某个GameObject设置在某个坐标, 第三个heightRatio表示放置的物体在星球上的高度(0: 中心, 1: 地表 2: 2倍地表高度)
        /// </summary>
        public void SetGameObjectAtIndex(GameObject obj, int index, float heightRatio = 1)
        {
            heightRatio = Mathf.Clamp(heightRatio, 0, 2f);
            index = GetValidIndex(index);
            var pos = GetIndexPosition(index);
            var rot = GetIndexRotation(index);
            Vector3 deltaToCenter = pos - transform.position;
            pos = transform.position + deltaToCenter * heightRatio;
            obj.transform.SetParent(buildingRoot);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.Euler(rot);
        }
        /// <summary>
        /// 获得指定下标位置的position
        /// </summary>
        public Vector3 GetIndexPosition(int index)
        {
            index = GetValidIndex(index);
            Vector3 center = transform.position;
            Vector3 vec = new Vector3(0, 1, 0);
            var rotateMat = Matrix4x4.Rotate(Quaternion.Euler(GetIndexRotation(index)));
            vec = rotateMat.MultiplyPoint3x4(vec);
            return center + vec*radius;
        }
        /// <summary>
        /// 获得指定下标位置的rotation(欧拉角)
        /// </summary>
        public Vector3 GetIndexRotation(int index)
        {
            index = GetValidIndex(index);
            // +0.5f ! 每个地块 从0~1, 0.5才是地块中心
            // 取负数, 顺时针旋转!
            Vector3 rot = new Vector3(0,0,-((float)index+0.5f)/areaCount*360);
            return rot;
        }
        /// <summary>
        /// 获得指定下标位置的土地类型, 返回名称
        /// </summary>
        public string GetIndexLandType(int index, out LandType type)
        {
            index = GetValidIndex(index);
            type = lands[index];
            return lands[index].ToString();
        }
        public int GetValidIndex(int index)
        {
            // 由于C#负数求余会得负数的问题(标准不一), 这里用 % 求余之后取正数
            index %= areaCount;
            if (index < 0)
                index += areaCount;
            return index;
        }
        #endregion
    }
}