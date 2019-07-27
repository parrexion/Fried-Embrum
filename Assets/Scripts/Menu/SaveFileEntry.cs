using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileEntry : ListEntry {

	public Text chapterText;
	public Text timeText;
	public GameObject emptyFile;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		chapterText.color = style.fontColor;
		chapterText.font = font;
		chapterText.resizeTextMaxSize = style.fontMaxSize;
		timeText.color = style.fontColor;
		timeText.font = font;
		timeText.resizeTextMaxSize = style.fontMaxSize;
	}

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(string mapName, int day, int time) {
		entryName.text = (time > 0) ? mapName : "";
		chapterText.text = "day  " + day;
		chapterText.gameObject.SetActive(time != 0);
		timeText.text = Utility.PlayTimeFromInt(time, false);
		timeText.gameObject.SetActive(time != 0);
		emptyFile.SetActive(time == 0);
	}
}
