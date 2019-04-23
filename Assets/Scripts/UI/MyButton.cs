using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour {

	public enum ButtonType { NONE, MAIN, BASE, ACTION }

	public ButtonType style;
	public Image buttonImage;
	public Text buttonText;
	public Image highlight;


	public void SetSelected(bool selected) {
		highlight.enabled = selected;
	}

	public void SetStyle(UIStyle style, Font font) {
		buttonImage.sprite = style.baseImage;
		buttonImage.color = style.baseColor;
		highlight.sprite = style.highImage;
		highlight.color = style.highColor;
		buttonText.font = font;
		buttonText.color = style.fontColor;
		buttonText.resizeTextMaxSize = style.fontMaxSize;
	}
}
