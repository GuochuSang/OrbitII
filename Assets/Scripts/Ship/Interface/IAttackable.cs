using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 可受攻击接口，继承该接口可以受到继承IDamage接口的对象的伤害
/// </summary>
public interface IAttackable
{
	void ReceiveDamage(IDamage damage, float damageValue);
}
