using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeListEntry : MonoBehaviour {

	public int index;
	public UpgradeEntry upgrade;
	public bool affordable;
	public Image highlight;
	public Image icon;
	public Text itemName;
	public Text scrapCost;
	public Text moneyCost;


    /// <summary>
    /// Fills the entry with the data of the character.
    /// </summary>
    /// <param name="statsCon"></param>
    public void FillData(int index, UpgradeEntry upgrade, int totalScrap, int totalMoney) {
		affordable = (totalMoney >= upgrade.cost && totalScrap >= upgrade.scrap);
		this.index = index;
		this.upgrade = upgrade;
		icon.color = upgrade.repColor;
		itemName.color = (affordable) ? Color.white : Color.black;
		itemName.text = upgrade.entryName;
		scrapCost.color = (affordable) ? Color.white : Color.black;
		scrapCost.text = scrapCost.ToString();
		moneyCost.color = (affordable) ? Color.white : Color.black;
		moneyCost.text = upgrade.cost.ToString();
    }

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}
	
}
