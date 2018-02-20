using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Universe
{
    public class TestShowResource : MonoBehaviour 
    {
        public Text text;
        public Planet curPlanet;
        public bool isOpen = false;
    	void Update () 
        {
            if (PlanetUI.Instance.CurrentPlanet != null)
            {
                curPlanet = PlanetUI.Instance.CurrentPlanet;
                Open();
            }
            else
                Close();
    	}
        void Open()
        {
            if (isOpen)
                return;
            text.text = "";
            foreach (var kv in curPlanet.resources.elementInLands)
            {
                text.text += kv.Key.ToString() + "\n";
                text.text += kv.Key.detail + " : \n";
                for (int i = 0; i < kv.Value.Length; i++)
                    text.text += kv.Value[i].ToString("0.0")+"\t\t";
                text.text += "\n\n"; 
            }
        }
        void Close()
        {
            if (!isOpen)
                return;
            text.text = "";
        }
    }
}