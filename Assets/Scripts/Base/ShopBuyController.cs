﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopBuyController : MonoBehaviour {
    
    public MyButton[] categories;

	[Header("Entry List")]
	public SaveListVariable playerData;
	public IntVariable totalMoney;
	public FloatVariable sellRatio;
    public Transform listParent;
	public Transform entryPrefab;
	private bool buyMode;
	private ItemListVariable shopList;
    private int currentCategory;
	private int currentListIndex;
	private int listSize;
	private List<ItemListEntry> entryList = new List<ItemListEntry>();

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
	public GameObject promptView;
	public Text promptText;
	public MyButton promptYesButton;
	public MyButton promptNoButton;
	private bool promptMode;
	private int promptPosition;


	public void GenerateLists(ItemListVariable currentShopList, bool buying) {
		promptView.SetActive(false);
		buyMode = buying;
		shopList = currentShopList;
        ChangeCategory(0);
		GenerateList();
		currentListIndex = 0;
		MoveSelection(0);
	}

    private void GenerateList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;
        for (int i = listParent.childCount - 1; i > 2; i--) {
            Destroy(listParent.GetChild(i).gameObject);
        }

        entryList = new List<ItemListEntry>();
        listSize = (buyMode) ? shopList.items.Count : playerData.items.Count;
        int tempListSize = 0;
        ItemType currentType = (ItemType)(1+currentCategory);
        for (int i = 0; i < listSize; i++) {
			ItemEntry item = (buyMode) ? shopList.items[i] : playerData.items[i].item;
			int charges = (buyMode) ? shopList.items[i].maxCharge : playerData.items[i].charges;

			if (item.researchNeeded && !playerData.upgrader.IsResearched(item.uuid))
				continue;

			if (currentCategory == 7 && item.itemCategory == ItemCategory.CONSUME) {
				CreateListEntry(i, item, charges);
				tempListSize++;
			}
			else if (currentCategory == 6 && item.itemCategory == ItemCategory.STAFF) {
				CreateListEntry(i, item, charges);
				tempListSize++;
			}
			else if (item.itemType == currentType) {
                CreateListEntry(i, item, charges);
                tempListSize++;
            }
        }
        listSize = tempListSize;
        entryPrefab.gameObject.SetActive(false);
    }

	private void CreateListEntry(int index, ItemEntry item, int charges) {
		Transform t = Instantiate(entryPrefab, listParent);

		ItemListEntry entry = t.GetComponent<ItemListEntry>();
		entry.FillData(index, item, charges, totalMoney.value, buyMode, sellRatio.value);
		entry.SetHighlight(false);
		entryList.Add(entry);

		t.gameObject.SetActive(true);
	}

	public void MoveSelection(int dir) {
		if (!promptMode) {
			if (entryList.Count > 0) {
				currentListIndex = OPMath.FullLoop(0, listSize, currentListIndex + dir);
				for (int i = 0; i < listSize; i++) {
					entryList[i].SetHighlight(currentListIndex == i);
				}
			}
            SetupItemInfo();
        }
	}

    public void ChangeCategory(int dir) {
        if (!promptMode) {
            do {
                currentCategory = OPMath.FullLoop(0, 8, currentCategory + dir);
            } while (!categories[currentCategory].gameObject.activeSelf);
			currentListIndex = 0;
            GenerateList();
			MoveSelection(0);
        }
		else {
			promptPosition = (promptPosition + dir) % 2;
			promptYesButton.SetSelected(promptPosition == 0);
			promptNoButton.SetSelected(promptPosition == 1);
		}
		for (int i = 0; i < categories.Length; i++) {
			categories[i].SetSelected(i == currentCategory);
		}
    }

	public void SelectItem(bool isBuy) {
		if (entryList.Count == 0 || !entryList[currentListIndex].affordable) {
			return;
		}
		else if (!promptMode) {
			promptMode = true;
			SetupTradePrompt(isBuy);
			promptView.SetActive(true);
		}
		else {
			if (promptPosition == 0) {
				Debug.Log((isBuy) ? "Buy item" : "Sell item");
				if (isBuy) { // Buy item
					ItemEntry item = ScriptableObject.CreateInstance<ItemEntry>();
					item.CopyValues(entryList[currentListIndex].item);
					totalMoney.value -= item.cost;
					playerData.items.Add(new InventoryItem { item = item, charges = item.maxCharge });
				}
				else { // Sell item
					ItemEntry item = entryList[currentListIndex].item;
					totalMoney.value += (int)(item.cost * sellRatio.value);
					playerData.items.RemoveAt(entryList[currentListIndex].index);
				}
			}
			GenerateList();
			DeselectItem();
			MoveSelection(currentListIndex >= entryList.Count ? entryList.Count - currentListIndex -1 : 0);
		}
	}

	public bool DeselectItem() {
		if (promptMode) {
			promptMode = false;
			promptView.SetActive(false);
			return false;
		}

		return true;
	}

	private void SetupTradePrompt(bool isBuy) {
		promptText.text = (isBuy) ? "Buy item?" : "Sell item?";
		promptPosition = 0;
		ChangeCategory(0);
	}

	private void SetupItemInfo() {
		if (entryList.Count == 0) {
			itemName.text = "";
			itemIcon.sprite = null;

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			weightText.text = "Weight:  ";
			return;
		}

		ItemEntry item = entryList[currentListIndex].item;
		itemName.text = item.entryName;
		itemIcon.sprite = entryList[currentListIndex].icon.sprite;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();
		weightText.text = "Weight:  " + item.weight.ToString();
	}

}