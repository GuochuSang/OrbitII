using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manager;

namespace Universe 
{
    public class ShowContainer : MonoSingleton<ShowContainer>, ITabsUI
    {
        public int gapSize = 10;// 相邻俩的间隔
        public int gridSize = 40;// 格子的大小
        public int evenLineDeltaX = 20;// 偶数行位移
        const int xGrid = 5;
        const int yGrid = 5;

        public GameObject uiTextPrefab;
        public GameObject uiGridPrefab;

        public int buttonSize = 20;

        RectTransform[,] grids;
        Text[,] gridsText;
        // 添加内容的区域, 从tabsUI获得
        RectTransform tabsUICanvas;

        Container currentBox;
        Vector2Int currentSize;

        // 打开背包调用一次
        public void Show(Container box)
        {
            currentBox = box;
            currentSize = new Vector2Int(xGrid, yGrid);
            grids = new RectTransform[xGrid, yGrid];
            gridsText = new Text[xGrid, yGrid];

            //int width = xGrid*gridSize + (xGrid+1)*gapSize;
            //int length = yGrid*gridSize + (yGrid+1)*gapSize;

            //int pageCount = Mathf.CeilToInt((float)box.GetAllItemsCount() / (float)(xGrid * yGrid));
            int pageCount = Mathf.CeilToInt((float)box.GridCount / (float)(xGrid * yGrid));


            tabsUICanvas = TabsUI.Instance.SetTabsUI(this, pageCount);

            for (int j = 0; j < yGrid; j++)
                for (int i = 0; i < xGrid; i++)
                {
                    grids[i, j] = (Instantiate(uiGridPrefab, tabsUICanvas) as GameObject).GetComponent<RectTransform>();
                    // 偶数行X坐标不同
                    grids[i, j].anchoredPosition = new Vector2( j%2*evenLineDeltaX + (i+1)*gapSize+i*gridSize + gridSize/2,-(buttonSize+(j+1)*gapSize+j*gridSize+gridSize/2));

                    GameObject go = Instantiate(uiTextPrefab,grids[i, j]) as GameObject;
                    gridsText[i,j] = go.GetComponent<Text>();
                }

            TabsUI.Instance.ShowPage(1);
            StartCoroutine(UpdatePage());
        }
        // 刷新Page
        IEnumerator UpdatePage()
        {
            while (currentBox!=null)
            {
                ShowPage(TabsUI.Instance.currentPage);
                yield return new WaitForFixedUpdate();
            }
        }
        // 自己通常不调用以下两个函数
        public void ShowPage(int page)
        {
            InitPage();

            // 当前需要显示的物品是第几个格子
            int pageIndex = 1;
            // 当前页需要显示的第一个格子是整体的第几个格子
            int neededIndex = (page-1) * currentSize.x * currentSize.y + 1;
            var firstItem = FindFirstItem(neededIndex);
            foreach (var kv in currentBox.Items)
            {
                if (pageIndex <= 1 && !kv.Key.Equals(firstItem.Key))
                    continue;
                if (pageIndex <= 1 && kv.Key.Equals(firstItem.Key))
                    pageIndex = ShowItemKeyValue(firstItem, pageIndex);
                else pageIndex = ShowItemKeyValue(kv, pageIndex);
                if (pageIndex > currentSize.x * currentSize.y)
                    break;
            }
        }
        public void CloseTabsUI()
        {
            for (int j = 0; j < yGrid; j++)
                for (int i = 0; i < xGrid; i++)
                {
                    Destroy(grids[i, j].gameObject);
                }
            //for (int i = 0; i < currentSize.x; i++)
            //    for (int j = 0; j < currentSize.y; j++)
            //    {
            //        Destroy(grids[i, j].gameObject);
            //    }
            grids = null;
            gridsText = null;
            tabsUICanvas = null;
            currentBox = null;
            currentSize = Vector2Int.zero;
        }

        // 开始时隐藏所有grid
        void InitPage()
        {
            for(int i=0;i<currentSize.x;i++)
                for(int j=0;j<currentSize.y;j++)
                {
                    grids[i, j].gameObject.SetActive(false);
                }
        }
        // 找到第一个应该显示的item, 以及应该显示的量
        KeyValuePair<string,float> FindFirstItem(int neededIndex)
        {
            int allGridIndex = 0;
            var curItems = currentBox.Items;
            // 显示背包, 先要找到开始显示的位置
            foreach (var kv in curItems)
            {
                int itemCount = currentBox.GetItemCount(kv.Key);
                allGridIndex += itemCount;
                if (allGridIndex >= neededIndex)
                {
                    string key = kv.Key;
                    // 需要显示的值 = 所有值 - 上一页显示过的值
                    float value = kv.Value - (itemCount - (allGridIndex - neededIndex + 1)) * Container.GRID_SIZE;
                    return new KeyValuePair<string, float>(key,value);
                }
            }
            return new KeyValuePair<string, float>("",0f);
        }
        // 返回最后的pageIndex
        int ShowItemKeyValue(KeyValuePair<string,float> item, int pageIndex)
        {
            float amount = item.Value;
            while (amount >= Container.GRID_SIZE)
            {
                ShowItem(item.Key, pageIndex, Container.GRID_SIZE);
                amount -= Container.GRID_SIZE;
                pageIndex++;
                if (pageIndex > currentSize.x * currentSize.y)
                    return pageIndex;
            }
            if (!amount.Equals(0f))
            {
                ShowItem(item.Key, pageIndex, amount);
                pageIndex++;
            }
            return pageIndex;
        }
        // 在一页的特定位置显示某个Item
        void ShowItem(string item, int pageIndex, float amount)
        {
            pageIndex -= 1;
            int xPos = pageIndex % currentSize.x;
            int yPos = pageIndex / currentSize.x;
            grids[xPos, yPos].gameObject.SetActive(true);
            gridsText[xPos,yPos].text = item + "\n" + amount.ToString();
        }
    }
}