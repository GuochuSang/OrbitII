using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe
{
    public class ChunkFlash : MonoSingleton<ChunkFlash> 
    {
        // 在CHUNK刷新时可能出现的物体
        [System.Serializable]
        public struct UniverseThing
        {
            public GameObject objPrefab;
            // 出现概率(随机数小于ratio则生成)
            public float showRatio;
            // 最大出现个数, showRatio依次递减!
            // (showRatio-value)/(ratio/maxCount)
            public int maxCount;
        }
        // 填写这个List
        public List<UniverseThing> flashableThings = new List<UniverseThing>();
        public Transform flashObjectRoot;
        void Awake()
        {
            flashObjectRoot = new GameObject("FlashObjectRoot").transform;
        }
        public void FlashHere(Chunk chunk)
        {
           // StartCoroutine(FlashCoroutine(chunk));
        }
        IEnumerator FlashCoroutine(Chunk chunk)
        {
            Vector3 chunkPos = new Vector3(chunk.Position.x*Chunk.SIZE, chunk.Position.y*Chunk.SIZE, 0);
            for (int index = 0;index<flashableThings.Count;index++)
            {
                var thing = flashableThings[index];
                int flashCount = Mathf.FloorToInt((thing.showRatio - Random.value)/(thing.showRatio/thing.maxCount));
                for (int i = 0; i < flashCount; i++)
                {
                    GameObject obj = PoolManager.Instance.GetCacheGameObejct(thing.objPrefab);
                    obj.transform.position = chunkPos + new Vector3(Random.value,Random.value,0)*Chunk.SIZE;
                    chunk.AddChild(obj.GetComponent<IChunkObject>());
                    obj.transform.SetParent(flashObjectRoot);
                    yield return null;
                }
                yield return null;
            }
        }
    }
}