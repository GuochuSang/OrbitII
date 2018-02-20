using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
	/// <summary>
	///         管理可采集元素资源, 全局
	/// </summary>
	public static class Resource
	{
		private const string loadPath = "Resource"; // 元素地址为 loadPath + 元素名

		private static Dictionary<ElementType, List<Element>> elements;

		// 初始化已定义的元素
		public static void InitElements()
		{
			DefineElements();
			foreach (var kv in elements)
			foreach (var e in kv.Value)
				e.icon = Resources.Load<Sprite>(loadPath + "/" + e);
		}

		// 在这里定义所有应该有的元素
		// 按照顺序添加
		private static void DefineElements()
		{
			elements = new Dictionary<ElementType, List<Element>>();
			Dictionary<Planet.LandType, ElementGrowInfo> growInfos; // temp

			#region 生物元素

			var bioElements = new List<Element>();
			// 生物元素一号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
			growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(1f,0.1f, 500f, 0f, 0.05f, 0.3f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(1f,0.1f,300f, 0f, 0.03f, 0.02f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(1f,0.1f,150f, 0f, 0.01f, 0.01f));
			bioElements.Add(new Element("野魔藤",ElementType.BIO, ElementIndex.I,
				"这是一种常见的绿色植被,凭借它强大的生命力和环境适应能力在各个星球各个区域广泛分布,被<<本草纲目>>收录于第53卷", growInfos));
			// 生物元素二号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(1f,0.1f,200f, 0f, 0.03f, 0.03f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(1f,0.1f,50f, 0f, 0.01f, 0.01f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(1f,0.1f,700f, 0f, 0.06f, 0.7f));
			bioElements.Add(new Element("鼻屎藻",ElementType.BIO, ElementIndex.II,
				"这是一种外貌独特的水藻,由于它快速的繁殖能力而被宇宙环境署列入危险生物名单, 银河系生物惊恐地称之为'鼻屎藻'", growInfos));
	
			elements.Add(ElementType.BIO, bioElements);

			#endregion

			#region 普通元素

			var xdElements = new List<Element>();
			// 普通元素一号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.8f,0.1f,500f, 0f, 0.05f, 0.3f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.8f,0.1f,300f, 0f, 0.03f, 0.02f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.8f,0.1f,150f, 0f, 0.01f, 0.01f));
			xdElements.Add(new Element("铁",ElementType.XD, ElementIndex.I, "普通元素1", growInfos));
			// 普通元素二号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.4f,0.1f,200f, 0f, 0.03f, 0.03f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.4f,0.1f,700f, 0f, 0.01f, 0.01f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.4f,0.1f,300f, 0f, 0.06f, 0.7f));
			xdElements.Add(new Element("硅",ElementType.XD, ElementIndex.II, "普通元素2", growInfos));
			// 普通元素三号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.6f,0.2f,200f, 0f, 0.03f, 0.03f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.6f,0.2f,700f, 0f, 0.01f, 0.01f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.6f,0.2f,300f, 0f, 0.06f, 0.7f));
			xdElements.Add(new Element("铜", ElementType.XD, ElementIndex.III, "普通元素3", growInfos));

			elements.Add(ElementType.XD, xdElements);
			#endregion

			#region 稀有元素

			var spElements = new List<Element>();
			// 稀有元素一号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.1f,0.1f,500f, 0f, 0.05f, 0.3f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.1f,0.1f,300f, 0f, 0.03f, 0.02f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.1f,0.1f,150f, 0f, 0.01f, 0.01f));
			spElements.Add(new Element("艾德曼金属",ElementType.SP, ElementIndex.I, "稀有元素1", growInfos));
			// 稀有元素二号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.2f,0.1f,200f, 0f, 0.03f, 0.03f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.2f,0.1f,50f, 0f, 0.01f, 0.01f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.2f,0.1f,700f, 0f, 0.06f, 0.7f));
			spElements.Add(new Element("振金",ElementType.SP, ElementIndex.II, "稀有元素2", growInfos));
			// 稀有元素三号
			growInfos = new Dictionary<Planet.LandType, ElementGrowInfo>();
            growInfos.Add(Planet.LandType.GRASS, new ElementGrowInfo(0.4f,0.1f,200f, 0f, 0.03f, 0.03f));
            growInfos.Add(Planet.LandType.SAND, new ElementGrowInfo(0.4f,0.1f,50f, 0f, 0.01f, 0.01f));
            growInfos.Add(Planet.LandType.SEA, new ElementGrowInfo(0.4f,0.1f,700f, 0f, 0.06f, 0.7f));
			spElements.Add(new Element("基岩矿", ElementType.SP, ElementIndex.III, "稀有元素3", growInfos));

			elements.Add(ElementType.SP, spElements);
			#endregion
		}

		/// <summary>
		///         获得某种元素的定义, 禁止对其进行修改
		/// </summary>
		public static bool GetElement(ElementType type, ElementIndex index, out Element element)
		{
			var count = GetElementCount(type);
			if ((int) index > count)
			{
				Debug.Log("你还没有定义" + type + "_" + index);
				element = null;
				return false;
			}

			element = elements[type][(int) index];
			return true;
		}

		/// <summary>
		///         (解析字符串)获得某种元素的定义, 禁止对其进行修改
		/// </summary>
		public static bool GetElement(string elementName, out Element element)
		{
			Debug.Assert(elementName != null);
			ElementType type;
			ElementIndex index;
			var parsed = elementName.Split('_');
			Debug.Assert(parsed.Length == 2, "这不是一个元素的字符串!");
			type = (ElementType) Enum.Parse(typeof(ElementType), parsed[0]);
			index = (ElementIndex) Enum.Parse(typeof(ElementIndex), parsed[1]);
			return GetElement(type, index, out element);
		}

		/// <summary>
		///         获得某一类元素的数量
		/// </summary>
		public static int GetElementCount(ElementType type)
		{
			if (!elements.ContainsKey(type))
			{
				Debug.Log("你还没有添加这种元素");
				return 0;
			}

			return elements[type].Count;
		}
	}

	public enum ElementType
	{
		// 稀有元素
		SP,

		// 普通元素
		XD,

		// 可再生元素(实际上都可再生, 但是这个类似生物, 再生速度很有趣)
		BIO
	}

	public enum ElementIndex
	{
		I,
		II,
		III,
		IV,
		V,
		VI
	}

	/// <summary>
	///         某种地形所对应的一个元素增长信息
	///         生物请设置GrowRatio
	///         非生物请设置GrowSpeed
	/// </summary>
	public class ElementGrowInfo
	{
        public ElementGrowInfo(float heightLocation, float locationRange, float maxAmount, float growSpeed, float growRatio, float transRatio)
		{
            this.heightLocation = heightLocation;
            this.locationRange = locationRange;

			this.maxAmount = maxAmount;
			this.growSpeed = growSpeed;
			this.growRatio = growRatio;
			this.transRatio = transRatio;
		}
        public float heightLocation; // 最佳高度[0,1]
        public float locationRange; // 高度聚集区, 偏离最佳高度数量减少

		public float maxAmount; // 最大容量
		public float growSpeed; // 增长速度

		public float growRatio; // 增长比例

		/*
		 * 生物元素专用
		 * 增长公式
		 * growSpeed + Base*(1+growRatio-(abs(Base-0.5Max)/0.5Max)*growRatio)
		*/
		public float transRatio; // 传播比例 (邻近Land*当前Land传播比例,传播给当前Land)
	}

	public class Element
	{
		public string elementName;
		public Element(string elementName,ElementType type, ElementIndex index, string detail,
			Dictionary<Planet.LandType, ElementGrowInfo> infos)
		{
			this.elementName = elementName;
			this.type = type;
			this.index = index;
			this.detail = detail;
			growInfos = infos;
		}

		// 这个string既是元素名,又是储存地址
		public override string ToString()
		{
            return type.ToString() + "_" + index.ToString();
		}

		// 图标 (根据type, index由Resource静态类加载)
		public Sprite icon;

		// 类型
		public ElementType type;

		// 下标
		public ElementIndex index;

		// 说明
		public string detail;

		// 元素对应每种地形的增长信息
		public Dictionary<Planet.LandType, ElementGrowInfo> growInfos;

		/// <summary>
		///         通过所在地形/当前地形元素总量/邻近地形元素总量---->确定 增长后的量!
		///         此处只计算单位时间增长, 若经历多个单位时间, 请酌情增加模拟次数
		/// </summary>
		/// <returns>经过计算的当前地区此元素的数量.</returns>
		/// <param name="landType">土地类型.</param>
		/// <param name="totalAmount">此元素数量.</param>
		/// <param name="lAmount">左边地区此元素数量,将返回数量减少后的值.</param>
		/// <param name="rAmount">另一边地区此元素数量,将返回数量减少后的值.</param>
		public float GrowedAmount(Planet.LandType landType, float totalAmount, ref float lAmount, ref float rAmount)
		{
			var info = growInfos[landType];
			// 超过最大值则不再增长
			if (totalAmount > info.maxAmount)
				return totalAmount;
			// 若未超过最大值, 变化后的量 = 原来量+自我增长+邻近扩散
			// 不必检测这次是否超过最大值
            Debug.Assert(info.growRatio - Mathf.Abs(totalAmount - 0.5f * info.maxAmount) / (0.5f * info.maxAmount) * info.growRatio >= 0);
			float afterGrowed =
				totalAmount +
				info.growSpeed + totalAmount * (info.growRatio - Mathf.Abs(totalAmount - 0.5f * info.maxAmount) / (0.5f * info.maxAmount) * info.growRatio) +
				lAmount * info.transRatio + 
                rAmount * info.transRatio;
            lAmount -= lAmount * info.transRatio;
            rAmount -= rAmount * info.transRatio;
			return afterGrowed;
		}

		public ElementID GetID()
		{
			return new ElementID(this);
		}
	}

	/// <summary>
	///         这个类是可序列化的,用作对外接口
	/// </summary>
	[Serializable]
	public class ElementID
	{
		// 类型
		public ElementType type;

		// 下标
		public ElementIndex index;

		public ElementID(Element e)
		{
			type = e.type;
			index = e.index;
		}

		public ElementID(ElementType type, ElementIndex index)
		{
			this.type = type;
			this.index = index;
		}

		public Element GetElement()
		{
			Element temp;
			Resource.GetElement(type, index, out temp);
			return temp;
		}
	}
}