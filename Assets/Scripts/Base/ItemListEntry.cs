using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEntry : ListEntry {

	public int index;
	public ItemEntry item;
	public Text maxCharge;
	public Text cost;
	public bool affordable;


    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, ItemEntry item, int charges, int totalMoney, bool buyMode, float sellRatio) {
		affordable = (!buyMode || totalMoney >= item.cost);
		this.index = index;
		this.item = item;
		icon.sprite = item.icon;
		icon.color = item.repColor;
		entryName.text = item.entryName;
		maxCharge.text = charges + " / " + item.maxCharge;
		cost.text = (buyMode) ? item.cost.ToString() : (Mathf.FloorToInt(item.cost * sellRatio)).ToString();
		SetDark(!affordable);
    }
}
