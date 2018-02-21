using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using S.Serialize;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
		public class ShipFactory : MonoBehaviour
		{
			/// <summary>
			/// 空飞船模板
			/// </summary>
			public GameObject EmptyShip;
			public void SaveShip(Ship ship)
			{
				List<ShipData.PlaceRecord> records = new List<ShipData.PlaceRecord>();
				foreach (var component in ship.ShipComponents.Values)
				{
					ShipData.PlaceRecord record;
					record.id = component.Id;
					record.level = component.ShipCompoionentLevel;
					record.mirror = (int)component.Mirror;
					record.pos = component.pos;
					record.rotation = (int)component.Rotation;
					records.Add(record);
				}
				
				byte[] shipBytes = SerializeHelp.WriteObjectData(new ShipData(ship.ShipName,records));
				SerializeHelp.WriteFile(Application.dataPath + @"/Resources/Ship/" + ship.ShipName, shipBytes);
			}

			public Ship LoadShipAtTransform(string shipName,Transform theTransform)
			{
				byte[] shipBytes = SerializeHelp.ReadFile(Application.dataPath + "/Resources/Ship/" + shipName);
				ShipData shipData = SerializeHelp.ReadObjectData<ShipData>(shipBytes);
				GameObject shipObject=Instantiate(EmptyShip, theTransform.position, theTransform.rotation);
				Ship ship = shipObject.GetComponent<Ship>();
				ship.ShipName = ship.name = shipData.ShipName;
				foreach (var component in shipData.Records)
				{
					ship.AddComponent(component.id, component.level, new Vector2Int((int)component.pos.x,(int)component.pos.y),
						(ShipEnum.ShipUnitRotation) Enum.ToObject(typeof(ShipEnum.ShipUnitRotation), component.rotation),
						(ShipEnum.ShipUnitMirror) Enum.ToObject(typeof(ShipEnum.ShipUnitMirror), component.mirror));
				}

				return ship;
			}

			#region test

			public Ship ship;
			public string sname;
			[Button("Save")]
			void Save()
			{
				SaveShip(ship);
			}

			[Button("Load")]
			void Load()
			{
				LoadShipAtTransform(sname, transform);
			}
			#endregion
		}
	public class ShipData
	{
		public struct PlaceRecord
		{
			public Vector2 pos;
			public int id;
			public int level;
			public int rotation;
			public int mirror;
		}
		public string ShipName;
		public List<PlaceRecord> Records;

		public ShipData()
		{

		}
		public ShipData(string shipName, List<PlaceRecord> records = null)
		{
			ShipName = shipName;
			if (records == null)
			{
				Records = new List<PlaceRecord>();
			}
			else
			{
				Records = records;
			}
		}
	}
}
