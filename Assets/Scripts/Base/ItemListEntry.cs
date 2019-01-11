using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEntry : MonoBehaviour {

	public int index;
	public ItemEntry item;
	public bool affordable;
	public Image highlight;
	public Image icon;
	public Text itemName;
	public Text maxCharge;
	public Text cost;


    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, ItemEntry item, int charges, int totalMoney, bool buyMode, float sellRatio) {
		affordable = (!buyMode || totalMoney >= item.cost);
		this.index = index;
		this.item = item;
		icon.color = item.repColor;
		itemName.color = (affordable) ? Color.white : Color.black;
		itemName.text = item.entryName;
		maxCharge.color = (affordable) ? Color.white : Color.black;
		maxCharge.text = charges.ToString();
		cost.color = (affordable) ? Color.white : Color.black;
		cost.text = (buyMode) ? item.cost.ToString() : (Mathf.FloorToInt(item.cost * sellRatio)).ToString();
    }

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}
	
}
