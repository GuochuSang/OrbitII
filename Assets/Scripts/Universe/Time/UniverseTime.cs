using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Universe
{
    [System.Serializable]
    public class UniverseTimeData : SaveData
    {
        public float time;
    }

    public class UniverseTime : MonoSingleton<UniverseTime>, ISaveable, IPauseable
    {
        ID id;
        float prevUnityTime;
        [SerializeField]
        float time;
        /// <summary>
        /// 这里获得准确秒的游戏时间, 特殊的游戏时间(年, 世纪等)由其他地方显示
        /// </summary>
        /// <value>The time.</value>
        public float Time
        {
            get{ return time;}
            private set{ }
        }
        void Start()
        {
            time = 0;
            prevUnityTime = UnityEngine.Time.time;
            id = new ID();
            id.className = "UniverseTime";
            id.sceneName = "Universe";
            id.Init();

            SaveManager.Instance.Load(this, id);
            StartCoroutine(UpdateTime());
        }
        // 持续更新时间
        IEnumerator UpdateTime()
        {
            while (true)
            {
                UpdateTimeOnce();
                yield return null;
            }
        }
        // 更新一次时间
        void UpdateTimeOnce()
        {
            float deltaTime = UnityEngine.Time.time - prevUnityTime;
            prevUnityTime = UnityEngine.Time.time;
            time += deltaTime;
        }

        #region ISaveable
        public SaveData toSaveData()
        {
            UniverseTimeData data = new UniverseTimeData();
            data.time = this.time;
            return data;
        }
        public void fromSaveData(SaveData saveData)
        {
            UniverseTimeData data = (UniverseTimeData)saveData;
            this.time = data.time;
        }
        public ID Save()
        {
            SaveManager.Instance.Save(this, id);
            return id;
        }
        #endregion

        #region 事件!
        public void OnPauseGame(GameEvent type, Component comp, object pram = null)
        {
            UpdateTimeOnce();
            StopCoroutine(UpdateTime());
        }
        public void OnUnPauseGame(GameEvent type, Component comp, object pram = null)
        {
            prevUnityTime = UnityEngine.Time.time;
            StartCoroutine(UpdateTime());
        }
        #endregion
    }
}