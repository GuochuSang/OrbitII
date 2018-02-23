using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

using Universe;

public class StupidShip : MonoBehaviour 
{
    public float shipFlySpeed;
    bool stoped = false;

    void Start()
    {
        CameraController.Instance.LookAt(transform);
    }
    void Awake()
    {
        EventManager.Instance.AddListener(this, GameEvent.STOP_INPUT, OnStopInput);
        EventManager.Instance.AddListener(this, GameEvent.RESTART_INPUT, OnRestartInput);
    }
    void OnDestroy()
    {
        EventManager.Instance.RemoveObjectEvent(this, GameEvent.STOP_INPUT);
        EventManager.Instance.RemoveObjectEvent(this, GameEvent.RESTART_INPUT);
    }
    void OnRestartInput(GameEvent gameEvent,Component comp, object param = null)
    {
        stoped = false;
    }
    void OnStopInput(GameEvent gameEvent,Component comp, object param = null)
    {
        stoped = true;
    }

	void Update () 
    {
        if (stoped)
            return;
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(hor, ver, 0);
        if (direction.magnitude > 0.5f)
        {
            Vector3 rot = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(new Vector3(0, 1, 0), direction), 0.2f).eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0,0,rot.z));
        }
        transform.position += direction * shipFlySpeed * Time.deltaTime;
        //transform.Translate(direction * shipFlySpeed * Time.deltaTime);

        float deltaScroll = Input.GetAxis("Mouse ScrollWheel");
        if (!deltaScroll.Equals(0f))
        {
            CameraController.Instance.DefaultFreeSize -= Input.GetAxis("Mouse ScrollWheel") * 20;
            CameraController.Instance.SetSize(CameraController.Instance.DefaultFreeSize);
        }
    }
}

