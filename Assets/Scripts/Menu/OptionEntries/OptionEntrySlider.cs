using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionEntrySlider : OptionEntry {

	[Header("Value")]
	public IntVariable value;
	public int minValue;
	public int maxValue;
	public int stepSize;
	public Text valueText;


	public override void UpdateUI() {
		valueText.text = value.value.ToString();
	}

    public override bool OnClick() {
        return false;
    }

    public override bool MoveValue(int dir) {
		int before = value.value;
		value.value = Mathf.Clamp(value.value + dir * stepSize, minValue, maxValue);
		valueText.text = value.value.ToString();
		updateEvent.Invoke();
		return (before != value.value);
    }

	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		valueText.font = font;
		valueText.color = style.fontColor;
		valueText.resizeTextMaxSize = style.fontMaxSize;
	}
}
