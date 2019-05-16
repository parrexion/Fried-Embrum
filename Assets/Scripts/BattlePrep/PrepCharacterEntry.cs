using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepCharacterEntry : ListEntry {

	public Text className;
	public Text level;
	public Text exp;
	public Text hp;
	public InventoryContainer invCon;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		Text[] texts = new Text[] { className, level, exp, hp };
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
	public void FillData(StatsContainer stats, InventoryContainer inventory, PrepCharacter prep) {
		invCon = inventory;
		icon.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		className.text = stats.classData.entryName;
		level.text = stats.level.ToString();
		exp.text = stats.currentExp.ToString();
		hp.text = stats.hp.ToString();

		entryName.color = (prep.forced) ? Color.green : Color.black;
		SetDark(!prep.selected || prep.locked);
	}

}
