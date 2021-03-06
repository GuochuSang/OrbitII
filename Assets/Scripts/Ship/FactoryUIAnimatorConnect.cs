﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipProject.Factory.UI
{
	public class FactoryUIAnimatorConnect : MonoBehaviour
	{
		public Animator a1, a2;
		public ShipFactoryTypeSelector selector;
		public void ConnectShow()
		{
			a1.SetTrigger("show");
			a2.SetTrigger("show");
			selector.SetKBState(true);
		}
		public void ConnectHide()
		{
			a1.SetTrigger("hide");
			a2.SetTrigger("hide");
		}
	}
}
