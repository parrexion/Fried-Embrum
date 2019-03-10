using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionEntryToggle : OptionEntry {

	[Header("Value")]
	public BoolVariable value;
	public Image checkmark;


    public override void UpdateUI() {
		checkmark.enabled = value.value;
	}

    public override bool OnClick() {
        value.value = !value.value;
		checkmark.enabled = value.value;
		updateEvent.Invoke();
		return true;
    }

    public override bool MoveValue(int dir) {
        return false;
    }

}
