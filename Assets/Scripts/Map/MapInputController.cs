using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapInputController : InputReceiverDelegate {

	public MapCursor clicker;
	public ScrObjEntryReference currentMap;
	public ActionModeVariable currentMode;

	[Header("Target")]
	public MapTileListVariable targetList;
	public MapTileVariable target;

	[Header("Cursor")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public IntVariable targetIndex;
	public IntVariable actionMenuPosition;

	[Header("Events")]
	public UnityEvent cursorMovedEvent;

	private MapEntry map;


	private void Start() {
		map = (MapEntry)currentMap.value;
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		bool active = UpdateState(MenuMode.MAP);
		clicker.cursorSprite.enabled = (active || mode != MenuMode.UNIT);
		if (!active)
			return;
		
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			target.value = targetList.values[targetIndex.value];
		}
		else if (currentMode.value == ActionMode.MOVE) {
			clicker.UndoMove();
		}
		cursorMovedEvent.Invoke();
    }

	public override void OnUpArrow() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			int prev = targetIndex.value;
			targetIndex.value = (targetIndex.value + 1) % targetList.values.Count;
			target.value = targetList.values[targetIndex.value];
			if (prev != targetIndex.value)
				menuMoveEvent.Invoke();
		}
		else {
			int prev = cursorY.value;
			cursorY.value = Mathf.Min(cursorY.value + 1, map.sizeY -1);
			if (prev != cursorY.value)
				menuMoveEvent.Invoke();
		}

		cursorMovedEvent.Invoke();
	}

	public override void OnDownArrow() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			int prev = targetIndex.value;
			targetIndex.value = (targetIndex.value -1 < 0) ? (targetList.values.Count-1) : targetIndex.value -1;
			target.value = targetList.values[targetIndex.value];
			if (prev != targetIndex.value)
				menuMoveEvent.Invoke();
		}
		else {
			int prev = cursorY.value;
			cursorY.value = Mathf.Max(cursorY.value -1, 0);
			if (prev != cursorY.value)
				menuMoveEvent.Invoke();
		}

		cursorMovedEvent.Invoke();
	}

	public override void OnLeftArrow() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			int prev = targetIndex.value;
			targetIndex.value = (targetIndex.value -1 < 0) ? (targetList.values.Count-1) : targetIndex.value -1;
			target.value = targetList.values[targetIndex.value];
			if (prev != targetIndex.value)
				menuMoveEvent.Invoke();
		}
		else {
			int prev = cursorX.value;
			cursorX.value = Mathf.Max(cursorX.value -1, 0);
			if (prev != cursorX.value)
				menuMoveEvent.Invoke();
		}
		cursorMovedEvent.Invoke();
	}

	public override void OnRightArrow() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			int prev = targetIndex.value;
			targetIndex.value = (targetIndex.value + 1) % targetList.values.Count;
			target.value = targetList.values[targetIndex.value];
			if (prev != targetIndex.value)
				menuMoveEvent.Invoke();
		}
		else {
			int prev = cursorX.value;
			cursorX.value = Mathf.Min(cursorX.value + 1, map.sizeX -1);
			if (prev != cursorX.value)
				menuMoveEvent.Invoke();
		}
		cursorMovedEvent.Invoke();
	}

	public override void OnOkButton() {
		if (currentMode.value == ActionMode.ATTACK) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.ATTACK);
		}
		else if (currentMode.value == ActionMode.HEAL) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.HEAL);
		}
		else if (currentMode.value == ActionMode.TRADE) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.TRADE);
		}
		else {
			targetIndex.value = 0;
			actionMenuPosition.value = -1;
			bool res = clicker.CursorClick(true);
			if (!res)
				return;
		}
		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			currentMode.value = ActionMode.MOVE;
			menuBackEvent.Invoke();
			target.value = null;
			InputDelegateController.instance.TriggerMenuChange(MenuMode.UNIT);
		}
		else if (currentMode.value == ActionMode.MOVE) {
			clicker.CursorBack();
			menuBackEvent.Invoke();
		}
	}

    public override void OnXButton() {
		clicker.DangerAreaToggle(true);
	}

    public override void OnRButton() {
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			int prev = targetIndex.value;
			targetIndex.value = (targetIndex.value + 1) % targetList.values.Count;
			target.value = targetList.values[targetIndex.value];
			if (prev != targetIndex.value)
				menuMoveEvent.Invoke();
		}
		else {
			clicker.JumpCursor();
			menuMoveEvent.Invoke();
		}
		cursorMovedEvent.Invoke();
	}

    public override void OnStartButton() {
		if (clicker.ShowIngameMenu())
			menuAcceptEvent.Invoke();
	}

    public override void OnLButton() {
		if (clicker.selectCharacter.value == null)
			return;
		
		InputDelegateController.instance.TriggerMenuChange(MenuMode.TOOL);
	}

	/// <summary>
	/// Triggered when battles end and updates the current menu mode.
	/// </summary>
	public void BattleEnd() {
		currentMode.value = ActionMode.NONE;
		clicker.ResetTargets();
		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
	}

	/// <summary>
	/// Shows the in-game menu with end turn and options.
	/// </summary>
	public void ShowIngameMenu() {
		InputDelegateController.instance.TriggerMenuChange(MenuMode.INGAME);
	}

    public override void OnYButton() { }
}
