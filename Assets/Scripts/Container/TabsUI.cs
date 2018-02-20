using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Universe 
{
/// <summary>
/// 背包这样的, 可以切换页面的UI的脚本, 需要继承这个接口
/// 使用UIManager打开/关闭 这个背包
/// </summary>
public interface ITabsUI
{
    void ShowPage(int page);
    void CloseTabsUI();
}

public class TabsUI : MonoBehaviour
{
    const int MAX_TABS = 9;
    const string RES_FOLDER = "UI\\TabsUI";
    const string BUTTONS_NAME = "Tab";

    public Transform buttonsRoot;

    // 所有按钮, 按钮名为 Tab+序号 , 每次重新加载
    List<Button> pageButtons;

    //当前绑定的对象
    ITabsUI tabsUI;
    int pageCount = 0;
    int currentPage = 1; 

    public Transform SetTabsUI(ITabsUI tabsUI,int pageCount, int width, int length)
    {
        for (int i = 1; i <= MAX_TABS; i++)
        {
            RectTransform buttonTrans = (RectTransform)buttonsRoot.Find(BUTTONS_NAME + i.ToString());
            if (i <= pageCount)
            {
                buttonTrans.gameObject.SetActive(true);
                buttonTrans.sizeDelta = new Vector2(50f, 34.375f);
                buttonTrans.anchoredPosition = new Vector2(25f+3.125f*i+50*(i-1),-14.0625f);
            }
            else
                buttonTrans.gameObject.SetActive(false);
        }
        this.tabsUI = tabsUI;
        this.pageCount = pageCount;
        ((RectTransform)transform).sizeDelta = new Vector2(width, length);
        gameObject.SetActive(true);

        GameObject go = new GameObject("TempRoot");
        go.transform.SetParent(this.transform);
        RectTransform rec = go.AddComponent<RectTransform>();
        rec.anchorMin = new Vector2(0,1);
        rec.anchorMax = new Vector2(0, 1);
        rec.sizeDelta = Vector2.zero;
        rec.anchoredPosition = Vector2.zero;
        return go.transform;
    }

    public void CloseTabsUI()
    {
        tabsUI.CloseTabsUI();
        gameObject.SetActive(false);
    }

    public void Next()
    {
        ShowPage(currentPage + 1);
    }
    public void Prev()
    {
        ShowPage(currentPage - 1);
    }
    public void ShowPage(int page)
    {
        if (page == 0)
            page = pageCount;
        else if (page == pageCount+1)
            page = 1;
        currentPage = page;
        tabsUI.ShowPage(page);
        EventSystem.current.SetSelectedGameObject(buttonsRoot.Find(BUTTONS_NAME+page.ToString()).gameObject);
    }
}
}