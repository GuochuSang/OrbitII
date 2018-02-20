using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SameKeyEventTrigger : MonoBehaviour
{
    public enum KeyEvent
    {
        Down, Up, Hold
    }
    public KeyCode key;
    public KeyEvent keyEvent;
    public bool isOpen = false;
    [SerializeField]
    public UnityEvent keyOpenEvent;
    [SerializeField]
    public UnityEvent keyCloseEvent;
        
    void Update()
    {
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