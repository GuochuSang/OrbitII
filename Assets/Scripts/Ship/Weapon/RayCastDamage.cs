using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class RayCastDamage : MonoBehaviour,IDamage,ICamp
	{
		public  GameCamp camp;
		/// <summary>
		/// 相杀等级
		/// </summary>
		public DamageDestroyLevel destroyLevel;
		/// <summary>
		/// 伤害值
		/// </summary>
		public float damageValue;
		public void Damage(IAttackable attackable, float value)
		{
			if (attackable != null)
			{
				attackable.ReceiveDamage(this, value);
				FinishWork();
			}
		}

			public virtual void FinishWork()
			{
				throw new NotImplementedException();
			}
			public virtual GameCamp GetCamp()
			{
				return camp;
			}

			public virtual void SetCamp(GameCamp camp)
			{
				this.camp = camp;
			}
		public virtual void Update()
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position+0.51f* transform.up, transform.up, 0.01f);
			if (hit.collider == null)
			{
				return;
			}
			RayCastDamage damage = hit.collider.GetComponent<RayCastDamage>();
			if (damage != null)
			{
				if (damage.GetCamp() != GetCamp())
				{
					if (damage.destroyLevel < destroyLevel)
					{
						damage.FinishWork();
					}
				}
			}
			IAttackable atk = hit.collider.GetComponent<IAttackable>();
			if (atk != null)
			{
				Debug.Log("In");
				if ((atk as ICamp).GetCamp() != GetCamp())
					Damage(atk, damageValue);
			}
		}
	}
	}
