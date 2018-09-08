using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapInputController : InputReceiver {

	public MapCursor clicker;
	public ScrObjEntryReference currentMap;
	public ActionModeVariable currentMode;

	[Header("Target")]
	public CharacterListVariable targetList;
	public TacticsMoveVariable target;

	[Header("Cursor")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public IntVariable targetIndex;
	public IntVariable buttonMenuPosition;

	[Header("Events")]
	public UnityEvent cursorMovedEvent;

	private MapEntry map;


	private void Start() {
		map = (MapEntry)currentMap.value;
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.MAP);
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
		if (!active)
			return;

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
		if (!active)
			return;

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
		if (!active)
			return;

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
		if (!active)
			return;

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
		if (!active)
			return;
		
		if (currentMode.value == ActionMode.ATTACK) {
			currentMenuMode.value = (int)MenuMode.ATTACK;
			StartCoroutine(MenuChangeDelay());
		}
		else if (currentMode.value == ActionMode.HEAL) {
			currentMenuMode.value = (int)MenuMode.HEAL;
			StartCoroutine(MenuChangeDelay());
		}
		else if (currentMode.value == ActionMode.TRADE) {
			currentMenuMode.value = (int)MenuMode.TRADE;
			StartCoroutine(MenuChangeDelay());
		}
		else {
			targetIndex.value = 0;
			buttonMenuPosition.value = -1;
			clicker.CursorClick();
		}
		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL || currentMode.value == ActionMode.TRADE) {
			currentMenuMode.value = (int)MenuMode.UNIT;
			menuBackEvent.Invoke();
			StartCoroutine(MenuChangeDelay());
		}
		else if (currentMode.value == ActionMode.MOVE) {
			clicker.CursorBack();
			menuBackEvent.Invoke();
		}
	}


	/// <summary>
	/// Triggered when battles end and updates the current menu mode.
	/// </summary>
	public void BattleEnd() {
		currentMenuMode.value = (int)MenuMode.MAP;
		currentMode.value = ActionMode.NONE;
		clicker.ResetTargets();
		menuModeChangedEvent.Invoke();
	}

	/// <summary>
	/// Shows the in-game menu with end turn and options.
	/// </summary>
	public void ShowIngameMenu() {
		currentMenuMode.value = (int)MenuMode.INGAME;
		StartCoroutine(MenuChangeDelay());
	}

    public override void OnSp1Button() {}
    public override void OnSp2Button() {}
    public override void OnStartButton() {}
}
