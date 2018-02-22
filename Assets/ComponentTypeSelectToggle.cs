using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using ShipProject;
using ShipProject.ShipEnum;

namespace ShipProject.Factory.UI
{
	public class ComponentTypeSelectToggle : Toggle
	{
		public Toggle leftToggle, rightToggle;
		public ShipComponentType componentType;
		public List<ShipComponent> components;
		public GameObject Panel;

		public  void Start()
		{
			base.Start();
			if(componentType != ShipComponentType.All)
			components = ShipUnitLister.GetInstance().ShipComponentList
				.FindAll((ShipComponent c) => { return c.ShipComponentType == componentType;});
			else
			{
				components = ShipUnitLister.GetInstance().ShipComponentList;
			}
		}
			
	}
}
