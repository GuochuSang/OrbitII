using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	/// <summary>
	/// 激光效果类
	/// </summary>
	[ExecuteInEditMode]
	public class LaserEffect : MonoBehaviour
	{
		/// <summary>
		/// 激光束长度
		/// </summary>
		[Tooltip("激光束长度")][TabGroup("Laser","属性")]
		public float LaserLength = 100f;

		protected float curLaserLength;
		/// <summary>
		/// 激光束宽度
		/// </summary>
		[Tooltip("激光束宽度")]
		[TabGroup("Laser", "属性")]
		[Range(0,1)]
		public float LaserGlowWidth;

		/// <summary>
		/// 激光工作状态设置
		/// </summary>
		[ShowInInspector]
		[TabGroup("Laser", "属性")]
		public virtual bool IsWork
		{
			get { return isWork; }
			set
			{
				if (value != isWork)
				{
					animator.SetBool("isWork", value);
				}
				isWork = value;
			}
		}

		protected bool isWork;
		protected float curLaserGlowWidth;

		protected void SetLaser()
		{
			LaserGlow.localScale = new Vector3(LaserGlowWidth, LaserLength, 1);
		}
		[Tooltip("激光束光球特效")]
		[TabGroup("Laser", "属性")]
		public Transform LaserGlow;

		/// <summary>
		/// 动画状态机
		/// </summary>
		protected Animator animator;
		protected virtual void Start()
		{
			animator = GetComponent<Animator>();
		}
		protected void Update()
		{
			if (curLaserGlowWidth != LaserGlowWidth || curLaserLength != LaserLength)
			{
				SetLaser();
				curLaserGlowWidth = LaserGlowWidth;
				curLaserLength = LaserLength;
			}
		}
	}
}

