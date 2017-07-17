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


    //出现还剩多个棋子但是没路走了怎么办
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
        for (int i = 0; i < 5; i++)
        {
            baseArr[i, 0].PutDownChess(Black, PlayerState.Black);
            BlackList.Add(baseArr[i, 0]);
            baseArr[i, 4].PutDownChess(Red, PlayerState.Red);
            RedList.Add(baseArr[i, 4]);
        }
        baseArr[0, 1].PutDownChess(Black, PlayerState.Black);
        BlackList.Add(baseArr[0, 1]);
        baseArr[4, 1].PutDownChess(Black, PlayerState.Black);
        BlackList.Add(baseArr[4, 1]);
        baseArr[0, 3].PutDownChess(Red, PlayerState.Red);
        RedList.Add(baseArr[0, 3]);
        baseArr[4, 3].PutDownChess(Red, PlayerState.Red);
        RedList.Add(baseArr[4, 3]);

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
        OnePickTwo(b);//一挑二
        TwoClipOne(b);//二夹一
        ToEat();
        if (!WinOrLose02() || !WinOrLose())
        {
            Debug.Log("有一方不能走了");
            return;
        }
    }

    /// <summary>
    /// 二夹一
    /// </summary>
    /// <param name="b"></param>
    void TwoClipOne(Base b)
    {
        //周围距离1格的棋子
        List<Base> AroundChess = aroundZeroBaseList(b,2);
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
                    Debug.Log(temp + "挑吃");
                    temp.BeEat();
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
        List<Base> aroundList = aroundZeroBaseList(b);
        List<Base> samePathList = GetSamePathList(b, aroundList);
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
                Debug.Log(temp01 + "卡吃");
                samePathList.Remove(temp01);
                samePathList.Remove(temp02);
                temp01.BeEat();
                temp02.BeEat();
                continue;
            }
            samePathList.Remove(temp01);
            samePathList.Remove(temp02);
        }
    }

    /// <summary>
    /// 打吃  同一条线上两颗相邻的相同颜色的棋子。可以吃在他俩同一条线上的其他棋子;
    /// </summary>
    void ToEat()
    {
        List<Base> AroundSameBase = new List<Base>();
        //遍历周围紧贴的格子。并且是相同路径的
        foreach (var item in GetSamePathList(EndBase, aroundZeroBaseList(EndBase)))
        {
            //如果这个格子跟方子的格子ps一样。加入LIST
            if (item.Ps==EndBase.Ps)
            {
                AroundSameBase.Add(item);
            }
        }
        foreach (var item in AroundSameBase)
        {
            List<Base> lb = GetLineBase(EndBase, item);
            foreach (var tempBase in lb)
            {
                Debug.Log(tempBase + "打吃");
                tempBase.BeEat();
            }
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
                PlayPanel.Instance.ChangsPromptText("红色赢了");
            }
            else
            {
                Debug.Log("黑色赢了");
                PlayPanel.Instance.ChangsPromptText("黑色赢了");
            }
        }
        else
        {   //人机对战
            if (ps==PlayerState.Red)//电脑输了 黑色输了 跳转最后界面
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
            ShowWin(PlayerState.Black);
            return false;
        }
        Debug.Log("出大问题了");
        return false;
    }

    /// <summary>
    /// 第二种输赢判断     看双方是否还剩棋子
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
            ShowWin(PlayerState.Red);
            return false;
        }
        return true;
    }

    #endregion

    /// <summary>
    /// 获得周围紧贴着的格子
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public List<Base> aroundZeroBaseList(Base b,int num =1)
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
    /// 周围距离1格的棋子   //没用了
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
        if (tempPs == PlayerState.Black)
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




}
