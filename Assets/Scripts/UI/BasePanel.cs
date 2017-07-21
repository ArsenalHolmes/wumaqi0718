using UnityEngine;
using System.Collections;
using DG.Tweening;
public class BasePanel : MonoBehaviour
{

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }
    /// <summary>
    /// 入栈
    /// </summary>
    public virtual void OnEnter()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        Vector3 temp = transform.localPosition;
        temp.x = 1500;
        transform.localPosition = temp;
        transform.DOLocalMoveX(0, 0.5f);
        //设置UI层级
        transform.SetSiblingIndex(transform.parent.childCount - 2);
    }
    /// <summary>
    /// 出栈
    /// </summary>
    public virtual void OnExit()
    {
        canvasGroup.blocksRaycasts = false;
        transform.DOLocalMoveX(1500, 0.5f).OnComplete(() => canvasGroup.alpha = 0);
    }
    /// <summary>
    /// 设置不能点击
    /// </summary>
    public virtual void OnPause()
    {
        canvasGroup.blocksRaycasts = false;
    }
    /// <summary>
    /// 设置点击
    /// </summary>
    public virtual void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }
}
