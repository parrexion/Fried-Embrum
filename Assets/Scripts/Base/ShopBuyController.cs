using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyController : MonoBehaviour {
    
	public SaveListVariable playerData;
	public IntVariable totalMoney;
	public FloatVariable sellRatio;

	[Header("Information box")]
	public Text TotalMoneyText;
	public Text itemName;
	public Image itemIcon;

	public Text pwrText;
	public Text rangeText;
	public Text hitText;
	public Text critText;
	public Text reqText;
	public Text weightText;

	[Header("Buy/Sell items")]
	public StorageList shopList;
	public MyPrompt buyPrompt;
	private bool promptMode;


	private void Start() {
		shopList.Setup();
	}

	public void GenerateShopList(ItemListVariable currentShopList) {
		shopList.SetupBuy(currentShopList);
	}

	public void GenerateSellList() {
		shopList.SetupSell();
	}

	public void MoveVertical(int dir) {
		if (promptMode)
			return;

		shopList.Move(dir);
        SetupItemInfo();
	}

    public void MoveHorizontal(int dir) {
        if (!promptMode) {
            shopList.ChangeCategory(dir);
			SetupItemInfo();
        }
		else {
			buyPrompt.Move(dir);
		}
    }

	public void SelectItem() {
		ItemListEntry itemEntry = shopList.GetEntry();
		if (!itemEntry || !itemEntry.affordable)
			return;

		if (!promptMode) {
			promptMode = true;
			buyPrompt.ShowWindow((shopList.buyMode) ? "Buy item?" : "Sell item?", true);
		}
		else {
			if (buyPrompt.Click(true) == MyPrompt.Result.OK1) {
				if (shopList.buyMode) { // Buy item
					ItemEntry item = ScriptableObject.CreateInstance<ItemEntry>();
					item.CopyValues(itemEntry.item);
					totalMoney.value -= item.cost;
					playerData.items.Add(new InventoryItem(item));
				}
				else { // Sell item
					ItemEntry item = itemEntry.item;
					totalMoney.value += (int)(item.cost * sellRatio.value);
					playerData.items.RemoveAt(itemEntry.index);
					shopList.RemoveEntry();
				}
			}
			shopList.UpdateCost();
			SetupItemInfo();
			DeselectItem();
		}
	}

	public bool DeselectItem() {
		if (promptMode) {
			promptMode = false;
			buyPrompt.Click(false);
			return false;
		}

		return true;
	}

	private void SetupItemInfo() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;

		ItemListEntry itemEntry = shopList.GetEntry();
		if (!itemEntry || !itemEntry.item) {
			itemName.text = "";
			itemIcon.sprite = null;
			itemIcon.color = new Color(0,0,0,0);

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			weightText.text = "Weight:  ";
			return;
		}

		ItemEntry item = itemEntry.item;
		itemName.text = item.entryName;
		itemIcon.sprite = item.icon;
		itemIcon.color = item.repColor;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();
		weightText.text = "Weight:  " + item.weight.ToString();
	}

}
