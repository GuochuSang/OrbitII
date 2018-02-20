using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class ShipMissileLauncher : Launcher, IShipWeapon
	{
		public MissileLoadingEffect[] holes = new MissileLoadingEffect[6];
		public ShipWeaponDefaultNum num;
		public float missileLoadGap = 1f;
		private ShipWeaponDefaultOperationMode mode;
		private ShipComponent component;
		private Ship ship;
		private ShipControl control;
		public Transform target;
		private float timer = 0f;
		public float SPConsume;
		public List<Transform> targetList = new List<Transform>();
		protected void Start()
		{
			component = GetComponent<ShipComponent>();
			ship = component.ParentShip;
			control = ship.GetComponent<ShipControl>();
			control.AddWeapon(this);
		}
		protected void Update()
		{
			timer += Time.deltaTime;
			if (timer >= missileLoadGap)
			{
				foreach (var hole in holes)
				{
					if (!hole.isReady)
					{
						hole.LoadMissile();
						break;
					}
				}

				timer = 0f;
			}
		}
		public GameCamp GetCamp()
		{
			return ship.GetCamp();
		}

		public void SetCamp(GameCamp camp)
		{
			throw ShipProjectExpection.SetCampException(1);
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
			if (mode == ShipWeaponDefaultOperationMode.Impulse)
			{
				control.Sp -= GetSPConsume();
			}
			if (mode == ShipWeaponDefaultOperationMode.Continuous)
			{
				control.Sp -= GetSPConsume()*Time.deltaTime;
			}

			if (targetList.Count > 0)
			{
				target = targetList[0];
			}
			else
			{
				target = null;
			}

			Debug.Log("work");
			foreach (var hole in holes)
			{
				if (hole.isReady)
				{
					hole.LaunchMissile();
					Debug.Log("Launch");
					Missile missile= CreateBullet(0, hole.transform.position, hole.transform.rotation) as Missile;
					missile.SetCamp(GetCamp());
					missile.SetTarget(target);
					break;
				}
			}
		}

		public void WeaponSkill()
		{
			if (mode == ShipWeaponDefaultOperationMode.Impulse)
			{
				control.Sp -= GetSPConsume()*6;
			}
			if (mode == ShipWeaponDefaultOperationMode.Continuous)
			{
				control.Sp -= GetSPConsume() * Time.deltaTime*6;
			}

			if (targetList.Count > 0)
			{
				target = targetList[0];
			}
			else
			{
				target = null;
			}
			bool AllReady = true;
			foreach (var hole in holes)
			{
				if (!hole.isReady)
					AllReady = false;
			}

			if (AllReady)
			{
				foreach (var hole in holes)
				{
					hole.LaunchMissile();
					Missile missile = CreateBullet(0, hole.transform.position, hole.transform.rotation) as Missile;
					missile.SetCamp(GetCamp());
					missile.SetTarget(target);
				}
			}
		}
		public void End()
		{
			//
		}
		protected void OnDestroy()
		{
			control.RemoveWeapon(this);
		}

		public float GetSPConsume()
		{
			return SPConsume;
		}
		public void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.GetComponent<ILockable>() != null)
			{
			 	ILockable lockable=collision.GetComponent<ILockable>();
				if ((lockable as ShipLocked).GetCamp() != GetCamp())
				{
					lockable.Lock();
					targetList.Add((lockable as MonoBehaviour).transform);
				}
			}
		}
		public void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.GetComponent<ILockable>() != null)
			{
				ILockable lockable = collision.GetComponent<ILockable>();
				if ((lockable as ShipLocked).GetCamp() != GetCamp())
				{
					lockable.CancelLock();
					targetList.Remove((lockable as MonoBehaviour).transform);
				}
			}
		}
	}
	}
