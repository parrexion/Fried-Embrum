using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeCalculator {

	public int listSize = 0;
	public List<UpgradeItem> upgrades = new List<UpgradeItem>();
	[SerializeField] private List<string> developed = new List<string>();


	public void AddEntry(UpgradeItem item) {
		upgrades.Add(item);
		if (item.researched) {
			developed.Add(item.upgrade.uuid);
		}
		else {
			listSize++;
		}
	}

	public void CalculateResearch() {
		developed = new List<string>();
		listSize = upgrades.Count;
		for (int i = 0; i < listSize; i++) {
			if (upgrades[i].upgrade.type == UpgradeType.INVENTION && upgrades[i].researched) {
				developed.Add(upgrades[i].upgrade.item.uuid);
				Debug.Log("Found an upgrade!  " + upgrades[i].upgrade.item.uuid);
			}
		}
	}

	/// <summary>
	/// Returns a list of all upgrades related to the given item.
	/// </summary>
	/// <param name="itemID"></param>
	/// <returns></returns>
	public List<UpgradeItem> GetItemUpgradeList(string itemID) {
		List<UpgradeItem> relatedUpgrades = new List<UpgradeItem>();
		for (int i = 0; i < upgrades.Count; i++) {
			if (upgrades[i].upgrade.item.uuid == itemID)
				relatedUpgrades.Add(upgrades[i]);
		}
		return relatedUpgrades;
	}

	public bool IsResearched(string uuid) {
		return developed.Contains(uuid);
	}
}
