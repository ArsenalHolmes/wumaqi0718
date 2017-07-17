using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayPanel : UIBase
{
    public static PlayPanel Instance;
    Text TimeText;
    Text PromptText;
    Button back;
    Image player1;
    Image player2;

    float Times = 30f;
    public float TotalTime;

    bool isPlay;
    private void Start()
    {
        
    }
    private void Awake()
    {
        Instance = this;
        UIInit();
        Close();
    }
    private void Update()
    {
        if (isPlay)
        {
            Times -= Time.deltaTime;
            ChangsTime(Times);
            if (Times<=0.5f)
            {
                //TODO 倒计时结束咋办
            }
            TotalTime += Time.deltaTime;
        }
    }
    #region 初始化UI及Btn赋值
    void UIInit()
    {
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
        TimeText = transform.Find("Time/Time_Text").GetComponent<Text>();
        PromptText = transform.Find("Prompt/Prompt_Text").GetComponent<Text>();
        player1 = transform.Find("qipan/Player1").GetComponent<Image>();
        player2 = transform.Find("qipan/Player2").GetComponent<Image>();
    }
    void back_Btn_Event()
    {
        Close();
        StartPanel.Instance.Open();
        BaseManger.Instance.EndGame();
        isPlay = false;
    }
    #endregion

    public override void Open(float Time = 0)
    {
        base.Open(Time);
        if (Time==1)
        {
            //TODO 人机
            player1.sprite = Resources.Load<Sprite>("Image/玩家");
            player2.sprite = Resources.Load<Sprite>("Image/电脑");
            BaseManger.Instance.AIPlay += CompterAi.Instance.AIPlayChess;
        }
        else
        {
            //TODO 人人
            player1.sprite = Resources.Load<Sprite>("Image/玩家");
            player2.sprite = player1.sprite;
        }
        isPlay = true;
        BaseManger.Instance.StartGame();
    }

    /// <summary>
    /// 修改提示文字
    /// </summary>
    /// <param name="str"></param>
    public void ChangsPromptText(string str)
    {
        PromptText.text = str;
    }

    public void ChangPlayer()
    {
        Times = 30;
    }

    void ChangsTime(float str)
    {
        TimeText.text = str.ToString("0");
    }
}
