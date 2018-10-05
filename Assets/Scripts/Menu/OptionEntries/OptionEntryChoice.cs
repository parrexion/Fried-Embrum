using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionEntryChoice : OptionEntry {

	[Header("Value")]
	public IntVariable[] selections;
	public string[] selectionNames;
	public IntVariable value;
	public Text valueText;
	private int index;


    public override void UpdateUI() {
		for (int i = 0; i < selections.Length; i++) {
			if (selections[i].value == value.value) {
				index = i;
				break;
			}
		}
		valueText.text = selectionNames[index];
	}

    public override bool OnClick() {
        return false;
    }

    public override bool OnLeft() {
		index--;
		if (index < 0)
			index = selections.Length - 1;

		value.value = selections[index].value;
		valueText.text = selectionNames[index];
		updateEvent.Invoke();
		return true;
    }

    public override bool OnRight() {
		index++;
		if (index >= selections.Length)
			index = 0;

		value.value = selections[index].value;
		valueText.text = selectionNames[index];
		updateEvent.Invoke();
		return true;
    }

}
