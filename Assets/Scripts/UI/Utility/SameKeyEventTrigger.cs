using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Manager;

namespace Universe
{
    /// <summary>
    /// 当一个按键按下两下, 需要执行打开和关闭两个操作
    /// </summary>
    public class SameKeyEventTrigger : MonoBehaviour
    {
        // 是否能禁用输入? (事件依然要注册, 但不一定执行, 因此可以动态改变这个变量)
        public bool isInputCanBeStoped = true;

        public KeyCode key;
        public KeyEvent keyEvent;

        [SerializeField]
        public UnityEvent keyOpenEvent;
        [SerializeField]
        public UnityEvent keyCloseEvent;
        [SerializeField]
        bool isOpen = false;
        [SerializeField]
        bool isStoped = false;

        #region 处理暂停输入的事件
        void OnEnable()
        {
            Manager.EventManager.Instance.AddListener(this, Manager.GameEvent.STOP_INPUT, OnStopInput);
            Manager.EventManager.Instance.AddListener(this, Manager.GameEvent.RESTART_INPUT, OnRestartInput);
        }
        void OnDisable()
        {
            Manager.EventManager.Instance.RemoveObjectEvent(this, Manager.GameEvent.STOP_INPUT);
            Manager.EventManager.Instance.RemoveObjectEvent(this, Manager.GameEvent.RESTART_INPUT);
        }
        void OnStopInput(GameEvent gameEvent,Component comp,object param = null)
        {
            isStoped = true;
        }
        void OnRestartInput(GameEvent gameEvent,Component comp,object param = null)
        {
            isStoped = false;
        }
        #endregion

        void Update()
        {
            if (isStoped && isInputCanBeStoped)
                return;
            switch (keyEvent)
            {
                case KeyEvent.Down:
                    if (Input.GetKeyDown(key))
                        ExcuteEvent();
                    break;
                case KeyEvent.Up:
                    if (Input.GetKeyUp(key))
                        ExcuteEvent();
                    break;
                case KeyEvent.Hold:
                    if (Input.GetKey(key))
                        ExcuteEvent();
                    break;
            }
        }
        void ExcuteEvent()
        {
            isOpen = !isOpen;
            if (isOpen)
                keyOpenEvent.Invoke();
            else
                keyCloseEvent.Invoke();
        }
    }
}