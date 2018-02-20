using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Manager;

public class MenuManager : MonoBehaviour 
{
	#region UI的Animator相关字符串常量
    public const string ANIM_CLOSED = "Closed";
    public const string ANIM_CLOSING = "Closing";
    public const string ANIM_OPEN = "Open";
	public const string ANIM_BOOL_OPEN = "Open";
	#endregion

    public enum ManagerState{OPEN, CLOSING, CLOSED};
    ManagerState state = ManagerState.CLOSED;
    public ManagerState State
    {
        get
        { 
            return state;
        }
        private set{ }
    }

    // 子物体始终显示
    public GameObject window;
    // 子物体只显示一个
    public GameObject panels;
    // 默认打开某个面板
    public GameObject defalutPanel;

    public enum ChildType{NORMAL, ANIMATOR, MANAGER};
    [System.Serializable]
    public class Child
    {
        public Child(GameObject obj, Component comp, ChildType type)
        {
            this.gameObject = obj;
            this.component = comp;
            this.type = type;
        }
        public ChildType type;
        public GameObject gameObject;
        public Component component;
    }
    public List<Child> windowChildren; 
    public List<Child> panelChildren;

    [SerializeField]
    Dictionary<GameObject, Child> windowChilds;
    [SerializeField]
    Dictionary<GameObject, Child> panelsChilds;

    public GameObject currentSelected;// 当前的选择 (windowChild)
    public GameObject currentPanel;// 当前打开的面板的对象 (panelsChild)

    bool initial = false;
   
   
    #region Manager生命周期
	void Initial()
	{
		DebugColoredLog(gameObject.name + " : Initial","green");
		if (GetComponent<Animator>())
			DebugColoredLog(this.gameObject.name+"这玩意不应该有Animator","red");
		ActivateRoot(window, ref windowChilds);
		ActivateRoot(panels, ref panelsChilds);


		windowChildren = new List<Child>();
		panelChildren = new List<Child>();
		foreach (KeyValuePair<GameObject,Child> objChild in windowChilds)
		{
			windowChildren.Add(objChild.Value);
		}
		foreach (KeyValuePair<GameObject,Child> objChild in panelsChilds)
		{
			panelChildren.Add(objChild.Value);
		}
	}
	void ActivateRoot(GameObject dad, ref Dictionary<GameObject, Child> childsOfDad)
    {
        childsOfDad = new Dictionary<GameObject, Child>();
        if (dad == null)
            return;
        dad.SetActive(true);



        foreach (Transform child in dad.transform)
        {
            Component comp = null;
            if (comp = child.GetComponent<MenuManager>())
            {
                childsOfDad.Add(child.gameObject, new Child(child.gameObject, comp,ChildType.MANAGER));
            }
            else if (comp = child.GetComponent<Animator>())
            {
                childsOfDad.Add(child.gameObject, new Child(child.gameObject, comp,ChildType.ANIMATOR));
            }
            else
            {
                childsOfDad.Add(child.gameObject, new Child(child.gameObject,child,ChildType.NORMAL));
            }
        }




    }
	// 令所有Child恢复到正常的状态(该打开的打开, 该关闭的关闭)

    void Awake()
    {
        if (!initial)
        {
            Initial();
            initial = true;
        }        
    }
	void OnEnable()
	{
		OpenManager ();
	}
	void OnDisable()
	{
		state = ManagerState.CLOSED;

	}


	void OpenManager()
	{
		DebugColoredLog(gameObject.name + " : OpenManager","green");

		if(state == ManagerState.CLOSING)
			StopCoroutine (ClosingManager ());
		state = ManagerState.OPEN;

		OpenWindow();
		ClosePanel ();
		OpenDefaultPanel ();

	}
	public void CloseManager()
	{
        if(gameObject.activeInHierarchy)
		    StartCoroutine(ClosingManager());   
	}
	IEnumerator ClosingManager()
	{
		state = ManagerState.CLOSING;
		bool closeCompleted = false;
		// 开始关闭
		foreach(KeyValuePair<GameObject,Child> objChild in windowChilds)
			CloseChild(objChild.Value);
		foreach(KeyValuePair<GameObject,Child> objChild in panelsChilds)
			CloseChild(objChild.Value);
		// 检查是否全部关闭
		while (!closeCompleted)
		{
			closeCompleted = true;
			foreach (KeyValuePair<GameObject,Child> objChild in windowChilds)
			{
				if (!IsChildClosed(objChild.Value))
				{
					closeCompleted = false;
					break;
				}
			}

			foreach (KeyValuePair<GameObject,Child> objChild in panelsChilds)
			{
				if (!closeCompleted)
					break;
				if (!IsChildClosed(objChild.Value))
				{
					closeCompleted = false;
					break;
				}
			}
			yield return new WaitForEndOfFrame();
		}
		state = ManagerState.CLOSED;
		this.gameObject.SetActive(false);
	}
	#endregion


    public void OpenPanel (GameObject onePanel)
    {
        Child child;
        // 只有当这个要打开的Panel属于Manager管理的Panel, 才可以打开
        if (!panelsChilds.TryGetValue(onePanel, out child))
            return;
        // 不重复打开
        if (currentPanel == onePanel)
            return;
        currentPanel = onePanel;
		OpenChild (child);
		UIManager.Instance.HighlightSelectFirstChild(onePanel);
        CloseBrothers(onePanel);
    }
    // 进入一个Panel, 禁用window以及其他Panel
    public void EnterPanel(GameObject onePanel)
    {
        OpenPanel(onePanel);
        CloseWindow();
        currentPanel = onePanel;
    }
    // 退出一个Panel, 打开window
    public void ExitPanel(GameObject onePanel)
    {
        CloseChild(panelsChilds[onePanel]);
        OpenWindow();
        currentPanel = null;
    }
    
	#region 私有方法....
	void OpenWindow()
    {
		foreach (KeyValuePair<GameObject,Child> objChild in windowChilds)
			OpenChild (objChild.Value);
		// 打开窗口时, 恢复上次选择的按钮..
		if (currentSelected)
			UIManager.Instance.SetSelected (currentSelected);
        else UIManager.Instance.HighlightSelectFirstChild(window);
    }
    void CloseWindow()
    {
		// 关闭窗口时, 记录当前选择的对象
		GameObject go = EventSystem.current.currentSelectedGameObject;
		if (go && windowChilds.ContainsKey (go))
			currentSelected = go;
        foreach (KeyValuePair<GameObject,Child> objChild in windowChilds)
            CloseChild(objChild.Value);
    }

	void ClosePanel()
	{
		foreach (KeyValuePair<GameObject,Child> objChild in panelsChilds)
			CloseChild (objChild.Value);
		currentPanel = null;
	}
	void OpenDefaultPanel()
	{
		if (defalutPanel == null)
			return;
		Child child;
		if (panelsChilds.TryGetValue(defalutPanel, out child))
			OpenChild (child);
		else
			DebugColoredLog("Not A Panel","red");
		currentPanel = defalutPanel;
	}
    // 关闭某个Panel的兄弟
    void CloseBrothers(GameObject onePanel)
    {
        foreach(KeyValuePair<GameObject,Child> objChild in panelsChilds)
        {
            if (objChild.Key.Equals(onePanel))
                continue;
            CloseChild(objChild.Value);
        }
    }

	void OpenChild(Child child)
	{
		if (!(IsChildClosed (child) || IsChildClosing (child)))
			return;
		child.gameObject.SetActive(true);
		if (child.type == ChildType.ANIMATOR)
			((Animator)child.component).SetBool(ANIM_BOOL_OPEN, true);
		else if (child.type == ChildType.MANAGER)
			((MenuManager)child.component).OpenManager ();
		// 将Animator所在的对象设置为同级最后物体 (显示在最前面)
		child.gameObject.transform.SetAsLastSibling();
	}
	void CloseChild(Child child)
    {
		if (IsChildClosed(child))
            return;
        if (child.type == ChildType.ANIMATOR)
        {
			if (IsChildClosing (child))
				return;
			((Animator)child.component).SetBool(ANIM_BOOL_OPEN, false);
            StartCoroutine(DisablePanelDeleyed((Animator)child.component));
        }
        else if (child.type == ChildType.MANAGER)
        {
			if (IsChildClosing (child))
				return;
            ((MenuManager)child.component).CloseManager();
        }
        else
            child.gameObject.SetActive(false);
    }

    // 检查 Child 是否关闭
    bool IsChildClosed(Child child)
    {
        if (!child.gameObject.activeInHierarchy)
            return true;
		if (child.type == ChildType.ANIMATOR) 
		{
			Animator anim = (Animator)child.component;
			if (!anim.IsInTransition (0))
				return anim.GetCurrentAnimatorStateInfo (0).IsName (ANIM_CLOSED) && !anim.GetBool (ANIM_BOOL_OPEN);
			return false;
		} 
		else if (child.type == ChildType.MANAGER)
			return ((MenuManager)child.component).state == ManagerState.CLOSED;
		else
			return false;
    }
    bool IsChildClosing(Child child)
    {
		if (!child.gameObject.activeInHierarchy)
			return false;
		if (child.type == ChildType.ANIMATOR)
		{
			Animator anim = (Animator)child.component;
			if (anim.IsInTransition(0))
				return !anim.GetBool (ANIM_BOOL_OPEN);
			else 
				return  !anim.GetBool (ANIM_BOOL_OPEN) && anim.GetCurrentAnimatorStateInfo (0).IsName (ANIM_CLOSING);
		}
		else if (child.type == ChildType.MANAGER)
			return ((MenuManager)child.component).state == ManagerState.CLOSING;
		return false;
    }

    // 带 Animator 的物体的关闭动画， 如果检测到 Open true, 就自己退出协程
    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            // 如果没有播放动画, 那么检查当前的状态是否为"关闭"
            if (!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(ANIM_CLOSED);

            wantToClose = !anim.GetBool(ANIM_BOOL_OPEN);//如果Animator 中 Open 参数为 False, 那就隐藏当前物体

            yield return new WaitForEndOfFrame();
        }
        if (wantToClose)
            anim.gameObject.SetActive(false);
    }
        
    void DebugColoredLog(string text, string color)
    {
        Debug.Log("<color="+color+">"+text+"</color>");
    }
	#endregion
}


