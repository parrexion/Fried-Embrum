using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepInventorySelect : MonoBehaviour {

	private enum State { CHAR, MENU, TAKE, STORE }

	public PlayerData playerData;
	public PrepListVariable prepList;
	public IntVariable totalMoney;
	private State currentMode;
	public MyPrompt prompt;

	[Header("Views")]
	public GameObject charListView;
	public GameObject inventoryView;
	public GameObject convoyView;
	public GameObject storeView;

	[Header("Character List")]
	public Transform charListParent;
	public Transform charEntryPrefab;
	public int visibleSize;
	private EntryList<PrepCharacterEntry> charList;
	
	[Header("Inventory box take")]
	public Text tCharName;
	public Text[] tInventory;

	[Header("Store List")]
    public Transform listParentRestock;
	public Transform restockPrefab;
	public int itemListSize;
	private EntryList<ItemListEntry> itemList;

	[Header("Stock List")]
    public StorageList convoy;

	[Header("Inventory box overview")]
	public Text charName;
	public Image portrait;
	public Text[] inventory;

	[Header("Information box")]
	public Text TotalMoneyText;
	public Text itemName;
	public Image itemIcon;

	public Text pwrText;
	public Text rangeText;
	public Text hitText;
	public Text critText;
	public Text reqText;


	public void GenerateList() {
		currentMode = State.CHAR;
		charListView.SetActive(true);
		inventoryView.SetActive(false);
		storeView.SetActive(false);
		convoyView.SetActive(false);
		if (charList == null)
			charList = new EntryList<PrepCharacterEntry>(visibleSize);
		charList.ResetList();
		
		for (int i = 0; i < prepList.values.Count; i++) {
			Transform t = Instantiate(charEntryPrefab, charListParent.transform);
			PrepCharacterEntry entry = charList.CreateEntry(t);
			entry.FillData(playerData.stats[prepList.values[i].index], playerData.inventory[prepList.values[i].index], prepList.values[i]);
		}
		ShowCharInfo();
		charEntryPrefab.gameObject.SetActive(false);
		convoy.Setup();
	}

    private void GenerateInventoryList() {
		inventoryView.SetActive(true);
		charListView.SetActive(false);
		storeView.SetActive(true);
		convoyView.SetActive(false);
		TotalMoneyText.text = "Money:  " + totalMoney.value;
		
		if (itemList == null)
			itemList = new EntryList<ItemListEntry>(itemListSize);
        itemList.ResetList();

        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = charList.GetEntry().invCon.GetTuple(i);
			if (!tuple.item) {
				Transform t2 = Instantiate(restockPrefab, listParentRestock);
				ItemListEntry entry2 = itemList.CreateEntry(t2);
				entry2.FillDataEmpty(i);
				continue;
			}
			Transform t = Instantiate(restockPrefab, listParentRestock);
			ItemListEntry entry = itemList.CreateEntry(t);
			int charges = 0;
			float cost = 0;
			CalculateCharge(tuple, ref cost, ref charges, false);
			string chargeStr = tuple.charge + " / " + tuple.item.maxCharge;
			string costStr = Mathf.CeilToInt(cost * charges).ToString();
			entry.FillDataSimple(i, tuple.item, chargeStr, costStr);
        }
        restockPrefab.gameObject.SetActive(false);
    }

	public void MoveSelection(int dir) {
		if (currentMode == State.CHAR) {
			charList.Move(dir);
			ShowCharInfo();
		}
		else if (currentMode == State.TAKE) {
			convoy.Move(dir);
			ShowItemInfo();
		}
		else if (currentMode == State.STORE) {
			itemList.Move(dir);
			ShowItemInfo();
		}
	}

	public void MoveHorizontal(int dir) {
		if (currentMode == State.MENU) {
			prompt.Move(dir);
		}
		else if (currentMode == State.TAKE) {
			convoy.ChangeCategory(dir);
			ShowItemInfo();
		}
	}

	public void SelectItem() {
		if (currentMode == State.CHAR) {
			prompt.Show3Options("What do you want to do?", "TAKE", "STORE", "CANCEL", true);
			currentMode = State.MENU;
		}
		else if (currentMode == State.MENU) {
			MyPrompt.Result res = prompt.Click(true);
			if (res == MyPrompt.Result.OK1) {
				currentMode = State.TAKE;
				convoy.SetupStorage();
				convoyView.SetActive(true);
				inventoryView.SetActive(true);
				charListView.SetActive(false);
				ShowItemInfo();
			}
			else if (res == MyPrompt.Result.OK2) {
				currentMode = State.STORE;
				GenerateInventoryList();
				ShowItemInfo();
			}
			else {
				currentMode = State.CHAR;
			}
		}
		else if (currentMode == State.TAKE) {
			TakeItem();
		}
		else if (currentMode == State.STORE) {
			StoreItem();
		}
	}

	public bool DeselectItem() {
		if (currentMode == State.TAKE || currentMode == State.STORE) {
			currentMode = State.CHAR;
			charListView.SetActive(true);
			inventoryView.SetActive(false);
			storeView.SetActive(false);
			convoyView.SetActive(false);
			ShowCharInfo();
			return false;
		}
		else if (currentMode == State.MENU) {
			prompt.Click(false);
			currentMode = State.CHAR;
			return false;
		}

		return true;
	}
	

	private void ShowCharInfo() {
		PrepCharacterEntry entry = charList.GetEntry();
		charName.text = entry.entryName.text;
		portrait.sprite = entry.icon.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = entry.invCon.GetTuple(i).item;
			inventory[i].text = (item) ? item.entryName : "-NONE-";
		}
	}

	private void ShowCharInfoTake() {
		PrepCharacterEntry entry = charList.GetEntry();
		tCharName.text = entry.entryName.text;
		//portrait.sprite = entry.icon.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = entry.invCon.GetTuple(i).item;
			tInventory[i].text = (item) ? item.entryName : "-NONE-";
		}
	}

	private void ShowItemInfo() {
		PrepCharacterEntry entry = charList.GetEntry();
		ItemEntry item = null;
		if (currentMode == State.STORE)
			item = itemList.GetEntry().item;
		else if (convoy.GetEntry())
			item = convoy.GetEntry().item;
		
		if (!entry || !item) {
			itemName.text = "";
			itemIcon.sprite = null;

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			return;
		}

		itemName.text = item.entryName;
		itemIcon.sprite = item.icon;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();

		if (currentMode == State.TAKE)
			ShowCharInfoTake();
	}

	private void TakeItem() {
		Debug.Log("Take item");
		ItemListEntry item = convoy.GetEntry();
		if (!item)
			return;

		InventoryContainer invCon = charList.GetEntry().invCon;
		if (!invCon.AddItem(playerData.items[item.index])) {
			prompt.ShowPopup("Inventory is full!");
			currentMode = State.MENU;
			return;
		}
		convoy.RemoveEntry();
		playerData.items.RemoveAt(item.index);

		ShowCharInfo();
		ShowCharInfoTake();
		ShowItemInfo();
	}
	
	private void StoreItem() {
		Debug.Log("Store item");
		if (!itemList.GetEntry().item)
			return;

		InventoryTuple tuple = charList.GetEntry().invCon.GetTuple(itemList.GetPosition());
		InventoryItem item = new InventoryItem(tuple);
		playerData.items.Add(item);
		tuple.item = null;
		charList.GetEntry().invCon.CleanupInventory();

		itemList.RemoveEntry();
		Transform t2 = Instantiate(restockPrefab, listParentRestock);
		ItemListEntry entry2 = itemList.CreateEntry(t2);
		entry2.FillDataEmpty(0);
		ShowItemInfo();
	}
	

	private void CalculateCharge(InventoryTuple t, ref float cost, ref int charges, bool affordable) {
		InventoryTuple tuple = t ?? charList.GetEntry().invCon.GetTuple(itemList.GetPosition());
		charges = tuple.GetMissingCharges();
		if (charges == 0)
			return;
		cost = tuple.ChargeCost();
		if (affordable) {
			int available = Mathf.FloorToInt(totalMoney.value / cost);
			charges = Mathf.Min(charges, available);
		}
	}
}
