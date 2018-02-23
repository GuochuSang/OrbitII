using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using UnityEngine.UI;

namespace Universe
{
    public class NewBuildingPanel : MonoBehaviour , ITabsUI
    {
        public const int BUILDINGS_COUNT_PER_PAGE = 9;
        [System.Serializable]
        public struct Building
        {
            public BuildingType building;
            public Sprite buildingSprite;
            public GameObject buildingObejct;
            public string detail;
        }
        #region 提前绑定
        public MenuManager itsMenuManager;
        public GameObject oneMoreClickToBuild;
        public RectTransform preParent;
        // 提前做好的九个Button
        public List<GameObject> buildingButtons;
        public RectTransform gridLayoutGroup;
        public Text showMessageText;
        // 请手动装填列表
        public List<Building> buildingsList = new List<Building>();
        #endregion

        // 当前显示的, 9个GameObject对应九幅图, 只改变数值
        Dictionary<GameObject,Image> buttonsToImages = new Dictionary<GameObject, Image>(); 

        #region 每次打开时需要更新的数据
        // 当前显示的, 九个对象分别对应的Building
        Dictionary<GameObject,Building> currentButtonsToBuildings;
        // 从 TabsUI.Instance.SetTabsUI() 获得的展示界面的页面
        RectTransform contentRoot;
        GameObject currentChoosed = null;
        #endregion

        void Awake()
        {
            foreach (var obj in buildingButtons)
            {
                buttonsToImages.Add(obj,obj.GetComponent<Image>());
            }
        }

        void OnEnable()
        {
            int pageCount = Mathf.CeilToInt(buildingsList.Count / (float)BUILDINGS_COUNT_PER_PAGE);
            contentRoot = TabsUI.Instance.SetTabsUI(this,pageCount);
            oneMoreClickToBuild.SetActive(false);
            this.transform.SetParent(contentRoot); 
            TabsUI.Instance.ShowPage(1);
        }

        #region ITabsUI
        /// <summary>
        /// 在这里显示每一页的内容
        /// </summary>
        public void ShowPage(int page)
        {
            currentButtonsToBuildings = new Dictionary<GameObject, Building>();
            int firstIndex = (page-1) * BUILDINGS_COUNT_PER_PAGE;
            int lastIndex = Mathf.Clamp(firstIndex + BUILDINGS_COUNT_PER_PAGE, 0, buildingsList.Count);
            for (int i = firstIndex; i < firstIndex+BUILDINGS_COUNT_PER_PAGE; i++)
            {
                int index = i - firstIndex;// 0-8
                if (i < lastIndex)
                {
                    buildingButtons[index].SetActive(true);
                    buttonsToImages[buildingButtons[index]].sprite = buildingsList[i].buildingSprite;
                    currentButtonsToBuildings.Add(buildingButtons[index], buildingsList[i]);
                }
                else
                {
                    buildingButtons[index].SetActive(false);
                }
            }
        }
        /// <summary>
        /// 在这里清空各种内容
        /// </summary>
        public void CloseTabsUI()
        {
            currentButtonsToBuildings = null;
            contentRoot = null;
            currentChoosed = null;
            this.transform.SetParent(preParent);
        }
        #endregion

        #region 建造/显示功能
        public void ShowBuildingMessage(GameObject obj)
        {
            Building b = currentButtonsToBuildings[obj];
            showMessageText.text = b.building + "\n"+ b.detail;
        }
        public void ClickBuildingButton(GameObject obj)
        {
            oneMoreClickToBuild.SetActive(true);
            if (currentChoosed == null)
            {
                currentChoosed = obj;
                ShowBuildingMessage(obj);
            }
            else if (currentChoosed.Equals(obj))
            {
                PlanetUI.Instance.New(currentButtonsToBuildings[obj].building);
                TabsUI.Instance.CloseTabsUI();
                itsMenuManager.ExitPanel(this.gameObject);
            }
            else
            {
                currentChoosed = obj;
                ShowBuildingMessage(obj);
            }
        }
        #endregion 
    }   
}