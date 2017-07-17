using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

public class BaseManger : MonoBehaviour
{
    #region 一些属性
    public Base CurrentBase;//选中的棋子

    Sprite Black;
    Sprite Red;
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

    //挑和夹。只会触发一个
    //不管胜几个棋子。只要没路走就算输。测试
    //有时候会出现奇怪的吃法


    //出现还剩两个棋子但是没路走了怎么办
    //缺少介绍的4个按钮动画和结束输赢的动画
    //人人对战输赢结束场景
    //人机对战  电脑赢了出现什么场景 
    //挑和夹得触发条件不明确。先后顺序

    private void Awake()
    {
        Instance = this;

        BasePre = Resources.Load<GameObject>("Base");
        Red = Resources.Load<Sprite>("Image/红棋子");
        Black = Resources.Load<Sprite>("Image/黑棋子");

        CurrentBase = GameObject.Find("Canvas/Current").GetComponent<Base>();
    }
    void Start()
    {

    }
    void Update()
    {
        CurrentMove();
    }
    #region 选中棋子的跟随。显示。隐藏

    /// <summary>
    /// 点中棋子跟随鼠标
    /// </summary>
    void CurrentMove()
    {
        if (isCurrent)
        {
            CurrentBase.transform.position = Input.mousePosition;
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
    /// 
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
    #endregion

    IEnumerator Aplay()
    {
        yield return new WaitForSeconds(0.35f);
        if (AIPlay != null && StartBase != EndBase && StartBase.aroundBaseList.Contains(EndBase) && CurrentBase.Ps != PlayerState.Black)
        {
            AIPlay();
        }
    }

    /// <summary>
    /// 开始游戏初始化
    /// </summary>
    public void StartGame()
    {
        baseArr = new Base[5, 7];
        RedList = new List<Base>();
        BlackList = new List<Base>();

        tempPs = PlayerState.Red;
        PlayPanel.Instance.ChangsPromptText("该红色方走了");
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
                DestroyImmediate(baseArr[i, j].gameObject);
            }
        }
        if (AIPlay != null)
        {
            AIPlay-=CompterAi.Instance.AIPlayChess;
        }
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
        for (int i = 0; i < 5; i++)
        {
            baseArr[i, 0].PutDownChess(Black, PlayerState.Black);
            BlackList.Add(baseArr[i, 0]);
            baseArr[i, 4].PutDownChess(Red, PlayerState.Red);
            RedList.Add(baseArr[i, 4]);
        }
    }

    /// <summary>
    /// 周围竖向和横向的格子--不限制距离
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundLineBase(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -4; i <= 4; i++)
        {
            int x = b.x + i;
            if ((x >= 5 || x < 0))
            {
                continue;
            }
            for (int j = -6; j <= 6; j++)
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
    /// 获得周围周围格子--不限制距离
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundBase(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -4; i <= 4; i++)
        {
            int x = b.x + i;
            if ((x >= 5 || x < 0))
            {
                continue;
            }
            for (int j = -6; j <= 6; j++)
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
    public List<Base> GetAppointBase(int x,int y)
    {
        List<Base> Lb = new List<Base>();
        if (x==1&&y==5)
        {
            Lb.Add(baseArr[2, 4]);
            Lb.Add(baseArr[2, 5]);
            Lb.Add(baseArr[2, 6]);
            Lb.Add(baseArr[3, 5]);
            Lb.Add(baseArr[3, 3]);
            Lb.Add(baseArr[4, 2]);
        }
        else if (x == 3 && y == 5)
        {
            Lb.Add(baseArr[2, 4]);
            Lb.Add(baseArr[2, 5]);
            Lb.Add(baseArr[2, 6]);
            Lb.Add(baseArr[1, 5]);
            Lb.Add(baseArr[1, 3]);
            Lb.Add(baseArr[0, 2]);
        }
        else if (x == 2 && y == 6)
        {
            for (int i = 0; i < 7; i++)
            {
                Lb.Add(baseArr[2, i]);
            }
            Lb.Add(baseArr[1, 5]);
            Lb.Add(baseArr[3, 5]);
        }
        return Lb;
    }

    #region 判断两点中是否有障碍物和棋子是否符合移动的条件
    /// <summary>
    /// 判断结束格子是否在可走格子内
    /// </summary>
    /// <returns></returns>
    public bool endBaseInStartBaseList()
    {
        if (StartBase.aroundBaseList.Contains(EndBase))
        {
            //在内。可以走
            return true;
        }
        return false;
    }

    /// <summary>
    /// 起点到终点中间的Base
    /// </summary>
    /// <returns></returns>
    List<Base> StartToEndBaseList()
    {
        List<Base> Lb = new List<Base>();
        int x = Mathf.Abs(StartBase.x - EndBase.x);
        int y = Mathf.Abs(StartBase.y - EndBase.y);
        if (x == 0 && x != y)
        {
            //在同一行
            for (int i = 0; i < 7; i++)
            {
                Base temp = baseArr[StartBase.x, i];
                if (ValueInAtoB(StartBase.y, EndBase.y, temp.y))
                {
                    if (temp.canChess)
                    {
                        Lb.Add(temp);
                    }

                }
            }
        }
        else if (y == 0 && x != y)
        {
            //在同一列
            for (int i = 0; i < 5; i++)
            {
                Base temp = baseArr[i, StartBase.y];
                if (ValueInAtoB(StartBase.x, EndBase.x, temp.x))
                {
                    if (temp.canChess)
                    {
                        Lb.Add(temp);
                    }
                }
            }
        }
        else if (x == y)
        {
            //同一条斜线上的
            int num = Mathf.Abs(x);//间隔数量
            if (num > 1)
            {
                int tempx = (StartBase.x - EndBase.x) / num;
                int tempy = (StartBase.y - EndBase.y) / num;
                for (int i = 1; i < num; i++)
                {
                    Base temp = baseArr[StartBase.x - tempx * i, StartBase.y - tempy * i];
                    if (temp.canChess)
                    {
                        Lb.Add(temp);
                    }
                }
            }
        }
        return Lb;
    }

    /// <summary>
    /// 判断两点间是否有障碍物
    /// </summary>
    /// <returns></returns>
    public bool aroundObstacle()
    {
        foreach (var item in StartToEndBaseList())
        {
            if (!item.isDropedChess)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 判断能否棋子移动
    /// </summary>
    /// <returns></returns>
    public bool ChessMove()
    {
        bool InList = endBaseInStartBaseList();

        if (!InList)
        {
            return false;
        }
        bool obs = aroundObstacle();
        if (obs)
        {
            // 走
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断一个数。是否在另外两个数中间
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool ValueInAtoB(int num1, int num2, int target)
    {
        int max = Mathf.Max(num1, num2);
        int min = Mathf.Min(num1, num2);
        if (target < max && target > min)
        {
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// 移动失败恢复走前状态
    /// </summary>
    public void DontMove()
    {
        StartBase.PutDownChess(CloseCurrent(), true);
    }

    /// <summary>
    /// 吃棋子 传入落子的base
    /// </summary>
    /// <param name="b"></param>
    public void EatChess(Base b)
    {
        if (!WinOrLose())
        {
            Debug.Log("有一方不能走了");
            return;
        }
        OnePickTwo(b);//一挑二
        TwoClipOne(b);//二夹一
    }

    /// <summary>
    /// 二夹一
    /// </summary>
    /// <param name="b"></param>
    void TwoClipOne(Base b)
    {
        if (allowEat(b))
        {
            return;
        }
        //周围距离1格的棋子
        List<Base> AroundChess = aroundOneChess(b);
        //跟可以走的路径的做对比
        //相同路径的棋子切距离1格
        List<Base> SamePathChess = GetSamePathList(b, AroundChess);//跟可走路径相同的格子

        //如果有棋子而且类型一样。找找俩中间是否有棋子
        for (int i = 0; i < SamePathChess.Count; i++)
        {
            Base item = SamePathChess[i];
            if (b.Ps == item.Ps && !item.isDropedChess)
            {
                Base temp = GetBaseAroundByAtoB(b, item);
                if (temp==null)
                {
                    return;
                }
                if (temp.Ps != b.Ps && !temp.isDropedChess)
                {
                    //说明吃掉了
                    //TODO 修改
                    if (RedList.Count==2||BlackList.Count==2)
                    {
                        temp.PutDownChess(b, true);
                        return;
                    }
                    temp.PutDownChess(b, true);
                }
            }
        }
    }

    /// <summary>
    /// 一挑二
    /// </summary>
    /// <param name="b"></param>
    void OnePickTwo(Base b)
    {
        if (allowEat(b))
        {
            return;
        }
        List<Base> aroundList = aroundZeroBaseList(b);
        List<Base> samePathList = GetSamePathList(b, aroundList);
        while (samePathList.Count > 0)
        {
            Base temp01 = samePathList[0];
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
                temp01.PutDownChess(b, true);
                //TODO 修改 莫名的好了 (之前问题。一方只剩一个的话。如果挑吃的话。只会挑一个)
                //之前     RedList.Count > 2 && BlackList.Count > 2
                if (RedList.Count >= 2 && BlackList.Count >= 2)
                {
                    temp02.PutDownChess(b, true);
                }
                continue;
            }
            samePathList.Remove(temp01);
            samePathList.Remove(temp02);
        }
    }

    #region 输赢显示判断  

    /// <summary>
    /// 展示输赢
    /// </summary>
    /// <param name="ps"></param>
    void ShowWin(PlayerState ps)
    {
        if (AIPlay == null)
        {
            //人人对战
            if (ps == PlayerState.Red)//电脑输了 黑色输了
            {
                Debug.Log("红色赢了");
            }
            else
            {
                Debug.Log("黑色赢了");
            }
        }
        else
        {   //人机对战
            if (ps==PlayerState.Red)//电脑输了 黑色输了
            {
                EndGame();
                PlayPanel.Instance.Close();
                EndPanel.Instance.Open(PlayPanel.Instance.TotalTime);
            }
            else
            {
                Debug.Log("电脑赢了");
            }
        }
    }

    /// <summary>
    /// 输赢判断    不管剩几个字。如果一方的周围都不能走了。就算该方输
    /// </summary>
    /// <param name="b"></param>
    bool WinOrLose()
    {
        List<Base> TempList = new List<Base>();
        //遍历红色方
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
            PlayPanel.Instance.ChangsPromptText("红色赢了");
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
            PlayPanel.Instance.ChangsPromptText("黑色赢了");
            ShowWin(PlayerState.Black);
            return false;
        }
        Debug.Log("出大问题了");
        return false;
    }

    #endregion

    /// <summary>
    /// 获得周围紧贴着的格子
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundZeroBaseList(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -1; i <= 1; i++)
        {
            int x = b.x + i;
            if (x > 4 || x < 0)
            {
                continue;
            }
            for (int j = -1; j <= 1; j++)
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
    /// 周围距离1格的棋子
    /// </summary>
    public List<Base> aroundOneChess(Base b)
    {
        List<Base> Lb = new List<Base>();
        for (int i = -2; i <= 2; i = i + 2)
        {
            int x = b.x + i;
            if (x >= 5 || x < 0)
            {
                continue;
            }
            for (int j = -2; j <= 2; j = j + 2)
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
    public List<Base> GetSamePathList(Base b, List<Base> Lb)
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
        if (CurrentBase.Ps==PlayerState.Black)
        {
            tempPs = PlayerState.Red;
            PlayPanel.Instance.ChangsPromptText("该红色方走了");
        }
        else
        {
            tempPs = PlayerState.Black;
            PlayPanel.Instance.ChangsPromptText("该黑色方走了");
        }
        PlayPanel.Instance.ChangPlayer();
    }

    /// <summary>
    /// 得到B点周围可走路径并且紧邻的格子中。为红色棋子的数组
    /// </summary>
    /// <returns></returns>
    public List<Base> AroundRedBase(Base b)
    {
        List<Base> lb = new List<Base>();
        //Debug.Log(GetSamePathList(b, aroundZeroBaseList(b)).Count);
        //foreach (var item in GetSamePathList(b, aroundZeroBaseList(b)))
        foreach (var item in b.aroundBaseList)
        {
            if (!item.isDropedChess&&item.canChess&&item.Ps==PlayerState.Red)
            {
                //Debug.Log("建立");
                //这个格子、不能走。已经放了东西了。而且是红色的
                lb.Add(item);
            }
        }
        return lb;
    }

    /// <summary>
    /// 判断该不该继续吃。根据列表里的格子种类数量
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    bool allowEat(Base b)
    {
        if (RedList.Count<=1&& b.Ps==PlayerState.Black)
        {
            return true;
        }
        else if (BlackList.Count <= 1 && b.Ps == PlayerState.Red)
        {
            return true;
        }
        return false;
    }



}
