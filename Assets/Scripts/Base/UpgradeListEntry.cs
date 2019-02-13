using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for showing all the different upgrades and developments available in the base.
/// </summary>
public class UpgradeListEntry : ListEntry, IComparer<UpgradeListEntry> {

	public Text upgradeType;
	public int index;
	public UpgradeEntry upgrade;
	public bool affordable;
	public bool done;


    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, UpgradeEntry upgrade, bool done, int totalScrap, int totalMoney) {
		entryName.text = upgrade.entryName;
		icon.color = upgrade.repColor;
		upgradeType.text = upgrade.item.itemType.ToString();
		this.index = index;
		this.upgrade = upgrade;
		this.done = done;
		affordable = (!done && totalMoney >= upgrade.cost && totalScrap >= upgrade.scrap);
    }

	/// <summary>
	/// Custom compare method.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	int IComparer<UpgradeListEntry>.Compare(UpgradeListEntry x, UpgradeListEntry y) {
		if (x.done != y.done) {
			return (x.done) ? 1 : -1;
		}
		else {
			return y.index - x.index;
		}
	}
}
