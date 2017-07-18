using UnityEngine;
using System.Collections;

public class StartDoor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //UI入口
        UIManger.Instance.PushPanel(UIName.StartPanel);
	}
	
}
