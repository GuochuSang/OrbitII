using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
    public class BlackHole : MonoBehaviour,IChunkObject,ISaveable 
    {
        ID id;
        Sprite sprite;

        void Awake()
        {
            id = new ID();
            id.idx = (int)transform.position.x;
            id.idy = (int)transform.position.y;
            id.className = "BlackHole";
            id.sceneName = "Universe";
            id.Init();
        }
        void Start()
        {
            sprite = Resources.Load<Sprite>("Universe/blackHole");

            SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = this.sprite;
            sr.color = Color.red;
            sr.sortingLayerName = "Planet";

        }
        void OnDestroy()
        {
            id.OnDestroy();
        }

        #region IChunkObject
        public Vector2 GetPosition()
        {
            return (Vector2)this.transform.position;
        }
        public ID Save()
        {
            Manager.SaveManager.Instance.Save(this, id);
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
        }
        #endregion
        #region ISaveable
        public SaveData toSaveData()
        {
            BlackHoleSaveData data = new BlackHoleSaveData();
            return data;
        }
        public void fromSaveData(SaveData saveData)
        {
            
        }
        #endregion
    }
    [System.Serializable]
    public class BlackHoleSaveData : CreatableSaveData
    {
        public override T Create<T>(ID id)
        {
            id.Init();
            return (T)(System.Object)(new BlackHole());
        }

    }
}