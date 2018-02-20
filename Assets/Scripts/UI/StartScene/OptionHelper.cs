using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Option
{
	public static class OptionHelper
	{
		/// <summary>
		/// 应用设置
		/// </summary>
		public  static void ConfirmOption(Option option)
		{
			ConfirmQuality(option.gameOption.quality);
			ConfirmScreen(option.gameOption.Resolution, option.gameOption.screenOption);
			ConfirmVolume(option.gameOption.Volume);
		}
		#region Confirm Option SumFunction
		public static void ConfirmScreen(Vector2IntData resolution,ScreenOption screenoption)
		{
			Screen.SetResolution(resolution.x,resolution.y,screenoption==ScreenOption.FullScreen);
		}

		public static void ConfirmVolume(float volume)
		{
            Manager.AudioManager.Instance.EffectIntensity = volume;
            Manager.AudioManager.Instance.AmbientIntensity = volume;
            Manager.AudioManager.Instance.MusicIntensity = volume;
		}

		public static void ConfirmQuality(QualityOption qualityOption)
		{
			int index = 1;
			switch (qualityOption)
			{
				case QualityOption.High:
					index= 5;
					break;
				case QualityOption.Medium:
					index = 3;
					break;
				case QualityOption.Low:
					index = 1;
					break;
			}
			QualitySettings.SetQualityLevel(index);
		}
		#endregion
		/// <summary>
		/// 保存设置
		/// </summary>
		public static void SaveOption(Option option)
		{
			//Debug.Log(option.ToString());
            Manager.SaveManager.Instance.SaveAtPath<Option>(option,"Option/Config",".ini");
		}
		/// <summary>
		/// 读取设置
		/// </summary>
		public static Option ReadOption()
		{
            Option option = Manager.SaveManager.Instance.LoadAtPath<Option>("Option/Config",".ini");
            if (option == null)
                option = new Option();
            return option;
		}
	}
}
