using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;
namespace ShipProject
{
	namespace ShipEnum
	{
		public enum MissileState
		{
			Ready=0,
			Launch=1,
			Follow=2
		}
	}
	public class Missile : TriggerDamage,IBullet
	{
		public MissileState state;
		public float MissileThrustForce = 30f;
		public float MissileTurningSpeed = 10f;
		public float MaxSpeed = 40f;
		public float LaunchStateTime;
		public Transform target;
		protected float timer;
		public GameObject Trail;
		public float LifeTime;
		private float lifeTimer;

		/// <inheritdoc />
		public override void FinishWork()
		{
			gameObject.SetActive(false);
			Trail.SetActive(false);
		}

		public void Init(Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			transform.rotation = rotation;
			state = MissileState.Ready;
		}

		public IBullet Inst()
		{
		 	return Instantiate(gameObject).GetComponent<IBullet>();
		}

		public bool isBulletActive()
		{
			return gameObject.activeInHierarchy;
		}

		public void SetBulletActive(bool value)
		{
			gameObject.SetActive(value);
		}

		public void Work()
		{
			timer = LaunchStateTime;
			state = MissileState.Launch;
			lifeTimer = LifeTime;
		}

		public void Follow(Vector2 pos)
		{
			Vector2 line = pos - (Vector2)transform.position;
			float angle = Vector2.SignedAngle(line, rig2d.velocity);
			Vector2 normVelocity = line.normalized * rig2d.velocity.magnitude * Mathf.Cos(angle / 180 * Mathf.PI);
			Vector2 horVelocity = rig2d.velocity - normVelocity;
			rig2d.AddForce(line.normalized * MissileThrustForce, ForceMode2D.Force);
			Debug.DrawRay(transform.position, line, Color.red);
			rig2d.velocity -= MissileTurningSpeed * Time.deltaTime * horVelocity;
			if (rig2d.velocity.magnitude >= MaxSpeed)
			{
				rig2d.velocity = rig2d.velocity.normalized * MaxSpeed;
			}
		}
		// Use this for initialization
		void Start()
		{
			rig2d = GetComponent<Rigidbody2D>();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			if (state == MissileState.Ready)
			{
				
			}
			if (state == MissileState.Launch)
			{
				rig2d.AddForce(transform.up * MissileThrustForce);
			}
			if (state == MissileState.Follow)
			{
				if (target == null)
				{
				}
				else if (target.gameObject.activeInHierarchy == true)
				{
					Follow((Vector2)target.position);
				}
			}
		}
		private void Update()
		{
			if (state == MissileState.Ready)
			{

			}
			if (state == MissileState.Launch)
			{
				timer -= Time.deltaTime;
				if (timer <= 0)
				{
					Trail.SetActive(true);
					state = MissileState.Follow;
				}
			}
			if (state == MissileState.Follow)
			{
			}
			if (state != MissileState.Launch && state != MissileState.Ready)
				transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector2(0, 1), rig2d.velocity));
			lifeTimer -= Time.deltaTime;
			if (lifeTimer <= 0)
				FinishWork();
		}
		public void SetTarget(Transform target)
		{
			this.target = target;
		}
		public override void Damage(IAttackable attackable, float value)
		{
			base.Damage(attackable, value);
			FinishWork();
		}
	}
}
