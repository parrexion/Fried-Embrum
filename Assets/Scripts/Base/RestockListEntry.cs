using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockListEntry : MonoBehaviour {

	public Image highlight;
	public Image portrait;
	public Text entryName;
	public Image canRestock;
	public InventoryContainer invCon;
	[HideInInspector]public bool isDark;


	/// <summary>
	/// Fills the entry with the data of the character.
	/// </summary>
	/// <param name="statsCon"></param>
	public void FillData(StatsContainer stats, InventoryContainer inv) {
		portrait.sprite = stats.charData.portrait;
		entryName.text = stats.charData.entryName;
		invCon = inv;
		UpdateRestock();
	}

	/// <summary>
	/// Darkens the portrait to show you can't use it.
	/// </summary>
	/// <param name="state"></param>
	public void SetDark(bool state) {
		portrait.color = (!state) ? Color.white : Color.grey;
		isDark = state;
	}

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

	/// <summary>
	/// Checks the inventory to see if there is any items to restock.
	/// </summary>
	public void UpdateRestock() {
		bool restock = false;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = invCon.GetTuple(i).item;
			if (item  && invCon.GetTuple(i).charge < item.maxCharge) {
				restock = true;
				break;
			}
		}
		canRestock.enabled = restock;
	}
}
