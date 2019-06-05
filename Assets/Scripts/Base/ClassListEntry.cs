using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassListEntry : ListEntry {

	public Text classLevel;

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(LevelGain gain) {
		classLevel.text = gain.level.ToString();
		icon.sprite = gain.classIcon;
		entryName.text = gain.className;
	}

}
