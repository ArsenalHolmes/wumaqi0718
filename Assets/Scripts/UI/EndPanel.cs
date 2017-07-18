using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndPanel : BasePanel
{
    public static EndPanel Instance;
    Text TimeText;
    Button back;
    Button ReturnPlay;
    private void Awake()
    {
        Instance = this;
        UIInit();
    }
    public void Open(float Time = 0)
    {
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
        //回主菜单
        UIManger.Instance.PushPanel(UIName.StartPanel);
    }
    void ReturnPlay_Btn_Event()
    {
        UIManger.Instance.PushPanel(UIName.PlayPanel);
        PlayPanel.Instance.Open(PlayerPrefs.GetFloat("state"));
    }
    #endregion


}
