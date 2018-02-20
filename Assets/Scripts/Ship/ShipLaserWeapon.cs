using System.Collections;
using System.Collections.Generic;
using ShipProject;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class ShipLaserWeapon : LaserEffect,IShipWeapon
	{
		public float SPConsume = 50f;
		public ShipWeaponDefaultNum GetDefaultOperation()
		{
			return weaponDefaultNum;
		}

		public ShipWeaponDefaultOperationMode GetOperationMode()
		{
			return operationMode;
		}

		public void WeaponWork()
		{
			if (GetOperationMode() == ShipWeaponDefaultOperationMode.Impulse)
			{
				control.Sp -= GetSPConsume();
			}

			if (GetOperationMode() == ShipWeaponDefaultOperationMode.Continuous)
			{
				control.Sp -= GetSPConsume() * Time.deltaTime;
			}
			this.IsWork = true;
		}

		public void WeaponSkill()
		{
			this.IsWork = true;
			if (GetOperationMode() == ShipWeaponDefaultOperationMode.Impulse)
			{
				control.Sp -= GetSPConsume() * 6;
			}

			if (GetOperationMode() == ShipWeaponDefaultOperationMode.Continuous)
			{
				control.Sp -= GetSPConsume() * Time.deltaTime * 6;
			}
		}

		public void End()
		{
			this.IsWork = false;
		}

		public ShipWeaponDefaultNum weaponDefaultNum;
		public ShipWeaponDefaultOperationMode operationMode;
		public ShipComponent shipComponent;
		public Ship ship;
		public ShipControl control;
		public Animator head,laserBase;
		public LaserTriggerDamage laserTriggeerDamage;
		/// <summary>
		/// 激光工作状态设置
		/// </summary>
		[ShowInInspector]
		[TabGroup("Laser", "属性")]
		public override bool IsWork
		{
			get { return isWork; }
			set
			{
				if (value != isWork)
				{
					animator.SetBool("isWork", value);
					laserBase.SetBool("isWork", value);
					head.SetBool("isWork", value);
				}
				isWork = value;
			}
		}
		protected override void Start()
		{
			shipComponent = GetComponent<ShipComponent>();
			ship = shipComponent.ParentShip;
			control = ship.GetComponent<ShipControl>();
			laserBase = shipComponent.Blocks[new Vector2Int(0, 0)].GetComponent<Animator>();
			head = shipComponent.Blocks[new Vector2Int(0, 1)].GetComponent<Animator>();
			laserTriggeerDamage = LaserGlow.GetComponent<LaserTriggerDamage>();
			laserTriggeerDamage.shipLaser = this;
			control.AddWeapon(this);
			base.Start();
		}
		public GameCamp GetCamp()
		{
			return ship.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
		}
		protected void OnDestroy()
		{
			control.RemoveWeapon(this);
		}

		public float GetSPConsume()
		{
			return SPConsume;
		}
	}
}
