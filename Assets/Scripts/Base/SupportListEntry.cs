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

	[HideInInspector] public string uuid;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="stats"></param>
	public void FillData(StatsContainer stats) {
		uuid = stats.charData.uuid;
		icon.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		supportC.enabled = false;
		supportB.enabled = false;
		supportA.enabled = false;
		supportS.enabled = false;
	}

	/// <summary>
	/// Shows which support levels are available and what the current level are
	/// </summary>
	/// <param name="tuple"></param>
	/// <param name="value"></param>
	public void SetSupportValue(SupportTuple tuple, int value) {
		SupportLetter max = (tuple != null) ? tuple.maxlevel : SupportLetter.NONE;
		supportS.enabled = ((int)max >= 4);
		supportA.enabled = ((int)max >= 3);
		supportB.enabled = ((int)max >= 2);
		supportC.enabled = ((int)max >= 1);
		
		SupportLetter current = (tuple != null) ? tuple.CalculateLevel(value) : SupportLetter.NONE;
		supportS.color = ((int)current >= 4) ? Color.green : Color.black;
		supportA.color = ((int)current >= 3) ? Color.white : Color.black;
		supportB.color = ((int)current >= 2) ? Color.white : Color.black;
		supportC.color = ((int)current >= 1) ? Color.white : Color.black;
	}
}
