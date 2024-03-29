﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyController : MonoBehaviour {
    
	public PlayerData playerData;
	public IntVariable totalMoney;
	public FloatVariable sellRatio;

	[Header("Information box")]
	public Text TotalMoneyText;
	public Text itemName;
	public Text itemType;
	public Image itemIcon;

	public Text pwrText;
	public Text rangeText;
	public Text hitText;
	public Text critText;
	public Text reqText;

	[Header("Buy/Sell items")]
	public StorageList shopList;
	public MyPrompt buyPrompt;
	private bool promptMode;


	private void Start() {
		shopList.Setup();
	}

	public void GenerateShopList(ItemListVariable currentShopList) {
		shopList.SetupBuy(currentShopList);
		SetupItemInfo();
	}

	public void GenerateSellList() {
		shopList.SetupSell();
		SetupItemInfo();
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
			buyPrompt.ShowYesNoPopup((shopList.buyMode) ? "Buy item?" : "Sell item?", true);
		}
		else {
			if (buyPrompt.Click(true) == MyPrompt.Result.OK1) {
				if (shopList.buyMode) { // Buy item
					totalMoney.value -= itemEntry.tuple.cost;
					playerData.items.Add(itemEntry.tuple.StoreData());
				}
				else { // Sell item
					totalMoney.value += (int)(itemEntry.tuple.cost * sellRatio.value);
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
		if (!itemEntry || string.IsNullOrEmpty(itemEntry.tuple.uuid)) {
			itemName.text = "";
			itemType.text = "";
			itemIcon.color = new Color(0,0,0,0);

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			return;
		}

		InventoryTuple item = itemEntry.tuple;
		itemName.text = item.entryName;
		itemType.text = InventoryContainer.GetWeaponTypeName(item.weaponType);
		itemIcon.sprite = item.icon;
		itemIcon.color = item.repColor;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();
	}

}
