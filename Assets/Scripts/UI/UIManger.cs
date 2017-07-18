using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum UIName
{
    EndPanel,
    IntroducePanel,
    StartPanel,
    PlayPanel,
}
public class UIManger  {

    /// 
    /// 单例模式的核心
    /// 1，定义一个静态的对象 在外界访问 在内部构造
    /// 2，构造方法私有化

    private static UIManger _instance;

    public static UIManger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManger();
            }
            return _instance;
        }
    }

    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }

    private Dictionary<UIName, string> panelPathDict;//存储所有面板Prefab的路径
    private Dictionary<UIName, BasePanel> panelDict;//保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack;

    private UIManger()
    {
        ParseUIPanelTypeJson();
    }

    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public void PushPanel(UIName panelType)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        //判断一下栈里面是否有页面
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause();
        }

        BasePanel panel = GetPanel(panelType);
        panel.OnEnter();
        panelStack.Push(panel);
    }

    /// <summary>
    /// 出栈 ，把页面从界面上移除
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        if (panelStack.Count <= 0) return;

        //关闭栈顶页面的显示
        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();

        if (panelStack.Count <= 0) return;
        BasePanel topPanel2 = panelStack.Peek();
        topPanel2.OnResume();

    }

    /// <summary>
    /// 根据面板类型 得到实例化的面板
    /// </summary>
    /// <returns></returns>
    private BasePanel GetPanel(UIName panelType)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<UIName, BasePanel>();
        }
        BasePanel panel = panelDict.TryGet(panelType);
        if (panel == null)
        {
            //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
            string path = panelPathDict.TryGet(panelType);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);
            panelDict.Add(panelType, instPanel.GetComponent<BasePanel>());
            return instPanel.GetComponent<BasePanel>();
        }
        else
        {
            return panel;
        }

    }

    void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<UIName, string>();
        TextAsset ta = Resources.Load<TextAsset>("UIPath");
        string[] strArr = ta.ToString().Trim().Split('\n');
        foreach (var item in strArr)
        {
            string[] Arr = item.Split('\t');
            UIName un = (UIName)Enum.Parse(typeof(UIName), Arr[0].Trim());
            panelPathDict.Add(un, Arr[1].Trim());
        }
    }
}
