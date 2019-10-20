using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingListEntry : ListEntry {
	
	public Text level;
	public Text exp;
	public Text currentClass;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		level.color = style.fontColor;
		level.font = font;
		level.resizeTextMaxSize = style.fontMaxSize;
		exp.color = style.fontColor;
		exp.font = font;
		exp.resizeTextMaxSize = style.fontMaxSize;
		currentClass.color = style.fontColor;
		currentClass.font = font;
		currentClass.resizeTextMaxSize = style.fontMaxSize;
	}

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats) {
		icon.sprite = stats.charData.portraitSet.small;
		entryName.text = stats.charData.entryName;
		level.text = stats.level.ToString();
		exp.text = stats.currentExp.ToString();
		currentClass.text = stats.currentClass.entryName;
	}

	/// <summary>
	/// Updates level, exp and class for the character
	/// </summary>
	/// <param name="tuple"></param>
	/// <param name="value"></param>
	public void SetBexpValue(int lvl, int bexp, string className) {
		level.text = lvl.ToString();
		exp.text = bexp.ToString();
		currentClass.text = className;
	}
}
