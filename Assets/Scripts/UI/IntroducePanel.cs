using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class IntroducePanel:UIBase 
{
    public static IntroducePanel Instance;
    Button tiao;
    Button ka;
    Button da;
    Button zhong;
    Button back;
    private void Awake()
    {
        Instance = this;
        UIInit();
        Close();
    }
    void UIInit()
    {
        tiao = transform.Find("tiao").GetComponent<Button>();
        ka = transform.Find("ka").GetComponent<Button>();
        da = transform.Find("da").GetComponent<Button>();
        zhong = transform.Find("zhong").GetComponent<Button>();
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
    }
    void tiao_Btn_Event()
    {

    }
    void ka_Btn_Event()
    {

    }
    void da_Btn_Event()
    {

    }
    void zhong_Btn_Event()
    {

    }
    void back_Btn_Event()
    {
        Close();
        StartPanel.Instance.Open();
    }
}
