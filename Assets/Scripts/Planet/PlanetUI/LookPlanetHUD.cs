using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
public class LookPlanetHUD : MonoBehaviour {
    [Header("按下此键总览星球")]
    public KeyCode key;
	void Update () 
    {
        if (PlanetUI.Instance.watchState != PlanetUI.WatchState.HALF_FREE)
        {
            PoolManager.Instance.ReturnCacheGameObejct(this.gameObject);
            return;
        }
        if (Input.GetKeyDown(key))
        {
            PlanetUI.Instance.LookPlanet();
            PoolManager.Instance.ReturnCacheGameObejct(this.gameObject);
        }
	}
}
}