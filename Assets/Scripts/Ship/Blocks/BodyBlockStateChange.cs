using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace  ShipProject
{
    namespace ShipEnum
    {
        /// <summary>
        /// 相邻状态
        /// </summary>
        public enum NearbyState
        {
            NULL=0,
            SAMEID=1,
            DIFID=2
        }
    }
    /// <summary>
    /// Body的贴图更改脚本
    /// </summary>
    public class BodyBlockStateChange : MonoBehaviour
    {
        #region 相邻贴图判定数组
        public static int[] UpDifId = new int[] { 0, 3, 4, 6, 9, 10, 11, 13, 17, 18, 19, 31, 34 };
        public static int[] DownDifId = new int[] { 0, 1, 2, 5, 9, 10, 11, 12, 15, 16, 20, 24, 27, };
        public static int[] LeftDifId = new int[] { 0, 2, 4, 7, 9, 12, 13, 14, 15, 17, 21, 28, 33 };
        public static int[] RightDifId = new int[] { 0, 1, 3, 8, 10, 12, 13, 14, 16, 18, 22, 25, 30 };
        public static int[] UpSameId = new int[] { 12, 14, 15, 16, 20, 21, 22, 23, 30, 32, 33, 35, 37 };
        public static int[] DownSameId = new int[] { 13, 14, 17, 18, 19, 21, 22, 23, 25, 26, 28, 29, 36 };
        public static int[] RightSameId = new int[] { 9, 11, 15, 17, 19, 20, 21, 23, 24, 26, 31, 32, 39 };
        public static int[] LeftSameId = new int[] { 10, 11, 16, 18, 19, 20, 22, 23, 27, 29, 34, 35, 38 };
        public static int[] UpNull = new int[] { 0, 1, 2, 5, 7, 8,9,10,11,13,17,18,19, 24, 25, 26, 27, 28, 29, 36, 38, 39 };
        public static int[] DownNull = new int[] { 0, 3, 4, 6, 7, 8,9,10,11,12,15,16,20, 30, 31, 32, 33, 34, 35, 37, 38, 39 };
        public static int[] RightNull = new int[] { 0, 2, 4, 5, 6, 7,10,12,13,14,16,18,22, 27, 28, 29, 33, 34, 35, 36, 37, 38 };
        public static int[] LeftNull = new int[] { 0, 1, 3, 5, 6, 8, 9,12,13,14,15,17,21,24, 25, 26, 30, 31, 32, 36, 37, 39 };
        #endregion
        /// <summary>
        /// body的所有贴图
        /// </summary>
        public Sprite[] StateSprites=new Sprite[40];
        /// <summary>
        /// 挂载的block脚本
        /// </summary>
        public Block block
        {
            get { return GetComponent<Block>(); }
            protected set{}
        }

	    private void Awake()
	    {
		    block.BlockStateChangeEventInt += SpriteOut;//订阅状态更新事件
	    }
        /// <summary>
        /// 从方块相邻状态得到贴图
        /// </summary>
        /// <param name="up">上面相邻状态</param>
        /// <param name="down">下面相邻状态</param>
        /// <param name="left">左边相邻状态</param>
        /// <param name="right">右边相邻状态</param>
        /// <returns>贴图编号</returns>
        public void SpriteOut(NearbyState up, NearbyState down, NearbyState left, NearbyState right)
        {
            List<int> outer=new List<int>();
            List<int> upArray=new List<int>(), downArray= new List<int>(), leftArray= new List<int>(), rightArray= new List<int>();
            //对四个方向状态进行判断
            switch (up)
            {
                case NearbyState.DIFID:
                    upArray = UpDifId.ToList();
                    break;
                case NearbyState.NULL:
                    upArray = UpNull.ToList();
                    break;
                case NearbyState.SAMEID:
                    upArray = UpSameId.ToList();
                    break;
            }
            switch (down)
            {
                case NearbyState.DIFID:
                    downArray = DownDifId.ToList();
                    break;
                case NearbyState.NULL:
                    downArray = DownNull.ToList();
                    break;
                case NearbyState.SAMEID:
                    downArray = DownSameId.ToList();
                    break;
            }
            switch (left)
            {
                case NearbyState.DIFID:
                    leftArray = LeftDifId.ToList();
                    break;
                case NearbyState.NULL:
                    leftArray = LeftNull.ToList();
                    break;
                case NearbyState.SAMEID:
                    leftArray = LeftSameId.ToList();
                    break;
            }
            switch (right)
            {
                case NearbyState.DIFID:
                    rightArray = RightDifId.ToList();
                    break;
                case NearbyState.NULL:
                    rightArray = RightNull.ToList();
                    break;
                case NearbyState.SAMEID:
                    rightArray = RightSameId.ToList();
                    break;
            }
            outer = upArray.ToList().Intersect(downArray).Intersect(leftArray).Intersect(rightArray).ToList();
            Debug.Log(outer[0]);
	        GetComponent<SpriteRenderer>().sprite = StateSprites[outer[outer.Count-1]];
        }
    }
}

