using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

using Universe;

public class StupidShip : MonoBehaviour 
{
    public float shipFlySpeed;
  
    void Start()
    {
        CameraController.Instance.LookAt(transform);
    }
	void Update () 
    {
        
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(hor, ver, 0);
        if (direction.magnitude > 0.5f)
        {
            Vector3 rot = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(new Vector3(0, 1, 0), direction), 0.2f).eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0,0,rot.z));
        }
        transform.position += direction * shipFlySpeed * Time.deltaTime;


        float deltaScroll = Input.GetAxis("Mouse ScrollWheel");
        if (!deltaScroll.Equals(0f))
        {
            CameraController.Instance.DefaultFreeSize -= Input.GetAxis("Mouse ScrollWheel") * 20;
            CameraController.Instance.SetSize(CameraController.Instance.DefaultFreeSize);
        }
    }
        
    void OnTriggerEnter2D(Collider2D other)
    {
        Planet pl = other.GetComponent<Planet>();
        if (pl == null)
            return;
        EventManager.Instance.PostEvent(GameEvent.ENTER_PLANET_AREA, this, pl);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Planet pl = other.GetComponent<Planet>();
        if (pl == null)
            return;
        EventManager.Instance.PostEvent(GameEvent.EXIT_PLANET_AREA, this, pl);
    }
}

