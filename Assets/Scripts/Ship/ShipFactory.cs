using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
				SerializeHelp.WriteFile(Application.dataPath + @"/Resources/Ship/" + ship.ShipName + ".ship", shipBytes);
			}

			public static ShipFactory instance;
			public void Awake()
			{
				instance = this;
			}
		public static string ShipPath;
			public Ship LoadShipAtTransform(string shipName,Transform theTransform)
			{
			byte[] shipBytes = SerializeHelp.ReadFile(Application.dataPath + "/Resources/Ship/" + shipName +".ship");
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

			public void SavePlayerShip(Ship ship)
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
				byte[] shipBytes = SerializeHelp.WriteObjectData(new ShipData(ship.ShipName, records));
				SerializeHelp.WriteFile(Application.dataPath + @"/Resources/Ship/Player_" + ship.ShipName + ".ship", shipBytes);
		}

			public Ship LoadPlayerShip(string shipName, Transform theTransform)
			{
				Debug.Log("Player_" + shipName);
				return LoadShipAtTransform("Player_" + shipName, theTransform);
			}
            // 玩家飞船列表
			public List<string> PlayerShipNames = new List<string>();
			public void ListPlayerShipName(string path)
			{
				//获取指定文件夹的所有文件  
				string[] paths = Directory.GetFiles(path);
				foreach (var item in paths)
				{
					//获取文件后缀名  
					string extension = Path.GetExtension(item).ToLower();
					if (extension == ".ship")
					{
						string theName = Path.GetFileNameWithoutExtension(item);
						if (theName.Contains("Player_"))
						{
							PlayerShipNames.Add(Regex.Replace(theName, "Player_", ""));
						}
					}
				}
			}
			///<summary>
			/// 截前后字符(串)
			///</summary>
			///<param name="val">原字符串</param>
			///<param name="str">要截掉的字符串</param>
			///<param name="all">是否贪婪</param>
			///<returns></returns>
			private string GetString(string val, string str, bool all)
			{
				return Regex.Replace(val, @"(^(" + str + ")" + (all ? "*" : "") + "|(" + str + ")" + (all ? "*" : "") + "$)", "");
			}
			public void Start()
			{
				LoadList();
			}
            
            // 更新列表
			public void LoadList()
			{
				PlayerShipNames.Clear();
				ShipPath = Application.dataPath + "/Resources/Ship";
				ListPlayerShipName(ShipPath);
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
