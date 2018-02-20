using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using ShipProject.ShipEnum.Storage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	namespace ShipEnum
	{
		namespace Storage
		{
			public enum StorageState
			{
				O=0,
				TL=1,
				TR=2,
				THL=3,
				THM=4,
				THR=5
			}
		}
	}
	public class StorageMerge : MonoBehaviour
	{
		/// <summary>
		/// 方块引用
		/// </summary>
		[HideInInspector]
		public Block block;
		/// <summary>
		/// 箱子贴图状态
		/// </summary>
		[DisableInPlayMode][DisableInEditorMode]
		public StorageState state = StorageState.O;
		/// <summary>
		/// 箱子贴图
		/// </summary>
		public Sprite[] StorageSprites = new Sprite[6];
		/// <summary>
		/// 贴图渲染器
		/// </summary>
		[HideInInspector]
		public SpriteRenderer renderer;
		private void Start()
		{
			renderer = GetComponent<SpriteRenderer>();
			block = GetComponent<Block>();
			if (block.ParentShipComponent.Id == 13)
			{
			}

			if (block.ParentShipComponent.Id == 14)
			{
				if (block.ParentShipComponent.Blocks[new Vector2Int(0, 0)] == block)
					state = StorageState.TL;
				if (block.ParentShipComponent.Blocks[new Vector2Int(1, 0)] == block)
					state = StorageState.TR;
			}

			if (block.ParentShipComponent.Id == 15)
			{
				if (block.ParentShipComponent.Blocks[new Vector2Int(0, 0)] == block)
					state = StorageState.THL;
				if (block.ParentShipComponent.Blocks[new Vector2Int(1, 0)] == block)
					state = StorageState.THM;
				if (block.ParentShipComponent.Blocks[new Vector2Int(2, 0)] == block)
					state = StorageState.THR;
			}

			renderer.sprite = StorageSprites[(int) state];
		}
		}
	}
