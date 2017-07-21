using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Test : MonoBehaviour,IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("qqq");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
