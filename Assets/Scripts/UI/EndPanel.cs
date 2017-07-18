using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndPanel : BasePanel
{
    public static EndPanel Instance;
    Text TimeText;
    Image WinImage;
    Button back;
    Button ReturnPlay;
    private void Awake()
    {
        Instance = this;
        UIInit();
        EventInit();
    }
    

    #region 初始化UI及Btn赋值
    void UIInit()
    {
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
        ReturnPlay = transform.Find("RetrunPlay").GetComponent<Button>();
        ReturnPlay.onClick.AddListener(ReturnPlay_Btn_Event);
        TimeText = transform.Find("Time/Time_Text").GetComponent<Text>();
        WinImage = transform.Find("WinImage").GetComponent<Image>();
    }
    void EventInit()
    {
        NotificationManger.Instance.AddEventListener(EventName.TotalTime, TotalTime);
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
    void TotalTime(Notification obj)
    {
        TimeText.text = "总共用时" + obj.Flo.ToString("0") + "秒";
        WinImage.sprite = Resources.Load<Sprite>("Image/"+obj.Str);
    }
    #endregion


}
