using System.Collections;
using System.Collections.Generic;
using ShipProject;
using UnityEngine;

namespace ShipProject
{
	public class ShipATPartManager : MonoBehaviour
	{
		public ShipAT at;

		// Use this for initialization
		void Start()
		{
			at = transform.parent.GetComponent<ShipAT>();
		}

		// Update is called once per frame
		void Update()
		{

		}
		public void SetPartActive()
		{
			at.SetPartActive();
		}
		public void SetPartActiveFalse()
		{
			at.SetPartActiveFalse();
		}
	}
}
