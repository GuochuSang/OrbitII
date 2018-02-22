using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelegate : MonoBehaviour {
    public delegate void OnEvent();
    Dictionary<int,OnEvent> listeners = new Dictionary<int,OnEvent>();

	void Start () 
    {
        listeners.Add(1, OnDown);
        listeners[1] += OnDown;
        listeners[1] += OnUp;
        listeners[1]();
        listeners[1] -= OnDown;
        listeners[1] -= OnDown;
        listeners[1]();
        listeners[1] -= OnDown;
        listeners[1]();

	}
    void OnDown()
    {
        Debug.Log("Down");
    }
    void OnUp()
    {
        Debug.Log("Up");
    }

}
