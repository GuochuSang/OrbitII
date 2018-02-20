using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Manager;
namespace Universe 
{
public class Chunk : ISaveable
{
    // 这个是Chunk的unit Size
    public const int SIZE = 400;
    // 这个是Chunk的 TileMap Size (16:2即一个TileSize占8个UnitSize , SIZE/TILE_SIZE必须为整数)
    // 注意 !!! 需要设置 TileGrid 的 CellSize = SIZE/TILE_SIZE
    // 即一个 Chunk 有 TILE_SIZE^2 个Block
    public const int TILE_SIZE = 4;

    public ID id; // 对于Chunk来说, 它只会被创建而不会被删除, 所以不需要调用 id.OnDestroy()
    bool enableUpdate = false; // 用于记录Chunk的状态, Chunk自身不能调用自己的Update (就像Monobehavior对象不调用自己的Update)

    #region 需要储存的数据
    Vector2Int position; // chunk的位置 (单个方块的位置为 position*SIZE+方块在Chunk中的位置*(SIZE/TILE_SIZE))
    List<ID> childObjectIDs;
    ID[,] frontBlockIDs;
    ID[,] bkBlockIDs;
    #endregion
    public Vector2Int Position
    {
        get{ return position;}
        private set{ }
    }

    #region Chunk管理的对象
    BlockBase[,] frontBlocks;// 前景
    BlockBase[,] bkBlocks;// 背景
    public List<IChunkObject> childObjects;// Chunk管理自己范围内的GameObject
    #endregion

    #region 新建/根据ID读取
    /// <summary>
    /// 构造方法1 : 创建全新的Chunk
    /// </summary>
    public Chunk(Vector2Int pos, string sceneName)
    {
        id = new ID();
        id.idx = pos.x;
        id.idy = pos.y;
        id.className = "Chunk";
        id.sceneName = sceneName;
        id.Init();   


        childObjectIDs = new List<ID>();
        frontBlockIDs = new ID[TILE_SIZE,TILE_SIZE];
        bkBlockIDs = new ID[TILE_SIZE, TILE_SIZE];
        position = pos;

        frontBlocks = new BlockBase[TILE_SIZE, TILE_SIZE];
        bkBlocks = new BlockBase[TILE_SIZE, TILE_SIZE];

        childObjects = new List<IChunkObject>();

        // 生成星球等...
        Generate();
    }
    /// <summary>
    /// 构造方法2 : 根据ID加载原来的Chunk
    /// </summary>
    public Chunk(ID id)
    {
        this.id = id;
        id.Init();
        SaveManager.Instance.Load(this, id);
    }
    #endregion

    #region Chunk生成
    // 生成宇宙地形, 星球....
    public void Generate()
    {
        PlanetGenerate.Generate(this);

        for (int i = 0; i < TILE_SIZE; i++)
        {
            for (int j = 0; j < TILE_SIZE; j++)
            {
                Space space = ScriptableObject.CreateInstance<Space>();
                space.Init(ToTilePos(new Vector2Int(i, j)), id.sceneName);
                SetBlockAt(space, new Vector2Int(i, j), true);
            }
        }
    }
    #endregion


    #region Save|实现ISaveable接口
    public ID Save()
    {
        SaveManager.Instance.Save(this, id);
        return id;
    }
    public void fromSaveData(SaveData chunkData)
    {
        ChunkData data = (ChunkData)chunkData;

        this.childObjects = new List<IChunkObject>();
        this.frontBlocks = new BlockBase[TILE_SIZE, TILE_SIZE];
        this.bkBlocks = new BlockBase[TILE_SIZE, TILE_SIZE];

        this.childObjectIDs = data.childObjectIDs;
        this.frontBlockIDs = data.frontBlockIDs;
        this.bkBlockIDs = data.bkBlockIDs;
        this.position = data.position.GetData();

        // 通过 ID 获得 CreatableSaveData , 然后创建Block 
        for (int i = 0; i < TILE_SIZE; i++)
            for (int j = 0; j < TILE_SIZE; j++)
            {
                ID blockId = frontBlockIDs[i, j];
                if (blockId == null)
                    continue;
                SaveData originData = SaveManager.Instance.GetSaveData(blockId);
                if (originData == null)
                    continue;
                CreatableSaveData blockData = (CreatableSaveData)originData;
                BlockBase block = blockData.Create<BlockBase>(blockId);
                frontBlocks[i, j] = block;
            }
        for (int i = 0; i < TILE_SIZE; i++)
            for (int j = 0; j < TILE_SIZE; j++)
            {
                ID blockId = bkBlockIDs[i, j];
                if (blockId == null)
                    continue;
                SaveData originData = SaveManager.Instance.GetSaveData(blockId);
                if (originData == null)
                    continue;
                CreatableSaveData blockData = (CreatableSaveData)originData;
                BlockBase block = blockData.Create<BlockBase>(blockId);
                bkBlocks[i, j] = block;
            }
        // childObjects
        foreach (ID childID in childObjectIDs)
        {
            Debug.Log("加载ChunkObjects : "+childID.ToString());
            CreatableSaveData childData = (CreatableSaveData)SaveManager.Instance.GetSaveData(childID);
            if (childData == null)
                break;
            IChunkObject child = childData.Create<IChunkObject>(childID);
            childObjects.Add(child);
        }

    }
    public SaveData toSaveData()
    {
        ChunkData data = new ChunkData();

        frontBlockIDs = new ID[TILE_SIZE, TILE_SIZE];
        bkBlockIDs = new ID[TILE_SIZE, TILE_SIZE];
        childObjectIDs = new List<ID>();

        for (int i = 0; i < TILE_SIZE; i++)
            for (int j = 0; j < TILE_SIZE; j++)
            {
                if(frontBlocks[i, j] != null)
                    frontBlockIDs[i, j] = frontBlocks[i, j].Save();
                if(bkBlockIDs[i, j] != null)
                    bkBlockIDs[i, j] = bkBlocks[i, j].Save();
            }
        foreach (IChunkObject child in childObjects)
        {
            childObjectIDs.Add(child.Save());
        }
        

        data.position.SaveData(this.position);
        data.childObjectIDs = this.childObjectIDs;
        data.frontBlockIDs = this.frontBlockIDs;
        data.bkBlockIDs = this.bkBlockIDs;
        return data;
    }
    #endregion
        

    #region 进入Update|正常Update|退出Update
    /// <summary>
    /// 更新管辖的所有Block (快速刷新)
    /// </summary>
    public void Update(Tilemap frontTilemap,Tilemap bkTilemap)
    {
        foreach (BlockBase block in frontBlocks)
        {
            if (block != null && block.Update())
                ShowBlockTile(block, frontTilemap);
        }
        foreach (BlockBase block in bkBlocks)
        {
            if (block != null && block.Update())
                ShowBlockTile(block, bkTilemap);
        }
    }
    /// <summary>
    /// 更新管辖的所有Block, 以及GameObject, 用于经过一段时间后重新加载此区块.
    /// </summary>
    public void EnterUpdate(Tilemap frontTilemap,Tilemap bkTilemap)
    {
        enableUpdate = true;
        foreach (BlockBase block in frontBlocks)
        {
            if(block!=null)
                block.EnterUpdate();
        }
        foreach (BlockBase block in bkBlocks)
        {
            if(block!=null)
                block.EnterUpdate();
        }
        foreach (IChunkObject child in childObjects)
        {
            child.EnterUpdate();
        }
        ShowAllBlockTiles(frontTilemap,bkTilemap);
    }
    /// <summary>
    /// 当Chunk远离摄像机, 停止更新并储存
    /// </summary>
    public void ExitUpdate(Tilemap frontTilemap,Tilemap bkTilemap)
    {
        enableUpdate = false;
        foreach (BlockBase block in frontBlocks)
        {
            if(block != null)
                block.ExitUpdate();
        }
        foreach (BlockBase block in bkBlocks)
        {
            if(block != null)
                block.ExitUpdate();
        }
        foreach (IChunkObject child in childObjects)
        {
            child.ExitUpdate();
        }
        Save();
    }
    #endregion

    // 检测Child GameObjects是否逃逸, 从列表删除逃逸的对象, 并返回逃逸的GameObject列表 (ChunkManager : 如果不在自己的范围, 就把它交给其他Chunk管理)
    public List<IChunkObject> CheckRunningChildren()
    {
        List<IChunkObject> outerChildren = new List<IChunkObject>();
        for (int i = childObjects.Count-1; i >= 0; i--)
        {
            IChunkObject child = childObjects[i];
            //如果不包含这个GameObject, 就加入返回列表
            if (!CheckContain(child.GetPosition()))
            {
                // 如果这个Chunk正在更新, 那么child物体退出更新
                if(enableUpdate)
                    child.ExitUpdate();
                childObjects.Remove(child);
                outerChildren.Add(child);
            }
        }
        return outerChildren;
    }
    public void AddChild(IChunkObject child)
    {
        childObjects.Add(child);
        // 如果这个Chunk正在更新, 那么child物体进入更新
        child.EnterUpdate();
    }

    public BlockBase GetBlockAt(Vector2Int blockPosChunk, bool frontBlock)
    {
        if (frontBlock)
            return frontBlocks[blockPosChunk.x, blockPosChunk.y];
        else
            return bkBlocks[blockPosChunk.x, blockPosChunk.y];
    }
    public void SetBlockAt(BlockBase block, Vector2Int blockPosChunk, bool frontBlock)
    {
        if (frontBlock)
            frontBlocks[blockPosChunk.x, blockPosChunk.y] = block;
        else 
            bkBlocks[blockPosChunk.x, blockPosChunk.y] = block;
    }

    #region 私有方法  显示单个/所有Block|获得Block世界坐标|检测GameObject是否在自己的范围内
    // 加载Block到TileMap上
    void ShowAllBlockTiles(Tilemap frontTilemap,Tilemap bkTilemap)
    {
        foreach (BlockBase block in frontBlocks)
        {
            if(block!=null)
                ShowBlockTile(block, frontTilemap);
        }
        foreach (BlockBase block in bkBlocks)
        {
            if(block!=null)
                ShowBlockTile(block, bkTilemap);
        }
    }
    void HideAllBlockTiles(Tilemap frontTilemap,Tilemap bkTilemap)
    {
        foreach (BlockBase block in frontBlocks)
        {
            if(block!=null)
                HideBlockTile(block, frontTilemap);
        }
        foreach (BlockBase block in bkBlocks)
        {
            if(block!=null)
                HideBlockTile(block, bkTilemap);
        }
    }
    void ShowBlockTile(BlockBase block,Tilemap tilemap)
    {
        tilemap.SetTile(new Vector3Int(block.position.x,block.position.y,0),block);
    }
    void HideBlockTile(BlockBase block,Tilemap tilemap)
    {
        tilemap.SetTile(new Vector3Int(block.position.x,block.position.y,0),null);
    }
    // Chunk内的某Block的局部坐标转世界坐标
    Vector2Int ToWorldPos(Vector2Int blockPos)
    {
        return blockPos*(SIZE/TILE_SIZE) + position * SIZE;
    }
    // Chunk内的某Block的局部坐标转Tile坐标
    Vector2Int ToTilePos(Vector2Int blockPos)
    {
        return blockPos + position * TILE_SIZE;
    }
    bool CheckContain(Vector2 childPosition)
    {
        Vector2 startPos = new Vector2((float)position.x*SIZE,(float)position.y*SIZE);
        Vector2 endPos = new Vector2(startPos.x + SIZE, startPos.y + SIZE);
        if (childPosition.x > startPos.x && childPosition.x < endPos.x)
        {
            if (childPosition.y > startPos.y && childPosition.y < endPos.y)
                return true;
        }
        return false;
    }
    #endregion
}

[System.Serializable]
public class ChunkData : SaveData
{
    public ChunkData()
    {
        position = new Vector2IntData();
    }
    public List<ID> childObjectIDs;
    public ID[,] frontBlockIDs;
    public ID[,] bkBlockIDs;
    public Vector2IntData position;
}
}