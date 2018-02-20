using System.Collections;
using System.Collections.Generic;
using Option;
using UnityEngine;
namespace Manager
{
    public class OptionManager:MonoSingleton<OptionManager>
    {
    	public Option.Option CurrentOption=new Option.Option();
        // 请勿改成Awake, 依赖Manager生成顺序
        public void Start()
    	{
    		CurrentOption=OptionHelper.ReadOption();
    		OptionHelper.ConfirmOption(CurrentOption);
    	}
    	/// <summary>
    	/// 更改设置
    	/// </summary>
    	/// <param name="option">设置</param>
    	public void SetOption(Option.Option option)
    	{
    		CurrentOption = option;
    		OptionHelper.ConfirmOption(option);
    		OptionHelper.SaveOption(option);
    	}
    }
}