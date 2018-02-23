using System.Collections;
using System.Collections.Generic;
using ShipProject;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{

	public class ShipUI : MonoBehaviour
	{
		public ShipHealthUI health;
		public ShipSpUI Sp;
		public Ship theShip;
		[Button("Set")]
		public void Set()
		{
			SetShip(theShip);
		}
		public void SetShip(Ship ship)
		{
			health.ship = ship;
			Sp.control = ship.GetComponent<ShipControl>();
		}

		public void ShowUI(bool value)
		{
			health.gameObject.SetActive(value);
			Sp.gameObject.SetActive(value);
		}
	}
}
