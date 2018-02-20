using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Option
{
	/// <summary>
	/// 质量设置
	/// </summary>
	public enum QualityOption
	{
		Low,
		Medium,
		High,
	}
	/// <summary>
	/// 屏幕设置
	/// </summary>
	public enum ScreenOption
	{
		Windowed,
		FullScreen,
	}
	/// <summary>
	/// 游戏设置（容器类）
	/// </summary>
    [System.Serializable]
	public class GameOption
	{
		public Vector2IntData Resolution =  new Vector2IntData(1920, 1080);
		public float Volume =1;
		public QualityOption quality = QualityOption.Medium;
		public ScreenOption screenOption = ScreenOption.FullScreen;
		public override string ToString()
		{
			return Resolution.ToString() + Volume.ToString() + quality.ToString() + screenOption.ToString();
		}
	}
}
