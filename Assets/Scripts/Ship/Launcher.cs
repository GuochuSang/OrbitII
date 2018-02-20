using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class Launcher : SerializedMonoBehaviour,ICamp
	{
		public GameCamp camp;

		/// <summary>
		/// 子弹组（子弹+子弹池）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class BulletGroup<T> where T : IBullet
		{
			public T Bullet;

			public List<T> BulletPool = new List<T>();
		}

		/// <summary>
		/// 子弹组列表（各种子弹以及它的池）
		/// </summary>
		public List<BulletGroup<IBullet>> BulletGroups = new List<BulletGroup<IBullet>>();

		/// <summary>
		/// 新建一发子弹
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual IBullet CreateBullet(int index,Vector3 position,Quaternion rotation)
		{
			if (BulletGroups[index].BulletPool.Count != 0)
			{
				foreach (var bul in BulletGroups[index].BulletPool)
				{
					if (!bul.isBulletActive())
					{
						bul.SetBulletActive(true);
						SetBulletInit(bul,position,rotation);
						return bul;
					}
				}
			}
			IBullet bullet = (IBullet) BulletGroups[index].Bullet.Inst();
			bullet.SetCamp(camp);
			bullet.SetBulletActive(true);
			BulletGroups[index].BulletPool.Add(bullet);
			SetBulletInit(bullet, position, rotation);
			return bullet;
		}

		public virtual void SetBulletInit(IBullet bullet, Vector3 position, Quaternion rotation)
		{
			bullet.Init(position,rotation);
			bullet.Work();
		}
		public virtual GameCamp GetCamp()
		{
			return camp;
		}

		public virtual void SetCamp(GameCamp camp)
		{
			this.camp = camp;
		}
	}
}
