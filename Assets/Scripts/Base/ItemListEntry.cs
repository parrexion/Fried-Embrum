using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEntry : ListEntry {

	public int index;
	public InventoryTuple tuple;
	public Text maxCharge;
	public Text cost;
	public bool affordable;


	public void FillDataEmpty(int index) {
		this.index = index;
		tuple = null;
		icon.sprite = null;
		icon.color = new Color(0,0,0,0);
		entryName.text = "- - -";
		maxCharge.text = "";
		cost.text = "";
	}

	public void FillDataSimple(int index, InventoryTuple tuple, string charges, string cost) {
		this.index = index;
		this.tuple = tuple;
		icon.sprite = tuple.icon;
		icon.color = tuple.repColor;
		entryName.text = tuple.entryName;
		maxCharge.text = charges;
		this.cost.text = cost;
	}

    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, InventoryTuple tuple, string charges, int totalMoney, bool buyMode, float sellRatio) {
		affordable = (!buyMode || totalMoney >= tuple.cost);
		this.index = index;
		this.tuple = tuple;
		icon.sprite = tuple.icon;
		icon.color = tuple.repColor;
		entryName.text = tuple.entryName;
		maxCharge.text = charges;
		cost.text = (buyMode) ? tuple.cost.ToString() : (Mathf.FloorToInt(tuple.cost * sellRatio)).ToString();
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
