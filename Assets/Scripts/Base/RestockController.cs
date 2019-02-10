using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestockController : MonoBehaviour {
    enum MenuState { CHARACTER, MENU, INV, PROMPT }
	private MenuState currentMode;
	public SaveListVariable playerData;
	public IntVariable totalMoney;

	[Header("Character List")]
    public Transform listParentCharacter;
	public Transform characterPrefab;
	private int currentListIndex;
	private List<RestockListEntry> entryList = new List<RestockListEntry>();

	[Header("Restock List")]
	public GameObject mainButtonView;
	public GameObject restockButtonView;
    public MyButton[] restockMenuButtons;
    public Transform listParentRestock;
	public Transform restockPrefab;
	private int currentMenuIndex;
	private int currentItemIndex;
	private List<ItemListEntry> restockList = new List<ItemListEntry>();

	[Header("Inventory box")]
	public GameObject inventoryArea;
	public TMPro.TextMeshProUGUI charName;
	public Image portrait;
	public TMPro.TextMeshProUGUI[] inventory;

	[Header("Information box")]
	public GameObject informationArea;
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
	public GameObject promptView;
	public Text promptText;
	public MyButton promptYesButton;
	public MyButton promptNoButton;
	private int promptPosition;


	private void Start() {
		restockButtonView.SetActive(false);
		inventoryArea.SetActive(false);
		informationArea.SetActive(false);
		promptView.SetActive(false);
	}

	public void GenerateLists() {
		inventoryArea.SetActive(true);
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
			promptPosition = OPMath.FullLoop(0, 2, promptPosition + dir);
		}
	}

	public void MoveSide(int dir) {

	}

	public void SelectItem() {
		if (currentMode == MenuState.CHARACTER) {
			currentMode = MenuState.MENU;
			restockButtonView.SetActive(true);
			mainButtonView.SetActive(false);
			currentMenuIndex = 0;
			GenerateInventoryList();
		}
		else if (currentMode == MenuState.MENU) {
			Debug.Log("Restock");
			currentMode = MenuState.INV;
			promptView.SetActive(true);
			promptText.text = "Restock item?";
			promptPosition = 0;
		}
		else if (currentMode == MenuState.PROMPT) {
			if (promptPosition == 0) {
				RestockItem();
			}
			currentMode = MenuState.INV;
			promptView.SetActive(false);
		}
	}

	public bool DeselectItem() {
		if (currentMode == MenuState.MENU) {
			restockButtonView.SetActive(false);
			mainButtonView.SetActive(true);
			currentMode =  MenuState.CHARACTER;
			return false;
		}
		else if (currentMode == MenuState.PROMPT) {
			if (promptPosition == 0) {
				RestockItem();
			}
			currentMode = MenuState.INV;
			promptView.SetActive(false);
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
