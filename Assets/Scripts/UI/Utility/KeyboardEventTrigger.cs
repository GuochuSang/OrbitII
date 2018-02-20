using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class KeyboardEventTrigger : MonoBehaviour
{
    public enum KeyEvent
    {
        Down, Up, Hold
    }
    public KeyCode key;
    public KeyEvent keyEvent;

    [SerializeField]
    public UnityEvent KeyboardEvent;

        
    void Update()
    {
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