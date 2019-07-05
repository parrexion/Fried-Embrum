using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListEntry : ListEntry {

	public Text charges;

	[Header("Ignore")]
	public int index;


	/// <summary>
	/// Fills the entry with an empty slot.
	/// </summary>
	/// <param name="index"></param>
	public void FillDataEmpty(int index) {
		this.index = index;
		icon.sprite = null;
		icon.color = new Color(0,0,0,0);
		entryName.text = "--";
		charges.text = "";
	}
	
    /// <summary>
    /// Fills the entry with the data of the inventory slot.
    /// </summary>
    /// <param name="statsCon"></param>
	public void FillData(int index, InventoryTuple tuple) {
		if (string.IsNullOrEmpty(tuple.uuid)) {
			FillDataEmpty(index);
			return;
		}
		this.index = index;
		icon.sprite = tuple.icon;
		icon.color = tuple.repColor;
		entryName.text = tuple.entryName;
		charges.text = (tuple.maxCharge != 1) ? tuple.currentCharges.ToString() : "";
	}

	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		charges.font = font;
		charges.color = style.fontColor;
		charges.resizeTextMaxSize = style.fontMaxSize;
		charges.resizeTextForBestFit = true;
	}
}
