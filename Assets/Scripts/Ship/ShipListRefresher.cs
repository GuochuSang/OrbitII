using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipProject.Factory.UI
{
	public class ShipListRefresher : MonoBehaviour
	{
		public LoadShipButton loader;
		public ShipFactoryTypeSelector selector;
		public void Refresh()
		{
			loader.LoadButtons();
		}

		public void ActiveFalse()
		{
			gameObject.SetActive(false);
		}

		public void SetKBTrue()
		{
			selector.KBSeletcable = true;
		}

		public void SetKBFalse()
		{
			selector.KBSeletcable = false;
		}
	}
}
