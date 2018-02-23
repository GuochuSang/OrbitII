using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	public class RayCastDamage : MonoBehaviour,IDamage,ICamp
	{
		public float lineLength;
		public float LineOffset;
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
/*		public virtual void FixedUpdate()
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position+transform.up*LineOffset, transform.up, lineLength);
			Debug.DrawLine(transform.position + transform.up * LineOffset, transform.position + transform.up * LineOffset + transform.up* lineLength);
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
				if ((atk as ICamp).GetCamp() != GetCamp())
					Damage(atk, damageValue);
			}
		}*/
		public void OnTriggerEnter2D(Collider2D collision)
		{
			RayCastDamage damage =collision.GetComponent<RayCastDamage>();
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
			IAttackable atk =collision.GetComponent<IAttackable>();
			if (atk != null)
			{
				if ((atk as ICamp).GetCamp() != GetCamp())
					Damage(atk, damageValue);
			}
		}
	}
	}
