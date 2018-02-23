using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipProject.Factory.UI
{
	public class ShipListRefresher : MonoBehaviour
	{
		public LoadShipButton loader;

		public void Refresh()
		{
			loader.LoadButtons();
		}

		public void ActiveFalse()
		{
			gameObject.SetActive(false);
		}
	}
}
