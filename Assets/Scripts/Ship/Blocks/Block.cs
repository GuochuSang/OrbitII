using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace  ShipProject
{
    public class Block : MonoBehaviour,IComparable,ICamp
    {
        /// <summary>
        /// 方块ID
        /// </summary>
        [TabGroup("Block", "属性")][Tooltip("方块ID")] public int Id;
	    /// <summary>
	    /// 方块等级
	    /// </summary>
	    [TabGroup("Block", "属性")] [Tooltip("方块等级")] public int BlockLevel;
		/// <summary>
		/// 方块名称
		/// </summary>
		[TabGroup("Block", "属性")] [Tooltip("方块名称")] public string BlockName;
		/// <summary>
		/// 是否可以在无名单处添加
		/// </summary>
		[TabGroup("Block", "结构")] [Tooltip("是否可以在无名单处添加")] public bool CanAddWithoutList;
	    /// <summary>
	    /// 是否可以在黑名单处添加
	    /// </summary>
	    [TabGroup("Block", "结构")] [Tooltip("是否可以在黑名单处添加")] public bool CanAddOnBlackList;
		/// <summary>
		/// 是否随组件坐标变换
		/// </summary>
		[TabGroup("Block", "结构")] [Tooltip("是否随组件坐标变换")] public bool PosTransWithComponent;
	    /// <summary>
	    /// 旋转附加
	    /// </summary>
	    [TabGroup("Block", "结构")] [Tooltip("旋转附加")] public ShipUnitRotation Rotation;
		/// <summary>
		/// 镜像附加
		/// </summary>
		[TabGroup("Block", "结构")] [Tooltip("镜像附加")] public ShipUnitMirror Mirror;
		/// <summary>
		/// 隶属飞船
		/// </summary>
		[TabGroup("Block", "结构")][DisableInEditorMode][DisableInPlayMode] [Tooltip("隶属飞船")] public Ship ParentShip;
        /// <summary>
        /// 隶属飞船组件
        /// </summary>
        [TabGroup("Block", "结构")] [DisableInEditorMode] [DisableInPlayMode] [Tooltip("隶属飞船组件")] public ShipComponent ParentShipComponent;
        /// <summary>
        /// 方块坐标
        /// </summary>
        [TabGroup("Block", "结构")] [Tooltip("方块坐标")] [DisableInEditorMode] [DisableInPlayMode] public Vector2Int Pos;
		/// <summary>
		/// 方块上面的方块
		/// </summary>
		[TabGroup("Block", "结构")] [DisableInPlayMode][DisableInEditorMode] [Tooltip("方块上面的方块")] public Block UpBlock;
        /// <summary>
        /// 方块下面的方块
        /// </summary>
        [TabGroup("Block", "结构")] [DisableInPlayMode] [DisableInEditorMode] [Tooltip("方块下面的方块")] public Block DownBlock;
        /// <summary>
        /// 方块右边的方块
        /// </summary>
        [TabGroup("Block", "结构")] [DisableInPlayMode] [DisableInEditorMode] [Tooltip("方块右边的方块")] public Block RightBlock;
        /// <summary>
        /// 方块左边的方块
        /// </summary>
        [TabGroup("Block", "结构")] [DisableInPlayMode] [DisableInEditorMode] [Tooltip("方块左边的方块")] public Block LeftBlock;
		/// <summary>
		/// 白名单坐标（相对）
		/// </summary>
		[TabGroup("Block", "附加规则")] [DisableInPlayMode][Tooltip("白名单坐标（相对）")]
	    public List<Vector2Int> WhiteListPos = new List<Vector2Int>();
		/// <summary>
		///黑名单坐标（相对）
		/// </summary>
		[TabGroup("Block", "附加规则")][DisableInPlayMode][Tooltip("黑名单坐标（相对）")]
	    public List<Vector2Int> BlackListPos = new List<Vector2Int>();
	    /// <summary>
	    ///附加检测（仅component添加下）
	    /// </summary>
	    [TabGroup("Block", "附加规则")]
	    [DisableInPlayMode]
	    [Tooltip("附加检测（仅component添加下）")]
	    public List<Vector2Int> CheckPos= new List<Vector2Int>();
		/// <summary>
		/// 进行递归搜索时的判据
		/// </summary>
		[HideInInspector] public bool searched = false;
        /// <summary>
        /// 从飞船坐标获取局部坐标
        /// </summary>
        /// <param name="shipPos">飞船坐标</param>
        /// <returns></returns>
        public static Vector2 GetLocalPositionFromShipPos(Vector2Int shipPos)
        {
            return new Vector2(shipPos.x*1.6f,shipPos.y*1.6f);
        }
/// <summary>
/// 飞船状态更新事件
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="up">上状态</param>
/// <param name="down">下状态</param>
/// <param name="left">左状态</param>
/// <param name="right">右状态</param>
/// <returns>返回值泛型</returns>
	    public  delegate void BlockStateChangeDelegate(NearbyState up,NearbyState down,NearbyState left,NearbyState right);
	    public event BlockStateChangeDelegate BlockStateChangeEventInt;
		public bool isShipBlock = true;
	    /// <summary>
	    /// 飞船刷新相邻状态时调用该函数
	    /// </summary>
	    public void OnRefresh()
	    {
		    NearbyState up=0, down=0, left=0, right=0;
			//up
		    if (UpBlock == null)
		    {
			    up = NearbyState.NULL;
		    }

			else if (UpBlock.Id == Id)
		    {
			    up = NearbyState.SAMEID;
		    }
		    else
		    {
			    up = NearbyState.DIFID;
		    }
			//down
		    if (DownBlock == null)
		    {
			    down = NearbyState.NULL;
		    }

		    else if (DownBlock.Id == Id)
		    {
			    down = NearbyState.SAMEID;
		    }
		    else
		    {
			    down = NearbyState.DIFID;
		    }
			//left
		    if (LeftBlock == null)
		    {
			    left = NearbyState.NULL;
		    }

		    else if (LeftBlock.Id == Id)
		    {
			    left = NearbyState.SAMEID;
		    }
		    else
		    {
			    left = NearbyState.DIFID;
		    }
			//right
			if (RightBlock == null)
			{
				right = NearbyState.NULL;
			}

			else if (RightBlock.Id == Id)
			{
				right = NearbyState.SAMEID;
			}
			else
			{
				right = NearbyState.DIFID;
			}

		    if (BlockStateChangeEventInt != null) BlockStateChangeEventInt(up, down, left, right);
	    }
		/// <summary>
		/// 根据ID顺序排序
		/// </summary>
		/// <param name="obj">其它block</param>
		/// <returns>相对大小</returns>
	    public int CompareTo(object obj)
		{
			Block other = obj as Block;
			if (other.Id > Id)
				return -1;
			if (other.Id == Id)
				return 0;
			return 1;
		}
	    public GameCamp GetCamp()
	    {
		    return ParentShip.GetCamp();
	    }

	    public void SetCamp(GameCamp camp)
	    {
		    throw ShipProjectExpection.SetCampException(1);
	    }
		public void OnDestroy()
		{
			if (!isShipBlock)
				return;
			foreach (var w in Ship.PosTransList(WhiteListPos, Pos, Rotation, Mirror))
			{
				if (ParentShip.WhiteListPos.Contains(w))
					for (int i = ParentShip.WhiteListPos.Count - 1; i > -1; i--)
					{
						if (ParentShip.WhiteListPos[i] == w)
						{
							ParentShip.WhiteListPos.RemoveAt(i);
							break;
						}
					}
			}
			foreach (var b in Ship.PosTransList(BlackListPos, Pos, Rotation,Mirror))
			{
				if (ParentShip.BlackListPos.Contains(b))
					for (int i = ParentShip.BlackListPos.Count - 1; i > -1; i--)
					{
						if (ParentShip.BlackListPos[i] == b)
						{
							ParentShip.BlackListPos.RemoveAt(i);
							break;
						}
					}
			}
			if (ParentShip.GetComponent<ShipControl>().mode == ShipControlMode.Building)
			{
				return;
			}
			GameObject obj = Instantiate(gameObject, transform.position, transform.rotation);
			Block bl = obj.GetComponent<Block>();
			bl.isShipBlock = false;
			Destroy(bl);
			Destroy(obj.GetComponent<BoxCollider2D>());
			Rigidbody2D rig = obj.AddComponent<Rigidbody2D>();
			rig.gravityScale = 0f;
			if (ParentShip.core != null)
			rig.AddForceAtPosition(Random.insideUnitCircle*20f, transform.position, ForceMode2D.Impulse);
			obj.AddComponent<SelfDestroy>();
		}
	}
}
