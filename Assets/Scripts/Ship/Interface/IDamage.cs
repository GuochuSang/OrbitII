using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 伤害接口，继承该接口可以对继承了IAttackable接口的对象造成伤害
/// </summary>
public interface IDamage
{
	void Damage(IAttackable attackable, float value);
	void FinishWork();
}
