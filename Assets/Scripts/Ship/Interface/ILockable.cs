using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 可锁定接口
/// </summary>
public interface ILockable:IDetectable
{
	/// <summary>
	/// 显示锁定特效
	/// </summary>
	void Lock();

	void CancelLock();
}
