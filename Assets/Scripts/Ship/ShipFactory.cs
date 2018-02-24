using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using S.Serialize;
using ShipProject.ShipEnum;
using ShipSeriazble;
using Sirenix.OdinInspector;
using UnityEngine;
namespace ShipProject
{
	[System.Serializable]
	public class ShipFactory : MonoBehaviour
		{
			/// <summary>
			/// 空飞船模板
			/// </summary>
			public GameObject EmptyShip;

			public GameCamp ShipCamp;
			public static ShipFactory instance;
			public void Awake()
			{
				instance = this;
			}
			public static string ShipPath;
			public Ship LoadShipAtTransform(string shipName, Transform theTransform,GameCamp camp,ShipControlMode mode=ShipControlMode.Building)
			{
				byte[] shipBytes = SerializeHelp.ReadFile(Application.dataPath + "/Resources/Ship/"+camp.ToString()+ shipName + ".ship");
				ShipData shipData = SerializeHelp.ReadObjectData<ShipData>(shipBytes);
				GameObject shipObject = Instantiate(EmptyShip, theTransform.position, theTransform.rotation);
				Ship ship = shipObject.GetComponent<Ship>();
				ship.ShipName = ship.name = shipData.ShipName;
				foreach (var component in shipData.Records)
				{
					ship.AddComponent(component.id, component.level, new Vector2Int((int)component.pos.x, (int)component.pos.y),
						(ShipEnum.ShipUnitRotation)Enum.ToObject(typeof(ShipEnum.ShipUnitRotation), component.rotation),
						(ShipEnum.ShipUnitMirror)Enum.ToObject(typeof(ShipEnum.ShipUnitMirror), component.mirror));
				}
				ship.SetCamp(camp);
				ShipManager.Instance.ships[camp].Add(ship);
				ship.control.mode = mode;
				return ship;
			}

			public Ship LoadShipWithData(SerializeShipData data)
			{
				byte[] shipBytes = SerializeHelp.ReadFile(Application.dataPath + "/Resources/Ship/" + data.shipCamp.ToString() + data.shipName + ".ship");
				ShipData shipData = SerializeHelp.ReadObjectData<ShipData>(shipBytes);
				GameObject shipObject = Instantiate(EmptyShip, (Vector3)data.position, Quaternion.Euler((Vector3)data.rotation));
				Ship ship = shipObject.GetComponent<Ship>();
				ship.ShipName = ship.name = shipData.ShipName;
				foreach (var component in shipData.Records)
				{
					ship.AddComponent(component.id, component.level, new Vector2Int((int)component.pos.x, (int)component.pos.y),
						(ShipEnum.ShipUnitRotation)Enum.ToObject(typeof(ShipEnum.ShipUnitRotation), component.rotation),
						(ShipEnum.ShipUnitMirror)Enum.ToObject(typeof(ShipEnum.ShipUnitMirror), component.mirror));
				}
				ship.SetCamp(data.shipCamp);
				foreach (Block block in ship.Blocks.Values)
				{
					BlockHealth health = block.GetComponent<BlockHealth>();
					if (health != null)
					{
						health.ReceiveDamage(null, data.blockHealthData[(SerializableVector2Int)block.Pos].health);
					}
				}
				Debug.Log("debug太多免得我看不见所以才打这一行字"+data.mode.ToString());
				ship.control.mode = data.mode;
				Debug.Assert(ShipManager.Instance.enabled);
				ship.control.Sp = data.Sp;
				Debug.Assert(ShipManager.Instance.enabled);
				ship.control.SpColding = data.isColding;
				Debug.Assert(ShipManager.Instance.enabled);
				ShipManager.Instance.ships[data.shipCamp].Add(ship);
				Debug.Assert(ShipManager.Instance.enabled);
				return ship;
		}
			public void SaveShip(Ship ship,GameCamp camp)
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
				SerializeHelp.WriteFile(Application.dataPath + @"/Resources/Ship/"+camp.ToString() + ship.ShipName + ".ship", shipBytes);
			}

			public Dictionary<GameCamp, List<string>> ShipNames = new Dictionary<GameCamp, List<string>>();
			public void ListShipName(string path,GameCamp camp)
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
						if (theName.Contains(camp.ToString()))
						{
							ShipNames[camp].Add(Regex.Replace(theName, camp.ToString(), ""));
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
				foreach (GameCamp t in Enum.GetValues(typeof(GameCamp)))
				{
					ShipNames.Add(t, new List<string>());
					LoadList(t);
				}
			}

			public void LoadList(GameCamp camp)

			{
				ShipNames[camp].Clear();
				ShipPath = Application.dataPath + "/Resources/Ship";
				ListShipName(ShipPath,camp);
		}
		#region test

		public Ship ship;
			public string sname;
			[Button("Save")]
			void Save()
			{
				SaveShip(ship,ShipCamp);
			}

			[Button("Load")]
			void Load()
			{
				LoadShipAtTransform(sname, transform, ShipCamp);
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
