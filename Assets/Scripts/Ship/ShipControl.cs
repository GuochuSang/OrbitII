using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using ShipProject.ShipEnum;

namespace ShipProject
{
	namespace ShipEnum
	{
		/// <summary>
		/// 控制行为模式枚举
		/// </summary>
		public enum ShipControlMode
		{
			Building=0,
			Player=1,
			AIFollow=2,
			AIDefense=3,
			AIAttack=4,
			AIRandom=5
		}
	}
	/// <summary>
	/// 飞船行为控制类
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	public class ShipControl : SerializedMonoBehaviour
	{
		/// <summary>
		///	绑定刚体
		/// </summary>
		[HideInInspector]
		public Rigidbody2D rig2d
		{
			get { return GetComponent<Rigidbody2D>(); }
		}
		[TabGroup("Por","SP")]
		public float MaxSP;
		[TabGroup("Por", "SP")][SerializeField()][DisableInPlayMode]
		private float sp;
		public float Sp
		{
			get
			{
				return sp;
			}
			set
			{
				if (value <= 0)
				{
					sp = 0;
					return;
				}

				if (value >= MaxSP)
				{
					sp = MaxSP;
					return;
				}
				sp = value;
			}
		}
		[TabGroup("Por", "SP")]
		public bool SpColding = false;
		[TabGroup("Por", "SP")]
		public float SpOutColdingRate = 0.7f;
		[TabGroup("Por", "SP")]
		[DisableInPlayMode]
		public float SpRecoverSpeed = 10f;
		/// <summary>
		/// 飞船操控模式
		/// </summary>
		[TabGroup("Por","Control")][Tooltip("飞船操控模式")]
		public ShipControlMode mode;
		/// <summary>
		/// 阻力率
		/// </summary>
		[TabGroup("Por", "Control")]
		[Tooltip("阻力率")]
		public float resistanceRate;
		/// <summary>
		/// 紧急回避加成率
		/// </summary>
		[TabGroup("Por", "Control")]
		[Tooltip("紧急回避加成率")]
		public float dashRate;
		/// <summary>
		/// 紧急回避时长
		/// </summary>
		[TabGroup("Por", "Control")]
		[Tooltip("紧急回避时长")]
		public float dashTime;
		/// <summary>
		/// 当前武器
		/// </summary>
		public ShipWeaponGroup currentWeaponIndex;
		/// <summary>
		/// 武器组结构体
		/// </summary>
		public class ShipWeaponGroup
		{
			public List<IShipWeapon> shipWeapons=new List<IShipWeapon>();
			public ShipWeaponDefaultNum num;
			public ShipWeaponDefaultOperationMode operationMode;
		}

		/// <summary>
		///	武器组
		/// </summary>
		[ShowInInspector]
		public List<ShipWeaponGroup> WeaponGroups = new List<ShipWeaponGroup>();
		private float dashTimer;
		#region 引擎组

		/// <summary>
		/// 垂直引擎组
		/// </summary>
		[TabGroup("Control", "Thruster")] [DisableInPlayMode]
		public List<ShipThruster> VT;
		/// <summary>
		/// 水平引擎组
		/// </summary>
		[TabGroup("Control", "Thruster")]
		[DisableInPlayMode]
		public List<ShipThruster> HT;
		public void AddThruster(ShipThruster thruster)
		{
			if (thruster.rotation == ShipUnitRotation.d0 || thruster.rotation == ShipUnitRotation.d180)
				VT.Add(thruster);
			if (thruster.rotation == ShipUnitRotation.d90 || thruster.rotation == ShipUnitRotation.d270)
				HT.Add(thruster);
		}
		#endregion
		#region 动力学状态

		/// <summary>
		/// 动力学状态，垂直平动加力中
		/// </summary>
		private int VM;

		/// <summary>
		/// 动力学状态，水平平动加力中
		/// </summary>
		private int HM;

		/// <summary>
		/// 动力学状态，旋转加力中
		/// </summary>
		private int TM;
		#endregion
		public void Start()
		{
			GetComponent<Ship>().BlocksChanged += SetMass;
			rig2d.gravityScale = 0;
			#region 飞船阻力
			rig2d.drag = resistanceRate;
			rig2d.angularDrag = resistanceRate;
			#endregion
		}
		/// <summary>
		/// 质量改变
		/// </summary>
		/// <param name="BlockNumber"></param>
		private void SetMass(int BlockNumber)
		{
			rig2d.mass = BlockNumber;
		}

		public void AddWeapon(IShipWeapon weapon)
		{
			ShipWeaponGroup group = WeaponGroups.Find((WeaponGroup) => WeaponGroup.num == weapon.GetDefaultOperation());
			if (group==null)
			{
				group = new ShipWeaponGroup();
				WeaponGroups.Add(group);
				if (WeaponGroups.Count == 1)
				{
					currentWeaponIndex = WeaponGroups[0];
				}
				group.shipWeapons.Add(weapon);
				group.num = weapon.GetDefaultOperation();
				group.operationMode = weapon.GetOperationMode();
			}
			else
			{
				group.shipWeapons.Add(weapon);
			}
		}
		private void Update()
		{
			#region 飞船控制状态机
			float v = 0, h = 0, t = 0;//各自由度引擎功率
			if (!SpColding)
			{
				if (mode == ShipControlMode.Player) //手控模式
				{
					#region 飞船形体控制

					v = Input.GetAxis("Vertical");
					h = Input.GetAxis("Horizontal");
					t = Input.GetAxis("Torque");
					if (Input.GetButtonDown("Dash"))
					{
						dashTimer = dashTime;
					}

					//紧急回避代码
					if (dashTimer > 0)
					{
						dashTimer -= Time.fixedDeltaTime;
						v *= dashRate;
						h *= dashRate;
					}

					#endregion
					#region 武器处理
					//武器选择
					foreach (var weaponGroup in WeaponGroups)
					{
						if (Input.GetButtonDown(NumToButton.NumToKey(weaponGroup.num)))
						{
							currentWeaponIndex = WeaponGroups.Find((ShipWeaponGroup g) => g.num == weaponGroup.num);
						}
					}
					//武器使用
					if (currentWeaponIndex != null)
					{
						if (WeaponGroups.Count > 0 && currentWeaponIndex.operationMode == ShipWeaponDefaultOperationMode.Impulse)
						{
							if (Input.GetButtonDown("Fire1"))
							{
								foreach (var weapon in currentWeaponIndex.shipWeapons)
								{
									weapon.WeaponWork();
								}
							}

							if (Input.GetButtonDown("Fire2"))
							{
								foreach (var weapon in currentWeaponIndex.shipWeapons)
								{
									weapon.WeaponSkill();
								}
							}
						}
						if (WeaponGroups.Count > 0 && currentWeaponIndex.operationMode == ShipWeaponDefaultOperationMode.Continuous)
						{
							if (Input.GetButton("Fire1"))
							{
								foreach (var weapon in currentWeaponIndex.shipWeapons)
								{
									weapon.WeaponWork();
								}
							}

							if (Input.GetButton("Fire2"))
							{
								foreach (var weapon in currentWeaponIndex.shipWeapons)
								{
									weapon.WeaponSkill();
								}
							}
						}
					}
					#endregion
				}
			}
			#endregion
			#region 引擎处理
			/*****************************************************************/
			if (v == 0)
				VM = 0;
			else
				VM = (int)Mathf.Sign(v);
			//**************************
			if (h == 0)
				HM = 0;
			else
				HM = (int)Mathf.Sign(h);
			//**************************
			if (t == 0)
				TM = 0;
			else
				TM = (int)Mathf.Sign(t);
			//**************************
			for (int i = VT.Count - 1; i > -1; i--)
			{
				if (VT[i] == null)
				{
					VT.RemoveAt(i);
					continue;
				}
				VT[i].SetWorkState(v, t);
				Sp -= VT[i].GetSPConsume() * Time.deltaTime;
			}
			for (int i = HT.Count - 1; i > -1; i--)
			{
				if (HT[i] == null)
				{
					HT.RemoveAt(i);
					continue;
				}
				HT[i].SetWorkState(h, t);
				Sp -= HT[i].GetSPConsume() * Time.deltaTime;
			}
			#endregion
			#region SP处理

			if (SpColding == false && Sp == 0)
			{
				SpColding = true;
				if (currentWeaponIndex != null && WeaponGroups.Count > 0)
					foreach (var weapon in currentWeaponIndex.shipWeapons)
						{
							weapon.End();
						}
			}

			if (SpColding == true && Sp / MaxSP > SpOutColdingRate)
			{
				SpColding = false;
			}
			Sp += SpRecoverSpeed * Time.deltaTime;
			#endregion
			#region 武器取消
			if (currentWeaponIndex != null&& WeaponGroups.Count > 0)
				if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
					{
						foreach (var weapon in currentWeaponIndex.shipWeapons)
						{
							weapon.End();
						}
					}
			#endregion
		}

		public void RemoveWeapon(IShipWeapon weapon)
		{
			ShipWeaponGroup group = WeaponGroups.Find((WeaponGroup) => WeaponGroup.num == weapon.GetDefaultOperation());
			group.shipWeapons.Remove(weapon);
			if (group.shipWeapons.Count == 0)
			{
				WeaponGroups.Remove(group);
			}
		}
	}
}
