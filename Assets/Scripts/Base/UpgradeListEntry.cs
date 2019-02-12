using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeListEntry : ListEntry {

	public int index;
	public UpgradeEntry upgrade;
	public bool affordable;
	public bool done;


    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, UpgradeEntry upgrade, bool done, int totalScrap, int totalMoney) {
		this.index = index;
		this.upgrade = upgrade;
		this.done = done;
		affordable = (!done && totalMoney >= upgrade.cost && totalScrap >= upgrade.scrap);
    }
	
}
