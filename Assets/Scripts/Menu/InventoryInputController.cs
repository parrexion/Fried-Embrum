using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
		UpdateState(MenuMode.INV);
    }

    public override void OnDownArrow() {
		Move(1);
    }

    public override void OnUpArrow() {
		Move(-1);
    }

	private void Move(int dir) {
		//do {
		//	inventoryIndex.value = OPMath.FullLoop(0, InventoryContainer.INVENTORY_SIZE, inventoryIndex.value + dir);
		//} while (inventoryIndex.value != 0 && selectCharacter.value.inventory.GetTuple(inventoryIndex.value).item == null);
		//menuMoveEvent.Invoke();
		//ui.UpdateSelection(selectCharacter.value);
	}

    public override void OnOkButton() {
		//if (selectCharacter.value.inventory.GetTuple(inventoryIndex.value).item == null)
		//	return;

		//inventoryMenuPosition.value = -1;
		//MenuChangeDelay(MenuMode.INV);
		//menuAcceptEvent.Invoke();
		//ui.UpdateSelection(selectCharacter.value);
    }
	
    public override void OnBackButton() {
		//Debug.Log("Now with UNiT!");
		//inventoryIndex.value = -1;
		//menuBackEvent.Invoke();
		//MenuChangeDelay(MenuMode.UNIT);
    }

    public override void OnYButton() {
		//bool hidden = (selectCharacter.value == null || currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		//bool spec = (currentDialogueMode.value != (int)DialogueMode.NONE || currentMenuMode.value == (int)MenuMode.STATS || currentMenuMode.value == (int)MenuMode.INV);
		//if (!hidden && !spec) {
		//	menuMoveEvent.Invoke();
		//	ChangeStatsScreen(1);
		//}
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnXButton() { }
    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }

}
