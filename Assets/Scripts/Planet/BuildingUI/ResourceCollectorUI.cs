using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Universe
{
    public class ResourceCollectorUI : MonoBehaviour 
    {
        public Slider slider;
        public Text heightText;
        [Header("采集范围(km)")]
        public float maxCollectDepth = 100;
        ResourceCollector collector = null;

        void OnEnable()
        {
            Debug.Assert(PlanetUI.Instance.CurrentBuilding.GetName().Equals("ResourceCollector"));
            collector = (ResourceCollector)PlanetUI.Instance.CurrentBuilding;
        }
        void OnDesable()
        {
            collector = null;
        }
        public void UpdateHeight()
        {
            heightText.text = "Depth : " + maxCollectDepth * (1 - slider.value);
            collector.currentCollectHeight = slider.value;
        }


    }
}