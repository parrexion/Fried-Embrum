using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyBar : MonoBehaviour {

	public enum StyleType { NONE, HEALTH, EXP, FULFILL }
	public StyleType style;

	public Image background;
	public Image fill;
	public Text valueText;
	public Text valueTextInverted;

	private float currentAmount;


	public void SetAmount(int current, int max) {
		currentAmount = (float)current/max;
		valueTextInverted.text = valueText.text = current + " / " + max;
		fill.fillAmount = currentAmount;
	}

	/// <summary>
	/// Replaces the normal value text with a custom value.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="text"></param>
	public void SetCustomText(float amount, string text) {
		fill.fillAmount = amount;
		valueText.text = text;
	}

	
    /// <summary>
	/// UI Style function
	/// </summary>
	/// <param name="style"></param>
	/// <param name="font"></param>
	public void SetStyle(BarStyle style, Font font) {
		background.sprite = style.backgroundImage;
		background.color = style.fillColor;
		fill.sprite = style.fillImage;
		fill.color = style.fillColor;

		Color inverted = Color.white - style.textColor;
		inverted.a = 1f;
		valueText.color = style.textColor;
		valueTextInverted.color = inverted;

		valueTextInverted.font = valueText.font = font;
		valueTextInverted.resizeTextMinSize = valueText.resizeTextMinSize = 10;
		valueTextInverted.resizeTextMaxSize = valueText.resizeTextMaxSize = style.fontMaxSize;
		valueTextInverted.resizeTextForBestFit = valueText.resizeTextForBestFit = true;
	}
}
