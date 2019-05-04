using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyFrame : MonoBehaviour {

	public enum TextType { NONE, HUGE, TITLE, SUBTITLE, BREAD, LIST_TITLE, MENU_TITLE, STATS_BIG, STATS_MID, STATS_SMALL }
	public TextType style;
    
	public void SetStyle(UISingleImage style) {
		Image image = GetComponent<Image>();
		image.sprite = style.image;
		image.color = style.color;
	}

}
