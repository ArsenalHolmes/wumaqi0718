using UnityEngine;
using System.Collections;

public class UIBase : MonoBehaviour {
    
	public virtual void Close()
    {
        //(transform as RectTransform).anchoredPosition3D = Vector3.one * 100;
        transform.localScale = Vector3.zero;
    }
    public virtual void Open(float Time=0)
    {
        //(transform as RectTransform).anchoredPosition3D = Vector3.zero * 100;
        transform.localScale = Vector3.one;
    }
}
