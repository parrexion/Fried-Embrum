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

	
	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="stats"></param>
	public void FillData(StatsContainer stats, PrepCharacter prep) {
		portrait.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		className.text = stats.charData.entryName;
		level.text = stats.currentLevel.ToString();
		exp.text = stats.currentExp.ToString();
		hp.text = stats.hp.ToString();

		entryName.color = (prep.forced) ? Color.green : Color.black;
		SetDark(!prep.selected || prep.locked);
	}

	/// <summary>
	/// Darkens the portrait to show that the entry is not active
	/// </summary>
	/// <param name="state"></param>
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
