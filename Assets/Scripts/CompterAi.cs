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
        List<MoveBase> MBL = new List<MoveBase>();
        //Base TempBase=null;
        //Base StartBase=null;
        foreach (var b in BaseManger.Instance.BlackList)
        {
            int index = Random.Range(0, BaseManger.Instance.BlackList.Count);
            foreach (var item in b.aroundBaseList)
            {
                if (item.isDropedChess)
                {
                    int count = BaseManger.Instance.GetNum(item);
                    //MoveBase mb = new MoveBase(b, item, BaseManger.Instance.AroundRedBase(item).Count);
                    MoveBase mb = new MoveBase(b, item, count);//TODO 打不过电脑的模式
                    MBL.Add(mb);
                }
            }
        }
        MBL = Sort(MBL);//排序
        MBL = GetMaxNumList(MBL);//得到最大值得列表
        if (MBL.Count==0)
        {
            Debug.Log("出大问题了AI没路可以走了");
            return;
        }
        MoveBase mbTemp = MBL[0];
        while (true)
        {
            int index = Random.Range(0, MBL.Count);
            Debug.Log(index + "   " + MBL.Count);
            mbTemp = MBL[index];
            if (mbTemp.Canmove)
            {
                break;
            }
            else
            {
                MBL.Remove(mbTemp);
            }
        }
        BaseManger.Instance.StartBase = mbTemp.Start;
        BaseManger.Instance.EndBase = mbTemp.End;
        AIBase = mbTemp.Start.TakeUpChess();//拿起
        mbTemp.End.PutDownChess(AIBase);//放下
        BaseManger.Instance.ChangsPlayer();
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
        for (int i = 0; i < 10; i++)
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
}
public class MoveBase 
{
    Base start;
    Base end;
    int num;
    bool canmove;

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
}
