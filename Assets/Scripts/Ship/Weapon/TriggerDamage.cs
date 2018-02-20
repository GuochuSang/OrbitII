using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	namespace ShipEnum
	{
		/// <summary>
		/// 何时施加伤害
		/// </summary>
		public enum TriggerDamageMode
		{
			DamageWhenEnter=0,
			DamageWhenStay=1,
			DamageWhenExit=2,
		}
		/// <summary>
		/// 相杀等级
		/// </summary>
		public enum DamageDestroyLevel
		{
			Missile=0,
			SmallBullet=1,
			LargeBullet=2,
			Laser=3
		}
	}
	/// <summary>
	/// 触发器伤害基类
	/// </summary>
	public abstract class TriggerDamage : MonoBehaviour, IDamage,ICamp
	{
		/// <summary>
		/// 伤害施加模式
		/// </summary>
		public TriggerDamageMode mode;
		/// <summary>
		/// 相杀等级
		/// </summary>
		public DamageDestroyLevel destroyLevel;
		/// <summary>
		/// 伤害值
		/// </summary>
		public float damageValue;

		public Rigidbody2D rig2d;
		public GameCamp camp;
		/// <summary>
		/// 施加伤害
		/// </summary>
		/// <param name="damage">伤害来源</param>
		/// <param name="value">伤害值</param>
		public virtual void Damage(IAttackable attackable, float value)
		{
			if(attackable!=null)
			attackable.ReceiveDamage(this, value);
		}

		public virtual void FinishWork()
		{
		}

		public virtual GameCamp GetCamp()
		{
			return camp;
		}

		public virtual void SetCamp(GameCamp camp)
		{
			this.camp = camp;
		}
		protected void OnTriggerEnter2D(Collider2D collision)
		{
			TriggerDamage damage = collision.GetComponent<TriggerDamage>();
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
			if (mode != TriggerDamageMode.DamageWhenEnter)
				return;
			IAttackable atk = collision.GetComponent<IAttackable>();
			if (atk != null)
			{
				if((atk as ICamp).GetCamp()!=GetCamp())
				Damage(atk, damageValue);
			}
		}
		protected void OnTriggerStay2D(Collider2D collision)
		{
			Debug.Log("Staying");
			if (mode != TriggerDamageMode.DamageWhenStay)
				return;
			IAttackable atk = collision.GetComponent<IAttackable>();
			if (atk != null)
			{
				if ((atk as ICamp).GetCamp() != GetCamp())
				Damage(atk, damageValue*Time.fixedDeltaTime);
			}
		}
		protected void OnTriggerExit2D(Collider2D collision)
		{
			if (mode != TriggerDamageMode.DamageWhenExit)
				return;
			IAttackable atk = collision.GetComponent<IAttackable>();
			if (atk != null)
			{
				if ((atk as ICamp).GetCamp() != GetCamp())
				Damage(atk, damageValue);
			}
		}
	}
}
