using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class LaserTriggerDamage :  TriggerDamage
	{
		public ShipLaserWeapon shipLaser;
		public override GameCamp GetCamp()
		{
			return shipLaser.GetCamp();
		}
		public override void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
	}
}
