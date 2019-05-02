using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyBar : MonoBehaviour {

	public Image background;
	public Image fill;
	public Text valueText;
	public Text valueTextInverted;

	private float currentAmount;


	private void Start() {
		Color inverted = Color.white - valueText.color;
		inverted.a = 1f;
		valueTextInverted.color = inverted;
	}

	public void SetAmount(int current, int max) {
		currentAmount = (float)current/max;
		valueTextInverted.text = valueText.text = current + " / " + max;
		fill.fillAmount = currentAmount;
	}

	public void SetCustomText(float amount, string text) {
		fill.fillAmount = amount;
		valueText.text = text;
	}
}
