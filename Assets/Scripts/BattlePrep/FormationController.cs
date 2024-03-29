﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormationController : InputReceiverDelegate {

	public BattlePrepController prepController;
	public BattleMap battleMap;
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
		bool active = UpdateState(MenuMode.FORMATION);

		if (active) {
			battleMap.ResetMap();
			battleMap.ClearDeployment();
			cursorMovedEvent.Invoke();
		}
		
		for (int i = 0; i < map.spawnPoints1.Count; i++) {
			battleMap.GetTile(map.spawnPoints1[i]).deployable = (active) ? 1 : 0;
		}
		for (int i = 0; i < map.spawnPoints2.Count; i++) {
			battleMap.GetTile(map.spawnPoints2[i]).deployable = (active) ? 2 : 0;
		}
    }

	public override void OnUpArrow() {
		int prev = cursorY.value;
		cursorY.value = Mathf.Min(cursorY.value + 1, map.sizeY -1);
		if (prev != cursorY.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnDownArrow() {
		int prev = cursorY.value;
		cursorY.value = Mathf.Max(cursorY.value -1, 0);
		if (prev != cursorY.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnLeftArrow() {
		int prev = cursorX.value;
		cursorX.value = Mathf.Max(cursorX.value -1, 0);
		if (prev != cursorX.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnRightArrow() {
		int prev = cursorX.value;
		cursorX.value = Mathf.Min(cursorX.value + 1, map.sizeX -1);
		if (prev != cursorX.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnOkButton() {
		targetIndex.value = 0;
		actionMenuPosition.value = -1;
		bool res = clicker.CursorClick(false);
		if (!res)
			return;

		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		if (currentMode.value == ActionMode.MOVE) {
			clicker.CursorBack();
			menuBackEvent.Invoke();
		}
		else {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.PREP);
			menuBackEvent.Invoke();
		}
	}

    public override void OnXButton() {
		clicker.DangerAreaToggle(true);
		menuAcceptEvent.Invoke();
	}

    public override void OnRButton() {
		clicker.JumpCursor();

		cursorMovedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

    public override void OnStartButton() {}

    public override void OnLButton() {
		if (!clicker.selectCharacter.value)
			return;
		
		MenuChangeDelay(MenuMode.TOOLTIP);
		menuAcceptEvent.Invoke();
	}

    public override void OnYButton() { }
}
