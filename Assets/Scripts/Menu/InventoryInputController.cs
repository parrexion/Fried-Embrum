using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryInputController : InputReceiver {

	public enum StatsType { BASIC, STATS, INVENTORY }

	public SimpleCharacterUI ui;
	public GameObject background;

	public TacticsMoveVariable selectCharacter;
	public MapTileVariable selectTile;
	public MapTileVariable moveTile;
	public MapTileVariable targetTile;
	public ActionModeVariable currentMode;
	public IntVariable currentPage;
	public IntVariable inventoryIndex;
	public IntVariable inventoryMenuPosition;


	private void Start() {
		currentPage.value = 0;
		inventoryIndex.value = -1;
	}

    public override void OnMenuModeChanged() {
        if (currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL
					|| (selectCharacter.value == null && selectTile.value.interactType == InteractType.NONE)) {
			active = false;
			background.SetActive(false);
		}
		else {
			active = (currentMenuMode.value == (int)MenuMode.STATS);
			background.SetActive(true);
			UpdateUI();
		}
		
		if (currentMenuMode.value == (int)MenuMode.MAP) {
			if (currentMode.value == ActionMode.MOVE) {
				ui.ShowTerrainInfo(moveTile.value.terrain, true);
			}
			else if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
				ui.ShowTerrainInfo(targetTile.value.terrain, true);
			}
			else {
				ui.ShowTerrainInfo(selectTile.value.terrain, true);
			}
		}
		else {
			ui.ShowTerrainInfo(null, false);
		}
    }

	/// <summary>
	/// Updates the information in the UI whenever the state or character changes.
	/// </summary>
	public void UpdateUI() {
		TacticsMove tactics = selectCharacter.value;
		MapTile tile = selectTile.value;
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			tile = targetTile.value;
			if (targetTile.value != null)
				tactics = targetTile.value.currentCharacter;
		}

		if (selectCharacter.value == null && selectTile.value.interactType != InteractType.NONE) {
			ui.ShowObjectStats(tile);
		}
		else if (currentPage.value == (int)StatsType.INVENTORY || currentMenuMode.value == (int)MenuMode.STATS || currentMenuMode.value == (int)MenuMode.INV || currentMode.value == ActionMode.TRADE) {
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
			} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item == null);
			menuMoveEvent.Invoke();
			ui.UpdateSelection(selectCharacter.value);
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
			} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item == null);
			menuMoveEvent.Invoke();
			ui.UpdateSelection(selectCharacter.value);
		}
    }

    public override void OnOkButton() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS && selectCharacter.value.inventory.GetItem(inventoryIndex.value).item != null) {
			currentMenuMode.value = (int)MenuMode.INV;
			inventoryMenuPosition.value = -1;
			StartCoroutine(MenuChangeDelay());
			menuAcceptEvent.Invoke();
			ui.UpdateSelection(selectCharacter.value);
		}
    }
	
    public override void OnBackButton() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.STATS){
			currentMenuMode.value = (int)MenuMode.UNIT;
			inventoryIndex.value = -1;
			menuBackEvent.Invoke();
			StartCoroutine(MenuChangeDelay());
		}
    }

    public override void OnSp1Button() {
		bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		if (!hidden && currentMenuMode.value != (int)MenuMode.STATS && currentMenuMode.value != (int)MenuMode.INV) {
			menuMoveEvent.Invoke();
			ChangeStatsScreen(-1);
		}
    }

    public override void OnSp2Button() {
		bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		if (!hidden && currentMenuMode.value != (int)MenuMode.STATS && currentMenuMode.value != (int)MenuMode.INV) {
			menuMoveEvent.Invoke();
			ChangeStatsScreen(1);
		}
	}

	/// <summary>
	/// Changes the stats screen to the next one.
	/// </summary>
	/// <param name="dir"></param>
	private void ChangeStatsScreen(int dir) {
		int nextPage = (int) currentPage.value + dir + 3;
		// if (nextPage < 0)
		// 	nextPage = nextPage + 3;
		
		currentPage.value = nextPage % 3;
		UpdateUI();
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnStartButton() { }

}
