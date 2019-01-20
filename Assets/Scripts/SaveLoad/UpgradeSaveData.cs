using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeSaveData {

	public string id;
	public bool researched;


	public UpgradeSaveData() {
		id = "";
		researched = false;
	}

	public void StoreData(UpgradeItem upgrade) {
		id = upgrade.upgrade.uuid;
		researched = upgrade.researched;
	}

}