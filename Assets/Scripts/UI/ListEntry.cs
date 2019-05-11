using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class ListEntry : MonoBehaviour {

	public enum StyleType { NONE, OPTIONS, THIN, SAVE, TRADE }
	public StyleType style;

	public Image background;
	public Image highlight;
	public bool dark;

	public Image icon;
	public Text entryName;


	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

	/// <summary>
	/// Updates the icon of the entry and greys it if it should be darkened.
	/// </summary>
	/// <param name="state"></param>
	public void SetDark(bool state) {
		dark = state;
		icon.color = (state) ? Color.grey : Color.white;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="style"></param>
	/// <param name="font"></param>
	public virtual void SetStyle(UIStyle style, Font font) {
		background.sprite = style.baseImage;
		background.color = style.baseColor;
		background.type = Image.Type.Sliced;
		highlight.sprite = style.highImage;
		highlight.color = style.highColor;
		highlight.type = Image.Type.Sliced;

		entryName.font = font;
		entryName.color = style.fontColor;
		entryName.resizeTextMaxSize = style.fontMaxSize;
		entryName.resizeTextForBestFit = true;
	}
}
