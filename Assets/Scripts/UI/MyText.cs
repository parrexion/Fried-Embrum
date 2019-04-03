using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour {

	public enum TextType { NONE, HUGE, TITLE, SUBTITLE, BREAD, LIST_TITLE, MENU_TITLE }
	public TextType style;
    
	public void SetStyle(TextStyle style, Font font) {
		Text text = GetComponent<Text>();
		text.color = style.color;
		text.font = font;
		text.resizeTextMaxSize = style.fontMaxSize;
		text.resizeTextForBestFit = true;
	}
}
