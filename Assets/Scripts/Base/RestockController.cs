using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockController : MonoBehaviour {
    enum MenuState { CHARACTER, MENU, RECHARGE, TAKE, STORE, PROMPT }
	private MenuState currentMode;
	public SaveListVariable playerData;
	public IntVariable totalMoney;

	[Header("Views")]
	public GameObject charListView;
	public GameObject restockView;
	public GameObject normalButtonMenu;
	public GameObject secondButtonMenu;

	[Header("Character List")]
    public Transform listParentCharacter;
	public Transform characterPrefab;
	public int characterListSize;
	private EntryList<RestockListEntry> characters;

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


	private void Start() {
		restockView.SetActive(false);
		charListView.SetActive(false);
		normalButtonMenu.SetActive(true);
		secondButtonMenu.SetActive(false);

		characters = new EntryList<RestockListEntry>(characterListSize);
		itemList = new EntryList<ItemListEntry>(itemListSize);
		restockMenuButtons.ResetButtons();
		restockMenuButtons.AddButton("RESTOCK");
		restockMenuButtons.AddButton("TAKE");
		restockMenuButtons.AddButton("STORE");
	}

	public void GenerateLists() {
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
			CalculateCharge(tuple, ref cost, ref charges);
			string chargeStr = tuple.charge.ToString();
			if (currentMode == MenuState.RECHARGE)
				chargeStr += " / " + tuple.item.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple.item, chargeStr, costStr);
        }
        restockPrefab.gameObject.SetActive(false);
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
			CalculateCharge(tuple, ref cost, ref charges);
			string chargeStr = tuple.charge + " / " + tuple.item.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple.item, chargeStr, costStr);
        }
		itemList.Move(0);
	}

	public void MoveSelection(int dir) {
		if (currentMode == MenuState.CHARACTER) {
			characters.Move(dir);
            SetupCharInfo();
        }
		else if (currentMode == MenuState.MENU) {
			restockMenuButtons.Move(dir);
		}
		else if (currentMode == MenuState.RECHARGE) {
			itemList.Move(dir);
			SetupItemInfo();
		}
		else if (currentMode == MenuState.TAKE) {

		}
		else if (currentMode == MenuState.STORE) {
			itemList.Move(dir);
			SetupItemInfo();
		}
	}

	public void MoveSide(int dir) {
		if (currentMode == MenuState.PROMPT) {
			restockPrompt.Move(dir);
		}
	}

	public void SelectItem() {
		if (currentMode == MenuState.CHARACTER) {
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
			}
			else if (buttonPos == 2) {
				currentMode = MenuState.STORE;
				GenerateInventoryList();
				restockView.SetActive(true);
			}
		}
		else if (currentMode == MenuState.RECHARGE) {
			currentMode = MenuState.PROMPT;
			int charges = 0;
			float cost = 0;
			CalculateCharge(null, ref cost, ref charges);
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
		else if (currentMode == MenuState.PROMPT) {
			currentMode = MenuState.RECHARGE;
			if (restockPrompt.Click(true)) {
				RestockItem();
			}
		}
	}

	public bool DeselectItem() {
		if (currentMode == MenuState.MENU) {
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
			UpdateCharacterList();
			return false;
		}
		else if (currentMode == MenuState.STORE) {
			currentMode = MenuState.MENU;
			UpdateCharacterList();
			restockView.SetActive(false);
			return false;
		}
		else if (currentMode == MenuState.PROMPT) {
			restockPrompt.Click(false);
			currentMode = MenuState.RECHARGE;
			return false;
		}
		normalButtonMenu.SetActive(true);
		charListView.SetActive(false);
		return true;
	}

	private void CalculateCharge(InventoryTuple t, ref float cost, ref int charges) {
		InventoryTuple tuple = (t != null) ? t : characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		charges = tuple.item.maxCharge - tuple.charge;
		if (charges == 0)
			return;
		cost = tuple.item.cost / (float)tuple.item.maxCharge;
		int affordable = Mathf.FloorToInt(totalMoney.value / cost);
		charges = Mathf.Min(charges, affordable);
	}

	private void RestockItem() {
		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		int charges = 0;
		float cost = 0;
		CalculateCharge(tuple, ref cost, ref charges);

		tuple.charge += charges;
		totalMoney.value -= Mathf.CeilToInt(cost * charges);
		TotalMoneyText.text = "Money:  " + totalMoney.value;
		UpdateInventoryList();
	}

	private void TakeItem() {
		Debug.Log("Take item");
	}

	private void StoreItem() {
		Debug.Log("Store item");
		if (!itemList.GetEntry().item)
			return;

		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		InventoryItem item = new InventoryItem() {
			charges = tuple.charge,
			item = tuple.item
		};
		playerData.items.Add(item);
		tuple.item = null;
		characters.GetEntry().invCon.CleanupInventory(null);

		int pos = itemList.GetPosition();
		GenerateInventoryList();
		itemList.ForcePosition(pos);
	}

	private void SetupCharInfo() {
		RestockListEntry restock = characters.GetEntry();
		charName.text = restock.entryName.text;
		portrait.sprite = restock.icon.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = restock.invCon.GetTuple(i).item;
			inventory[i].text = (item) ? item.entryName : "-NONE-";
		}
	}

	private void SetupItemInfo() {
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
