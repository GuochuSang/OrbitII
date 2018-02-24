using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using ShipSeriazble;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class BlockHealth : MonoBehaviour, IAttackable,ICamp
	{
		public event Action<IDamage, float> OnAttack;
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
			block.ParentShip.serializeShip.data.blockHealthData.Add(new SerializableVector2Int(block.Pos),new BlockHealthData(healthRate));
		}
		public void ReceiveDamage(IDamage damage, float damageValue)
		{
			healthRate -= damageValue / (MaxHealth * block.BlockLevel);
			if (healthRate <= 0)
			{
				block.ParentShip.CollapseBlockWithRecursion(block.Pos);
			}
			block.ParentShip.serializeShip.data.blockHealthData[new SerializableVector2Int(block.Pos)].health = healthRate;
			if (OnAttack != null) OnAttack(damage, damageValue);
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