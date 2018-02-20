using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum.ThrusterEnum;
using UnityEngine;
using Sirenix.OdinInspector;
namespace ShipProject
{
	namespace ShipEnum
	{
		namespace ThrusterEnum
		{
			/// <summary>
			/// 引擎工作状态
			/// </summary>
			public enum ThrustWorkState
			{
				Await=0,
				Move=1,
			}
		}
	}
	/// <summary>
	/// 引擎类
	/// </summary>
	public class ShipThruster : MonoBehaviour,ISPConsumer
	{
		[HideInInspector]
		public ShipControl control;
		[HideInInspector]
		public ShipComponent shipComponent;
		[HideInInspector]
		public Ship ship;
		/// <summary>
		/// 平动推进力
		/// </summary>
		[Tooltip("平动推进力")][TabGroup("Thrust", "引擎工作")]
		public float ThrustMovePower = 10f;
		/// <summary>
		/// 角动推进力
		/// </summary>
		[Tooltip("角动推进力")]
		[TabGroup("Thrust", "引擎工作")]
		public float ThrustTorquePower = 10f;
		/// <summary>
		/// 工作状态
		/// </summary>
		[Tooltip("工作状态")]
		[TabGroup("Thrust", "引擎工作")]
		public ThrustWorkState workState;
		/// <summary>
		/// 平动SP消耗
		/// </summary>
		[Tooltip("平动SP消耗")]
		[TabGroup("Thrust", "SP相关")]
		public float MoveSpConsume = 10f;
		/// <summary>
		/// 角动SP消耗
		/// </summary>
		[Tooltip("角动SP消耗")]
		[TabGroup("Thrust", "SP相关")]
		public float TorqueSpConsume = 10f;
		/// <summary>
		/// 引擎旋转
		/// </summary>
		[HideInInspector]
		public ShipEnum.ShipUnitRotation rotation
		{
			get { return shipComponent.Rotation; }
		}
		/// <summary>
		/// 平动功率附加值
		/// </summary>
		public float MoveAddedValue;
		/// <summary>
		/// 角动功率附加值
		/// </summary>
		public float TorqueAddedValue;
		public GameObject EffectFlame;

		private Vector2 dir
		{
			get
			{
				switch (rotation)
				{
					case ShipEnum.ShipUnitRotation.d0:
						return control.transform.up;
						break;
					case ShipEnum.ShipUnitRotation.d90:
						return -control.transform.right;
						break;
					case ShipEnum.ShipUnitRotation.d180:
						return -control.transform.up;
						break;
					case ShipEnum.ShipUnitRotation.d270:
						return control.transform.right;
						break;
				}

				return Vector3.zero;
			}
		}
		public void Start()
		{
			shipComponent = GetComponent<ShipComponent>();
			ship = shipComponent.ParentShip;
			control = ship.GetComponent<ShipControl>();
			control.AddThruster(this);
		}
		/// <summary>
		/// 引擎SP消耗
		/// </summary>
		/// <returns></returns>
		public float GetSPConsume()
		{
			switch (workState)
			{
				case ThrustWorkState.Await:
					return 0f;
					break;
				case ThrustWorkState.Move:
					return MoveSpConsume*Mathf.Abs(MoveAddedValue)+ TorqueSpConsume * Mathf.Abs(TorqueAddedValue);
					break;
			}
			return 0;
		}
		/// <summary>
		/// 设置引擎工作状态
		/// </summary>
		/// <param name="state">工作状态</param>
		public void SetWorkState(float Move,float Torque)
		{
			ThrustWorkState state;
			if (isMoveWork() || isTorqueWork())
			{
				state = ThrustWorkState.Move;
			}
			else
			{
				state = ThrustWorkState.Await;
			}
			workState = state;
			MoveAddedValue = Move;
			TorqueAddedValue = Torque;
			switch (state)
			{
				case ThrustWorkState.Await:
					EffectFlame.SetActive(false);
					break;
				case ThrustWorkState.Move:
					EffectFlame.SetActive(true);
					break;
			}
		}
		private void FixedUpdate()
		{
			switch (workState)//引擎状态机
			{
				case ThrustWorkState.Await:
					break;
				case ThrustWorkState.Move:
					if (isMoveWork())
					{
						control.rig2d.AddForceAtPosition(ThrustMovePower * Mathf.Abs(MoveAddedValue) *transform.up,
							transform.position, ForceMode2D.Force);
					}
					if (isTorqueWork())
						control.rig2d.AddTorque(TorqueAddedValue * ThrustTorquePower);
					break;
			}
		}

		/// <summary>
		/// 是否作为旋转工作引擎
		/// </summary>
		/// <returns></returns>
		private bool isTorqueWork()
		{
				Vector3 L = transform.localPosition - (Vector3)control.rig2d.centerOfMass;
				Vector3 dir=Vector3.zero;
				switch (rotation)
				{
					case ShipEnum.ShipUnitRotation.d0:
						dir= Vector3.up;
						break;
					case ShipEnum.ShipUnitRotation.d90:
						dir = Vector3.left;
						break;
					case ShipEnum.ShipUnitRotation.d180:
						dir = Vector3.down;
						break;
					case ShipEnum.ShipUnitRotation.d270:
						dir = Vector3.right;
						break;
				}
				Vector3 Cross = Vector3.Cross(L, dir);
				if (Cross.z*TorqueAddedValue > 0)
					return true;
			return false;
		}

		private bool isMoveWork()
		{
			int dir = 0;
			switch (rotation)
			{
				case ShipEnum.ShipUnitRotation.d0:
					dir = 1;
					break;
				case ShipEnum.ShipUnitRotation.d90:
					dir = -1;
					break;
				case ShipEnum.ShipUnitRotation.d180:
					dir = -1;
					break;
				case ShipEnum.ShipUnitRotation.d270:
					dir = 1;
					break;
			}
			if (dir * MoveAddedValue > 0)
				return true;
			return false;
		}
	}
}

