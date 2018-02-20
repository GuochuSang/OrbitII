using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Option
{/// <summary>
/// 设置（容器类）
/// </summary>
    [System.Serializable]
	public class Option
	{
		public GameOption gameOption;
        public Option()
        {
            gameOption = new GameOption();
        }
		public override string ToString()
		{
			return gameOption.ToString();
		}
	}
}
