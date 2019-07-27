using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEquipment : InputReceiverDelegate {
	enum MenuState { MENU, CHARACTER, RECHARGE, STORE, TAKE }
	private MenuState currentMode;
	public PlayerData playerData;
	public IntVariable totalMoney;
	public MyButtonList menuButtons;
	public Text buttonTitle;

	[Header("Views")]
	public GameObject emptyView;
	public GameObject charListView;
	public GameObject convoyView;
	public GameObject restockView;
	public GameObject infoBoxView;

	[Header("Character List")]
	public Transform listParentCharacter;
	public Transform characterPrefab;
	public int characterListSize;
	private EntryList<RestockListEntry> characters;

	[Header("Stock List")]
	public StorageList convoy;

	[Header("Restock List")]
	public Transform listParentRestock;
	public Transform restockPrefab;
	public int itemListSize;
	private EntryList<ItemListEntry> itemList;

	[Header("Inventory box")]
	public Text charName;
	public Image portrait;
	public Text[] inventory;

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

	[Header("Restock promt")]
	public MyPrompt restockPrompt;
	private bool promptMode;


	private void Start() {
		currentMode = MenuState.MENU;
		emptyView.SetActive(true);
		restockView.SetActive(false);
		charListView.SetActive(false);
		convoyView.SetActive(false);
		infoBoxView.SetActive(false);

		characters = new EntryList<RestockListEntry>(characterListSize);
		itemList = new EntryList<ItemListEntry>(itemListSize);
		convoy.Setup();

		buttonTitle.text = "EQUIPMENT";
		menuButtons.ResetButtons();
		menuButtons.AddButton("RESTOCK");
		menuButtons.AddButton("STORE");
		menuButtons.AddButton("TAKE");
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

		characters.GetEntry().invCon.CleanupInventory();
		itemList.ResetList();

		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (string.IsNullOrEmpty(tuple.uuid)) {
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
			string chargeStr = tuple.currentCharges.ToString();
			if (currentMode == MenuState.RECHARGE)
				chargeStr += " / " + tuple.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple, chargeStr, costStr);
		}
		restockPrefab.gameObject.SetActive(false);
		ShowItemInfo();
	}

	private void UpdateInventoryList() {
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(i);
			if (string.IsNullOrEmpty(tuple.uuid)) {
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
			string chargeStr = tuple.currentCharges + " / " + tuple.maxCharge;
			string costStr = (currentMode == MenuState.RECHARGE) ? Mathf.CeilToInt(cost * charges).ToString() : "";
			entry.FillDataSimple(i, tuple, chargeStr, costStr);
		}
		itemList.Move(0);
	}

	public bool MoveVertical(int dir) {
		if (currentMode == MenuState.MENU) {
			menuButtons.Move(dir);
			return true;
		}
		else if (currentMode == MenuState.CHARACTER) {
			characters.Move(dir);
			ShowCharInfo();
			return true;
		}
		else if (currentMode == MenuState.RECHARGE) {
			itemList.Move(dir);
			ShowItemInfo();
			return true;
		}
		else if (currentMode == MenuState.TAKE) {
			convoy.Move(dir);
			ShowCharInfo();
			return true;
		}
		else if (currentMode == MenuState.STORE) {
			itemList.Move(dir);
			ShowItemInfo();
			return true;
		}
		return false;
	}

	public bool MoveHorizontal(int dir) {
		if (promptMode) {
			restockPrompt.Move(dir);
			return true;
		}
		else if (currentMode == MenuState.TAKE) {
			convoy.ChangeCategory(dir);
			return true;
		}
		return false;
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
		else if (currentMode == MenuState.MENU) {
			currentMode = MenuState.CHARACTER;
			GenerateCharacterList();
			characters.ForcePosition(0);
			ShowCharInfo();
			emptyView.SetActive(false);
			charListView.SetActive(true);
			infoBoxView.SetActive(false);
		}
		else if (currentMode == MenuState.CHARACTER) {
			int buttonPos = menuButtons.GetPosition();
			if (buttonPos == 0) {
				currentMode = MenuState.RECHARGE;
				GenerateInventoryList();
				restockView.SetActive(true);
			}
			else if (buttonPos == 1) {
				currentMode = MenuState.STORE;
				GenerateInventoryList();
				restockView.SetActive(true);
			}
			else if (buttonPos == 2) {
				currentMode = MenuState.TAKE;
				convoy.SetupStorage();
				convoyView.SetActive(true);
			}
		}
		else if (currentMode == MenuState.RECHARGE) {
			promptMode = true;
			int charges = 0;
			float cost = 0;
			CalculateCharge(null, ref cost, ref charges, true);
			if (charges == 0)
				return;

			restockPrompt.ShowYesNoPopup("Restock item?\n" + charges + " / " + Mathf.CeilToInt(cost * charges) + " cost", true);
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
			MenuChangeDelay(MenuMode.BASE_MAIN);
			menuBackEvent.Invoke();
			return true;
		}
		else if (currentMode == MenuState.CHARACTER) {
			currentMode = MenuState.MENU;
			emptyView.SetActive(true);
			charListView.SetActive(false);
			return false;
		}
		else if (currentMode == MenuState.RECHARGE) {
			currentMode = MenuState.CHARACTER;
			UpdateCharacterList();
			restockView.SetActive(false);
			infoBoxView.SetActive(false);
			return false;
		}
		else if (currentMode == MenuState.TAKE) {
			currentMode = MenuState.CHARACTER;
			convoyView.SetActive(false);
			infoBoxView.SetActive(false);
			UpdateCharacterList();
			return false;
		}
		else if (currentMode == MenuState.STORE) {
			currentMode = MenuState.CHARACTER;
			UpdateCharacterList();
			restockView.SetActive(false);
			infoBoxView.SetActive(false);
			return false;
		}
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

		tuple.currentCharges += charges;
		totalMoney.value -= Mathf.CeilToInt(cost * charges);
		TotalMoneyText.text = "Money:  " + totalMoney.value;
		UpdateInventoryList();
	}

	private void TakeItem() {
		ItemListEntry item = convoy.GetEntry();
		if (!item)
			return;
		Debug.Log("Take item");

		InventoryContainer invCon = characters.GetEntry().invCon;
		if (!invCon.AddItem(playerData.items[item.index])) {
			restockPrompt.ShowOkPopup("Inventory is full!");
			promptMode = true;
			return;
		}
		convoy.RemoveEntry();
		playerData.items.RemoveAt(item.index);

		ShowCharInfo();
	}

	private void StoreItem() {
		ItemListEntry item = itemList.GetEntry();
		if (item == null || string.IsNullOrEmpty(item.tuple.uuid))
			return;

		Debug.Log("Store item");
		int index = itemList.GetPosition();
		InventoryTuple tuple = characters.GetEntry().invCon.GetTuple(index);
		playerData.items.Add(tuple.StoreData());
		characters.GetEntry().invCon.DropItem(index);

		GenerateInventoryList();
		itemList.ForcePosition(index);
		ShowItemInfo();
	}

	private void ShowCharInfo() {
		RestockListEntry restock = characters.GetEntry();
		charName.text = restock.entryName.text;
		portrait.sprite = restock.icon.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = restock.invCon.GetTuple(i);
			inventory[i].text = (!string.IsNullOrEmpty(tuple.uuid)) ? tuple.entryName : "-NONE-";
		}
	}

	private void ShowItemInfo() {
		infoBoxView.SetActive(true);
		RestockListEntry entry = characters.GetEntry();
		InventoryTuple tuple = itemList.GetEntry().tuple;
		if (!entry || tuple == null || string.IsNullOrEmpty(tuple.uuid)) {
			itemName.text = "";
			itemType.text = "";
			itemIcon.sprite = null;

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			return;
		}

		itemName.text = tuple.entryName;
		itemType.text = InventoryContainer.GetWeaponTypeName(tuple.weaponType);
		itemIcon.sprite = tuple.icon;

		pwrText.text = "Pwr:  " + tuple.power.ToString();
		rangeText.text = "Range:  " + tuple.range.ToString();
		hitText.text = "Hit:  " + tuple.hitRate.ToString();
		critText.text = "Crit:  " + tuple.critRate.ToString();
		reqText.text = "Req:  " + tuple.skillReq.ToString();
	}

	public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_EQUIP);
	}

	public override void OnUpArrow() {
		if (MoveVertical(-1))
			menuMoveEvent.Invoke();
	}

	public override void OnDownArrow() {
		if (MoveVertical(1))
			menuMoveEvent.Invoke();
	}

	public override void OnLeftArrow() {
		if (MoveHorizontal(-1))
			menuMoveEvent.Invoke();
	}

	public override void OnRightArrow() {
		if (MoveHorizontal(1))
			menuMoveEvent.Invoke();
	}

	public override void OnOkButton() {
		SelectItem();
		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		DeselectItem();
		menuBackEvent.Invoke();
	}

	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
	public override void OnStartButton() { }
}
