/// <summary>
/// ChunkManager 使用说明:
/// 一个存档只有一个ChunkManager
/// 调用Load()加载数据
/// 调用Save()保存数据
/// 
/// 调用UpdateArea()刷新区域
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Manager;
namespace Universe 
{

    public class ChunkManager : MonoSingleton<ChunkManager>,ISaveable
    {
        
        public void TestShowAll()
        {
            foreach(KeyValuePair<Vector2Int,ID> vecID in allChunkIDs)
            {
                if (allChunks.ContainsKey(vecID.Key))
                    allChunks[vecID.Key].EnterUpdate(frontTilemap,backTilemap);
                else
                    CreateChunkAt(vecID.Key).EnterUpdate(frontTilemap,backTilemap);
            }
        }

        const int flashArea = 3;// 刷新一层
        public float updatePeriod = 0.1f;// 0.1s更新一次

        #region 要储存的数据
        public Dictionary<Vector2Int,ID> allChunkIDs;
        #endregion

        Dictionary<Vector2Int,Chunk> allChunks;// 所有的Chunk
        List<Chunk> currentChunks; // 当前要更新的Chunk列表

        ID id;// 对于ChunkManager来说, 它只会被创建而不会被删除, 所以不需要调用 id.OnDestroy()
        Vector2Int centerChunkPos;// 摄像机中心的Chunk坐标

        Tilemap frontTilemap;// ChunkManager管理Tilemap
        Tilemap backTilemap;

        public Transform updateCamera;
        void Awake()
        {
            id = new ID();
            id.className = "ChunkManager";
            id.sceneName = "Universe";
            id.Init();

            allChunkIDs = new Dictionary<Vector2Int, ID>();
            allChunks = new Dictionary<Vector2Int, Chunk>();
            currentChunks = new List<Chunk>();
            GenerateTilemap();
            Load();

            EventManager.Instance.AddListener(this, GameEvent.SAVE_GAME, OnSaveGame);
        }
        void Start()
        {
            StartCoroutine(UpdateCoroutine());
            StartCoroutine(UpdateAreaCoroutine());
        }
        void OnDestroy()
        {
            EventManager.Instance.RemoveObjectEvent(this, GameEvent.SAVE_GAME);
        }


        IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                UpdateArea((Vector2)updateCamera.position);
                yield return new WaitForSeconds(updatePeriod);
            }
        }
        #region 事件: 保存..
        public void OnSaveGame(GameEvent gameEvent,Component comp,object param = null)
        {
            this.Save();
        }
        #endregion

        #region Chunks数据存储/读取 (实现ISaveable接口|公有的Save,Load接口)
        public void fromSaveData(SaveData chunkManagerData)
        {
            Debug.Log("读取ChunkManager");
            allChunkIDs = new Dictionary<Vector2Int, ID>();
            allChunks = new Dictionary<Vector2Int, Chunk>();
            currentChunks = new List<Chunk>();

            ChunkManagerData data = (ChunkManagerData)chunkManagerData;
            foreach (KeyValuePair<Vector2IntData,ID> vecID in data.allChunkIDs)
            {
                this.allChunkIDs.Add(vecID.Key.GetData(), vecID.Value);
            }
        }
        public SaveData toSaveData()
        {
            ChunkManagerData data = new ChunkManagerData();
            foreach (KeyValuePair<Vector2Int,ID> vecID in this.allChunkIDs)
            {
                Vector2IntData vecData = new Vector2IntData();
                vecData.SaveData(vecID.Key);
                data.allChunkIDs.Add(vecData, vecID.Value);
            }
            return data;
        }
        public void Save()
        {
            foreach (Chunk currentChunk in currentChunks)
                currentChunk.Save();
            SaveManager.Instance.Save(this, id);
        }
        public void Load()
        {
            SaveManager.Instance.Load(this, id);
        }
        #endregion


        #region 刷新区域|获得特定位置的Block|设置特定位置的Block|分配一个Child到某个Chunk
        enum UpdateType{ENTER,UPDATE,EXIT};
        Queue<KeyValuePair<Chunk,UpdateType>> chunksToUpdate = new Queue<KeyValuePair<Chunk, UpdateType>>();

        /// <summary>
        /// 输入位置, 刷新区域
        /// </summary>
        /*
        public void UpdateArea(Vector2 cameraPos)
        {
            // 1. 首先先检查并管理当前的Chunks的对象列表
            AllotChunkChildren();

            // 2. 有的Chunk开始更新, 有的正常更新, 有的结束更新 
            Vector2Int chunkPos = ToChunkPos(FloorToVector2Int(cameraPos));
            // 只有当坐标不同 或 当前没有要更新的Chunk, 才需要更新currentChunks
            if (currentChunks.Count==0 || chunkPos != centerChunkPos)
            {
                centerChunkPos = chunkPos;
                List<Chunk> newChunks = GetChunksNearby(chunkPos,flashArea);// !!!!!!!!!!!!!!!!!!!!!!!
                for(int i=currentChunks.Count-1;i>=0;i--)
                { 
                    Chunk oldChunk = currentChunks[i];
                    // 在新的列表中的oldChunk正常更新
                    if (newChunks.Contains(oldChunk))
                    {
                        newChunks.Remove(oldChunk);
                        oldChunk.Update(frontTilemap,backTilemap);
                    }
                    // 不在新的列表中的oldChunk退出更新
                    else
                    {
                        currentChunks.Remove(oldChunk);
                        oldChunk.ExitUpdate(frontTilemap,backTilemap);
                    }
                }
                // 新增的Chunk进入更新状态
                foreach (Chunk newChunk in newChunks)
                {
                    newChunk.EnterUpdate(frontTilemap,backTilemap);
                    currentChunks.Add(newChunk);
                }
             
            }
        }
        */
        public void UpdateArea(Vector2 cameraPos)
        {
            // 1. 首先先检查并管理当前的Chunks的对象列表
            AllotChunkChildren();

            // 2. 有的Chunk开始更新, 有的正常更新, 有的结束更新 
            Vector2Int chunkPos = ToChunkPos(FloorToVector2Int(cameraPos));
            // 只有当坐标不同 或 当前没有要更新的Chunk, 才需要更新currentChunks
            if (currentChunks.Count==0 || chunkPos != centerChunkPos)
            {
                centerChunkPos = chunkPos;
                List<Chunk> newChunks = GetChunksNearby(chunkPos,flashArea);// !!!!!!!!!!!!!!!!!!!!!!!
                for(int i=currentChunks.Count-1;i>=0;i--)
                { 
                    Chunk oldChunk = currentChunks[i];
                    // 在新的列表中的oldChunk正常更新
                    if (newChunks.Contains(oldChunk))
                    {
                        newChunks.Remove(oldChunk);
                        chunksToUpdate.Enqueue(new KeyValuePair<Chunk, UpdateType>(oldChunk,UpdateType.UPDATE));
                    }
                    // 不在新的列表中的oldChunk退出更新
                    else
                    {
                        currentChunks.Remove(oldChunk);
                        chunksToUpdate.Enqueue(new KeyValuePair<Chunk, UpdateType>(oldChunk,UpdateType.EXIT));
                    }
                }
                // 新增的Chunk进入更新状态
                foreach (Chunk newChunk in newChunks)
                {
                    chunksToUpdate.Enqueue(new KeyValuePair<Chunk, UpdateType>(newChunk,UpdateType.ENTER));
                    currentChunks.Add(newChunk);
                }
            }
        }
        IEnumerator UpdateAreaCoroutine()
        {
            while (true)
            {
                if (chunksToUpdate.Count > 0)
                {
                    var chunkToType = chunksToUpdate.Dequeue();
                    switch (chunkToType.Value)
                    {
                        case UpdateType.ENTER:
                            StartCoroutine(chunkToType.Key.EnterUpdate(frontTilemap, backTilemap));
                            break;
                        case UpdateType.UPDATE:
                            chunkToType.Key.Update(frontTilemap, backTilemap);
                            break;
                        case UpdateType.EXIT:
                            StartCoroutine(chunkToType.Key.ExitUpdate(frontTilemap, backTilemap));
                            break;
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// 获得pos位置(默认前景)的Block
        /// </summary>
        public BlockBase GetBlockAt(Vector2Int blockPosWorld, bool frontBlock = true)
        {
            Chunk thatChunk = null;
            thatChunk = GetChunk(ToChunkPos(blockPosWorld));
            return thatChunk.GetBlockAt(ToBlockPos(blockPosWorld), frontBlock);
        }
        /// <summary>
        /// 设置pos位置(默认前景)的Block
        /// </summary>
        public void SetBlockAt(BlockBase block, Vector2Int blockPosWorld, bool frontBlock = true)
        {
            Chunk thatChunk = null;
            thatChunk = GetChunk(ToChunkPos(blockPosWorld));
            thatChunk.SetBlockAt(block,ToBlockPos(blockPosWorld), frontBlock);
        }
        // 分配一个 GameObject 到某个 Chunk
        public void AllotChunkChild(IChunkObject child)
        {
            Vector2 childPos = child.GetPosition();
            Vector2Int worldPos = FloorToVector2Int(childPos.x, childPos.y);
            Vector2Int chunkPos = ToChunkPos(worldPos);
            Chunk newParentChunk = GetChunk(chunkPos);
            newParentChunk.AddChild(child);
        }

        #endregion


        #region 私有方法  世界坐标转换Chunk/Block坐标|获得坐标处的Chunk/Chunks|在某位置创建新的Chunk|为逃逸的GameObjects找个新的Chunk来管理|
        // 向下取整!!!
        Vector2Int FloorToVector2Int(Vector2 vec2)
        {
            return new Vector2Int(Mathf.FloorToInt(vec2.x), Mathf.FloorToInt(vec2.y));
        }
        Vector2Int FloorToVector2Int(float x, float y)
        {
            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        Vector2Int ToBlockPos(Vector2Int worldPos)
        {
            return new Vector2Int((worldPos.x % Chunk.SIZE)/(Chunk.SIZE/Chunk.TILE_SIZE), (worldPos.y % Chunk.SIZE)/(Chunk.SIZE/Chunk.TILE_SIZE));
        }
       
        Vector2Int ToChunkPos(Vector2Int worldPos)
        {
            return FloorToVector2Int((float)worldPos.x/ (float)Chunk.SIZE,(float)worldPos.y/ (float)Chunk.SIZE);
        }
        List<Chunk> TryGetChunksNearby(Vector2Int chunkPos,int circleCount = 1)
        {
            List<Chunk> chunksToGet = new List<Chunk>();
            for (int i = -circleCount; i <= circleCount; i++)
                for (int j = -circleCount; j <= circleCount; j++)
                {
                    Chunk chunk = TryGetChunk(new Vector2Int(chunkPos.x + i, chunkPos.y + j));
                    if (chunk != null)
                        chunksToGet.Add(chunk);
                }
            return chunksToGet;
        }
        // 获得以一个Chunk位置为中心的附近的Chunks (circleCount表示层数, 1层表示九宫格, 2层表示25...)
        List<Chunk> GetChunksNearby(Vector2Int chunkPos,int circleCount = 1)
        {
            List<Chunk> chunksToGet = new List<Chunk>();
            for (int i = -circleCount; i <= circleCount; i++)
                for (int j = -circleCount; j <= circleCount; j++)
                {
                    Chunk chunk = GetChunk(new Vector2Int(chunkPos.x + i, chunkPos.y + j));
                    chunksToGet.Add(chunk);
                }
            return chunksToGet;
        }
        // 检查一个坐标是否在附近
        bool CheckChunkNearby(Vector2Int chunkPos, Vector2Int centerPos,int circleCount = 1)
        {
            return Mathf.Abs(chunkPos.x - centerPos.x) <= 1 && Mathf.Abs(chunkPos.y - centerPos.y) <= 1;
        }
        Chunk TryGetChunk(Vector2Int chunkPos)
        {
            Chunk thatChunk = null;
            allChunks.TryGetValue(chunkPos,out thatChunk);
            if (thatChunk == null)
                return null;
            return thatChunk;
        }
        // 获得某位置的Chunk (如果那个位置没有Chunk将新建一个, 所以不会返回null)
        Chunk GetChunk(Vector2Int chunkPos)
        {
            Chunk thatChunk = null;
            allChunks.TryGetValue(chunkPos,out thatChunk);
            if (thatChunk == null)
            {
                thatChunk = CreateChunkAt(chunkPos);
            }
            return thatChunk;
        }
        // 在某位置创建新的Chunk, 或者从数据中加载Chunk
        Chunk CreateChunkAt(Vector2Int chunkPos)
        {
            ID chunkID = null;
            allChunkIDs.TryGetValue(chunkPos,out chunkID);
            if (chunkID == null)
            {
                // 这里创建了全新的Chunk, 需要初始化Chunk
                Chunk newChunk = new Chunk(chunkPos, id.sceneName);
                allChunkIDs.Add(chunkPos, newChunk.id);
                allChunks.Add(chunkPos, newChunk);

                return newChunk;
            }
            else
            {
                if (chunkID == null)
                    Debug.Log("FUCK NULLLLLLLL!!!!!!!!!!");
                Chunk oldChunk = new Chunk(chunkID);
                allChunks.Add(chunkPos, oldChunk);
                return oldChunk;
            }
        }

        // 管理逃逸到其他Chunk的GameObjects
        void AllotChunkChildren()
        {
            // 对于每个Chunk
            foreach (Chunk chunk in currentChunks)
            {
                // 找到逃亡的Children
                List<IChunkObject> runningObjects = new List<IChunkObject>();
                runningObjects = chunk.CheckRunningChildren();
                // 把这些Children分配给其他的Chunk
                foreach (IChunkObject child in runningObjects)
                {
                    AllotChunkChild(child);
                }
            }

        }

        void GenerateTilemap()
        {
            GameObject spaceGrid = GameObject.Instantiate(Resources.Load("ChunkManager/UniverseGrid")) as GameObject;
            GameObject tile1 = GameObject.Instantiate(Resources.Load("ChunkManager/Tilemap"), spaceGrid.transform) as GameObject;
            GameObject tile2 = GameObject.Instantiate(Resources.Load("ChunkManager/Tilemap"), spaceGrid.transform) as GameObject;
            frontTilemap = tile1.GetComponent<Tilemap>();
            backTilemap = tile2.GetComponent<Tilemap>();
        }
        #endregion


    }

    [System.Serializable]
    public class ChunkManagerData : SaveData
    {
        public Dictionary<Vector2IntData,ID> allChunkIDs;
        public ChunkManagerData()
        {
            allChunkIDs = new Dictionary<Vector2IntData, ID>();
        }
    }
}