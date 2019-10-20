using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockListEntry : ListEntry {
	
	public Image canRestock;
	public InventoryContainer invCon;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats, InventoryContainer inv) {
		icon.sprite = stats.charData.portraitSet.small;
		entryName.text = stats.charData.entryName;
		invCon = inv;
		UpdateRestock();
	}

	/// <summary>
	/// Checks the inventory to see if there is any items to restock.
	/// </summary>
	public void UpdateRestock() {
		bool restock = false;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = invCon.GetTuple(i);
			if (!string.IsNullOrEmpty(tuple.uuid) && tuple.currentCharges < tuple.maxCharge) {
				restock = true;
				break;
			}
		}
		canRestock.enabled = restock;
	}
}
