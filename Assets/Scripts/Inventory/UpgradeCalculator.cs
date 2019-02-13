using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeCalculator {

	public int listSize = 0;
	public List<UpgradeItem> upgrades = new List<UpgradeItem>();
	[SerializeField] private List<string> developed = new List<string>();


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

	public bool IsResearched(string uuid) {
		return developed.Contains(uuid);
	}
}
