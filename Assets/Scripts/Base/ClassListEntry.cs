using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassListEntry : MonoBehaviour {

	public Image highlight;
	public Image icon;
	public Text className;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(CharClass charClass) {
		icon.color = charClass.repColor;
		className.text = charClass.entryName;
	}

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

}
