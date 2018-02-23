using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipProject.Factory.UI
{
	public class ShipLoaderButton : MonoBehaviour
	{
		public Text text;
		public int index = 0;
		public void LoadShip()
		{
			FactoryListener.Instance.LoadShip(text.text);
		}
	}
}
