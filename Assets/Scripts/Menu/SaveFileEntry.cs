using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileEntry : ListEntry {

	public Text chapterText;
	public Text timeText;
	public GameObject emptyFile;
	

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(string mapName, int chapter, int time) {
		entryName.text = (time > 0) ? mapName : "";
		chapterText.text = "ch  " + chapter;
		timeText.text = Utility.PlayTimeFromInt(time, false);
		emptyFile.SetActive(time == 0);
	}
}
