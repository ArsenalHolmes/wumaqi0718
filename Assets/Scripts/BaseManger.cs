﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;


public class BaseManger : MonoBehaviour
{
    #region 一些属性
    public Base CurrentBase;//选中的棋子

    GameObject BasePre;

    public static BaseManger Instance;
    
    public bool isCurrent;
    public Base[,] baseArr;
    Action InitBack;

    public Base StartBase;
    public Base EndBase;

    public List<Base> RedList;
    public List<Base> BlackList;

    public PlayerState tempPs;
    public Action AIPlay;
    #endregion

    //TODO 注意点  

    //不管胜几个棋子。只要没路走就算输。测试
    //一次走一个格子

    //平局咋算
    //缺少介绍的4个按钮动画和结束输赢的动画

    private void Awake()
    {
        Instance = this;

        BasePre = Resources.Load<GameObject>("Base");

        CurrentBase = GameObject.Find("Canvas/Current").GetComponent<Base>();
    }

    void Start()
    {
        
    }
    void Test()
    {
        List<int> list01 = new List<int>() {1,3,5,7,9};
        List<int> list02 = new List<int>() {2,4,6,8,10};
        List<int> list03 = new List<int>();
        int alen = list01.Count;
        int blen = list02.Count;
        int a=0, b=0;
        while (a<alen&&b<blen)
        {
            if (list01[a]<list02[b])
            {
                list03.Add(list01[a]);
                a++;
            }
            else
            {
                list03.Add(list02[b]);
                b++;
            }
        }
        while (a<alen)
        {
            list03.Add(list01[a]);
            a++;
        }
        while (b<blen)
        {
            list03.Add(list02[b]);
            b++;
        }
        foreach (var item in list03)
        {
            Debug.Log(item);
        }
    }

    void Update()
    {
        //CurrentMove();
        //if (isCurrent && !Input.GetMouseButton(0))
        if (isCurrent && Input.GetMouseButtonUp(0))
        {
            DontMove();
        }
    }
    private void FixedUpdate()
    {
        //CurrentMove();
        if (isCurrent && Input.GetMouseButton(0))
        {
            CurrentBase.transform.position = Input.mousePosition;
        }
    }

    #region 选中棋子的跟随。显示。隐藏

    /// <summary>
    /// 点中棋子跟随鼠标
    /// </summary>
    void CurrentMove()
    {
        if (isCurrent&&Input.GetMouseButton(0))
        {
            CurrentBase.transform.position = Input.mousePosition;
        }
        else if (isCurrent&&!Input.GetMouseButton(0))
        {
            DontMove();
        }
    }

    /// <summary>
    /// 显示点中棋子
    /// </summary>
    /// <param name="s"></param>
    public void ShowCurrent(Base b)
    {
        CurrentBase.transform.position = Input.mousePosition;
        isCurrent = true;
        CurrentBase.PutDownChess(b,true,true);
        CurrentBase.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏点中棋子
    /// </summary>
    /// <returns></returns>
    public Base CloseCurrent(bool back=false)
    {
        isCurrent = false;
        CurrentBase.gameObject.SetActive(false);
        if (back)
        {
            ChangsPlayer();
            StartCoroutine(Aplay());
        }

        return CurrentBase;
    }

    /// <summary>
    /// 实现AI的移动
    /// </summary>
    /// <returns></returns>
    IEnumerator Aplay()
    {
        yield return new WaitForSeconds(0.35f);
        if (AIPlay != null && StartBase != EndBase && StartBase.aroundBaseList.Contains(EndBase) && CurrentBase.Ps != PlayerState.Black)
        {
            AIPlay();
        }
    }

    #endregion

    #region 一些初始化

    /// <summary>
    /// 开始游戏初始化
    /// </summary>
    public void StartGame()
    {
        baseArr = new Base[5, 7];
        RedList = new List<Base>();
        BlackList = new List<Base>();

        tempPs = PlayerState.Red;
        NotificationManger.Instance.DispatchEvent(EventName.PlayerChang, new Notification(1));
        InitBase();
        InitPlayerChess();
    }

    /// <summary>
    /// 结束游戏初始化
    /// </summary>
    public void EndGame()
    {
        if (isCurrent)
        {
            DontMove();
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                InitBack -= baseArr[i, j].InitList;
                Destroy(baseArr[i, j].gameObject);
            }
        }
        if (AIPlay != null)
        {
            AIPlay-=CompterAi.Instance.AIPlayChess;
        }
        PlayPanel.Instance.isPlay = false;
    }

    /// <summary>
    /// 初始化Base
    /// </summary>
    void InitBase()
    {
        for (int i = 0; i <= 4; i++)
        {
            for (int j = 0; j <= 6; j++)
            {
                GameObject g = Instantiate(BasePre);
                g.transform.SetParent(transform);
                g.transform.localScale = Vector3.one;
                
                Base b = g.GetComponent<Base>();
                if (j==5&&(i==0||i==4))
                {
                    b.canChess = false;
                }
                else if (j==6&&i!=2)
                {
                    b.canChess = false;
                }
                baseArr[i, j] = b;
                b.x = i;
                b.y = j;
                b.Ps = PlayerState.None;
                InitBack += b.InitList;

                g.name = i + "-" + j;
            }
        }
        InitBack();
    }

    /// <summary>
    /// 初始化玩家格子
    /// </summary>
    void InitPlayerChess()
    {

        Sprite Black = Resources.Load<Sprite>("Image/黑棋子");
        Sprite Red = Resources.Load<Sprite>("Image/红棋子");
        for (int i = 0; i < 5; i++)
        {
            baseArr[0, i].PutDownChess(Black, PlayerState.Black);
            BlackList.Add(baseArr[0, i]);
            baseArr[4, i].PutDownChess(Red, PlayerState.Red);
            RedList.Add(baseArr[4, i]);
        }
        baseArr[1, 0].PutDownChess(Black, PlayerState.Black);
        BlackList.Add(baseArr[1, 0]);
        baseArr[1, 4].PutDownChess(Black, PlayerState.Black);
        BlackList.Add(baseArr[1, 4]);
        baseArr[3, 0].PutDownChess(Red, PlayerState.Red);
        RedList.Add(baseArr[3, 0]);
        baseArr[3, 4].PutDownChess(Red, PlayerState.Red);
        RedList.Add(baseArr[3, 4]);

    }

    #endregion

    #region 判断两点中是否有障碍物和棋子是否符合移动的条件

    /// <summary>
    /// 判断结束格子是否在可走格子内
    /// </summary>
    /// <returns></returns>
    public bool endBaseInStartBaseList(Base a ,Base b)
    {
        if (a.aroundBaseList.Contains(b))
        {
            //在内。可以走
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断能否棋子移动
    /// </summary>
    /// <returns></returns>
    public bool ChessMove(Base start = null,Base end = null)
    {
        if (start==null&&end==null)
        {
            start = StartBase;
            end = EndBase;
        }
        bool InList = endBaseInStartBaseList(start,end);

        if (InList)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region 一些其他的方法

    /// <summary>
    /// 移动失败恢复走前状态
    /// </summary>
    public void DontMove()
    {
        StartBase.PutDownChess(CloseCurrent(), true);
    }

    /// <summary>
    /// 倒计时到。随机走一步
    /// </summary>
    public void TimeOut()
    {
        List<Base> tempList = GetPlayerBaseList(tempPs);
        Base temp;
        Base temp02;
        while (true)
        {
            temp = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            temp02 = temp.aroundBaseList[UnityEngine.Random.Range(0, temp.aroundBaseList.Count)];
            if (temp02.Ps == PlayerState.None && temp02.isDropedChess)
            {
                break;
            }
        }
        Instance.StartBase = temp;
        Instance.EndBase = temp02;
        Base AIBase = temp.TakeUpChess();//拿起
        temp02.PutDownChess(AIBase, false, false, true);//放下
        ChangsPlayer();//换人走
    }

    #endregion

    #region 吃棋子的判断和实现

    /// <summary>
    /// 吃棋子 传入落子的base
    /// </summary>
    /// <param name="b"></param>
    public void EatChess(Base b)
    {
        ToEatBase(OnePickTwo(b));//一挑二
        ToEatBase(TwoClipOne(b));//二夹一
        ToEatBase(dachi(b));//打吃

        if (WinOrLose())
        {
            WinOrLose02();
        }
    }

    /// <summary>
    /// 二夹一
    /// </summary>
    /// <param name="b"></param>
     List<Base> TwoClipOne(Base b)
    {
        //周围距离1格的棋子
        List<Base> AroundChess = aroundZeroBaseList(b,2);
        List<Base> lb = new List<Base>();
        //如果有棋子而且类型一样。找找俩中间是否有棋子
        for (int i = 0; i < AroundChess.Count; i++)
        {
            Base item = AroundChess[i];
            if (b.Ps == item.Ps && !item.isDropedChess)
            {
                Base temp = GetBaseAroundByAtoB(b, item);
                if (temp==null)
                {
                    continue;
                }
                if (temp.Ps != b.Ps && !temp.isDropedChess)
                {
                    //说明吃掉了
                    lb.Add(temp);
                }
            }
        }
        return lb;
    }

    /// <summary>
    /// 一挑二
    /// </summary>
    /// <param name="b"></param>
     List<Base> OnePickTwo(Base b)
    {
        List<Base> aroundList = aroundZeroBaseList(b);
        List<Base> samePathList = GetSamePathList(b, aroundList);
        List<Base> lb = new List<Base>();
        while (samePathList.Count > 0)
        {
            Base temp01 = samePathList[0];
            if (temp01.Ps==b.Ps)
            {
                samePathList.Remove(temp01);
                continue;
            }
            Base temp02 = GetSymmetricBase(b, temp01);
            if (temp02 == null)//如果temp02为空说明那边没格子
            {
                samePathList.Remove(temp01);
                continue;
            }
            else if (temp01.isDropedChess || temp02.isDropedChess || temp01.Ps != temp02.Ps)
            {
                //如果2个格子中有一个格子没棋子或者两个格子的颜色不一致则执行
                //要求两个格子都必须都东西。并且两个格子的颜色一致
                samePathList.Remove(temp01);
                samePathList.Remove(temp02);
                continue;
            }
            else if (temp01.Ps == temp02.Ps && !temp01.isDropedChess && !temp02.isDropedChess)
            {
                samePathList.Remove(temp01);
                samePathList.Remove(temp02);
                lb.Add(temp01);
                lb.Add(temp02);
                continue;
            }
            Debug.Log("不会到这");
            samePathList.Remove(temp01);
            samePathList.Remove(temp02);
        }
        return lb;
    }

    /// <summary>
    /// 打吃  同一条线上两颗相邻的相同颜色的棋子。可以吃在他俩同一条线上的其他棋子;
    /// </summary>
     List<Base> dachi(Base b)
    {
        List<Base> AroundSameBase = new List<Base>();
        //遍历周围紧贴的格子。并且是相同路径的
        foreach (var item in GetSamePathList(b, aroundZeroBaseList(b)))
        {
            //如果这个格子跟方子的格子ps一样。加入LIST
            if (item.Ps==EndBase.Ps)
            {
                AroundSameBase.Add(item);
            }
        }
        List<Base> lbs = new List<Base>();
        foreach (var item in AroundSameBase)
        {
            List<Base> lb = GetLineBase(b, item);
            foreach (var tempBase in lb)
            {
                lbs.Add(tempBase);
            }
        }
        return lbs;
    }

    /// <summary>
    /// 实现列表中的格子被吃掉
    /// </summary>
    /// <param name="lb"></param>
    void ToEatBase(List<Base> lb)
    {
        foreach (var item in lb)
        {
            item.BeEat();
        }
    }

    #endregion

    #region 输赢显示和判断  

    /// <summary>
    /// 展示输赢
    /// </summary>
    /// <param name="ps"></param>
    void ShowWin(PlayerState ps)
    {
        Notification no = new Notification();
        //TODO 人人对战的结局
        if (ps == PlayerState.Red)
        {
            no.Str = "RedWin";
        }
        else if (ps==PlayerState.Black)
        {
            no.Str = "BlackWin";
        }
        EndGame();

        UIManger.Instance.PushPanel(UIName.EndPanel);
        NotificationManger.Instance.DispatchEvent(EventName.WinImage, no);
    }

    /// <summary>
    /// 输赢判断    不管剩几个字。如果一方的周围都不能走了。就算该方输
    /// </summary>
    /// <param name="b"></param>
    bool WinOrLose()
    {
        List<Base> TempList = new List<Base>();
        //遍历黑色方
        if (EndBase.Ps == PlayerState.Red)
        {
            foreach (var item in BlackList)
            {
                TempList = GetSamePathList(item, aroundZeroBaseList(item));
                foreach (var item02 in TempList)
                {
                    if (item02.isDropedChess)
                    {
                        return true;
                    }
                }
            }
            //说明黑色的无路可走了
            Debug.Log("qq");
            ShowWin(PlayerState.Red);
            return false;
        }
        else if (EndBase.Ps == PlayerState.Black)
        {
            foreach (var item in RedList)
            {
                TempList = GetSamePathList(item, aroundZeroBaseList(item));
                foreach (var item02 in TempList)
                {
                    if (item02.isDropedChess)
                    {
                        return true;
                    }
                }
            }
            //说明红色的无路可走了
            Debug.Log("ww");
            ShowWin(PlayerState.Black);
            return false;
        }
        Debug.Log("出大问题了");
        return false;
    }

    /// <summary>
    /// 第二种输赢判断     看双方剩余棋子的数量
    /// </summary>
    /// <returns></returns>
    bool WinOrLose02()
    {
        if (RedList.Count==0)
        {
            ShowWin(PlayerState.Black);
            return false;
        }
        else if (BlackList.Count==0)
        {
            Debug.Log("ee");
            ShowWin(PlayerState.Red);
            return false;
        }
        else if (BlackList.Count==1&&RedList.Count==1)
        {
            Debug.Log("rr");
            //TODO 平局算黑手赢
            ShowWin(PlayerState.Black);
            return false;
        }
        return true;
    }

    #endregion

    #region 获得特定格子的方法

    /// <summary>
    /// 周围竖向和横向的格子--一格
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundLineBase(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -1; i <= 1; i++)
        {
            int x = b.x + i;
            if ((x >= 5 || x < 0))
            {
                continue;
            }
            for (int j = -1; j <= 1; j++)
            {
                int y = b.y + j;
                if (y >= 7 || y < 0 || Mathf.Abs(i) == Mathf.Abs(j))
                {
                    continue;
                }
                if ((i != 0 && j != 0))
                {
                    continue;
                }
                if (baseArr[x, y].canChess)
                {
                    Lb.Add(baseArr[x, y]);
                }

            }
        }
        return Lb;
    }

    /// <summary>
    /// 获得周围周围格子--格
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundBase(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -1; i <= 1; i++)
        {
            int x = b.x + i;
            if ((x >= 5 || x < 0))
            {
                continue;
            }
            for (int j = -1; j <= 1; j++)
            {
                if ((Mathf.Abs(i) == Mathf.Abs(j) || i == 0 || j == 0) && !(i == 0 && j == 0))
                {
                    int y = b.y + j;
                    if (y >= 7 || y < 0)
                    {
                        continue;
                    }
                    if (baseArr[x, y].canChess)
                    {
                        Lb.Add(baseArr[x, y]);
                    }
                }

            }
        }
        return Lb;
    }

    /// <summary>
    /// 得到特定点的可移动的base
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<Base> GetAppointBase(int x, int y)
    {
        List<Base> Lb = new List<Base>();
        if ((x == 1 && y == 5) || (x == 3 && y == 5))
        {
            Lb.Add(baseArr[2, 4]);
            Lb.Add(baseArr[2, 5]);
            Lb.Add(baseArr[2, 6]);
        }
        else if (x == 2 && y == 6)
        {

            Lb.Add(baseArr[2, 5]);
            Lb.Add(baseArr[1, 5]);
            Lb.Add(baseArr[3, 5]);
        }
        return Lb;
    }

    /// <summary>
    /// 获得周围紧贴着的格子
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    List<Base> aroundZeroBaseList(Base b,int num =1)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -1*num; i <= 1 * num; i=i+1 * num)
        {
            int x = b.x + i;
            if (x > 4 || x < 0)
            {
                continue;
            }
            for (int j = -1 * num; j <= 1 * num; j=j+1 * num)
            {
                int y = b.y + j;
                if (y > 6 || y < 0)
                {
                    continue;
                }
                if (baseArr[x, y].canChess)
                {
                    Lb.Add(baseArr[x, y]);
                }
            }
        }
        return Lb;
    }

    /// <summary>
    /// 返回两个格子中间的格子
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    Base GetBaseAroundByAtoB(Base a, Base b)
    {
        int x = (a.x + b.x) / 2;
        int y = (a.y + b.y) / 2;
        //这个格子可以走并且这个格子在两个格子的可走路径中
        if (baseArr[x, y].canChess && a.aroundBaseList.Contains(baseArr[x, y]) && b.aroundBaseList.Contains(baseArr[x, y]))
        {
            return (baseArr[x, y]);
        }
        return null;
    }

    /// <summary>
    /// 得到存储玩家Base的List
    /// </summary>
    /// <param name="ps"></param>
    /// <returns></returns>
    public List<Base> GetPlayerBaseList(PlayerState ps)
    {
        if (ps == PlayerState.Red)
        {
            return RedList;
        }
        else if (ps==PlayerState.Black)
        {
            return BlackList;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 根据b可走路径的list和周围格子的list 得到b周围可走的格子
    /// </summary>
    /// <param name="b"></param>
    /// <param name="Lb"></param>
    /// <returns></returns>
    List<Base> GetSamePathList(Base b, List<Base> Lb)
    {
        List<Base> lb = new List<Base>();
        foreach (var item in Lb)
        {
            if (b.aroundBaseList.Contains(item))
            {
                if (item.canChess)
                {
                    lb.Add(item);
                }
            }
        }
        return lb;
    }

    /// <summary>
    /// 得到以center为中心。point为点的。对称对应的base
    /// </summary>
    /// <param name="center"></param>
    /// <param name="Point"></param>
    /// <returns></returns>
    Base GetSymmetricBase(Base center, Base Point)
    {
        int tempx = center.x - Point.x;
        int tempy = center.y - Point.y;
        int x = center.x + tempx;
        int y = center.y + tempy;
        if (x > 4 || x < 0 || y > 6 || y < 0|| !baseArr[x, y].canChess)
        {
            return null;
        }
        //另外一个格子的可走路径必须有中心点
        if (!baseArr[x, y].aroundBaseList.Contains(center))
        {
            return null;
        }
        return baseArr[x, y];
    }

    /// <summary>
    /// 该另外一个人走了
    /// </summary>
    public void ChangsPlayer()
    {
        if (StartBase==EndBase||!StartBase.aroundBaseList.Contains(EndBase))
        {
            return;
        }
        if (tempPs == PlayerState.Black)
        {
            tempPs = PlayerState.Red;
            Notification no = new Notification();
            no.Flo = 1f;
            NotificationManger.Instance.DispatchEvent(EventName.PlayerChang, no);
            return;
        }
        else if (tempPs==PlayerState.Red)
        {
            tempPs = PlayerState.Black;
            Notification no = new Notification();
            no.Flo = 2f;
            NotificationManger.Instance.DispatchEvent(EventName.PlayerChang, no);
            return;
        }
        Debug.Log(tempPs);
        Debug.Log("出问题了");
    }

    /// <summary>
    /// 返回根据在两点确定的同一条线上的另外两个格子
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    List<Base> GetLineBase(Base start,Base end)
    {
        int tempx = start.x - end.x;
        int tempy = start.y - end.y;
        int x1 = start.x + tempx;
        int y1 = start.y + tempy;
        int x2 = end.x - tempx;
        int y2 = end.y - tempy;
        List<Base> lb = new List<Base>();
        if (x1 >= 0 && x1 < 5 && y1 >= 0 && y1 < 7) 
        {
            if (baseArr[x1, y1].Ps != start.Ps&& baseArr[x1, y1].Ps !=PlayerState.None)
            {
                lb.Add(baseArr[x1, y1]);
            }
        }
        if (x2 >= 0 && x2 < 5 && y2 >= 0 && y2 < 7)
        {
            if (baseArr[x2, y2].Ps != start.Ps && baseArr[x2, y2].Ps != PlayerState.None)
            {
                lb.Add(baseArr[x2, y2]);
            }
        }
        return lb;
    }

    /// <summary>
    /// 得到指定格子可以吃其他格子的数量
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public int GetNum(Base b)
    {
        int num = OnePickTwo(b).Count + TwoClipOne(b).Count + dachi(b).Count;
        return num;
    }

    #endregion

}
