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
        GAME_INIT,
        GAME_END,
        GAME_PAUSE,
        GAME_UNPAUSE,
        ENTER_RECORD, // 进入了一个存档!!!加载新场景...
        EXIT_RECORD_WITH_SAVE, // 退出了一个存档!!!加载主菜单...
        SAVE_GAME,

        ENTER_SHIP_FACTORY,
        EXIT_SHIP_FACTORY,

        ENTER_PLANET_AREA, // 进入星球范围 (参数为: 飞船, 星球)
        EXIT_PLANET_AREA,  // 离开星球范围
        ENTER_BUILDING_AREA, // 进入了某建筑的范围
        EXIT_BUILDING_AREA, // 离开了某建筑的范围
        LOOK_PLANET, // 总览星球
        EXIT_LOOKING_PLANET, // 退出总览星球
        ENTER_PLANET, // 进入星球建造
        EXIT_PLANET,//退出星球建造

        COLONY_SET_UP, // 殖民星球
        LOOK_BUILDING, // 查看某个建筑(可能是空地)

        STOP_INPUT, // 暂停除了当前界面以外的其他输入
        RESTART_INPUT, // 重新接收输入
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
        // 每一个事件对应一系列对象, 每一个对象有一个OnEvent委托,  
        Dictionary<GameEvent,Dictionary<object,OnEvent>> listeners = new Dictionary<GameEvent, Dictionary<object, OnEvent>>();

        /// <summary>
        /// 添加监听者(要执行的函数)
        /// </summary>
        public void AddListener(object obj, GameEvent eventType, OnEvent listener)
        {
            Dictionary<System.Object,OnEvent> objsEvents = null;
            if (listeners.TryGetValue(eventType, out objsEvents))
            {
                // 事件存在
                OnEvent objEvent = null;
                // 如果对象存在, 添加事件
                if (objsEvents.TryGetValue(obj, out objEvent))
                {
                    if (objEvent == null)
                        objEvent = listener;
                    else
                        objEvent += listener;
                }
                // 如果对象不存在, 添加对象
                else
                {
                    objsEvents.Add(obj, listener);
                }
                return;
            }
            // 如果事件不存在
            objsEvents = new Dictionary<object, OnEvent>();
            objsEvents.Add(obj, listener);
            listeners.Add(eventType, objsEvents);
        }
        /// <summary>
        /// 当事件发生, 奔走相告
        /// </summary>
        public void PostEvent(GameEvent eventType, Component sender, object param = null)
        {
            Dictionary<object,OnEvent> listenerList = null;
            if (!listeners.TryGetValue(eventType, out listenerList))
                return;
            // 不能迭代直接修改
            /*
            foreach (var events in listenerList)
            {
                events.Value(eventType, sender, param);
            }
            */
            List<object> objs = new List<object>(listenerList.Keys);
            foreach (var obj in objs)
            {
                listeners[eventType][obj](eventType, sender, param);
            }
        }
        /// <summary>
        /// 移除一种事件
        /// </summary>
        public void RemoveEvent(GameEvent eventType)
        {
            listeners.Remove(eventType);
        }
        /// <summary>
        /// 移除某个Object的某类事件
        /// </summary>
        public void RemoveObjectEvent(object obj,GameEvent eventType)
        {
            Dictionary<System.Object,OnEvent> objsEvents = null;
            if (listeners.TryGetValue(eventType, out objsEvents))
            {
                objsEvents.Remove(obj);
            }
        }
        /// <summary>
        /// 清理冗余的null事件
        /// </summary>
        public void RemoveRedundancies()
        {
            Dictionary<GameEvent,Dictionary<object,OnEvent>> tempList = new Dictionary<GameEvent, Dictionary<object, OnEvent>>();

            // 游戏事件-事件下的各个对象
            foreach (var gameEvents in listeners)
            {
                Dictionary<object, OnEvent> tempObjEvents = new Dictionary<object, OnEvent>();
                // 对象-对象的事件
                foreach (KeyValuePair<object, OnEvent> objEvent in gameEvents.Value)
                {
                    // 委托不为null, 添加这个对象的事件
                    if (!objEvent.Value.Equals(null))
                        tempObjEvents.Add(objEvent.Key,objEvent.Value);
                }
                // 添加到暂存字典
                if (tempObjEvents.Count > 0)
                    tempList.Add(gameEvents.Key, tempObjEvents);
            }
            listeners = tempList;
        }
    }
}
