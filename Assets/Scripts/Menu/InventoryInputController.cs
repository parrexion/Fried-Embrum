using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryInputController : InputReceiver {

	public enum StatsType { BASIC, STATS, INVENTORY }

	public SimpleCharacterUI ui;
	public GameObject background;

	public TacticsMoveVariable selectCharacter;
	public TacticsMoveVariable targetCharacter;
	public ActionModeVariable currentMode;
	public IntVariable currentPage;
	public IntVariable inventoryIndex;
	public IntVariable inventoryMenuPosition;
	
	public UnityEvent hideTooltipEvent;


	private void Start() {
		currentPage.value = 0;
		inventoryIndex.value = -1;
	}

    public override void OnMenuModeChanged() {
        if (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL) {
			active = false;
			background.SetActive(false);
			hideTooltipEvent.Invoke();
		}
		else {
			active = (currentMenuMode.value == (int)MenuMode.STATS);
			background.SetActive(true);
			UpdateUI();
		}
    }

	/// <summary>
	/// Updates the information in the UI whenever the state or character changes.
	/// </summary>
	public void UpdateUI() {
		TacticsMove tactics = (currentMode.value != ActionMode.ATTACK && currentMode.value != ActionMode.HEAL && currentMode.value != ActionMode.TRADE) ? selectCharacter.value : targetCharacter.value;

		if (currentPage.value == (int)StatsType.INVENTORY || currentMenuMode.value == (int)MenuMode.STATS || currentMenuMode.value == (int)MenuMode.INV || currentMode.value == ActionMode.TRADE) {
			ui.ShowInventoryStats(tactics);
		}
		else if (currentPage.value == (int)StatsType.STATS) {
			ui.ShowStatsStats(tactics);
		}
		else if (currentPage.value == (int)StatsType.BASIC)
			ui.ShowBasicStats(tactics);
	}

    public override void OnDownArrow() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS) {
			do {
				inventoryIndex.value++;
				if (inventoryIndex.value >= InventoryContainer.INVENTORY_SIZE)
					inventoryIndex.value = 0;
				ui.UpdateSelection();
			} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item == null);
		}
		
    }

    public override void OnUpArrow() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS) {
			do {
				inventoryIndex.value--;
				if (inventoryIndex.value < 0)
					inventoryIndex.value = InventoryContainer.INVENTORY_SIZE -1;
				ui.UpdateSelection();
			} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item == null);
		}
    }

    public override void OnOkButton() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item != null) {
			Debug.Log("That's a thing");
			currentMenuMode.value = (int)MenuMode.INV;
			inventoryMenuPosition.value = -1;
			StartCoroutine(MenuChangeDelay());
			ui.UpdateSelection();
		}
    }
	
    public override void OnBackButton() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS){
			Debug.Log("Don't look at that!");
			currentMenuMode.value = (int)MenuMode.UNIT;
			inventoryIndex.value = -1;
			StartCoroutine(MenuChangeDelay());
		}
    }

    public override void OnSp1Button() {
		bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		if (!hidden && currentMenuMode.value != (int)MenuMode.STATS && currentMenuMode.value != (int)MenuMode.INV)
			ChangeStatsScreen(-1);
    }

    public override void OnSp2Button() {
		bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		if (!hidden && currentMenuMode.value != (int)MenuMode.STATS && currentMenuMode.value != (int)MenuMode.INV)
			ChangeStatsScreen(1);
	}

	/// <summary>
	/// Changes the stats screen to the next one.
	/// </summary>
	/// <param name="dir"></param>
	private void ChangeStatsScreen(int dir) {
		int nextPage = (int) currentPage.value + dir;
		if (nextPage < 0)
			nextPage = nextPage + 3;
		
		currentPage.value = nextPage % 3;
		hideTooltipEvent.Invoke();
		UpdateUI();
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnStartButton() { }

}
