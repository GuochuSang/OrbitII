/// <summary>
/// Seed.
/// 在生成存档之后, seed被设置
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager 
{
public static class Seed
{
    public static int originSeed;// 初始种子
    public static List<Vector2> perlinOrigin;// 根据初始种子生成一组噪音原点
    public static void InitSeed(int seed)
    {
        originSeed = seed;
        Random.InitState(seed);
        perlinOrigin = new List<Vector2>();
        for (int i = 0; i < 10; i++)
            perlinOrigin.Add(new Vector2(Random.value * 10000f, Random.value * 10000f));
    }
    /// <summary>
    /// 输入原点, perlinScale, 以及要使用的perlinOrigin序号[0, 9]
    /// </summary>
    public static float GetNoise(Vector2 pos,float perlinScale,int originIndex)
    {
        Debug.Assert(perlinOrigin != null, "你还没有初始化Seed!");
        originIndex = originIndex % 10;
        if (originIndex < 0)
            originIndex += 10;
        return Mathf.PerlinNoise((pos.x + perlinOrigin[originIndex].x) * perlinScale, (pos.y + perlinOrigin[originIndex].y) * perlinScale);
    }
	
}
}