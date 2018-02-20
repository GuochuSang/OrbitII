using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class TestTrigger : TriggerDamage
	{
		public override GameCamp GetCamp()
		{
			return GameCamp.enemy;
		}
	}
}
