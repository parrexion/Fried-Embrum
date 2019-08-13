using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour {

	public enum StyleType { NONE, HUGE, TITLE, SUBTITLE, BREAD, OBJECTIVE, MENU_TITLE,
		STATS_BIG, STATS_MID, STATS_SMALL, STATS_PENALTY, BASE_TITLE, BASE_HUGE, BASE_MID, BASE_SMALL,
		LEVEL_BONUS, DAMAGE }
	public StyleType style;
    
	public void SetStyle(TextStyle style, Font font) {
		Text text = GetComponent<Text>();
		if (text == null) {
			Debug.LogError("Missing text in object:  " + name);
			return;
		}
		text.color = style.color;
		text.font = font;
		text.resizeTextMinSize = 10;
		text.resizeTextMaxSize = style.fontMaxSize;
		text.resizeTextForBestFit = true;
	}
}
