using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InventoryStatsType { BASIC, STATS, INVENTORY }

public class InventoryInputController : InputReceiverDelegate {

	public SimpleCharacterUI ui;
	public GameObject background;

	public TacticsMoveVariable selectCharacter;
	public MapTileVariable selectTile;
	public MapTileVariable moveTile;
	public MapTileVariable targetTile;
	public ActionModeVariable currentMode;
	public IntVariable currentDialogueMode;
	public IntVariable currentPage;
	public IntVariable inventoryIndex;
	public IntVariable inventoryMenuPosition;


	private void Start() {
		currentPage.value = 0;
		inventoryIndex.value = -1;
	}

    public override void OnMenuModeChanged() {
        bool active = UpdateState(MenuMode.STATS);
		background.SetActive(active);

		UpdateUI();

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
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE || (currentMenuMode.value == (int)MenuMode.FORMATION && currentMode.value == ActionMode.MOVE)) {
			tile = targetTile.value;
			if (tile && tile.currentCharacter) {
				tactics = tile.currentCharacter;
			}
		}

		if (selectCharacter.value == null && selectTile.value.interactType != InteractType.NONE) {
			ui.ShowObjectStats(tile);
		}
		else if (currentPage.value == (int)InventoryStatsType.INVENTORY || currentMenuMode.value == (int)MenuMode.STATS || currentMenuMode.value == (int)MenuMode.INV || currentMode.value == ActionMode.TRADE) {
			ui.ShowInventoryStats(tactics);
		}
		else if (currentPage.value == (int)InventoryStatsType.STATS) {
			ui.ShowStatsStats(tactics);
		}
		else if (currentPage.value == (int)InventoryStatsType.BASIC)
			ui.ShowBasicStats(tactics);
	}

    public override void OnDownArrow() {
		Move(1);
    }

    public override void OnUpArrow() {
		Move(-1);
    }

	private void Move(int dir) {
		do {
			inventoryIndex.value = OPMath.FullLoop(0, InventoryContainer.INVENTORY_SIZE, inventoryIndex.value + dir);
		} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetTuple(inventoryIndex.value).item == null);
		menuMoveEvent.Invoke();
		ui.UpdateSelection(selectCharacter.value);
	}

    public override void OnOkButton() {
		if (selectCharacter.value.inventory.GetTuple(inventoryIndex.value).item == null)
			return;

		inventoryMenuPosition.value = -1;
		MenuChangeDelay(MenuMode.INV);
		menuAcceptEvent.Invoke();
		ui.UpdateSelection(selectCharacter.value);
    }
	
    public override void OnBackButton() {
		Debug.Log("Now with UNiT!");
		inventoryIndex.value = -1;
		menuBackEvent.Invoke();
		MenuChangeDelay(MenuMode.UNIT);
    }

    public override void OnYButton() {
		bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		bool spec = (currentDialogueMode.value != (int)DialogueMode.NONE || currentMenuMode.value == (int)MenuMode.STATS || currentMenuMode.value == (int)MenuMode.INV);
		if (!hidden && !spec) {
			menuMoveEvent.Invoke();
			ChangeStatsScreen(1);
		}
	}

	/// <summary>
	/// Changes the stats screen to the next one.
	/// </summary>
	/// <param name="dir"></param>
	private void ChangeStatsScreen(int dir) {
		int nextPage = currentPage.value + dir + 3;
		currentPage.value = nextPage % 3;
		UpdateUI();
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnXButton() { }
    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }

}
