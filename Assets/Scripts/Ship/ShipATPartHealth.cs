using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class ShipATPartHealth : MonoBehaviour, IAttackable, ICamp
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
		private ShipAT at;
		private void Start()
		{
			at = transform.parent.parent.GetComponent<ShipAT>();
			healthRate = 1f;
		}
		public void ReceiveDamage(IDamage damage, float damageValue)
		{
			healthRate -= damageValue / MaxHealth;
			if (healthRate <= 0)
			{
				at.healthRate -= 0.25f;
				gameObject.SetActive(false);
			}
		}

		public GameCamp GetCamp()
		{
			return at.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
	}
}
