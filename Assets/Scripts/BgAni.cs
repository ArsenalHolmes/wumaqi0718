using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BgAni : MonoBehaviour {
    Image image;
    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        image.sprite = sr.sprite;
	}
}
