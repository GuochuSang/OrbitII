using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private GameObject CachePanel;

        private Dictionary<string, Queue<GameObject>> m_Pool = new Dictionary<string, Queue<GameObject>>();

        private Dictionary<GameObject, string> m_GoTag = new Dictionary<GameObject, string>();


        /// <summary>
        /// 清空缓存池，释放所有引用
        /// </summary>
        public void ClearCachePool()
        {
            m_Pool.Clear();
            m_GoTag.Clear();
        }

        /// <summary>
        /// 回收GameObject
        /// </summary>
        public void ReturnCacheGameObejct(GameObject go)
        {
            go.SetActive(false);
            if (CachePanel == null)
            {
                CachePanel = new GameObject();
                CachePanel.name = "CachePanel";
                GameObject.DontDestroyOnLoad(CachePanel);
            }

            if (go == null)
            {
                return;
            }

            go.transform.parent = CachePanel.transform;

            if (m_GoTag.ContainsKey(go))
            {
                string tag = m_GoTag[go];
                RemoveOutMark(go);

                if (!m_Pool.ContainsKey(tag))
                {
                    m_Pool[tag] = new Queue<GameObject>();
                }

                m_Pool[tag].Enqueue(go);
            }
        }

        /// <summary>
        /// 请求GameObject
        /// </summary>
        public GameObject GetCacheGameObejct(GameObject prefab)
        {
            string tag = prefab.GetInstanceID().ToString();
            GameObject go = GetFromPool(tag);
            if (go == null)
            {
                go = GameObject.Instantiate<GameObject>(prefab);
                go.name = prefab.name + Time.time;
            }


            MarkAsOut(go, tag);
            return go;
        }


        private GameObject GetFromPool(string tag)
        {
            if (m_Pool.ContainsKey(tag) && m_Pool[tag].Count > 0)
            {
                GameObject obj = m_Pool[tag].Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                return null;
            }
        }


        private void MarkAsOut(GameObject go, string tag)
        {
            m_GoTag.Add(go, tag);
        }

        private void RemoveOutMark(GameObject go)
        {
            if (m_GoTag.ContainsKey(go))
            {
                m_GoTag.Remove(go);
            }
            else
            {
                Debug.LogError("remove out mark error, gameObject has not been marked");
            }
        }
    }
}