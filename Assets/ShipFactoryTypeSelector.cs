using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ShipProject.Factory.UI;
using UnityEngine.EventSystems;

namespace ShipProject.Factory
{
	public class ShipFactoryTypeSelector : MonoBehaviour
	{
		/// <summary>
		/// Toggle组
		/// </summary>
		public ToggleGroup toggleGroup;
		private IEnumerable<Toggle> toggles;
		public string LB="LB",RB="RB";
		public Text toggleText;
		public Toggle toggle;
		public EventSystem es;
		public void Start()
		{
			toggles = toggleGroup.ActiveToggles();
		}
		public void Update()
		{
			foreach (var t in toggles)
			{	
				if (t.isOn)
				{
					toggle = t;
					((ComponentTypeSelectToggle)t).Panel.SetActive(t.isOn);
				}
			}

			if (Input.GetButtonDown(RB))
			{
				toggleGroup.SetAllTogglesOff();
				if (((ComponentTypeSelectToggle) toggle).rightToggle != null)
				{
					((ComponentTypeSelectToggle) toggle).rightToggle.isOn = true;
					es.SetSelectedGameObject((((toggle as ComponentTypeSelectToggle).rightToggle) as ComponentTypeSelectToggle).Panel
						.transform.GetChild(0).gameObject);
				}
			}
			if (Input.GetButtonDown(LB))
			{
				toggleGroup.SetAllTogglesOff();
				if ((toggle as ComponentTypeSelectToggle).leftToggle != null)
				{
					(toggle as ComponentTypeSelectToggle).leftToggle.isOn = true;
					es.SetSelectedGameObject((((toggle as ComponentTypeSelectToggle).leftToggle) as ComponentTypeSelectToggle).Panel
						.transform.GetChild(0).gameObject);
				}
			}
			toggleText.text = (toggle as ComponentTypeSelectToggle).componentType.ToString();
		}
	}
}
