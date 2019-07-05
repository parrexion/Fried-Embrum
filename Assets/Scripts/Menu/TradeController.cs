using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeController : InputReceiverDelegate {

	[Header("References")]
	public ActionModeVariable currentMode;
	public TacticsMoveVariable selectedCharacter;
	public MapTileVariable targetTile;
	public GameObject statsObject;

	[Header("Trade Windows")]
	public GameObject tradeWindowLeft;
	public GameObject tradeWindowRight;
	public Image portraitLeft;
	public Image portraitRight;
	public Color selectedColor;
	public Color cursorColor;

	[Header("Lists")]
	public List<InventoryListEntry> slots = new List<InventoryListEntry>();

	private int menuPosition = 0;
	private int selectedIndex = -1;


	private void Start() {
		tradeWindowLeft.SetActive(false);
		tradeWindowRight.SetActive(false);
	}


	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.TRADE);
		tradeWindowLeft.SetActive(active);
		tradeWindowRight.SetActive(active);
		if (!active)
			return;

		menuPosition = 0;
		selectedIndex = -1;
		statsObject.SetActive(false);

		UpdateInventories();
		UpdateSelection();
	}

	public override void OnDownArrow() {
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
		menuMoveEvent.Invoke();
	}

	public override void OnUpArrow() {
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
		menuMoveEvent.Invoke();
	}

	public override void OnLeftArrow() {
		if (menuPosition >= InventoryContainer.INVENTORY_SIZE) {
			menuPosition -= InventoryContainer.INVENTORY_SIZE;
			UpdateSelection();
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (menuPosition < InventoryContainer.INVENTORY_SIZE) {
			menuPosition += InventoryContainer.INVENTORY_SIZE;
			UpdateSelection();
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (selectedIndex != -1) {
			SwapItems();
			menuAcceptEvent.Invoke();
		}
		else if (slots[menuPosition].entryName.text != "--") {
			selectedIndex = menuPosition;
			UpdateSelection();
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (selectedIndex != -1) {
			selectedIndex = -1;
			UpdateSelection();
		}
		else {
			currentMode.value = ActionMode.ACTION;
			selectedIndex = -1;
			statsObject.SetActive(true);
			MenuChangeDelay(MenuMode.MAP);
		}
		menuBackEvent.Invoke();
	}

	/// <summary>
	/// Updates the UI to show the current state of the inventories.
	/// </summary>
	private void UpdateInventories() {
		TacticsMove targetCharacter = targetTile.value.currentCharacter;
		portraitLeft.sprite = selectedCharacter.value.stats.charData.bigPortrait;
		portraitRight.sprite = targetCharacter.stats.charData.bigPortrait;

		for (int i = 0, j = InventoryContainer.INVENTORY_SIZE; i < InventoryContainer.INVENTORY_SIZE; i++, j++) {
			slots[i].FillData(i, selectedCharacter.value.inventory.GetTuple(i));
			slots[j].FillData(j, targetCharacter.inventory.GetTuple(i));
		}
	}

	/// <summary>
	/// Updates the current trade menu selection.
	/// </summary>
	private void UpdateSelection() {
		for (int i = 0; i < slots.Count; i++) {
			slots[i].SetHighlight(i == menuPosition || i == selectedIndex);
			slots[i].highlight.color = (i == selectedIndex) ? selectedColor : cursorColor;
		}
	}

	/// <summary>
	/// Swaps the two items selected by menuPosition and selectedIndex and the updates the inventories.
	/// </summary>
	private void SwapItems() {
		TacticsMove leftCharacter = null;
		TacticsMove rightCharacter = null;

		int leftIndex = 0;
		int rightIndex = 0;

		InventoryTuple tupLeft = null;
		InventoryTuple tupRight = null;

		leftCharacter = (selectedIndex >= InventoryContainer.INVENTORY_SIZE) ? targetTile.value.currentCharacter : selectedCharacter.value;
		rightCharacter = (menuPosition >= InventoryContainer.INVENTORY_SIZE) ? targetTile.value.currentCharacter : selectedCharacter.value;
		leftIndex = (selectedIndex >= InventoryContainer.INVENTORY_SIZE) ? selectedIndex - InventoryContainer.INVENTORY_SIZE : selectedIndex;
		rightIndex = (menuPosition >= InventoryContainer.INVENTORY_SIZE) ? menuPosition - InventoryContainer.INVENTORY_SIZE : menuPosition;

		tupLeft = leftCharacter.inventory.GetTuple(leftIndex);
		tupRight = rightCharacter.inventory.GetTuple(rightIndex);
		
		if (leftCharacter != rightCharacter) {
			if (string.IsNullOrEmpty(tupLeft.uuid) && rightIndex == 0 && !rightCharacter.inventory.CanEquip(tupLeft)) {
				return;
			}
			if (string.IsNullOrEmpty(tupRight.uuid) && leftIndex == 0 && !leftCharacter.inventory.CanEquip(tupRight)) {
				return;
			}
		}

		//InventoryTuple temp = new InventoryTuple();
		//temp.Charge = tupLeft.Charge;
		//temp.droppable = tupLeft.droppable;
		//temp.item = tupLeft.item;
		//tupLeft.Charge = tupRight.Charge;
		//tupLeft.droppable = tupRight.droppable;
		//tupLeft.item = tupRight.item;
		//tupRight.Charge = temp.Charge;
		//tupRight.droppable = temp.droppable;
		//tupRight.item = temp.item;
		
		InventoryTuple temp = tupLeft;
		tupLeft = tupRight;
		tupRight = temp;
		
		if (leftCharacter == rightCharacter)
			selectedCharacter.value.canUndoMove = false;
		selectedIndex = -1;
		leftCharacter.inventory.CleanupInventory();
		rightCharacter.inventory.CleanupInventory();
		UpdateInventories();
		UpdateSelection();
	}

	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
	public override void OnStartButton() { }
}
