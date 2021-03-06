﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public enum PlayerState
{
    Red,
    Black,
    None,
}

public class Base : MonoBehaviour ,IDropHandler, IInitializePotentialDragHandler,IDragHandler
{
    Color c = new Color(1, 1, 1, 0);//初始颜色
    Image _image;//图片
    public bool canChess=true;//能不能下棋//默认可以
    public bool isDropedChess;//能下棋。true 可以走
    public int x;
    public int y;
    public PlayerState Ps;

    public List<Base> aroundBaseList;//周围可走路径
    
    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _image.color = c;
    }

    /// <summary>
    /// 初始化可走格子
    /// </summary>
    public void InitList()
    {
        //待优化 初始化可走格子
        bool b1 = ((x == 0||x==2||x==4) && (y == 1 || y == 3||y==5));
        bool b2 = ((x == 1||x==3) && (y == 0 || y == 2|| y == 4));
        if (b1||b2)
        {
            aroundBaseList = BaseManger.Instance.aroundLineBase(this);
        }
        else
        {
            aroundBaseList = BaseManger.Instance.aroundBase(this);
        }
        if ((x==0&&y==2)|| (x == 1 && y == 3) || (x == 4 && y == 2) || (x == 3 && y == 3) )
        {
            return;
        }
        //if (x==2&&y==5)
        //{
        //    aroundBaseList.Remove(BaseManger.Instance.baseArr[0, 5]);
        //    aroundBaseList.Remove(BaseManger.Instance.baseArr[4, 5]);
        //}
        if ((x==3||x==1||x==0||x==4)&&y==4)
        {
            aroundBaseList.Remove(BaseManger.Instance.baseArr[1, 5]);
            aroundBaseList.Remove(BaseManger.Instance.baseArr[3, 5]);
            //aroundBaseList.Remove(BaseManger.Instance.baseArr[2, 6]);
        }
        if (((x==1||x==3)&&y==5)||(x==2&&y==6))
        {
            aroundBaseList = BaseManger.Instance.GetAppointBase(x, y);
        }
    }

    /// <summary>
    /// //放下棋子
    /// </summary>
    /// <param name="b">实现的目标</param>
    /// <param name="back">是否是退回  用来区别是否是退回-退回true正常下false</param>
    /// <param name="current">是否是选中的棋子</param>
    /// <param name="ai">是否是AI放置的棋子</param>
    public void PutDownChess(Base b,bool back=false,bool current=false,bool ai=false)
    {
        if (ai)
        {
            _image.DOFade(1, 1);//TODO 修改AI棋子显示的时间
            _image.sprite = b._image.sprite;
        }
        else
        {
            _image.color = Color.white;
            _image.sprite = b._image.sprite;
        }

        isDropedChess = false;
        Ps = b.Ps;
        b.Ps = PlayerState.None;
        if (current)
        {
            return;
        }
        BaseManger.Instance.GetPlayerBaseList(Ps).Add(this);
        if (back)
        {
            return;
        }
        BaseManger.Instance.EatChess(this);
    }

    /// <summary>
    /// 初始化图片
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p"></param>
    public void PutDownChess(Sprite s,PlayerState p)
    {
        _image.color = Color.white;
        _image.sprite = s;
        isDropedChess = false;
        Ps = p;
    }

    /// <summary>
    /// 拿起棋子
    /// </summary>
    /// <returns></returns>
    public Base TakeUpChess()
    {
        isDropedChess = true;
        _image.color = c;
        BaseManger.Instance.GetPlayerBaseList(Ps).Remove(this);
        return this;
    }

    /// <summary>
    /// 被吃之后初始化
    /// </summary>
    public void BeEat()
    {
        _image.DOFade(0, 0.5f);//TODO 修改被吃掉棋子消失的时间
        BaseManger.Instance.GetPlayerBaseList(Ps).Remove(this);
        isDropedChess = true;
        Ps = PlayerState.None;
    }

    #region 旧的棋子移动模式 点击模式  要改回去需要改BaseManger.CurrentMove

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Ps != BaseManger.Instance.tempPs && !BaseManger.Instance.isCurrent)
        {
            //该走颜色的棋子。跟你选择的棋子不一致
            return;
        }
        if (eventData == null || PointerEventData.InputButton.Left == eventData.button)
        {
            if (BaseManger.Instance.isCurrent)//选中东西了
            {
                if (isDropedChess)
                {
                    //先判断能否走这个格子
                    BaseManger.Instance.EndBase = this;
                    if (!BaseManger.Instance.ChessMove())
                    {
                        //不能走
                        BaseManger.Instance.DontMove();
                        return;
                    }
                    //这块没东西把东西放下
                    PutDownChess(BaseManger.Instance.CloseCurrent(true));
                }
            }
            else
            {
                if (!isDropedChess)
                {
                    //拿起东西
                    BaseManger.Instance.CurrentBase.Ps = Ps;
                    BaseManger.Instance.ShowCurrent(TakeUpChess());
                    BaseManger.Instance.StartBase = this;
                }
            }
        }
    }

    #endregion

    public void OnDrop(PointerEventData eventData)
    {
        //放下时候执行
        if (Ps != BaseManger.Instance.tempPs && !BaseManger.Instance.isCurrent)
        {
            //该走颜色的棋子。跟你选择的棋子不一致
            return;
        }
        if (eventData == null || PointerEventData.InputButton.Left == eventData.button)
        {
            if (BaseManger.Instance.isCurrent)//选中东西了
            {
                if (isDropedChess)
                {
                    //先判断能否走这个格子
                    BaseManger.Instance.EndBase = this;
                    if (!BaseManger.Instance.ChessMove())
                    {
                        //不能走
                        BaseManger.Instance.DontMove();
                        return;
                    }
                    //这块没东西把东西放下
                    PutDownChess(BaseManger.Instance.CloseCurrent(true));
                }
            }
        }

    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (Ps != BaseManger.Instance.tempPs && !BaseManger.Instance.isCurrent)
        {
            //该走颜色的棋子。跟你选择的棋子不一致
            return;
        }
        if (eventData == null || PointerEventData.InputButton.Left == eventData.button)
        {
            if (!isDropedChess)
            {
                //拿起东西
                BaseManger.Instance.CurrentBase.Ps = Ps;
                BaseManger.Instance.ShowCurrent(TakeUpChess());
                BaseManger.Instance.StartBase = this;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
}
