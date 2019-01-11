using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSaveData {

	public string id;
	public int charges;


	public ItemSaveData() {
		id = "";
		charges = -1;
	}

	public void StoreData(InventoryItem invItem) {
		id = invItem.item.uuid;
		charges = invItem.charges;
	}

}