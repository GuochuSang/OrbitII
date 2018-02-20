using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  ShipProject
{
	namespace ShipEnum
	{
		public enum ShipComponentType
		{
			Control=0,
			Armor=1,
			Dynamic=2,
			Storage=4,
			Weapon=5
		}
	}
	public class ShipComponent : SerializedMonoBehaviour,IComparable
	{
		/// <summary>
		///组件名称 
		/// </summary>
		[DisableInPlayMode][Tooltip("组件名称")]
		[TabGroup("组件", "属性")]
		public string ShipComponentName;
		/// <summary>
		/// 组件ID
		/// </summary>
		[DisableInPlayMode]
		[Tooltip("组件ID")]
		[TabGroup("组件", "属性")]
		public int Id;
		/// <summary>
		/// 组件等级
		/// </summary>
		[TabGroup("组件", "属性")] [Tooltip("组件等级")] [DisableInPlayMode] public int ShipCompoionentLevel;
		/// <summary>
		///组件分类
		/// </summary>
		[DisableInPlayMode]
		[Tooltip("组件分类")]
		[TabGroup("组件", "属性")]
		public ShipComponentType ShipComponentType;
		/// <summary>
		/// 组件旋转
		/// </summary>
		[DisableInPlayMode] [DisableInEditorMode] [Tooltip("组件旋转")] [TabGroup("组件", "属性")]
		public ShipUnitRotation Rotation;
		/// <summary>
		/// 组件镜像
		/// </summary>
		[DisableInPlayMode]
		[DisableInEditorMode]
		[Tooltip("组件镜像")]
		[TabGroup("组件", "属性")]
		public ShipUnitMirror Mirror;
		/// <summary>
		/// 组件原点的飞船坐标
		/// </summary>
		[DisableInPlayMode][DisableInEditorMode][Tooltip("组件原点的飞船坐标")][TabGroup("组件","属性")]
		public Vector2Int pos;
		/// <summary>
		/// 组件清单
		/// </summary>
		public class ComponentBlock
		{
			/// <summary>
			/// 组件位置
			/// </summary>
			public int posX,posY;
			/// <summary>
			/// 组建id
			/// </summary>
			public int id;
		}
		/// <summary>
		/// 组件清单
		/// </summary>
		[Tooltip("组件清单")]
		[TabGroup("组件", "清单")]
		public List<ComponentBlock> BlockList=new List<ComponentBlock>();
		/// <summary>
		/// 组件的方块组
		/// </summary>
		[HideInInspector]
		public Dictionary<Vector2Int,Block> Blocks=new Dictionary<Vector2Int, Block>();
		/// <summary>
		/// 隶属于飞船
		/// </summary>
		[DisableInPlayMode]
		[DisableInEditorMode]
		[Tooltip("隶属于飞船")]
		[TabGroup("组件", "属性")]
		public Ship ParentShip;
		/// <summary>
		/// UI标识
		/// </summary>
		[Tooltip("UI标识")]
		[TabGroup("组件", "属性")]
		public Sprite SpriteOnUI;
		/// <summary>
		/// 根据ID顺序排序
		/// </summary>
		/// <param name="obj">其它block</param>
		/// <returns>相对大小</returns>
		public int CompareTo(object obj)
		{
			ShipComponent other = obj as ShipComponent;
			if (other.Id > Id)
				return -1;
					if (other.Id == Id)
				return 0;
			return 1;
		}
	}
}
