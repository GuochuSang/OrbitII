/// <summary>
/// 注意 : 
/// 请将每一个场景下的UI Canvas 命名为 MainCanvas
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Universe;

namespace Manager
{
    public class UIManager : MonoSingleton<UIManager> 
    {
        #region Prefab Resources to Load
        // 要生成 ModelCanvas, 还要找到主Canvas
        // 要生成两个Camera, 分别绑定到两个Canvas
        public GameObject uiCameraPrefab;
        public GameObject modelCameraPrefab;
        public GameObject modelCanvasPrefab;
        // 消息框
        public GameObject messageBoxPrefab;
        #endregion

        public Camera uiCamera;
        public Camera modelCamera;
        public Transform modelCanvas;
        public Transform mainCanvas;// "Main Canvas"

        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            string loadPath = "UIManager/";
            uiCameraPrefab = Resources.Load<GameObject>(loadPath+"UICamera");
            modelCameraPrefab = Resources.Load<GameObject>(loadPath+"ModelCamera");
            modelCanvasPrefab = Resources.Load<GameObject>(loadPath+"ModelCanvas");
            messageBoxPrefab = Resources.Load<GameObject>(loadPath+"MessageBox");
        }
        // 新场景被加载, 建立模态对话框环境
        public void OnSceneLoaded(Scene scene,LoadSceneMode mode)
        {
            // 查找或建立实例
            mainCanvas = GameObject.Find("MainCanvas").transform;
            modelCanvas = Instantiate<GameObject>(modelCanvasPrefab).transform;

            GameObject go = GameObject.Find("CamerasRoot");
            if (go == null)
                go = new GameObject("CamerasRoot");
            uiCamera = Instantiate<GameObject>(uiCameraPrefab,go.transform).GetComponent<Camera>();
            modelCamera = Instantiate<GameObject>(modelCameraPrefab,go.transform).GetComponent<Camera>();

            // 绑定
            mainCanvas.GetComponent<Canvas>().worldCamera = uiCamera;
            modelCanvas.GetComponent<Canvas>().worldCamera = modelCamera;

            // 正常状态下隐藏模态Canvas
            modelCanvas.gameObject.SetActive(false);
        }

        #region Model Window
        GameObject currentModelWindow;// 当前设置的模态窗口
        Transform savedParent;// 记录这个模态窗口之前的parent

        /// <summary>
        /// 一个基础的消息框
        /// 通过返回的RectTransform, 对消息框内容进行定制 
        /// </summary>
        public RectTransform MessageBox(Vector2 size,string title,UnityAction confirmFunc,UnityAction cancelFunc,float blurStrength = -1f)
        {
            RectTransform box = (RectTransform)(Instantiate<GameObject>(messageBoxPrefab).transform);
            SetAsModel(box.gameObject,blurStrength);
            box.localScale = new Vector3(1f,1f,1f);
            box.anchoredPosition3D = Vector3.zero;
            box.anchorMin = new Vector2(0.5f, 0.5f);
            box.anchorMax = new Vector2(0.5f, 0.5f);
            box.sizeDelta = size;
            Button confirm = box.Find("Confirm").GetComponent<Button>();
            Button cancel = box.Find("Cancel").GetComponent<Button>();
            confirm.onClick.AddListener(confirmFunc);
            cancel.onClick.AddListener(cancelFunc);
            Text titleText = box.Find("Title").GetComponent<Text>();
            titleText.text = title;
            return box;
        }
        /// <summary>
        /// 将游戏对象变成model状态
        /// </summary>
        public void SetAsModel(GameObject go,float blurStrength = -1f)
        {
            if (blurStrength > 1f)
                uiCamera.GetComponent<CameraBlur>().StartBlur(blurStrength);
            currentModelWindow = go;
            savedParent = go.transform.parent;

            modelCanvas.gameObject.SetActive(true);
            go.transform.SetParent(modelCanvas.transform);
            go.transform.SetAsLastSibling();
            HighlightSelectFirstChild(go);
        }
        /// <summary>
        /// 无论是建立MessageBox还是SetAsModel, 最后都需要CloseModel
        /// </summary>
        public void CloseModel()
        {
            uiCamera.GetComponent<CameraBlur>().StopBlur();
            if (savedParent != null)
                currentModelWindow.transform.SetParent(savedParent);
            else
                Destroy(currentModelWindow);// 如果没有parent, 说明他是临时新建的..
            currentModelWindow = null;

            savedParent = null;
            modelCanvas.gameObject.SetActive(false);
        }
        #endregion

        #region 一些基础功能
        // 高亮选择第一个
        public void HighlightSelectFirstChild(GameObject onePanel)
        {
            GameObject go = FindFirstEnabledSelectable(onePanel);
            SetSelected(go);
        }
        public void SetSelected(GameObject go)
        {
            Debug.Assert(EventSystem.current != null);
            EventSystem.current.SetSelectedGameObject(go);
        }  
        GameObject FindFirstEnabledSelectable (GameObject gameObject)
        {
            GameObject go = null;
            var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
            foreach (var selectable in selectables) 
            {
                if (selectable.IsActive () && selectable.IsInteractable ()) 
                {
                    go = selectable.gameObject;
                    break;
                }
            }
            return go;
        }
        #endregion
    }
}