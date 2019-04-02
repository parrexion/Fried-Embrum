using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour {

	public enum TextType { NONE, HUGE, TITLE, SUBTITLE, BREAD }
	public TextType style;
    
	public void SetStyle(TextStyle style) {
		Text text = GetComponent<Text>();
		text.color = style.color;
		text.font = style.font;
		text.resizeTextMaxSize = style.fontMaxSize;
		text.resizeTextForBestFit = true;
	}
}
