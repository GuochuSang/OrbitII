using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  ShipProject
{
    /// <summary>
    /// 单例类，飞船元件列表
    /// </summary>
    public class ShipUnitLister : MonoBehaviour
    {
        /*******************单例化*******************/
        private static ShipUnitLister instance;

        public static ShipUnitLister GetInstance()
        {
            return instance;
        }

        private void Awake()
        {
            instance = this;
        }

        /*********************************************/
        /// <summary>
        /// 元件索引函数
        /// </summary>
        /// <param name="id">方块ID</param>
        /// <returns></returns>
        public T GetUnit<T>(int id,int level) where T:class
	        {
				if(typeof(T)==typeof(Block))
		        return BlockList.Find((block) => block.Id == id&&block.BlockLevel==level) as T;
		        if (typeof(T)==typeof(ShipComponent))
			    return ShipComponentList.Find((shipComponent) => shipComponent.Id ==id&&shipComponent.ShipCompoionentLevel==level) as T;
		        return null;
	        }

		/// <summary>
        /// 方块集合
        /// </summary>
        [OnValueChanged("BlockListSort")][TabGroup("集合","方块集合")]
        public List<Block> BlockList=new List<Block>();
	    /// <summary>
	    /// 组件集合
	    /// </summary>
	    [OnValueChanged("ComponentListSort")]
	    [TabGroup("集合", "组件集合")]
		public List<ShipComponent> ShipComponentList = new List<ShipComponent>();
		/// <summary>
		/// 方块集合排序
		/// </summary>
		private void BlockListSort()
	    {
		    BlockList.Sort();
	    }
		/// <summary>
		/// 组件集合排序
		/// </summary>
		private void ComponentListSort()
	    {
		    ShipComponentList.Sort();
	    }
	}
}

