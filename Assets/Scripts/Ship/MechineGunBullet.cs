using System.Collections;
using System.Collections.Generic;
using ShipProject;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class MechineGunBullet : RayCastDamage, IBullet
	{
		public float BulletSpeed;
		public float BulletLifeTime;
		private float lifeTimer;
		private bool shoot = false;
		public bool isBulletActive()
		{
			return gameObject.activeInHierarchy;
		}

		public void Init(Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			transform.rotation = rotation;
			shoot = false;
		}

		public IBullet Inst()
		{
			return Instantiate(gameObject).GetComponent<IBullet>();
		}

		public void SetBulletActive(bool value)
		{
			gameObject.SetActive(value);
		}

		public void Work()
		{
			lifeTimer = 0;
			shoot = true;
		}
		public override void FinishWork()
		{
			SetBulletActive(false);
		}
		private void Awake()
		{
		}
		private void Update()
		{
			base.Update();
			if (shoot)
			{
				transform.Translate(transform.InverseTransformDirection(transform.up)* BulletSpeed * Time.deltaTime);
			}
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= BulletLifeTime)
			{
				FinishWork();
			}
		}
	}
}
