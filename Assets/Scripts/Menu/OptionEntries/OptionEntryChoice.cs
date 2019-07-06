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
			int current = (selections[i] != null) ? selections[i].value : i;
			if (current == value.value) {
				index = i;
				break;
			}
		}
		valueText.text = selectionNames[index];
	}

    public override bool OnClick() {
        return false;
    }

    public override bool MoveValue(int dir) {
		index = OPMath.FullLoop(0, selections.Length, index + dir);

		value.value = (selections[index] != null) ? selections[index].value : index;
		valueText.text = selectionNames[index];
		updateEvent.Invoke();
		return true;
    }

	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		valueText.font = font;
		valueText.color = style.fontColor;
		valueText.resizeTextMaxSize = style.fontMaxSize;
	}
}
