using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class BlockHealth : MonoBehaviour, IAttackable,ICamp
	{
		public float MaxHealth;
		private float HealthRate;
		[ShowInInspector]
		public float healthRate
		{
			get { return HealthRate; }
			set
			{
				if (value < 0)
				{
					HealthRate = 0f;
				}
				else if (value > 1)
				{
					HealthRate = 1;
				}
				else
				{
					HealthRate = value;
				}
			}
		}
		private Block block;
		private void Start()
		{
			block = GetComponent<Block>();
			healthRate = 1f;
		}
		public void ReceiveDamage(IDamage damage, float damageValue)
		{
			healthRate -= damageValue / (MaxHealth * block.BlockLevel);
			if (healthRate <= 0)
			{
				block.ParentShip.CollapseBlockWithRecursion(block.Pos);
			}
		}

		public GameCamp GetCamp()
		{
			return block.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
	}
}