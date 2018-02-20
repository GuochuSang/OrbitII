/// <summary>
/// Chunk 初始生成时, 调用 PlanetGenerate 的 Generate, 并且将自己作为参数传递即可
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
public class PlanetGenerate : MonoBehaviour
{
    // 可以生成的物体的类型
    public enum PlanetType
    {
        PLANET,  SUN, BLACKHOLE
    }
    // 可以生成的物体的参数
    [System.Serializable]
    public class GenerateInfo
    {
        // 类型
        public PlanetType type;
        // 噪音, 生成控制
        public int originIndex;
        public float perlinScale;
        public float minLimit;

        public bool Generate(ChunkGenerateInfo info, out IChunkObject generated)
        {
            generated = null;
            float noise = Seed.GetNoise(info.maxPos,perlinScale,originIndex);
            if (noise < minLimit)
                return false;
            GameObject go = new GameObject(type.ToString());
            go.transform.position = new Vector3(info.maxPos.x+(info.maxValue-0.4f)*10, info.maxPos.y+(info.maxValue-0.4f)*5, 0);

            if (type == PlanetType.PLANET)
            {
                generated = go.AddComponent<Planet>();
                ((Planet)generated).GeneratePlanet(noise);
            }
            else if (type == PlanetType.SUN)
            {
                generated = go.AddComponent<Sun>();
                go.transform.localScale *= 20 * (noise * noise + 0.5f);
            }
            else if (type == PlanetType.BLACKHOLE)
            {
                generated = go.AddComponent<BlackHole>();
                go.transform.localScale *= 20 * (noise * noise + 0.5f);
            }
            return true;
        }
    }
    // 记录传入的Chunk
    public class ChunkGenerateInfo
    {
        public Vector2Int pos;
        public Vector2 maxPos;
        public float maxValue;
    } 


    // 每次按照这个顺序检测是否应该生成
    public List<GenerateInfo> GenerateQueue;
    static List<GenerateInfo> generateQueue;
    // 生成后的Parent
    public static Transform generateRoot;

    [Header("星球生成密度"),Tooltip("值很大时, 密度为约每5个Chunk有一个星球")]
    public float PerlinScale = 0.0023f;
    static float perlinScale = 0.0023f;
    static int perlinOriginIndex = 0;

    // 设置星球根节点, 将预制体中的 generateQueue 放到静态数据中
	void Start () 
	{
        GameObject go = new GameObject ("Universe Generate Root");
        generateRoot = go.transform;
        generateQueue = GenerateQueue;
        perlinScale = PerlinScale;
	}

    public static void Generate(Chunk chunk)
    {
        ChunkGenerateInfo centerInfo = new ChunkGenerateInfo();
        if (!CountChunk(chunk, out centerInfo))
            return;
        IChunkObject mayGenerated;
        foreach(GenerateInfo toGen in generateQueue)
        {
            // 只生成一个
            if (toGen.Generate(centerInfo, out mayGenerated))
            {
                ((Component)mayGenerated).transform.SetParent(generateRoot);
                chunk.AddChild(mayGenerated);
                return;
            }
        }
    }
    // 计算新来的Chunk, 判断它是否能生成星球
    static bool CountChunk(Chunk chunk, out ChunkGenerateInfo maxInfo)
    {
        maxInfo = new ChunkGenerateInfo();
        maxInfo.pos = chunk.Position;

        float maxNoise = 0;
        Vector2 maxPos = Vector2.zero;
        for (int i = 0; i < Chunk.SIZE; i += Chunk.SIZE-1)
        {
            for (int j = 0; j < Chunk.SIZE; j += Chunk.SIZE-1)
            {
                Vector2 pos = new Vector2((float)i+(float)maxInfo.pos.x*Chunk.SIZE, (float)j+(float)maxInfo.pos.y*Chunk.SIZE);
                float noiseValue = Seed.GetNoise(pos,perlinScale,perlinOriginIndex);
                if (noiseValue > maxNoise)
                {
                    maxNoise = noiseValue;
                    maxPos = pos;
                }
            }
        }
        Vector2 center = new Vector2((float)Chunk.SIZE/2 + (float)maxInfo.pos.x * Chunk.SIZE, (float)Chunk.SIZE/2 + (float)maxInfo.pos.y * Chunk.SIZE);
        float centerNoise = Seed.GetNoise(center,perlinScale,perlinOriginIndex);
        if (centerNoise <= maxNoise)
            return false;
        maxInfo.maxValue = centerNoise;
        maxInfo.maxPos = center;
        return true;
    }
}
}