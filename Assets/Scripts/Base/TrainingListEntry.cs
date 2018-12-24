using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingListEntry : MonoBehaviour {

	public Image highlight;
	public Image portrait;
	public Text entryName;
	public Text level;
	public Text exp;
	public Text currentClass;
	[HideInInspector]public bool isDark;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats) {
		portrait.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		level.text = stats.currentLevel.ToString();
		exp.text = stats.currentExp.ToString();
		currentClass.text = stats.classData.entryName;
	}

	/// <summary>
	/// Darkens the portrait to show you can't use it.
	/// </summary>
	/// <param name="state"></param>
	public void SetDark(bool state) {
		portrait.color = (!state) ? Color.white : Color.grey;
		isDark = state;
	}

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
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
