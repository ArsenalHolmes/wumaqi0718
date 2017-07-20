using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndPanel : BasePanel
{
    Image WinImage;
    Button back;
    Button ReturnPlay;
    private void Awake()
    {
        UIInit();
    }
    

    #region 初始化UI及Btn赋值
    void UIInit()
    {
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
        ReturnPlay = transform.Find("RetrunPlay").GetComponent<Button>();
        ReturnPlay.onClick.AddListener(ReturnPlay_Btn_Event);
        WinImage = transform.Find("WinImage").GetComponent<Image>();
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
