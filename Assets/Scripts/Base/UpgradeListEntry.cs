using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for showing all the different upgrades and developments available in the base.
/// </summary>
public class UpgradeListEntry : ListEntry {

	[HideInInspector]public UpgradeEntry upgrade;
	public int index;
	public Text upgradeType;
	public Image doneIcon;

	public bool affordable;
	public bool done;


	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		upgradeType.color = style.fontColor;
		upgradeType.font = font;
		upgradeType.resizeTextMaxSize = style.fontMaxSize;
	}

	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(int index, UpgradeEntry upgrade, bool done, int totalScrap, int totalMoney) {
		entryName.text = upgrade.entryName;
		icon.color = upgrade.repColor;
		upgradeType.text = upgrade.item.weaponType.ToString();
		this.index = index;
		this.upgrade = upgrade;
		this.done = done;
		affordable = (!done && totalMoney >= upgrade.cost && totalScrap >= upgrade.scrap);
		doneIcon.enabled = done;
    }
	
}
