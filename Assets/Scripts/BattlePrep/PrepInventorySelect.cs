using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepInventorySelect : MonoBehaviour {

	private enum State { CHAR, MENU, TAKE, STORE }

	public SaveListVariable playerData;
	public PrepListVariable prepList;
	public IntVariable totalMoney;
	private State currentMode;
	public MyPrompt prompt;

	[Header("Views")]
	public GameObject charListView;
	public GameObject convoyView;
	public GameObject storeView;

	[Header("Character List")]
	public Transform charListParent;
	public Transform charEntryPrefab;
	public int visibleSize;
	private EntryList<PrepCharacterEntry> charList;

	[Header("Restock List")]
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


	public void GenerateList() {
		currentMode = State.CHAR;
		charListView.SetActive(true);
		storeView.SetActive(false);
		convoyView.SetActive(false);
		if (charList == null)
			charList = new EntryList<PrepCharacterEntry>(visibleSize);
		charList.ResetList();
		
		for (int i = 0; i < prepList.preps.Count; i++) {
			Transform t = Instantiate(charEntryPrefab, charListParent.transform);
			PrepCharacterEntry entry = charList.CreateEntry(t);
			entry.FillData(playerData.stats[prepList.preps[i].index], playerData.inventory[prepList.preps[i].index], prepList.preps[i]);
		}
		ShowCharInfo();
		charEntryPrefab.gameObject.SetActive(false);
	}

    private void GenerateInventoryList() {
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
		ShowItemInfo();
    }

	public void MoveSelection(int dir) {
		if (currentMode == State.CHAR) {
			charList.Move(dir);
			ShowCharInfo();
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
	}

	public void SelectItem() {
		if (currentMode == State.CHAR) {
			prompt.Show2Options("What do you want to do?", "TAKE", "STORE", "CANCEL", true);
			currentMode = State.MENU;
		}
		else if (currentMode == State.MENU) {
			MyPrompt.Result res = prompt.Click(true);
			if (res == MyPrompt.Result.OK1) {
				currentMode = State.TAKE;
			}
			else if (res == MyPrompt.Result.OK2) {
				currentMode = State.STORE;
				GenerateInventoryList();
			}
			else {
				currentMode = State.CHAR;
			}
		}
	}

	public bool DeselectItem() {
		if (currentMode == State.TAKE) {
			currentMode = State.CHAR;
			return false;
		}
		else if (currentMode == State.STORE) {
			GenerateList();
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

	private void ShowItemInfo() {
		PrepCharacterEntry entry = charList.GetEntry();
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
