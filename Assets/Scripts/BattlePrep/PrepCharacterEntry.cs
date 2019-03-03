using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepCharacterEntry : ListEntry {
	
	public TMPro.TextMeshProUGUI className;
	public TMPro.TextMeshProUGUI level;
	public TMPro.TextMeshProUGUI exp;
	public TMPro.TextMeshProUGUI hp;
	public InventoryContainer invCon;

	
	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="stats"></param>
	public void FillData(StatsContainer stats, InventoryContainer inventory, PrepCharacter prep) {
		invCon = inventory;
		icon.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		className.text = stats.charData.entryName;
		level.text = stats.currentLevel.ToString();
		exp.text = stats.currentExp.ToString();
		hp.text = stats.hp.ToString();

		entryName.color = (prep.forced) ? Color.green : Color.black;
		SetDark(!prep.selected || prep.locked);
	}

}
