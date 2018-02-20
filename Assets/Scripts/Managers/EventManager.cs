/// <summary>
/// 处理游戏事件!!!(委托)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 在这里定义游戏中的各种事件名
    /// </summary>
    public enum GameEvent
    {
        START_NULL,

        GAME_INIT,
        GAME_END,
        GAME_PAUSE,
        GAME_UNPAUSE,
        ENTER_RECORD, // 进入了一个存档!!!加载新场景...
        EXIT_RECORD_WITH_SAVE, // 退出了一个存档!!!加载主菜单...

        MID_NULL,

        // 在退出存档时, 对后面的事件进行清空, 前面的事件需要自行清空
        ENTER_PLANET_AREA, // 进入星球范围 (参数为: 飞船, 星球)
        EXIT_PLANET_AREA,  // 离开星球范围
        LOOK_PLANET, // 总览星球
        EXIT_LOOKING_PLANET, // 退出总览星球

        COLONY_SET_UP, // 殖民星球
        LOOK_BUILDING, // 查看某个建筑(可能是空地)

        END_NULL,
    };

    /// <summary>
    /// 声明一个事件委托类型(类似函数指针),参数如下:
    /// EventType: 事件类型
    /// Component: 事件主体(发送者)
    /// param: 其他要传递的参数
    /// </summary>
    public delegate void OnEvent(GameEvent eventType, Component sender, object param = null);

    public class EventManager : MonoSingleton<EventManager>
    {
        private Dictionary<GameEvent,List<OnEvent>> listeners = new Dictionary<GameEvent,List<OnEvent>>();

        /// <summary>
        /// 添加监听者(要执行的函数)
        /// </summary>
        /// <param name="eventType">Event type.</param>
        /// <param name="listener">Listener.</param>
        public void AddListener(GameEvent eventType, OnEvent listener)
        {
            List<OnEvent> listenerList = null;
            if (listeners.TryGetValue(eventType, out listenerList))
            {
                //List exists
                listenerList.Insert(0,listener);
                return;
            }
            listenerList = new List<OnEvent>();
            listenerList.Insert(0,listener);
            listeners.Add(eventType,listenerList);  
        }
        /// <summary>
        /// 某个事件的最后的监听者!!
        /// </summary>
        /// <param name="eventType">Event type.</param>
        /// <param name="listener">Listener.</param>
        public void AddFinalListener(GameEvent eventType, OnEvent listener)
        {
            List<OnEvent> listenerList = null;
            if (listeners.TryGetValue(eventType, out listenerList))
            {
                //List exists
                listenerList.Add(listener);
                return;
            }
            listenerList = new List<OnEvent>();
            listenerList.Add(listener);
            listeners.Add(eventType,listenerList);  
        }
        /// <summary>
        /// 当事件发生, 奔走相告
        /// </summary>
        /// <param name="eventType">Event type.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="param">Parameter.</param>
        public void PostEvent(GameEvent eventType, Component sender, object param = null)
        {
            List<OnEvent> listenerList = null;
            if (!listeners.TryGetValue(eventType, out listenerList))
                return;
            foreach (OnEvent e in listenerList)
            {
                if(e != null)
                    e(eventType, sender, param);
            }
        }
        /// <summary>
        /// 移除一种事件
        /// </summary>
        /// <param name="eventType">Event type.</param>
        public void RemoveEvent(GameEvent eventType)
        {
            listeners.Remove(eventType);
        }
        /// <summary>
        /// 清理冗余的null事件
        /// </summary>
        public void RemoveRedundancies()
        {
            Dictionary<GameEvent, List<OnEvent>> tempListeners = new Dictionary<GameEvent, List<OnEvent>>();
            foreach (KeyValuePair<GameEvent,List<OnEvent>> item in listeners)
            {
                for(int i=0;i<item.Value.Count;i++)
                {
                    if (item.Value[i].Equals(null))
                    {
                        item.Value.RemoveAt(i);
                    }
                }
                if (item.Value.Count > 0)
                    tempListeners.Add(item.Key,item.Value);
            }
            listeners = tempListeners;
        }
    }
}
