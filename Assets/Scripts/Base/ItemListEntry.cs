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


	public void FillDataEmpty(int index) {
		this.index = index;
		item = null;
		icon.sprite = null;
		icon.color = new Color(0,0,0,0);
		entryName.text = "- - -";
		maxCharge.text = "";
		cost.text = "";
	}

	public void FillDataSimple(int index, ItemEntry item, string charges, string cost) {
		this.index = index;
		this.item = item;
		icon.sprite = item.icon;
		icon.color = item.repColor;
		entryName.text = item.entryName;
		maxCharge.text = charges;
		this.cost.text = cost;
	}

    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, ItemEntry item, string charges, int totalMoney, bool buyMode, float sellRatio) {
		affordable = (!buyMode || totalMoney >= item.cost);
		this.index = index;
		this.item = item;
		icon.sprite = item.icon;
		icon.color = item.repColor;
		entryName.text = item.entryName;
		maxCharge.text = charges;
		cost.text = (buyMode) ? item.cost.ToString() : (Mathf.FloorToInt(item.cost * sellRatio)).ToString();
		SetDark(!affordable);
    }

	public void SetAffordable(bool affordable) {
		this.affordable = affordable;
		SetDark(!affordable);
	}

	public override void SetStyle(UIStyle style, Font font) {
		base.SetStyle(style, font);
		maxCharge.font = font;
		maxCharge.color = style.fontColor;
		maxCharge.resizeTextMaxSize = style.fontMaxSize;
		maxCharge.resizeTextForBestFit = true;
		cost.font = font;
		cost.color = style.fontColor;
		cost.resizeTextMaxSize = style.fontMaxSize;
		cost.resizeTextForBestFit = true;
	}
}
