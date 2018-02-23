using System.Collections;
using System.Collections.Generic;
using ShipProject.ShipEnum;
using UnityEngine;

namespace ShipProject
{
	namespace ShipEnum
	{
		public enum GameCamp
		{
			Player_=0,
			Enemy_=1
		}
	}
	/// <summary>
	/// 阵营接口，继承该接口表示该类拥有阵营
	/// </summary>
	public interface ICamp
	{/// <summary>
	/// 获取阵营
	/// </summary>
	/// <returns></returns>
		GameCamp GetCamp();
		/// <summary>
		/// 设置阵营
		/// </summary>
		/// <param name="camp">阵营</param>
		void SetCamp(GameCamp camp);
	}
}
