using System.Collections;
using System.Collections.Generic;
using ShipProject;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class ShipLocked : MonoBehaviour, ILockable, ICamp
	{
		public Animator ShipLock;
		public Ship ship;
		public void Start()
		{
			ship = GetComponent<Ship>();
		}

		public void Lock()
		{
			ShipLock.SetBool("lock", true);
		}

		public void CancelLock()
		{
			ShipLock.SetBool("lock", false);
		}

		public GameCamp GetCamp()
		{
			return ship.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
	}
}
