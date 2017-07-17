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
        MoveBase mbTemp = MBL[0];
        while (true)
        {
            int index = Random.Range(0, MBL.Count);
            Debug.Log(index + "   " + MBL.Count);
            mbTemp = MBL[index];
            if (mbTemp.CanMove())
            {
                break;
            }
            else
            {
                MBL.Remove(mbTemp);
            }
        }
        AIBase = mbTemp.Start.TakeUpChess();//拿起
        mbTemp.End.PutDownChess(AIBase);//放下
        BaseManger.Instance.ChangsPlayer();
    }

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
    List<MoveBase> GetMaxNumList(List<MoveBase> lmb)
    {
        List<MoveBase> lb = new List<MoveBase>();
        for (int i = 0; i < 2; i++)
        {
            lb.Add(lmb[i]);
        }
        return lb;
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

    public bool CanMove()
    {
        BaseManger.Instance.StartBase = start;
        BaseManger.Instance.EndBase = end;
        return BaseManger.Instance.ChessMove();
    }
}
