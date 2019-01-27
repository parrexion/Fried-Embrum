using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepCharacterEntry : MonoBehaviour {

	public Image highlight;
	public Image portrait;
	public TMPro.TextMeshProUGUI entryName;
	public TMPro.TextMeshProUGUI className;
	public TMPro.TextMeshProUGUI level;
	public TMPro.TextMeshProUGUI exp;
	public TMPro.TextMeshProUGUI hp;
	public bool selected;
	public bool available;

	
	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="stats"></param>
	public void FillData(StatsContainer stats, bool selected, bool available) {
		portrait.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		className.text = stats.charData.entryName;
		level.text = stats.currentLevel.ToString();
		exp.text = stats.currentExp.ToString();
		hp.text = stats.hp.ToString();
		this.selected = selected;
		this.available = available;
		SetDark(!selected || !available);
	}

	public void SetDark(bool state) {
		portrait.color = (!state) ? Color.white : Color.grey;
	}

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

}
