using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using ShipProject.ShipEnum;
using UnityEngine;
using Universe;

namespace ShipProject
{
	public class ShipManager : MonoSingleton<ShipManager>, ISaveable
	{
		private ID id;
		private bool isNewGameWithNewShip = true; // 默认为新游戏
		public Dictionary<GameCamp, List<Ship>> ships = new Dictionary<GameCamp, List<Ship>>();
		public Ship currentControl;
		public Ship controllingShip;
		private void Start()
		{
			if (isNewGameWithNewShip)
			{
				SetCurrentControlShip(ShipFactory.instance.LoadShipAtTransform("NewPlayerShip", transform, GameCamp.Player_));
				SetCurrentShipMode(ShipControlMode.Player);
				isNewGameWithNewShip = false;
			}
			else
			{
				; //不新建
			}
		}

		public Ship SetCurrentControlShip(Ship ship)
		{
			currentControl = ship;
			CameraController.Instance.LookAt(currentControl.transform);
			return ship;
		}

		public void SetCurrentShipMode(ShipControlMode mode)
		{
			currentControl.control.mode = mode;
			if (mode == ShipControlMode.Player)
			{
				if (controllingShip != null)
					controllingShip.control.mode = ShipControlMode.AIDefense;
				controllingShip = currentControl;
			}
		}

		public void SetCurrentShipMode(string mode)
		{
			if (currentControl == null) return;
			var theMode = (ShipControlMode) Enum.Parse(typeof(ShipControlMode), mode);
			currentControl.control.mode = theMode;
			if (theMode == ShipControlMode.Player)
			{
				if (controllingShip != null)
					if (controllingShip != currentControl)
						controllingShip.control.mode = ShipControlMode.AIDefense;
				controllingShip = currentControl;
			}

//			else
//			{
//				if (controllingShip != null)
//				{
//					controllingShip = null;
//				}
//			}
		}

		public void SetShipMode(Ship ship, ShipControlMode mode)
		{
			if (ship == null) return;

			if (ship == currentControl)
			{
				SetCurrentShipMode(mode);
				return;
			}

			ship.control.mode = mode;
			if (mode == ShipControlMode.Player)
			{
				if (controllingShip != null)
					if (controllingShip != ship)
						controllingShip.control.mode = ShipControlMode.AIDefense;
				controllingShip = currentControl;
			}
		}

		public void SetShipMode(Ship ship, string mode)
		{
			if (ship == null) return;

			if (ship == currentControl)
			{
				SetCurrentShipMode(mode);
				return;
			}

			var theMode = (ShipControlMode) Enum.Parse(typeof(ShipControlMode), mode);
			ship.control.mode = theMode;
			if (theMode == ShipControlMode.Player)
			{
				if (controllingShip != null)
					if (controllingShip != ship)
						controllingShip.control.mode = ShipControlMode.AIDefense;
				controllingShip = currentControl;
			}
		}

		#region ISaveable

		public void fromSaveData(SaveData data)
		{
			var shipData = (ShipManagerData) data;
			isNewGameWithNewShip = shipData.isNewGameWithNewShip;
			if(shipData.shipDatas.Count>0)
			for (int i = shipData.shipDatas.Count - 1; i > -1; i--)
			{
				ShipFactory.instance.LoadShipWithData(shipData.shipDatas[i]);
			}
			//			foreach (var dt in shipData.shipDatas)
			//			{
			//				var s = ShipFactory.instance.LoadShipWithData(dt);
			//				if (dt.isCurrentShip) SetCurrentControlShip(s);
			//				SetShipMode(s, dt.mode);
			//			}
		}

		public void OnSaveGame(GameEvent gameEvent, Component comp, object param = null)
		{
			Save();
		}

		public void Save()
		{
			SaveManager.Instance.Save(this, id);
		}

		public SaveData toSaveData()
		{
			var data = new ShipManagerData();
			data.isNewGameWithNewShip = isNewGameWithNewShip;
			foreach (var sl in ships.Values)
			foreach (var sd in sl)
			{
				var sdData = sd.serializeShip.data;
				sdData.isColding = sd.control.SpColding;
				sdData.mode = sd.control.mode;
				sdData.position = (Vector3Data) sd.transform.position;
				sdData.rotation = (Vector3Data) sd.transform.rotation.eulerAngles;
				sdData.scale = (Vector3Data) sd.transform.localScale;
				sdData.shipCamp = sd.ShipCamp;
				sdData.shipName = sd.ShipName;
				sdData.Sp = sd.control.Sp;
				data.shipDatas.Add(sdData);
				if (sd == currentControl) sdData.isCurrentShip = true;
				if (sd == controllingShip) sdData.isControllingShip = true;
			}
			return data;
		}

		#endregion

		private void Awake()
		{
			id = new ID();
			id.className = "ShipManager";
			id.sceneName = "Universe";
			id.Init();
			SaveManager.Instance.Load(this, id);
			foreach (GameCamp gc in Enum.GetValues(typeof(GameCamp))) ships.Add(gc, new List<Ship>());
			EventManager.Instance.AddListener(this, GameEvent.SAVE_GAME, OnSaveGame);
		}

		private void OnDestroy()
		{
			EventManager.Instance.RemoveObjectEvent(this, GameEvent.SAVE_GAME);
		}
	}

	[Serializable]
	public class ShipManagerData : SaveData
	{
		public bool isNewGameWithNewShip;
		public List<SerializeShipData> shipDatas = new List<SerializeShipData>();
	}
}