using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Manager;

public class RecordManager : MonoBehaviour 
{
    // 生成Record列表的Canvas
    public Transform recordViewCanvas;
    public GameObject deleteButton;
    public GameObject enterButton;
    public Scrollbar scrollBar;

    public GameObject newRecordWindowPrefab;
    public GameObject recordUIPrefab;
    public Sprite choosedSprite;
    public Sprite unChoosedSprite;

    Dictionary<GameObject,Record> currentRecords;// 列出当前生成的存档
    List<Record> sortedRecords; // 排好序的存档
    GameObject choosedGameObject;// 当前选择的存档
    GameObject ChoosedGameObject
    {
        get
        { 
            return choosedGameObject;
        }
        set 
        { 
            choosedGameObject = value;
            if (value == null)
            {
                deleteButton.SetActive(false);
                enterButton.SetActive(false);
            }
            else
            {
                deleteButton.SetActive(true);
                enterButton.SetActive(true);
            }
        }
    }
    GameObject newRecordWindow;
    public float smoothScrollSpeed = 0.1f;
    float scrollValueTarget = 1f;

    void OnEnable()
    {
        ShowAllRecord();
    }

    void Update()
    {
        SelectRecord();
    }
    public void ShowAllRecord()
    {
        Debug.Assert(recordViewCanvas != null);
        ClearRecordsView();
        List<Record> records = Record.GetRecords();
        if (records == null || records.Count == 0)
        {
            ChoosedGameObject = null;
            return;
        }
        records.Sort();
        currentRecords = new Dictionary<GameObject, Record>();
        sortedRecords = records;
        for(int i=records.Count-1;i>=0;i--)
        {
            Record r = records[i];
            GameObject go = Instantiate<GameObject>(recordUIPrefab,recordViewCanvas);
            currentRecords.Add(go, r);

            Text[] texts= go.GetComponentsInChildren<Text>();
            foreach (var t in texts)
            {
                if (t.gameObject.name.Equals("Name"))
                    t.text = r.name;
                else if (t.gameObject.name.Equals("CreateTime"))
                    t.text = "Create Time : \n"+r.createTime;
                else if (t.gameObject.name.Equals("UpdateTime"))
                    t.text = "Update Time : \n"+r.updateTime;
            }
        }
        UIManager.Instance.HighlightSelectFirstChild(recordViewCanvas.gameObject);
    }

    // 检测当前 EventSystem 的 Select, 如果是 Record, 就选择它
    void SelectRecord()
    {
        if (currentRecords == null || currentRecords.Count == 0)
            return;
        GameObject go = EventSystem.current.currentSelectedGameObject;
        if (go == null)
            return;
        if (currentRecords.ContainsKey(go))
            if(!go.Equals(ChoosedGameObject))
                Choose(go); 
    }
    // 选中某个Record
    void Choose(GameObject go)
    {
        // 1. 更换Shader
        if (ChoosedGameObject != null)
        {
            Image image = ChoosedGameObject.transform.Find("BackImage").GetComponent<Image>();
            image.sprite = unChoosedSprite;
        }
        ChoosedGameObject = go;
        Image curImage = go.transform.Find("BackImage").GetComponent<Image>();
        curImage.sprite = choosedSprite;

        ScrollToCurrent(go);
    }
    // 调整scroll bar
    void ScrollToCurrent(GameObject go)
    {
        Record cur = currentRecords[go];
        int recordIndex = sortedRecords.IndexOf(cur);
        float scrollValue = (float)recordIndex/(sortedRecords.Count-1f+0.0001f);
        StartCoroutine(SmoothScroll(scrollValue));
    }

    IEnumerator SmoothScroll(float targetValue)
    {
        targetValue = Mathf.Clamp01(targetValue);
        scrollValueTarget = targetValue;
        while ((Mathf.Abs(scrollBar.value - targetValue)>0.001f)
            && (Mathf.Abs(scrollValueTarget - targetValue) < 0.001f))
        {
            scrollBar.value = Mathf.Lerp(scrollBar.value, targetValue, smoothScrollSpeed);
            yield return null;
        }
    }

    public void NewRecord()
    {
        RectTransform box = UIManager.Instance.MessageBox(new Vector2(500,300),"新建存档",ConfirmNewRecord,CancelNewRecord,4f);
        newRecordWindow = Instantiate<GameObject>(newRecordWindowPrefab, box);
        newRecordWindow.transform.Find("WarningText").gameObject.SetActive(false);
    }
    void ConfirmNewRecord()
    {
        Debug.Assert(newRecordWindow != null,"没有newRecordWindow");
        Text[] allTexts = newRecordWindow.GetComponentsInChildren<Text>();
        Text recordName = null;
        Text seedText = null;
        foreach (Text t in allTexts)
        {
            if (t.gameObject.name.Equals("NameText"))
                recordName = t;
            else if (t.gameObject.name.Equals("SeedText"))
                seedText = t;
        }
        Debug.Assert(recordName != null,"没有找到Name");
        Debug.Assert(seedText != null,"没有找到Seed");

        Record newReco = new Record();
        if (newReco.SetName(recordName.text) != null)
        {
            // 地图种子为
            string seedString = seedText.text+"";
            if (seedString.Equals(""))
                seedString = System.DateTime.Now.ToString();
            newReco.seed = seedString.GetHashCode();
            UIManager.Instance.CloseModel();
            newReco.SaveInfo();
            SaveManager.Instance.EnterRecord(newReco);
            return;
        }
        else
        {
            newRecordWindow.transform.Find("WarningText").gameObject.SetActive(true);
        }
    }
    void CancelNewRecord()
    {
        UIManager.Instance.CloseModel();
    }

    public void DeleteRecord()
    {
        Debug.Assert(ChoosedGameObject != null);
        UIManager.Instance.MessageBox(new Vector2(500,300),"你确定要删除这个存档吗?",ConfirmDelete,CancelDelete,5f);
    }
    void ConfirmDelete()
    {
        UIManager.Instance.CloseModel();
        currentRecords[ChoosedGameObject].Delete();
        ShowAllRecord();
    }
    void CancelDelete()
    {
        UIManager.Instance.CloseModel();
        UIManager.Instance.HighlightSelectFirstChild(recordViewCanvas.gameObject);
    }

    // 进入存档, 由SaveManager来发送 "EnterRecord"事件
    public void EnterRecord()
    {
        Debug.Assert(ChoosedGameObject != null);
        SaveManager.Instance.EnterRecord(currentRecords[ChoosedGameObject]);
    }

    void ClearRecordsView()
    {
        ChoosedGameObject = null;
        if (currentRecords == null)
            return;
        foreach (var kv in currentRecords)
            Destroy(kv.Key);
        currentRecords = null;
    }
}
