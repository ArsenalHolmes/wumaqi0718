using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EventName
{
    TimeChangs,//时间
    PromptChangs,//提醒
    TotalTime,//总时间
    PlayerChang,//修改玩家
}
public class NotificationManger  {

	public delegate void OnNotification(Notification obj);
    private static NotificationManger instance = null;
    public static NotificationManger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NotificationManger();
                return instance;
            }
            return instance;
        }
    }


    /// <summary>
    /// 存储事件的字典
    /// </summary>
    private Dictionary<EventName, OnNotification> eventListeners = new Dictionary<EventName, OnNotification>();

    /// <summary>
    /// 注册事件
    /// </summary>
    ///<param name="eventKey">事件Key
    ///<param name="eventListener">事件监听器
    public void AddEventListener(EventName eventKey, OnNotification eventListener)
    {
        if (!eventListeners.ContainsKey(eventKey))
        {
            eventListeners.Add(eventKey, eventListener);
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    ///<param name="eventKey">事件Key
    public void RemoveEventListener(EventName eventKey)
    {
        if (!eventListeners.ContainsKey(eventKey))
            return;

        eventListeners[eventKey] = null;
        eventListeners.Remove(eventKey);
    }

    /// <summary>
    /// 分发事件
    /// </summary>
    ///<param name="eventKey">事件Key
    ///<param name="param">通知内容
    public void DispatchEvent(EventName eventKey, Notification obj)
    {
        if (!eventListeners.ContainsKey(eventKey))
            return;
        eventListeners[eventKey](obj);
    }

    /// <summary>
    /// 是否存在指定事件的监听器
    /// </summary>
    public bool HasEventListener(EventName eventKey)
    {
        return eventListeners.ContainsKey(eventKey);
    }
}

public class Notification
{
    string str;
    float flo;
    object obj;

    public string Str
    {
        get
        {
            return str;
        }

        set
        {
            str = value;
        }
    }

    public float Flo
    {
        get
        {
            return flo;
        }

        set
        {
            flo = value;
        }
    }

    public object Obj
    {
        get
        {
            return obj;
        }

        set
        {
            obj = value;
        }
    }

    public Notification(string str)
    {
        this.Str = str;
    }

    public Notification(float flo)
    {
        this.Flo = flo;
    }

    public Notification(object obj)
    {
        this.Obj = obj;
    }
}
