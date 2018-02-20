using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
	public void Awake()
	{
		StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => Destroy(gameObject), 3f));
	}
}
