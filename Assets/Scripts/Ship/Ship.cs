using System;
using System.Collections.Generic;
using System.Linq;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShipProject
{
	namespace ShipEnum
	{
		/// <summary>
		///     飞船元件旋转
		/// </summary>
		public enum ShipUnitRotation
		{
			d0 = 0,
			d90 = 1,
			d180 = 2,
			d270 = 3
		}

		/// <summary>
		///     飞船元件镜像
		/// </summary>
		public enum ShipUnitMirror
		{
			Normal = 0,
			MirrorX = 1,
			MirrorY=2,
			MirrorBoth=3
		}
		/// <summary>
		/// 飞船状态
		/// </summary>
		public enum ShipState
		{
			Building=0,
			Working=1
		}
	}
	/// <summary>
	///  飞船异常类
	/// </summary>
	public class ShipProjectExpection:Exception
	{
		public ShipProjectExpection(string message):base(message)
		{
		}
		public override string Message
		{
			get
			{
				return base.Message;
			}
		}

		public static ShipProjectExpection SetCampException(int id)
		{
			if (id == 1)
				return new ShipProjectExpection("阵营从父层级取得，无法修改");
			else
				return null;
		}
	}
	/// <summary>
	///     飞船的结构管理类
	/// </summary>
	public class Ship : MonoBehaviour
	{
		/// <summary>
		///     飞船名称（即型号）
		/// </summary>
		[TabGroup("飞船", "属性")] [DisableInPlayMode][Tooltip(" 飞船名称（即型号）")]
		public string ShipName;
		/// <summary>
		/// 飞船当前工作状态
		/// </summary>
		[TabGroup("飞船", "属性")]
		[DisableInPlayMode][Tooltip("飞船当前工作状态")]
		public ShipState ShipCurrentState;
		/// <summary>
		/// 飞船阵营
		/// </summary>
		[TabGroup("飞船", "属性")]
		[DisableInPlayMode]
		[Tooltip("飞船阵营")]
		public GameCamp ShipCamp;
		/// <summary>
		///     飞船的方块组
		/// </summary>
		[TabGroup("飞船", "元件组")] [DisableInEditorMode] [DisableInPlayMode]
		[Tooltip("飞船的方块组")]
		public Dictionary<Vector2Int, Block> Blocks = new Dictionary<Vector2Int, Block>();
		/// <summary>
		/// 飞船的组件组
		/// </summary>
		[TabGroup("飞船", "元件组")]
		[DisableInEditorMode]
		[DisableInPlayMode]
		[Tooltip("飞船的组件组")]
		public Dictionary<Vector2Int, ShipComponent> ShipComponents = new Dictionary<Vector2Int, ShipComponent>();
		/// <summary>
		/// 白名单坐标
		/// </summary>
		//[HideInInspector]
		public List<Vector2Int> WhiteListPos=new List<Vector2Int>();
		/// <summary>
		/// 黑名单坐标
		/// </summary>
		//[HideInInspector]
		public List<Vector2Int> BlackListPos=new List<Vector2Int>();
		/// <summary>
		/// 核心
		/// </summary>
		[HideInInspector]
		public Block core;
		/// <summary>
		/// 方块改变事件
		/// </summary>
		public event Action<int> BlocksChanged; 
		/// <summary>
		///     在飞船坐标pos上添加某ID的方块
		/// </summary>
		/// <param name="id">方块ID</param>
		/// <param name="level">方块等级</param>
		/// <param name="pos">飞船坐标</param>
		/// <param name="rotation">元件旋转附加</param>
		/// <param name="mirror">元件镜像附加</param>
		public Block AddBlockAtPosition(int id,int level, Vector2Int pos, ShipUnitRotation rotation = ShipUnitRotation.d0,ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			var blockPrefab = GetBlock(id,level);
			if (!Putable(id,level, pos))
			{
				return null;
			}
			var blockTransform = Instantiate(blockPrefab.gameObject, transform).transform;
			var block = blockTransform.GetComponent<Block>();
			if (id == 0)
			{
				core = block;
			}
			blockTransform.localPosition = Block.GetLocalPositionFromShipPos(pos);
			var rot = Vector3.zero;
			//镜像处理
			switch (mirror)
			{
				case ShipUnitMirror.Normal:
					break;
				case ShipUnitMirror.MirrorX:
					rot.x = 180;
					break;
				case ShipUnitMirror.MirrorY:
					rot.y = 180;
					break;
				case ShipUnitMirror.MirrorBoth:
					rot.x = 180;
					rot.y = 180;
					break;
			}
			//旋转处理
			switch (rotation)
			{
				case ShipUnitRotation.d0:
					rot.z = 0;
					break;
				case ShipUnitRotation.d90:
					rot.z = 90;
					break;
				case ShipUnitRotation.d180:
					rot.z = 180;
					break;
				case ShipUnitRotation.d270:
					rot.z = 270;
					break;
			}

			if (block.PosTransWithComponent)
			{
				blockTransform.rotation = Quaternion.Euler(rot);
			}
			block.Rotation = rotation;
			block.Mirror = mirror;
			block.ParentShip = this;
			block.Pos = pos;
			Blocks.Add(pos,block);
			AddPosList(block,pos,rotation,mirror);
			SetFourWaysNearby(pos);
			if (BlocksChanged != null) BlocksChanged(Blocks.Count);
			return block;
		}
		/// <summary>
		/// 移除某坐标的方块
		/// </summary>
		/// <param name="pos">飞船坐标</param>
		public void RemoveBlockAtPosition(Vector2Int pos)
		{
			if (!Blocks.ContainsKey(pos))
			{
				Debug.Log("坐标没有方块");
				return;
			}

			if (Blocks[pos].ParentShipComponent != null)
			{
				Vector2Int p = Blocks[pos].ParentShipComponent.pos;
				Destroy(ShipComponents[p].gameObject);
				ShipComponents.Remove(p);
			}
			Destroy(Blocks[pos].gameObject);
			Blocks.Remove(pos);
			SetFourWaysNearby(pos);
			if (BlocksChanged != null) BlocksChanged(Blocks.Count);
		}

		private void WhiteExcept(Vector2Int pos)
		{

		}
		/// <summary>
		/// 设置某坐标处方块的相邻方块
		/// </summary>
		/// <param name="pos">飞船坐标</param>
		public void SetNearby(Vector2Int pos)
		{
			if (!Blocks.ContainsKey(pos)) return;//坐标处无方块返回
			Blocks[pos].UpBlock = Blocks.ContainsKey(pos + new Vector2Int(0, 1)) ? Blocks[pos + new Vector2Int(0, 1)] : null;
			Blocks[pos].DownBlock = Blocks.ContainsKey(pos + new Vector2Int(0, -1)) ? Blocks[pos + new Vector2Int(0, -1)] : null;
			Blocks[pos].RightBlock = Blocks.ContainsKey(pos + new Vector2Int(1, 0)) ? Blocks[pos + new Vector2Int(1, 0)] : null;
			Blocks[pos].LeftBlock = Blocks.ContainsKey(pos + new Vector2Int(-1,0)) ? Blocks[pos + new Vector2Int(-1, 0)] : null;
			Blocks[pos].OnRefresh();
		}
		/// <summary>
		/// 对周围方块状态进行重设定
		/// </summary>
		/// <param name="pos"></param>
		public void SetFourWaysNearby(Vector2Int pos)
		{
			SetNearby(pos);
			SetNearby(pos + new Vector2Int(0, 1));
			SetNearby(pos + new Vector2Int(0, -1));
			SetNearby(pos + new Vector2Int(1, 0));
			SetNearby(pos + new Vector2Int(-1, 0));
		}

		public bool ComponentPutable(int id, int level, Vector2Int pos, ShipUnitRotation rotation = ShipUnitRotation.d0,
			ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			if (GetShipComponent(id, level) == null)
			{
				return false;
				Debug.Log("1");
			}
			if (id == 0 && core != null)
			{
				return false;
				Debug.Log("2");

			}
			List<Vector2Int> whiteList = new List<Vector2Int>(), blackList = new List<Vector2Int>();
			ShipComponent component = GetShipComponent(id, level);
			bool whiteCheck = false, blackCheck = true;
			foreach (var bk in component.BlockList)
			{
				if (!GetBlock(bk.id, level).CanAddWithoutList && WhiteListPos.Contains(PosTrans(new Vector2Int(bk.posX, bk.posY), rotation, mirror) + pos))
				{
					whiteCheck = true;
				}

				if (GetBlock(bk.id, level).CanAddWithoutList)
				{
					whiteCheck = true;
				}
			}

			Debug.Log("white" + whiteCheck);
			foreach (var bk in component.BlockList)
			{
				if ((Blocks.ContainsKey(PosTrans(new Vector2Int(bk.posX, bk.posY), rotation, mirror) + pos)))
				{
					blackCheck = false;
					Debug.Log("b1");
				}

				if (((BlackListPos.Contains(PosTrans(new Vector2Int(bk.posX, bk.posY), rotation, mirror) + pos)) &&
				     (!GetBlock(bk.id, level).CanAddOnBlackList)))
				{
					blackCheck = false;
					Debug.Log("b2");

				}
				foreach (var p in GetBlock(bk.id, level).CheckPos)
				{
					if (Blocks.ContainsKey(PosTrans(new Vector2Int(bk.posX, bk.posY) + p, rotation, mirror) + pos))
					{
						blackCheck = false;
						Debug.Log("b3");

					}
				}
			}
			if (!whiteCheck || !blackCheck)
			{
				return false;
				Debug.Log("3");
			}
			return true;
		}
		/// <summary>
		/// 添加某个ID的组件
		/// </summary>
		/// <param name="id">组件ID</param>
		/// <param name="level">组件等级</param>
		/// <param name="pos">组件原点坐标</param>
		/// <param name="rotation">组件旋转</param>
		/// <param name="mirror">组件镜像</param>
		public ShipComponent AddComponent(int id,int level, Vector2Int pos, ShipUnitRotation rotation = ShipUnitRotation.d0,ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{

			if (!ComponentPutable(id, level, pos, rotation, mirror))
			{
				return null;
			}
			ShipComponent component = GetShipComponent(id, level);
			var componentTransform= Instantiate(GetShipComponent(id,level).gameObject, transform).transform;
			var shipComponent = componentTransform.GetComponent<ShipComponent>();
			componentTransform.localPosition= Block.GetLocalPositionFromShipPos(pos);
			var rot = Vector3.zero;
			//镜像处理
			switch (mirror)
			{
				case ShipUnitMirror.Normal:
					break;
				case ShipUnitMirror.MirrorX:
					rot.x = 180;
					break;
				case ShipUnitMirror.MirrorY:
					rot.y = 180;
					break;
				case ShipUnitMirror.MirrorBoth:
					rot.x = 180;
					rot.y = 180;
					break;
			}
			//旋转处理
			switch (rotation)
			{
				case ShipUnitRotation.d0:
					rot.z = 0;
					break;
				case ShipUnitRotation.d90:
					rot.z = 90;
					break;
				case ShipUnitRotation.d180:
					rot.z = 180;
					break;
				case ShipUnitRotation.d270:
					rot.z = 270;
					break;
			}
			componentTransform.rotation = Quaternion.Euler(rot);
			shipComponent.ParentShip = this;
			shipComponent.pos = pos;
			shipComponent.Rotation = rotation;
			shipComponent.Mirror = mirror;
			ShipComponents.Add(pos,shipComponent);
			shipComponent.Blocks.Clear();
			foreach (var bk in component.BlockList)
			{
				Block blo = ForceAddBlockAtPosition(bk.id, level, PosTrans(new Vector2Int(bk.posX, bk.posY), rotation, mirror) + pos,rotation, mirror);
				shipComponent.Blocks.Add(new Vector2Int(bk.posX, bk.posY), blo);
				if (blo != null)
				{
					blo.ParentShipComponent = shipComponent;
					blo.transform.parent = shipComponent.transform;
				}
			}
			return shipComponent;
		}
		/// <summary>
		/// 坐标转换函数
		/// </summary>
		/// <param name="orginPos">需转换的坐标</param>
		/// <param name="rotation">旋转变换</param>
		/// <param name="mirror">镜像变换</param>
		/// <returns>转换后坐标</returns>
		public static Vector2Int PosTrans(Vector2Int orginPos, ShipUnitRotation rotation = ShipUnitRotation.d0,ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			Vector2Int pos = orginPos;

			switch (mirror)
			{
				case ShipUnitMirror.Normal:
					break;
				case ShipUnitMirror.MirrorX:
					pos.x = -pos.x;
					break;
				case ShipUnitMirror.MirrorY:
					pos.y = -pos.y;
					break;
				case ShipUnitMirror.MirrorBoth:
					pos.x = -pos.x;
					pos.y = -pos.y;
					break;
			}
			switch (rotation)
			{
				case ShipUnitRotation.d0:
					break;
				case ShipUnitRotation.d90:
					pos=new Vector2Int(-pos.y,pos.x);
					break;
				case ShipUnitRotation.d180:
					pos= new Vector2Int(-pos.x, -pos.y);
					break;
				case ShipUnitRotation.d270:
					pos=new Vector2Int(pos.y,-pos.x);
					break;
			}
			return pos;
		}
		/// <summary>
		/// 坐标组转换函数
		/// </summary>
		/// <param name="PosList">坐标组</param>
		/// <param name="offset">偏移</param>
		/// <param name="rotation">旋转</param>
		/// <param name="mirror">镜像</param>
		/// <returns></returns>
		public static List<Vector2Int> PosTransList(List<Vector2Int> PosList ,Vector2Int offset,ShipUnitRotation rotation = ShipUnitRotation.d0,
			ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			List<Vector2Int> poses = new List<Vector2Int>(PosList);
			for (int i = 0; i < poses.Count; i++)
			{
				poses[i] = PosTrans(poses[i], rotation, mirror)+offset;
			}
			return poses;
		}
		/// <summary>
		/// 从block中添加规则列表
		/// </summary>
		/// <param name="block">方块</param>
		/// <param name="pos">方块被添加的坐标（偏移）</param>
		/// <param name="rotation">旋转附加</param>
		/// <param name="mirror">镜像附加</param>
		public void AddPosList(Block block,Vector2Int pos, ShipUnitRotation rotation = ShipUnitRotation.d0,ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			List<Vector2Int> white=new List<Vector2Int>(), black=new List<Vector2Int>();
			//添加白名单
			foreach (var wpos in block.WhiteListPos)
			{
				WhiteListPos.Add(PosTrans(wpos,rotation,mirror)+pos);
			}

			foreach (var bpos in block.BlackListPos)
			{
				BlackListPos.Add(PosTrans(bpos, rotation, mirror)+pos);
			}
		}
		/// <summary>
		/// 查找函数，递归查找返回所有与block相连的方块
		/// </summary>
		/// <param name="block">相连的方块</param>
		/// <returns></returns>
		private List<Block> Search(Block block)
		{
			if (block.searched)
			{
				return  new List<Block>();
			}
			block.searched = true;
			List<Block> up=new List<Block>(), down = new List<Block>(), left = new List<Block>(), right = new List<Block>();
			List<Block> theList = new List<Block>();
			theList.Add(block);
			if (block.UpBlock != null)
			{
				up = Search(block.UpBlock);
			}
			if (block.DownBlock != null)
			{
				down = Search(block.DownBlock);
			}
			if (block.LeftBlock != null)
			{
				left = Search(block.LeftBlock);
			}
			if (block.RightBlock != null)
			{
				right = Search(block.RightBlock);
			}
			return theList.Union(up).Union(down).Union(left).Union(right).ToList();
		}
		/// <summary>
		/// 查找函数，查找返回所有与block相邻的block
		/// </summary>
		/// <param name="block"></param>
		/// <returns>列表</returns>
		public List<Block> SearchBlocks(Block block)
		{
			foreach (var bk in Blocks.Values)
			{
				bk.searched = false;
			}
			return Search(block);
		}
		/// <summary>
		/// 获得方块模型
		/// </summary>
		/// <param name="id">方块id</param>
		/// <param name="level">方块等级</param>
		/// <returns></returns>
		public static Block GetBlock(int id,int level)
		{
			return ShipUnitLister.GetInstance().GetUnit<Block>(id,level);
		}
		/// <summary>
		/// 获得飞船组件模型
		/// </summary>
		/// <param name="id">组件ID</param>
		/// <returns></returns>
		public static ShipComponent GetShipComponent(int id,int level)
		{
			return ShipUnitLister.GetInstance().GetUnit<ShipComponent>(id,level);
		}
		/// <summary>
		/// 某ID的方块是否可以被放置在pos上
		/// </summary>
		/// <param name="id">ID</param>
		/// <param name="level">组件等级</param>
		/// <param name="pos">飞船坐标</param>
		/// <returns></returns>
		public bool Putable(int id,int level, Vector2Int pos)
		{
			//检测坐标占用
			if (Blocks.ContainsKey(pos))
			{
				Debug.Log("坐标被占用");
				return false;
			}

			var blockPrefab = GetBlock(id,level);
			//检测是否需要名单
			if (!blockPrefab.CanAddWithoutList)
			{
				if (!blockPrefab.CanAddOnBlackList)
				{
					if (BlackListPos.Contains(pos))
					{
						return false;
					}
				}

				if (!WhiteListPos.Contains(pos) && !BlackListPos.Contains(pos))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 替换组件
		/// </summary>
		/// <param name="id"></param>
		/// <param name="level"></param>
		/// <param name="pos"></param>
		/// <param name="rotation"></param>
		/// <param name="mirror"></param>
		public void ReplaceComponent(int id, int level, Vector2Int Dpos,Vector2Int Ppos, ShipUnitRotation rotation = ShipUnitRotation.d0,ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			RemoveBlockAtPosition(Dpos);
			AddComponent(id, level, Ppos, rotation, mirror);
		}
		public GameCamp GetCamp()
		{
			return ShipCamp;
		}
		public void SetCamp(GameCamp camp)
		{
			ShipCamp = camp;
		}
		/// <summary>
		/// 含递归搜索方块崩落
		/// </summary>
		/// <param name="pos"></param>
		public void CollapseBlockWithRecursion(Vector2Int pos)
		{
			if (core.Pos == pos)
			{
				CollapseShip();
				return;
			}
			RemoveBlockAtPosition(pos);
			List<Block> connected = SearchBlocks(core);
			List<Block> disconnected = Blocks.Values.ToList<Block>().Except(connected).ToList();
			foreach (var bk in disconnected)
			{
				RemoveBlockAtPosition(bk.Pos);
			}
		}
		public Block ForceAddBlockAtPosition(int id, int level, Vector2Int pos, ShipUnitRotation rotation = ShipUnitRotation.d0, ShipUnitMirror mirror = ShipUnitMirror.Normal)
		{
			var blockPrefab = GetBlock(id, level);
			var blockTransform = Instantiate(blockPrefab.gameObject, transform).transform;
			var block = blockTransform.GetComponent<Block>();
			if (id == 0)
			{
				core = block;
			}
			blockTransform.localPosition = Block.GetLocalPositionFromShipPos(pos);
			var rot = Vector3.zero;
			//镜像处理
			switch (mirror)
			{
				case ShipUnitMirror.Normal:
					break;
				case ShipUnitMirror.MirrorX:
					rot.x = 180;
					break;
				case ShipUnitMirror.MirrorY:
					rot.y = 180;
					break;
				case ShipUnitMirror.MirrorBoth:
					rot.x = 180;
					rot.y = 180;
					break;
			}
			//旋转处理
			switch (rotation)
			{
				case ShipUnitRotation.d0:
					rot.z = 0;
					break;
				case ShipUnitRotation.d90:
					rot.z = 90;
					break;
				case ShipUnitRotation.d180:
					rot.z = 180;
					break;
				case ShipUnitRotation.d270:
					rot.z = 270;
					break;
			}

			if (block.PosTransWithComponent)
			{
				blockTransform.rotation = Quaternion.Euler(rot);
			}
			block.Rotation = rotation;
			block.Mirror = mirror;
			block.ParentShip = this;
			block.Pos = pos;
			Blocks.Add(pos, block);
			AddPosList(block, pos, rotation, mirror);
			SetFourWaysNearby(pos);
			if (BlocksChanged != null) BlocksChanged(Blocks.Count);
			return block;
		}
		public void CollapseShip()
		{
			Destroy(gameObject);
		}

		#region Test

		public int id,level;
		public ShipUnitRotation rotation;
		public ShipUnitMirror mirror;
		public Vector2Int pos;
		public List<Block> 相邻测试=new List<Block>();
		public bool Component;
		[Button("Add")]
		private void Add()
		{
			if(!Component)
			AddBlockAtPosition(id,level,pos,rotation,mirror);
			else
			AddComponent(id,level, pos, rotation, mirror);
		}

		[Button("Remove")]
		private void Remove()
		{
			RemoveBlockAtPosition(pos);
		}
		[Button("Collapse")]
		private void Collapse()
		{
			CollapseBlockWithRecursion(pos);
		}

		[Button("查")]
		private void 查()
		{
			相邻测试 = SearchBlocks(Blocks[pos]);
		}
		#endregion
	}
}