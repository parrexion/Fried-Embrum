using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingListEntry : ListEntry {
	
	public Text level;
	public Text exp;
	public Text currentClass;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats) {
		icon.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		level.text = stats.currentLevel.ToString();
		exp.text = stats.currentExp.ToString();
		currentClass.text = stats.classData.entryName;
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
