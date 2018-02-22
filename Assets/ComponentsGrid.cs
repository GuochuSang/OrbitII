using System.Collections;
using System.Collections.Generic;
using ShipProject.Factory.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ShipProject.Factory.UI
{
	public class ComponentsGrid : MonoBehaviour
	{
		public GameObject TogglePrefab;
		public ComponentTypeSelectToggle toggle;
		[Button("AddToggles")]
		public void AddToggles()
		{
			foreach (var c in toggle.components)
			{
				ComponentToggle tog = Instantiate(TogglePrefab, transform).GetComponent<ComponentToggle>();
				tog.shipComponent = c;
				tog.SetToggle();
				Toggle t = tog.GetComponent<Toggle>();
				if (toggle.components.IndexOf(c) == 0)
				{
					t.isOn = true;
				}
				else
				{
					t.isOn = false;
				}
				t.group = GetComponent<ToggleGroup>();
			}
		}
		public void Update()
		{
			gameObject.SetActive(toggle.isOn);
		}
	}
}