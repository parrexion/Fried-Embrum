using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadMemberEntry : ListEntry {

	public Text level;
	public int index = -1;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		level.color = style.fontColor;
		level.font = font;
		level.resizeTextMaxSize = style.fontMaxSize;
	}

	/// <summary>
	/// Fills the entry with the data of the squad member.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats, int dataIndex) {
		icon.sprite = stats.charData.portraitSet.small;
		entryName.text = stats.charData.entryName;
		level.text = stats.level.ToString();
		index = dataIndex;
	}

}
