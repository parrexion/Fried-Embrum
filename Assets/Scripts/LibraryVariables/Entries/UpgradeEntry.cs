using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { UPGRADE, INVENTION }

[CreateAssetMenu(menuName = "LibraryEntries/Upgrade")]
public class UpgradeEntry : ScrObjLibraryEntry {

	public ItemEntry item;
	public UpgradeType type;
	public int cost;
	public int scrap;

	[Header("Stats increases")]
	public int power;
	public int hit;
	public int crit;
	public int charges;
	public int costValue;


	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		UpgradeEntry up = (UpgradeEntry)other;

		item = up.item;
		type = up.type;
		cost = up.cost;
		scrap = up.scrap;

		power = up.power;
		hit = up.hit;
		crit = up.crit;
		charges = up.charges;
		costValue = up.costValue;
	}

	public override void ResetValues() {
		base.ResetValues();

		item = null;
		type = UpgradeType.UPGRADE;
		cost = 0;
		scrap = 0;

		power = 0;
		hit = 0;
		crit = 0;
		charges = 0;
		costValue = 0;
	}
}
