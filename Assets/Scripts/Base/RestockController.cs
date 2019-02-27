using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockController : MonoBehaviour {
    enum MenuState { CHARACTER, MENU, INV, TAKE, STORE, PROMPT }
	private MenuState currentMode;
	public SaveListVariable playerData;
	public IntVariable totalMoney;

	[Header("Views")]
	public GameObject charListView;
	public GameObject charMenuView;
	public GameObject restockView;

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
		charMenuView.SetActive(false);

		characters = new EntryList<RestockListEntry>(characterListSize);
		itemList = new EntryList<ItemListEntry>(itemListSize);
		restockMenuButtons.ResetButtons();
		restockMenuButtons.AddButton("RESTOCK");
		restockMenuButtons.AddButton("TAKE");
		restockMenuButtons.AddButton("STORE");
	}

	public void GenerateLists() {
		charListView.SetActive(true);
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

    private void GenerateInventoryList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;

        itemList.ResetList();

        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (!tuple.item)
				break;
			Transform t = Instantiate(restockPrefab, listParentRestock);
			ItemListEntry entry = itemList.CreateEntry(t);
			entry.FillData(i, tuple.item, tuple.charge, 9999, false, 1);
        }
        restockPrefab.gameObject.SetActive(false);
    }

	private void UpdateInventoryList() {
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (!tuple.item)
				break;

			ItemListEntry entry = itemList.GetEntry(i);
			entry.FillData(i, tuple.item, tuple.charge, 9999, false, 1);
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
			SetupItemInfo();
		}
		else if (currentMode == MenuState.INV) {
			itemList.Move(dir);
			SetupItemInfo();
		}
		else if (currentMode == MenuState.PROMPT) {
			restockPrompt.Move(dir);
		}
	}

	public void MoveSide(int dir) {

	}

	public void SelectItem() {
		if (currentMode == MenuState.CHARACTER) {
			currentMode = MenuState.MENU;
			charMenuView.SetActive(true);
			restockMenuButtons.ForcePosition(0);
			GenerateInventoryList();
		}
		else if (currentMode == MenuState.MENU) {
			int buttonPos = restockMenuButtons.GetPosition();
			if (buttonPos == 0) {
				charListView.SetActive(false);
				restockView.SetActive(true);
				currentMode = MenuState.INV;
			}
			else if (buttonPos == 1) {
				charListView.SetActive(false);
				currentMode = MenuState.TAKE;
			}
			else if (buttonPos == 2) {
				charListView.SetActive(false);
				currentMode = MenuState.STORE;
			}
		}
		else if (currentMode == MenuState.INV) {
			int charges = 0;
			float cost = 0;
			CalculateCharge(ref cost, ref charges);
			if (charges == 0)
				return;

			restockPrompt.ShowWindow("Restock item?\n" + charges + " / " + Mathf.CeilToInt(cost * charges) + " cost", true);
		}
		else if (currentMode == MenuState.PROMPT) {
			if (restockPrompt.Click(true)) {
				RestockItem();
			}
			currentMode = MenuState.INV;
		}
	}

	private void CalculateCharge(ref float cost, ref int charges) {
		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		charges = tuple.item.maxCharge - tuple.charge;
		if (charges == 0)
			return;
		currentMode = MenuState.PROMPT;
		cost = tuple.item.cost / (float)tuple.item.maxCharge;
		int affordable = Mathf.FloorToInt(totalMoney.value / cost);
		charges = Mathf.Min(charges, affordable);
	}

	public bool DeselectItem() {
		if (currentMode == MenuState.MENU) {
			charMenuView.SetActive(false);
			currentMode =  MenuState.CHARACTER;
			return false;
		}
		else if (currentMode == MenuState.INV) {
			GenerateCharacterList();
			charListView.SetActive(true);
			restockView.SetActive(false);
			currentMode = MenuState.MENU;
			return false;
		}
		else if (currentMode == MenuState.TAKE) {
			GenerateCharacterList();
			charListView.SetActive(true);
			currentMode = MenuState.MENU;
		}
		else if (currentMode == MenuState.STORE) {
			GenerateCharacterList();
			charListView.SetActive(true);
			currentMode = MenuState.MENU;
		}
		else if (currentMode == MenuState.PROMPT) {
			restockPrompt.Click(false);
			currentMode = MenuState.INV;
			return false;
		}
		return true;
	}

	private void RestockItem() {
		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(itemList.GetPosition());
		int charges = 0;
		float cost = 0;
		CalculateCharge(ref cost, ref charges);

		tuple.charge += charges;
		totalMoney.value -= Mathf.CeilToInt(cost * charges);
		TotalMoneyText.text = "Money:  " + totalMoney.value;
		UpdateInventoryList();
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
		if (!entry) {
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

		ItemEntry item = entry.invCon.GetTuple(itemList.GetPosition()).item;
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
