using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {

	public ItemEntry item;
	public int charges;


	public InventoryItem(InventoryTuple tuple) {
		item = tuple.item;
		charges = tuple.charge;
	}

	public InventoryItem(ItemEntry item, int charges = 0) {
		this.item = item;
		this.charges = (charges > 0) ? charges : item.maxCharge;
	}
}
