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

    public override bool OnLeft() {
		int before = value.value;
		value.value = Mathf.Max(minValue, value.value - stepSize);
		valueText.text = value.value.ToString();
		updateEvent.Invoke();
		return (before != value.value);
    }

    public override bool OnRight() {
		int before = value.value;
		value.value = Mathf.Min(maxValue, value.value + stepSize);
		valueText.text = value.value.ToString();
		updateEvent.Invoke();
		return (before != value.value);
    }

}
