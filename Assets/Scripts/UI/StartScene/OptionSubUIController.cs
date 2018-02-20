using System;
using System.Collections;
using System.Collections.Generic;
using Option;
using UnityEngine;
using UnityEngine.UI;
using Manager;
public class OptionSubUIController : MonoBehaviour
{
	public Slider volumeSlider;
	public Dropdown resolutionDropdown, screenDropdown, qualityDropdown;
	public Button confirm, cancel;

	public List<Vector2Int> IDToResolution = new List<Vector2Int>();
	public List<ScreenOption> IDToScreenOption = new List<ScreenOption>();
	public List<QualityOption> IDToQuality = new List<QualityOption>();

	public void Confirm()
	{
		Option.Option option = new Option.Option();
		option.gameOption.quality = IDToQuality[qualityDropdown.value];
		option.gameOption.screenOption = IDToScreenOption[screenDropdown.value];
		Vector2Int temp = IDToResolution[resolutionDropdown.value];
        option.gameOption.Resolution = new Vector2IntData(temp.x, temp.y);
        option.gameOption.Volume = volumeSlider.value;
		OptionManager.Instance.SetOption(option);
	}

    // 初始化
	public void SetValue()
	{
		Option.Option option = OptionManager.Instance.CurrentOption;
		if (option == null) throw new ArgumentNullException("option");
		 qualityDropdown.value=IDToQuality.IndexOf(option.gameOption.quality);
		 screenDropdown.value = IDToScreenOption.IndexOf(option.gameOption.screenOption);
        resolutionDropdown.value= IDToResolution.IndexOf(option.gameOption.Resolution.GetData());
		 volumeSlider.value=option.gameOption.Volume;
	}
	public void Start()
	{
		SetValue();
	}
}
