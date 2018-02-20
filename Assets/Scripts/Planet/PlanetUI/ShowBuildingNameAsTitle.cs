using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Universe 
{
public class ShowBuildingNameAsTitle : MonoBehaviour 
{
    Text text;
    void Awake()
    {
        text = GetComponent<Text>();
	}
	
	void Update () 
    {
        text.text = PlanetUI.Instance.CurrentBuildingName;	
	}
}
}