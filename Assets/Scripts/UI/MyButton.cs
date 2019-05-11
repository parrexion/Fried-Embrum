using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour {

	public enum StyleType { NONE, MAIN, BASE, ACTION, NOSELECT, ICON, OPTIONS }

	public StyleType style;
	public Image buttonImage;
	public Text buttonText;
	public Image buttonIcon;
	public Image highlight;


	public void SetSelected(bool selected) {
		highlight.enabled = selected;
	}

	public void SetStyle(UIStyle style, Font font) {
		buttonImage.sprite = style.baseImage;
		buttonImage.color = style.baseColor;
		buttonImage.type = Image.Type.Sliced;
		highlight.sprite = style.highImage;
		highlight.color = style.highColor;
		highlight.type = Image.Type.Sliced;
		buttonText.font = font;
		buttonText.color = style.fontColor;
		buttonText.resizeTextMaxSize = style.fontMaxSize;
	}
}
