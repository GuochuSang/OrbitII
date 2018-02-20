using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class ShipAT : MonoBehaviour,IAttackable,ICamp
	{
		public Ship ship;
		public GameObject Broken;
		public CircleCollider2D col2D;
		public bool charging;
		public float chargingTime = 10f;
		private float timer;
		public Animator animator;
		public GameObject[] Part = new GameObject[4];
		public float MaxHealth;
		private float HealthRate;
		[ShowInInspector]
		public float healthRate
		{
			get { return HealthRate;}
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
		void Start()
		{
			ship = GetComponent<ShipComponent>().ParentShip;
		}
		private void Update()
		{
			if (charging)
			{
				timer += Time.deltaTime;
				if (timer >= chargingTime)
				{
					healthRate = 1f;
					charging = false;
					timer = 0;
					col2D.enabled = true;
					animator.SetBool("charging", false);
					Broken.SetActive(false);
				}
			}
			else
			{
				if (healthRate <= 0.5f)
				{
					Broken.SetActive(true);
				}
				else
				{
					Broken.SetActive(false);
				}
				if (healthRate == 0)
				{
					charging = true;
					timer = 0;
					col2D.enabled = false;
					animator.SetBool("charging", true);
					Broken.SetActive(false);
				}
			}
		}
		public void ReceiveDamage(IDamage damage, float damageValue)
		{
			Debug.Log(damageValue);
			healthRate -= damageValue / MaxHealth;
		}

		public GameCamp GetCamp()
		{
			return ship.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
		public void SetPartActive()
		{
			foreach (var part in Part)
			{
				if (part != null)
					part.SetActive(true);
			}
		}
		public void SetPartActiveFalse()
		{
			foreach (var part in Part)
			{
				if (part != null)
					part.SetActive(false);
			}
		}
	}
}
