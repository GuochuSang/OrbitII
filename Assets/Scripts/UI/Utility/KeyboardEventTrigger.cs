using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Manager;

namespace Universe
{
    public enum KeyEvent
    {
        Down, Up, Hold
    }
    public class KeyboardEventTrigger : MonoBehaviour
    {
        // 是否能禁用输入? (事件依然要注册, 但不一定执行, 因此可以动态改变这个变量)
        public bool isInputCanBeStoped = true;
        public KeyCode key;
        public KeyEvent keyEvent;
        [SerializeField]
        bool isStoped = false;

        [SerializeField]
        public UnityEvent KeyboardEvent;

        #region 处理暂停输入的事件
        void Awake()
        {
            Manager.EventManager.Instance.AddListener(this, Manager.GameEvent.STOP_INPUT, OnStopInput);
            Manager.EventManager.Instance.AddListener(this, Manager.GameEvent.RESTART_INPUT, OnRestartInput);
        }
        void OnDestroy()
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
                        KeyboardEvent.Invoke();
                    break;
                case KeyEvent.Up:
                    if (Input.GetKeyUp(key))
                        KeyboardEvent.Invoke();
                    break;
                case KeyEvent.Hold:
                    if (Input.GetKey(key))
                        KeyboardEvent.Invoke();
                    break;
            }
        }
        }
}