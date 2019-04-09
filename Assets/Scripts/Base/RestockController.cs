﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockController : MonoBehaviour {
    enum MenuState { CHARACTER, MENU, RECHARGE, TAKE, STORE }
	private MenuState currentMode;
	public PlayerData playerData;
	public IntVariable totalMoney;

	[Header("Views")]
	public GameObject charListView;
	public GameObject convoyView;
	public GameObject restockView;
	public GameObject normalButtonMenu;
	public GameObject secondButtonMenu;

	[Header("Character List")]
    public Transform listParentCharacter;
	public Transform characterPrefab;
	public int characterListSize;
	private EntryList<RestockListEntry> characters;

	[Header("Stock List")]
    public StorageList convoy;

	[Header("Restock List")]
    public MyButtonList restockMenuButtons;
    public Transform listParentRestock;
	public Transform restockPrefab;
	public int itemListSize;
	private EntryList<ItemListEntry> itemList;

	[Header("Inventory box")]
	public TMPro.TextMeshProUGUI charName;
	public Image portrait;
	public TMPro.TextMeshProUGUI[] inventory;

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

	[Header("Restock promt")]
	public MyPrompt restockPrompt;
	private bool promptMode;


	private void Start() {
		restockView.SetActive(false);
		charListView.SetActive(false);
		convoyView.SetActive(false);
		normalButtonMenu.SetActive(true);
		secondButtonMenu.SetActive(false);

		characters = new EntryList<RestockListEntry>(characterListSize);
		itemList = new EntryList<ItemListEntry>(itemListSize);
		restockMenuButtons.ResetButtons();
		restockMenuButtons.AddButton("RESTOCK");
		restockMenuButtons.AddButton("TAKE");
		restockMenuButtons.AddButton("STORE");
		convoy.Setup();
	}

	public void ShowRestock() {
		charListView.SetActive(true);
		normalButtonMenu.SetActive(false);
		GenerateCharacterList();
		MoveSelection(0);
	}

    private void GenerateCharacterList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;

        characters.ResetList();

        for (int i = 0; i < playerData.stats.Count; i++) {
			Transform t = Instantiate(characterPrefab, listParentCharacter);
			RestockListEntry entry = characters.CreateEntry(t);
			entry.FillData(playerData.stats[i], playerData.inventory[i]);
        }
        characterPrefab.gameObject.SetActive(false);
    }

    private void UpdateCharacterList() {
        for (int i = 0; i < playerData.stats.Count; i++) {
			characters.GetEntry(i).UpdateRestock();
        }
    }

    private void GenerateInventoryList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;

        itemList.ResetList();

        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (!tuple.item) {
				if (currentMode == MenuState.STORE) {
					Transform t2 = Instantiate(restockPrefab, listParentRestock);
					ItemListEntry entry2 = itemList.CreateEntry(t2);
					entry2.FillDataEmpty(i);
				}
				continue;
			}
			Transform t = Instantiate(restockPrefab, listParentRestock);
			ItemListEntry entry = itemList.CreateEntry(t);
			int charges = 0;
			float cost = 0;
			CalculateCharge(tuple, ref cost, ref charges, false);
			string chargeStr = tuple.charge.ToString();
			if (currentMode == MenuState.RECHARGE)
				chargeStr += " / " + tuple.item.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple.item, chargeStr, costStr);
        }
        restockPrefab.gameObject.SetActive(false);
		ShowItemInfo();
    }

	private void UpdateInventoryList() {
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (!tuple.item) {
				if (currentMode == MenuState.STORE) {
					Transform t2 = Instantiate(restockPrefab, listParentRestock);
					ItemListEntry entry2 = itemList.CreateEntry(t2);
					entry2.FillDataEmpty(i);
				}
				continue;
			}

			ItemListEntry entry = itemList.GetEntry(i);
			int charges = 0;
			float cost = 0;
			CalculateCharge(tuple, ref cost, ref charges, false);
			string chargeStr = tuple.charge + " / " + tuple.item.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple.item, chargeStr, costStr);
        }
		itemList.Move(0);
	}

	public void MoveSelection(int dir) {
		if (currentMode == MenuState.CHARACTER) {
			characters.Move(dir);
            ShowCharInfo();
        }
		else if (currentMode == MenuState.MENU) {
			restockMenuButtons.Move(dir);
		}
		else if (currentMode == MenuState.RECHARGE) {
			itemList.Move(dir);
			ShowItemInfo();
		}
		else if (currentMode == MenuState.TAKE) {
			convoy.Move(dir);
			ShowCharInfo();
		}
		else if (currentMode == MenuState.STORE) {
			itemList.Move(dir);
			ShowItemInfo();
		}
	}

	public void MoveSide(int dir) {
		if (promptMode) {
			restockPrompt.Move(dir);
		}
		else if (currentMode == MenuState.TAKE) {
			convoy.ChangeCategory(dir);
		}
	}

	public void SelectItem() {
		if (promptMode) {
			if (restockPrompt.Click(true) == MyPrompt.Result.OK1) {
				if (currentMode == MenuState.RECHARGE) {
					RestockItem();
				}
			}
			promptMode = false;
		}
		else if (currentMode == MenuState.CHARACTER) {
			currentMode = MenuState.MENU;
			secondButtonMenu.SetActive(true);
			restockMenuButtons.ForcePosition(0);
		}
		else if (currentMode == MenuState.MENU) {
			int buttonPos = restockMenuButtons.GetPosition();
			if (buttonPos == 0) {
				currentMode = MenuState.RECHARGE;
				GenerateInventoryList();
				restockView.SetActive(true);
			}
			else if (buttonPos == 1) {
				currentMode = MenuState.TAKE;
				convoy.SetupStorage();
				convoyView.SetActive(true);
			}
			else if (buttonPos == 2) {
				currentMode = MenuState.STORE;
				GenerateInventoryList();
				restockView.SetActive(true);
			}
		}
		else if (currentMode == MenuState.RECHARGE) {
			promptMode = true;
			int charges = 0;
			float cost = 0;
			CalculateCharge(null, ref cost, ref charges, true);
			if (charges == 0)
				return;

			restockPrompt.ShowWindow("Restock item?\n" + charges + " / " + Mathf.CeilToInt(cost * charges) + " cost", true);
		}
		else if (currentMode == MenuState.TAKE) {
			TakeItem();
		}
		else if (currentMode == MenuState.STORE) {
			StoreItem();
		}
	}

	public bool DeselectItem() {
		if (promptMode) {
			restockPrompt.Click(false);
			promptMode = false;
			return false;
		}
		else if (currentMode == MenuState.MENU) {
			secondButtonMenu.SetActive(false);
			currentMode =  MenuState.CHARACTER;
			return false;
		}
		else if (currentMode == MenuState.RECHARGE) {
			currentMode = MenuState.MENU;
			UpdateCharacterList();
			restockView.SetActive(false);
			return false;
		}
		else if (currentMode == MenuState.TAKE) {
			currentMode = MenuState.MENU;
			convoyView.SetActive(false);
			UpdateCharacterList();
			return false;
		}
		else if (currentMode == MenuState.STORE) {
			currentMode = MenuState.MENU;
			UpdateCharacterList();
			restockView.SetActive(false);
			return false;
		}
		normalButtonMenu.SetActive(true);
		charListView.SetActive(false);
		return true;
	}

	private void CalculateCharge(InventoryTuple t, ref float cost, ref int charges, bool affordable) {
		InventoryTuple tuple = t ?? characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		charges = tuple.GetMissingCharges();
		if (charges == 0)
			return;
		cost = tuple.ChargeCost();
		if (affordable) {
			int available = Mathf.FloorToInt(totalMoney.value / cost);
			charges = Mathf.Min(charges, available);
		}
	}

	private void RestockItem() {
		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		int charges = 0;
		float cost = 0;
		CalculateCharge(tuple, ref cost, ref charges, true);

		tuple.charge += charges;
		totalMoney.value -= Mathf.CeilToInt(cost * charges);
		TotalMoneyText.text = "Money:  " + totalMoney.value;
		UpdateInventoryList();
	}

	private void TakeItem() {
		Debug.Log("Take item");
		ItemListEntry item = convoy.GetEntry();
		if (!item)
			return;

		InventoryContainer invCon = characters.GetEntry().invCon;
		if (!invCon.HasRoom()) {
			restockPrompt.ShowPopup("Inventory is full!");
			promptMode = true;
			return;
		}
		invCon.AddItem(playerData.items[item.index]);
		convoy.RemoveEntry();
		playerData.items.RemoveAt(item.index);

		ShowCharInfo();
	}

	private void StoreItem() {
		Debug.Log("Store item");
		if (!itemList.GetEntry().item)
			return;

		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		InventoryItem item = new InventoryItem(tuple);
		playerData.items.Add(item);
		tuple.item = null;
		characters.GetEntry().invCon.CleanupInventory(null);

		itemList.RemoveEntry();
		Transform t2 = Instantiate(restockPrefab, listParentRestock);
		ItemListEntry entry2 = itemList.CreateEntry(t2);
		entry2.FillDataEmpty(0);
		ShowItemInfo();
	}

	private void ShowCharInfo() {
		RestockListEntry restock = characters.GetEntry();
		charName.text = restock.entryName.text;
		portrait.sprite = restock.icon.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = restock.invCon.GetTuple(i).item;
			inventory[i].text = (item) ? item.entryName : "-NONE-";
		}
	}

	private void ShowItemInfo() {
		RestockListEntry entry = characters.GetEntry();
		ItemEntry item = itemList.GetEntry().item;
		if (!entry || !item) {
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

		itemName.text = item.entryName;
		itemIcon.sprite = item.icon;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();
		weightText.text = "Weight:  " + item.weight.ToString();
	}

}
