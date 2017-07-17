using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndPanel : UIBase
{
    public static EndPanel Instance;
    Text TimeText;
    Button back;
    Button ReturnPlay;
    private void Awake()
    {
        Instance = this;
        UIInit();
        Close();
    }
    public override void Open(float Time = 0)
    {
        base.Open(Time);
        TimeText.text = "总共用时"+ Time.ToString("0")+"秒";
    }

    #region 初始化UI及Btn赋值
    void UIInit()
    {
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
        ReturnPlay = transform.Find("RetrunPlay").GetComponent<Button>();
        ReturnPlay.onClick.AddListener(ReturnPlay_Btn_Event);
        TimeText = transform.Find("Time/Time_Text").GetComponent<Text>();
    }
    void back_Btn_Event()
    {
        Close();
        StartPanel.Instance.Open();
    }
    void ReturnPlay_Btn_Event()
    {
        Close();
        PlayPanel.Instance.Open(PlayerPrefs.GetFloat("state"));
    }
    #endregion


}
