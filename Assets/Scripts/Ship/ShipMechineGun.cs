
using System.Collections;
using System.Collections.Generic;
using ShipProject;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class ShipMechineGun : Launcher, IShipWeapon
	{
		public float SPConsume;
		public ShipWeaponDefaultNum num;
		public ShipWeaponDefaultOperationMode mode;
		private ShipComponent component;
		private Ship ship;
		private ShipControl control;
		public Animator animator;
		public bool isSkilling;
		public Transform hole1, hole2;
		private bool shoot;
		public bool Shoot
		{
			get { return shoot; }
			set
			{
				if (value != shoot)
				{
					animator.SetBool("shoot", value);
				}
				shoot = value;
			}
		}
		protected void Start()
		{
			component = GetComponent<ShipComponent>();
			ship = component.ParentShip;
			control = ship.GetComponent<ShipControl>();
			control.AddWeapon(this);
			animator = component.Blocks[new Vector2Int(0,0)].GetComponent<Animator>();
		}
		public override GameCamp GetCamp()
		{
			return ship.GetCamp();
		}

		public override void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
		public float GetSPConsume()
		{
			return SPConsume;
		}

		public ShipWeaponDefaultNum GetDefaultOperation()
		{
			return num;
		}

		public ShipWeaponDefaultOperationMode GetOperationMode()
		{
			return mode;
		}

		public void WeaponWork()
		{
			Shoot = true;
			isSkilling = false;
		}

		public void WeaponSkill()
		{
			Shoot = true;
			isSkilling = true;
		}

		public void End()
		{
			Shoot = false;
		}
		public void OnShoot()
		{
			if (!Shoot)
			{
				return;
			}
			MechineGunBullet b1, b2;
			if (isSkilling)
			{
				b1=CreateBullet(1, hole1.position, hole1.rotation) as MechineGunBullet;
				b2=CreateBullet(1, hole2.position, hole2.rotation) as MechineGunBullet;
				control.Sp -= GetSPConsume() * 6f;
			}
			else
			{
				b1 = CreateBullet(0, hole1.position, hole1.rotation) as MechineGunBullet;
				b2 = CreateBullet(0, hole2.position, hole2.rotation) as MechineGunBullet;
				control.Sp -= GetSPConsume();
			}
			b1.SetCamp(GetCamp());
			b2.SetCamp(GetCamp());
		}
	}
}
