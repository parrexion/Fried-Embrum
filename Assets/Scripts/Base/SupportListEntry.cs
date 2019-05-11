using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportListEntry : ListEntry {

	[Header("Supports")]
	public Text supportC;
	public Text supportB;
	public Text supportA;
	public Text supportS;
	public Image newLevel;

	[HideInInspector] public int index;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		Text[] texts = new Text[] { supportA, supportB, supportC, supportS };
		for (int i = 0; i < texts.Length; i++) {
			texts[i].color = style.fontColor;
			texts[i].font = font;
			texts[i].resizeTextMaxSize = style.fontMaxSize;
		}
	}

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="stats"></param>
	public void FillData(int index, StatsContainer stats) {
		this.index = index;
		icon.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		supportC.enabled = false;
		supportB.enabled = false;
		supportA.enabled = false;
		supportS.enabled = false;
		newLevel.enabled = false;
	}

	/// <summary>
	/// Shows which support levels are available and what the current level are
	/// </summary>
	/// <param name="tuple"></param>
	/// <param name="value"></param>
	public void SetSupportValue(SupportTuple tuple, SupportValue value) {
		SupportLetter max = (tuple != null) ? tuple.maxlevel : SupportLetter.NONE;
		supportS.enabled = ((int)max >= 4);
		supportA.enabled = ((int)max >= 3);
		supportB.enabled = ((int)max >= 2);
		supportC.enabled = ((int)max >= 1);
		
		SupportLetter achieved = (tuple != null && value != null) ? (SupportLetter)value.currentLevel : SupportLetter.NONE;
		supportS.color = ((int)achieved >= 4) ? Color.green : Color.black;
		supportA.color = ((int)achieved >= 3) ? Color.white : Color.black;
		supportB.color = ((int)achieved >= 2) ? Color.white : Color.black;
		supportC.color = ((int)achieved >= 1) ? Color.white : Color.black;
		
		SupportLetter current = (tuple != null && value != null) ? tuple.CalculateLevel(value.value) : SupportLetter.NONE;
		newLevel.enabled = (current > achieved);
	}
}
