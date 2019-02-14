using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassListEntry : ListEntry {


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(CharClass charClass) {
		icon.color = charClass.repColor;
		entryName.text = charClass.entryName;
	}

}
