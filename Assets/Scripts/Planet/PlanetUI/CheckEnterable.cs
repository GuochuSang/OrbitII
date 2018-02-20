using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
public class CheckEnterable : MonoBehaviour 
{
    void Awake()
    {
        Manager.EventManager.Instance.AddListener(GameEvent.COLONY_SET_UP,OnConlonySetUp);
        Manager.EventManager.Instance.AddListener(GameEvent.LOOK_PLANET,OnLookPlanet);
    }
    public void OnConlonySetUp(GameEvent eventType, Component sender, object param = null)
    {
        this.gameObject.SetActive(true);
    }
    public void OnLookPlanet(GameEvent eventType, Component sender, object param = null)
    {
        if(PlanetUI.Instance.CurrentPlanet.colony == null)
            this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }
}
}