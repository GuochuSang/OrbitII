using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universe;
namespace ProjectMerge
{
	/// <summary>
	/// 飞船建造资源消耗清单
	/// </summary>
	public static class BuildingResourcesList
	{
		public struct BuildingResource
		{
			public ElementID elementID;
			public float amount;
			public BuildingResource(ElementID elementID, float amount)
			{
				this.elementID = elementID;
				this.amount = amount;
			}
		}

		public static void AddConsume(BuildingResourceConsume buildingResourceConsume, ElementType type, ElementIndex index,
			float amount)
		{
			buildingResourceConsume.consumeList.Add(new BuildingResource(new ElementID(type, index), amount));
		}
		public struct BuildingResourceConsume
		{
			public List<BuildingResource> consumeList;
		}
		public static BuildingResourceConsume GetBlockResourceConsume(int blockID)
		{
			BuildingResourceConsume buildingResourceConsume = new BuildingResourceConsume();
			switch (blockID)
			{
				//核心
				case 0:
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.I, 10);
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.II, 10);
					break;
				//结构方块
				case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11:
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.I, 2);
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.II, 2);
					break;
				//引擎
				case 12:
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.I, 20);
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.II, 20);
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.III, 20);
					break;
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
					AddConsume(buildingResourceConsume, ElementType.XD, ElementIndex.I, 10);
					break;
			}

			return buildingResourceConsume;
		}
	}
}
