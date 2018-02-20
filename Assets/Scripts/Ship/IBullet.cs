using System.Collections;
using System.Collections.Generic;
using ShipProject;
using UnityEngine;

namespace ShipProject
{
	public interface IBullet : ICamp, IDamage
	{
		bool isBulletActive();
		void Init(Vector3 position, Quaternion rotation);
		IBullet Inst();
		void SetBulletActive(bool value);
		void Work();
	}
}
