﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StartPanel:UIBase
{
    public static StartPanel Instance;
    Button Introduce;
    Button PeoplePlay;
    Button CompterPlay;
    GameObject g;
    private void Awake()
    {
        Instance = this;
        InitUI();
    }
    #region 初始化BTN。绑定对应事件
    void InitUI()
    {
        Introduce = transform.Find("Introduce").GetComponent<Button>();
        Introduce.onClick.AddListener(Introduce_Event);
        PeoplePlay = transform.Find("PeoplePlay").GetComponent<Button>();
        PeoplePlay.onClick.AddListener(PeoplePlay_Event);
        CompterPlay = transform.Find("CompterPlay").GetComponent<Button>();
        CompterPlay.onClick.AddListener(CompterPlay_Event);
    }
    public void Introduce_Event()
    {
        IntroducePanel.Instance.Open();
        Close();
    }
    void PeoplePlay_Event()
    {
        PlayPanel.Instance.Open(0);
        Close();
    }
    void CompterPlay_Event()
    {
        PlayPanel.Instance.Open(1);
        Close();
    }
    #endregion


}
