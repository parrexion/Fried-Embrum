using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockController : MonoBehaviour {
    enum MenuState { CHARACTER, MENU, INV, PROMPT }
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
	private int currentListIndex;
	private List<RestockListEntry> entryList = new List<RestockListEntry>();

	[Header("Restock List")]
    public MyButton[] restockMenuButtons;
    public Transform listParentRestock;
	public Transform restockPrefab;
	private int currentMenuIndex;
	private int currentItemIndex;
	private List<ItemListEntry> restockList = new List<ItemListEntry>();

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
	}

	public void GenerateLists() {
		charListView.SetActive(true);
		GenerateCharacterList();
		currentListIndex = 0;
		currentItemIndex = 0;
		currentMenuIndex = 0;
		MoveSelection(0);
	}

    private void GenerateCharacterList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;
        for (int i = listParentCharacter.childCount - 1; i > 2; i--) {
            Destroy(listParentCharacter.GetChild(i).gameObject);
        }

        entryList = new List<RestockListEntry>();

        for (int i = 0; i < playerData.stats.Count; i++) {
			Transform t = Instantiate(characterPrefab, listParentCharacter);

			RestockListEntry entry = t.GetComponent<RestockListEntry>();
			entry.FillData(playerData.stats[i], playerData.inventory[i]);
			entryList.Add(entry);

			t.gameObject.SetActive(true);
        }
        characterPrefab.gameObject.SetActive(false);
    }

    private void GenerateInventoryList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;
        for (int i = listParentRestock.childCount - 1; i > 2; i--) {
            Destroy(listParentRestock.GetChild(i).gameObject);
        }

        restockList = new List<ItemListEntry>();

        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = entryList[i].invCon.GetTuple(i);
			if (!tuple.item)
				break;
			Transform t = Instantiate(restockPrefab, listParentRestock);

			ItemListEntry entry = t.GetComponent<ItemListEntry>();
			entry.FillData(i, tuple.item, tuple.charge, 9999, false, 1);
			restockList.Add(entry);

			t.gameObject.SetActive(true);
        }
        characterPrefab.gameObject.SetActive(false);
    }

	public void MoveSelection(int dir) {
		if (currentMode == MenuState.CHARACTER) {
			currentListIndex = OPMath.FullLoop(0, playerData.stats.Count, currentListIndex + dir);
			for (int i = 0; i < playerData.stats.Count; i++) {
				entryList[i].SetHighlight(currentListIndex == i);
			}
            SetupCharInfo();
        }
		else if (currentMode == MenuState.MENU) {
			currentMenuIndex = OPMath.FullLoop(0, restockMenuButtons.Length, currentMenuIndex + dir);
			for (int i = 0; i < restockMenuButtons.Length; i++) {
				restockMenuButtons[i].SetSelected(currentMenuIndex == i);
			}
			SetupItemInfo();
		}
		else if (currentMode == MenuState.INV) {
			currentMenuIndex = OPMath.FullLoop(0, restockList.Count, currentMenuIndex + dir);
			for (int i = 0; i < restockList.Count; i++) {
				restockList[i].SetHighlight(currentMenuIndex == i);
			}
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
			currentMenuIndex = 0;
			GenerateInventoryList();
		}
		else if (currentMode == MenuState.MENU) {
			Debug.Log("Restock");
			currentMode = MenuState.INV;
			restockPrompt.ShowWindow("Restock item?", true);
		}
		else if (currentMode == MenuState.PROMPT) {
			if (restockPrompt.Click(true)) {
				RestockItem();
			}
			currentMode = MenuState.INV;
		}
	}

	public bool DeselectItem() {
		if (currentMode == MenuState.MENU) {
			charMenuView.SetActive(false);
			currentMode =  MenuState.CHARACTER;
			return false;
		}
		else if (currentMode == MenuState.PROMPT) {
			restockPrompt.Click(false);
			currentMode = MenuState.INV;
			return false;
		}
		return true;
	}

	private void RestockItem() {

	}

	private void SetupCharInfo() {
		RestockListEntry restock = entryList[currentListIndex];
		charName.text = restock.entryName.text;
		portrait.sprite = restock.portrait.sprite;
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			ItemEntry item = restock.invCon.GetTuple(i).item;
			inventory[i].text = (item) ? item.entryName : "-NONE-";
		}
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

		ItemEntry item = entryList[currentListIndex].invCon.GetTuple(currentItemIndex).item;
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
