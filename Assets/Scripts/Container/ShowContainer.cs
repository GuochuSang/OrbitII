using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manager;

namespace Universe 
{
public class ShowContainer : MonoBehaviour, ITabsUI
{
    const int gapSize = 10;
    const int gridSize = 40;

    public GameObject uiTextPrefab;
    public GameObject uiGridPrefab;

    public int buttonSize = 20;
    RectTransform[,] grids;
    Text[,] gridsText;

    Container currentBox;
    Vector2Int currentSize;

    void Start()
    {
        ID id;
        Container c = new Container("as", 700, out id);
        c.AddItem("资源A",99f*20.2134f);
        c.AddItem("资源B",99f*45.23f);
        c.AddItem("资源C",99f*10f);
        c.AddItem("资源D",99f*67.534f);
        c.AddItem("资源E",99f*20.2134f);
        c.AddItem("资源F",99f*267.23f);
        c.AddItem("资源G",99f*10f);
        c.AddItem("资源H",99f*164.534f);
        Show(c, 12, 8);
    }

    // 打开背包调用一次
    public void Show(Container box, int xGrid, int yGrid)
    {
        currentBox = box;
        currentSize = new Vector2Int(xGrid, yGrid);
        grids = new RectTransform[xGrid, yGrid];
        gridsText = new Text[xGrid, yGrid];

        int width = xGrid*gridSize + (xGrid+1)*gapSize;
        int length = yGrid*gridSize + (yGrid+1)*gapSize;

        int pageCount = Mathf.CeilToInt((float)box.GetAllItemsCount() / (float)(xGrid * yGrid));

        TabsUI tabsManager = GameObject.Find("TabsUI").GetComponent<TabsUI>();
        Transform uiRoot = tabsManager.SetTabsUI(this, pageCount, width, length + buttonSize);

        for (int j = 0; j < yGrid; j++)
            for (int i = 0; i < xGrid; i++)
            {
                RectTransform gridXY = (Instantiate(uiGridPrefab, uiRoot) as GameObject).GetComponent<RectTransform>();
                grids[i, j] = gridXY;
                gridXY.sizeDelta = new Vector2(gridSize, gridSize);
                gridXY.anchoredPosition = new Vector2((i+1)*gapSize+i*gridSize + gridSize/2,-(buttonSize+(j+1)*gapSize+j*gridSize+gridSize/2));

                GameObject go = Instantiate(uiTextPrefab,gridXY) as GameObject;
                gridsText[i,j] = go.GetComponent<Text>();
            }

        tabsManager.ShowPage(1);
    }
    public void CloseTabsUI()
    {
        for (int i = 0; i < currentSize.x; i++)
            for (int j = 0; j < currentSize.y; j++)
            {
                Destroy(grids[i, j].gameObject);
            }
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