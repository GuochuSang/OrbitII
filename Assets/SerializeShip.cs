using System.Collections;
using System.Collections.Generic;
using Manager;
using ShipProject.ShipEnum;
using ShipSeriazble;
using UnityEngine;

namespace ShipProject
{
	public class SerializeShip : MonoBehaviour
	{
		public SerializeShipData data;
	}
	[System.Serializable]
	public class SerializeShipData
	{
		public string shipName;
		public GameCamp shipCamp;
		public Vector3Data position;
		public Vector3Data rotation;
		public Vector3Data scale;
		public float Sp;
		public bool isColding;
		public ShipControlMode mode;
		public bool isCurrentShip=false;
		public bool isControllingShip=false;
		public Dictionary<SerializableVector2Int, BlockHealthData> blockHealthData = new Dictionary<SerializableVector2Int, BlockHealthData>();
	}
}

namespace ShipSeriazble
{
	[System.Serializable]
	public class BlockHealthData
	{
		public float health;
		public BlockHealthData(float healthRate)
		{
			health = healthRate;
		}
	}
	[System.Serializable]
	public class SerializableVector2Int
	{
		public int x, y;

		public static explicit operator Vector2Int(SerializableVector2Int svec)
		{
			Vector2Int vec2 = new Vector2Int
			{
				x = svec.x,
				y = svec.y
			};
			return vec2;
		}
		public static explicit operator SerializableVector2Int(Vector2Int svec)
		{
			return  new SerializableVector2Int(svec);
		}
		public SerializableVector2Int (Vector2Int vec2)
		{
			x = vec2.x;
			y = vec2.y;
		}
	}
}
