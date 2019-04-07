using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeItem {

	public UpgradeEntry upgrade;
	public bool researched;


	public UpgradeItem(UpgradeEntry entry, bool researched = false) {
		upgrade = entry;
		this.researched = researched;
	}
}
