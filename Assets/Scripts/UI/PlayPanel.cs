using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PlayPanel : BasePanel
{
    public static PlayPanel Instance;
    Text TimeText;
    Image PromptImage;
    Button back;
    Image player1;
    Image player2;
    public float AppointTime=30f;
    float Times;
    Color _color01 = new Color(90 / 255f, 90 / 255f, 90 / 255f, 1);
    Color _color02 = Color.white;

    public bool isPlay;
    private void Start()
    {
        Times = AppointTime;
    }
    private void Awake()
    {
        Instance = this;
        UIInit();
        EventInit();
    }
    private void Update()
    {
        if (isPlay)
        {
            if (Times < 0.5f)
            {
                BaseManger.Instance.TimeOut();
            }
            Times -= Time.deltaTime;
            ChangsTime(Times);
            ChangsImage();
        }
    }

    #region 初始化UI及Btn赋值

    void UIInit()
    {
        back = transform.Find("Back").GetComponent<Button>();
        back.onClick.AddListener(back_Btn_Event);
        TimeText = transform.Find("Time/Time_Text").GetComponent<Text>();
        PromptImage = transform.Find("Prompt/PromptImage").GetComponent<Image>();
        player1 = transform.Find("qipan/Player1").GetComponent<Image>();
        player2 = transform.Find("qipan/Player2").GetComponent<Image>();
    }

    void EventInit()
    {
        NotificationManger.Instance.AddEventListener(EventName.PlayerChang, ChangPlayer);
        //NotificationManger.Instance.AddEventListener(EventName.PromptChangs, ChangsPromptText);
    }

    void back_Btn_Event()
    {
        BaseManger.Instance.EndGame();
        isPlay = false;
        UIManger.Instance.PushPanel(UIName.StartPanel);
    }

    #endregion

    public void Open(float Time = 0)
    {
        PlayerPrefs.SetFloat("state", Time);
        Times = AppointTime;
        timeKeep = 0;
        if (Time==1)
        {
            player1.sprite = Resources.Load<Sprite>("Image/玩家");
            player2.sprite = Resources.Load<Sprite>("Image/电脑");
            BaseManger.Instance.AIPlay += CompterAi.Instance.AIPlayChess;
        }
        else
        {
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
    /// 
    public void ChangPlayer(Notification obj)
    {
        //PromptText.text = obj.Str;
        if (obj.Flo==1)
        {
            player1.color = _color02;
            player2.color = _color01;
        }
        else if (obj.Flo==2)
        {
            player1.color = _color01;
            player2.color = _color02;
        }
        else
        {
            Debug.Log("出问题了");
        }
        Times = AppointTime;
    }

    float timeKeep = 0;
    int index = 0;
    void ChangsImage()
    {
        timeKeep += Time.deltaTime;
        if (timeKeep >= 3)
        {
            PromptImage.sprite = Resources.Load<Sprite>("Image/TS0" + index % 3);
            //AniChangs();
            timeKeep = 0;
            index++;
        }
    }
    void AniChangs()
    {
        PromptImage.DOFade(0, 0.5f*2).OnComplete(() => {
            PromptImage.sprite = Resources.Load<Sprite>("Image/TS0" + index % 3);
            PromptImage.DOFade(1, 0.5f*2);
        });
    }


    void ChangsTime(float Times)
    {
        TimeText.text = Times.ToString("0");
    }
}
