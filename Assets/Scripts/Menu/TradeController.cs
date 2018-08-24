using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeController : InputReceiver {

	[Header("References")]
	public TacticsMoveVariable selectedCharacter;
	public TacticsMoveVariable targetCharacter;

	[Header("Trade Windows")]
	public GameObject tradeWindowLeft;
	public GameObject tradeWindowRight;
	public Image portraitLeft;
	public Image portraitRight;
	public Color selectedColor;
	public Color cursorColor;

	[Header("Lists")]
	public List<Image> slots = new List<Image>();
	// public List<Image> itemIcons = new List<Image>();
	public List<Text> itemNames = new List<Text>();
	public List<Text> itemCharges = new List<Text>();

	private int menuPosition = 0;
	private int selectedIndex = -1;


	private void Start() {
		tradeWindowLeft.SetActive(false);
		tradeWindowRight.SetActive(false);
	}


    public override void OnMenuModeChanged() {
        MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.TRADE);
		tradeWindowLeft.SetActive(active);
		tradeWindowRight.SetActive(active);

		menuPosition = 0;
		selectedIndex = -1;

		UpdateInventories();
		UpdateSelection();
    }

    public override void OnDownArrow() {
		if (!active)
			return;

        if (menuPosition < InventoryContainer.INVENTORY_SIZE) {
			menuPosition++;
			if (menuPosition >= InventoryContainer.INVENTORY_SIZE)
				menuPosition -= InventoryContainer.INVENTORY_SIZE;
		}
		else {
			menuPosition++;
			if (menuPosition >= 2 * InventoryContainer.INVENTORY_SIZE)
				menuPosition -= InventoryContainer.INVENTORY_SIZE;
		}
		UpdateSelection();
    }

    public override void OnUpArrow() {
		if (!active)
			return;

        if (menuPosition < InventoryContainer.INVENTORY_SIZE) {
			menuPosition--;
			if (menuPosition < 0)
				menuPosition += InventoryContainer.INVENTORY_SIZE;
		}
		else {
			menuPosition--;
			if (menuPosition < InventoryContainer.INVENTORY_SIZE)
				menuPosition += InventoryContainer.INVENTORY_SIZE;
		}
		UpdateSelection();
    }

    public override void OnLeftArrow() {
		if (!active)
			return;

        if (menuPosition >= InventoryContainer.INVENTORY_SIZE)
			menuPosition -= InventoryContainer.INVENTORY_SIZE;
		UpdateSelection();
    }

    public override void OnRightArrow() {
		if (!active)
			return;

        if (menuPosition < InventoryContainer.INVENTORY_SIZE)
			menuPosition += InventoryContainer.INVENTORY_SIZE;
		UpdateSelection();
    }

    public override void OnOkButton() {
		if (!active)
			return;

		if (selectedIndex != -1) {
			SwapItems();
		}
		else if (itemNames[menuPosition].text != "--") {
			selectedIndex = menuPosition;
			UpdateSelection();
		}
    }

    public override void OnBackButton() {
		if (!active)
			return;

        if (selectedIndex != -1) {
			selectedIndex = -1;
			UpdateSelection();
		}
		else {
			active = false;
			currentMenuMode.value = (int)MenuMode.UNIT;
			StartCoroutine(MenuChangeDelay());
		}
    }

	private void UpdateInventories() {
		if (!active)
			return;

		portraitLeft.sprite = selectedCharacter.value.stats.charData.bigPortrait;
		portraitRight.sprite = targetCharacter.value.stats.charData.bigPortrait;

		for (int i = 0, j = InventoryContainer.INVENTORY_SIZE; i < InventoryContainer.INVENTORY_SIZE; i++, j++) {
			InventoryTuple tup = selectedCharacter.value.inventory.GetItem(i);
			itemNames[i].text = (tup.item != null) ? tup.item.name : "--";
			itemCharges[i].text = (tup.item != null) ? tup.charge.ToString() : "";
			
			tup = targetCharacter.value.inventory.GetItem(i);
			itemNames[j].text = (tup.item != null) ? tup.item.name : "--";
			itemCharges[j].text = (tup.item != null && tup.item.maxCharge != 1) ? tup.charge.ToString() : "";
		}
	}

	private void UpdateSelection() {
		if (!active)
			return;

		for (int i = 0; i < slots.Count; i++) {
			slots[i].enabled = (i == menuPosition || i == selectedIndex);
			slots[i].color = (i == selectedIndex) ? selectedColor : cursorColor;
		}
	}

	private void SwapItems() {
		selectedCharacter.value.canUndoMove = false;
		InventoryTuple tup1 = (selectedIndex > InventoryContainer.INVENTORY_SIZE) ? 
				targetCharacter.value.inventory.GetItem(selectedIndex-InventoryContainer.INVENTORY_SIZE) : 
				selectedCharacter.value.inventory.GetItem(selectedIndex);
		InventoryTuple tup2 = (menuPosition > InventoryContainer.INVENTORY_SIZE) ? 
				targetCharacter.value.inventory.GetItem(menuPosition-InventoryContainer.INVENTORY_SIZE) : 
				selectedCharacter.value.inventory.GetItem(menuPosition);

		InventoryTuple temp = new InventoryTuple();
		temp.charge = tup1.charge;
		temp.droppable = tup1.droppable;
		temp.index = tup1.index;
		temp.item = tup1.item;
		tup1.charge = tup2.charge;
		tup1.droppable = tup2.droppable;
		tup1.index = tup2.index;
		tup1.item = tup2.item;
		tup2.charge = temp.charge;
		tup2.droppable = temp.droppable;
		tup2.index = temp.index;
		tup2.item = temp.item;

		selectedIndex = -1;
		UpdateInventories();
		UpdateSelection();
	}

    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}
