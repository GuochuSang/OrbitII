using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipProject
{
	public class ShipSpUI : MonoBehaviour
	{
		public ShipControl control;

		private Image image;
		// Use this for initialization
		void Start()
		{
			image = GetComponent<Image>();
		}

		// Update is called once per frame
		void Update()
		{
			if (control != null)
			{
				Debug.Log(control.Sp / control.MaxSP);
				image.fillAmount = control.Sp / control.MaxSP;
			}
		}
	}
}