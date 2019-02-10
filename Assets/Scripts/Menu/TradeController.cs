using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeController : InputReceiver {

	[Header("References")]
	public ActionModeVariable currentMode;
	public TacticsMoveVariable selectedCharacter;
	public MapTileVariable targetTile;

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
		menuMoveEvent.Invoke();
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
		menuMoveEvent.Invoke();
		UpdateSelection();
    }

    public override void OnLeftArrow() {
		if (!active)
			return;

        if (menuPosition >= InventoryContainer.INVENTORY_SIZE) {
			menuPosition -= InventoryContainer.INVENTORY_SIZE;
			menuMoveEvent.Invoke();
			UpdateSelection();
		}
    }

    public override void OnRightArrow() {
		if (!active)
			return;

        if (menuPosition < InventoryContainer.INVENTORY_SIZE) {
			menuPosition += InventoryContainer.INVENTORY_SIZE;
			menuMoveEvent.Invoke();
			UpdateSelection();
		}
    }

    public override void OnOkButton() {
		if (!active)
			return;

		if (selectedIndex != -1) {
			SwapItems();
			menuAcceptEvent.Invoke();
		}
		else if (itemNames[menuPosition].text != "--") {
			selectedIndex = menuPosition;
			UpdateSelection();
			menuAcceptEvent.Invoke();
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
			currentMode.value = ActionMode.MOVE;
			StartCoroutine(MenuChangeDelay());
			selectedIndex = -1;
			UpdateSelection();
		}
		menuAcceptEvent.Invoke();
    }

	/// <summary>
	/// Updates the UI to show the current state of the inventories.
	/// </summary>
	private void UpdateInventories() {
		if (!active)
			return;

		TacticsMove targetCharacter = targetTile.value.currentCharacter;
		portraitLeft.sprite = selectedCharacter.value.stats.charData.bigPortrait;
		portraitRight.sprite = targetCharacter.stats.charData.bigPortrait;

		for (int i = 0, j = InventoryContainer.INVENTORY_SIZE; i < InventoryContainer.INVENTORY_SIZE; i++, j++) {
			InventoryTuple tup = selectedCharacter.value.inventory.GetTuple(i);
			itemNames[i].text = (tup.item != null) ? tup.item.entryName : "--";
			itemCharges[i].text = (tup.item != null) ? tup.charge.ToString() : "";
			
			tup = targetCharacter.inventory.GetTuple(i);
			itemNames[j].text = (tup.item != null) ? tup.item.entryName : "--";
			itemCharges[j].text = (tup.item != null && tup.item.maxCharge != 1) ? tup.charge.ToString() : "";
		}
	}

	/// <summary>
	/// Updates the current trade menu selection.
	/// </summary>
	private void UpdateSelection() {
		if (!active)
			return;

		for (int i = 0; i < slots.Count; i++) {
			slots[i].enabled = (i == menuPosition || i == selectedIndex);
			slots[i].color = (i == selectedIndex) ? selectedColor : cursorColor;
		}
	}

	/// <summary>
	/// Swaps the two items selected by menuPosition and selectedIndex and the updates the inventories.
	/// </summary>
	private void SwapItems() {
		selectedCharacter.value.canUndoMove = false;
		TacticsMove targetCharacter = targetTile.value.currentCharacter;
		InventoryTuple tup1 = (selectedIndex >= InventoryContainer.INVENTORY_SIZE) ? 
				targetCharacter.inventory.GetTuple(selectedIndex-InventoryContainer.INVENTORY_SIZE) : 
				selectedCharacter.value.inventory.GetTuple(selectedIndex);
		InventoryTuple tup2 = (menuPosition >= InventoryContainer.INVENTORY_SIZE) ? 
				targetCharacter.inventory.GetTuple(menuPosition-InventoryContainer.INVENTORY_SIZE) : 
				selectedCharacter.value.inventory.GetTuple(menuPosition);
		InventoryTuple temp = new InventoryTuple();
		temp.charge = tup1.charge;
		temp.droppable = tup1.droppable;
		temp.item = tup1.item;
		tup1.charge = tup2.charge;
		tup1.droppable = tup2.droppable;
		tup1.item = tup2.item;
		tup2.charge = temp.charge;
		tup2.droppable = temp.droppable;
		tup2.item = temp.item;

		selectedIndex = -1;
		targetCharacter.inventory.CleanupInventory(targetCharacter.stats);
		selectedCharacter.value.inventory.CleanupInventory(selectedCharacter.value.stats);
		UpdateInventories();
		UpdateSelection();
	}

    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
