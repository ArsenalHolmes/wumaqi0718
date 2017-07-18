using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class CompterAi : MonoBehaviour
{

    public static CompterAi Instance;
    Base AIBase;
    private void Awake()
    {
        Instance = this;
    }
    public void AIPlayChess()
    {
        //TODO 当玩家棋子只剩一个的时候。会弱智
        List<MoveBase> MBL = GetBaseCanMoveList();
        MBL = Sort(MBL);//根据NUM排序  NUM是走的那里可以吃的格子的数量
        MoveBase mbTemp;
        if (BaseManger.Instance.RedList.Count==1)
        {
            Debug.Log("红色就剩一个了");
            mbTemp = GetMovePath(BaseManger.Instance.RedList[0], MBL);
        }
        else
        {
            MBL = GetMaxNumList(MBL);//得到最大值得列表
            if (MBL.Count == 0)
            {
                Debug.Log("出大问题了AI没路可以走了");
                return;
            }
            
            int index = Random.Range(0, MBL.Count);
            mbTemp = MBL[index];
        }
        BaseManger.Instance.StartBase = mbTemp.Start;
        BaseManger.Instance.EndBase = mbTemp.End;
        AIBase = mbTemp.Start.TakeUpChess();//拿起
        mbTemp.End.PutDownChess(AIBase,false,false,true);//放下
        BaseManger.Instance.ChangsPlayer();//换人走
    }

    /// <summary>
    /// 得到黑色格子所有可以走的路径
    /// </summary>
    /// <returns></returns>
    List<MoveBase> GetBaseCanMoveList()
    {
        List<MoveBase> MBL = new List<MoveBase>();
        foreach (var b in BaseManger.Instance.BlackList)
        {
            foreach (var item in b.aroundBaseList)
            {
                if (item.isDropedChess)
                {
                    int count = BaseManger.Instance.GetNum(item);
                    //MoveBase mb = new MoveBase(b, item, BaseManger.Instance.AroundRedBase(item).Count);
                    MoveBase mb = new MoveBase(b, item, count);
                    if (mb.Canmove)
                    {
                        MBL.Add(mb);
                    }
                    
                }
            }
        }
        return MBL;
    }

    /// <summary>
    /// 列表根据NUM排序
    /// </summary>
    /// <param name="lmb"></param>
    /// <returns></returns>
    List<MoveBase> Sort(List<MoveBase> lmb)
    {
        //外层循环控制的是趟数
        for (int i = 0; i < lmb.Count - 1; i++)
        {
            for (int j = 0; j < lmb.Count - 1 - i; j++)
            {
                if (lmb[j].Num < lmb[j + 1].Num)
                {
                    MoveBase temp = lmb[j];
                    lmb[j] = lmb[j + 1];
                    lmb[j + 1] = temp;
                }
            }
        }
        return lmb;
    }

    /// <summary>
    /// 获得列表中。前两个可以走的格子
    /// </summary>
    /// <param name="lmb"></param>
    /// <returns></returns>
    List<MoveBase> GetMaxNumList(List<MoveBase> lmb)
    {
        List<MoveBase> lb = new List<MoveBase>();
        for (int i = 0; i < lmb.Count; i++)
        {
            if (lmb[i].Canmove)
            {
                lb.Add(lmb[i]);
            }
            if (lb.Count>=2)
            {
                break;
            }
        }
        return lb;
    }

    MoveBase GetMovePath(Base b , List<MoveBase> lmb)
    {
        foreach (var item in lmb)
        {
            if (b.aroundBaseList.Contains(item.End))
            {
                return item;
            }
        }
        Debug.Log("没找到");
        return null;
    }


}
public class MoveBase 
{
    Base start;
    Base end;
    int num;

    public MoveBase(Base start, Base end, int num)
    {
        this.start = start;
        this.end = end;
        this.num = num;
    }

    public Base Start
    {
        get
        {
            return start;
        }

        set
        {
            start = value;
        }
    }

    public Base End
    {
        get
        {
            return end;
        }

        set
        {
            end = value;
        }
    }

    public int Num
    {
        get
        {
            return num;
        }

        set
        {
            num = value;
        }
    }

    public bool Canmove
    {
        get
        {
            return BaseManger.Instance.ChessMove(start, end);
        }
    }
    public override string ToString()
    {
        return (start + "  " + end + " " + num);
    }
}
