using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { UPGRADE, INVENTION }

[CreateAssetMenu(menuName = "LibraryEntries/Upgrade")]
public class UpgradeEntry : ScrObjLibraryEntry {
	
	public UpgradeType type;
	public ItemEntry item;
	public int cost;
	public int scrap;
	public int level;
	public int hit;
	public int power;
	public int weight;
	public int minRange;
	public int maxRange;

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		UpgradeEntry up = (UpgradeEntry)other;
		type = up.type;
		item = up.item;
		cost = up.cost;
		scrap = up.scrap;
		level = up.level;
		hit = up.hit;
		power = up.power;
		weight = up.weight;
		minRange = up.minRange;
		maxRange = up.maxRange;
	}

	public override void ResetValues() {
		base.ResetValues();
		type = UpgradeType.UPGRADE;
		item = null;
		cost = 0;
		scrap = 0;
		level = 1;
		hit = 0;
		power = 0;
		weight = 0;
		minRange = 0;
		maxRange = 0;
	}
}
