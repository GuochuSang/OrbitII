using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	namespace ShipEnum
	{
		/// <summary>
		/// 船载武器默认操作枚举
		/// </summary>
		public enum ShipWeaponDefaultNum
		{
			Alpha0=0,
			Alpha1=1,
			Alpha2=2,
			Alpha3=3,
			Alpha4=4,
			Alpha5=5,
			Alpha6=6,
			Alpha7=7,
			Alpha8=8,
			Alpha9=9
		}
		public enum ShipWeaponDefaultOperationMode
		{
			Impulse=0,
			Continuous=1
		}
	}
	public static class NumToButton
	{
		public static string NumToKey(ShipWeaponDefaultNum num)
		{
			return ((int) num).ToString();
		}

		public static int NumToInt(ShipWeaponDefaultNum num)
		{
			return (int) num;
		}
	}
/// <summary>
/// 船载武器接口
/// </summary>
public interface IShipWeapon:ICamp,ISPConsumer
	{
		ShipWeaponDefaultNum GetDefaultOperation();
		ShipWeaponDefaultOperationMode GetOperationMode();
		void WeaponWork();
		void WeaponSkill();
		void End();
	}
}

