using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormationController : InputReceiver {

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
		active = ((MenuMode)currentMenuMode.value == MenuMode.FORMATION);
		clicker.cursorSprite.enabled = active;
		if (!active)
			return;
		
		battleMap.ResetMap();
		battleMap.ClearDeployment();
		for (int i = 0; i < map.spawnPoints.Count; i++) {
			if (map.spawnPoints[i].stats != null)
				continue;

			battleMap.GetTile(map.spawnPoints[i].x, map.spawnPoints[i].y).deployable = true;
		}
		cursorMovedEvent.Invoke();
    }

	public override void OnUpArrow() {
		if (!active)
			return;

		int prev = cursorY.value;
		cursorY.value = Mathf.Min(cursorY.value + 1, map.sizeY -1);
		if (prev != cursorY.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
	}

	public override void OnDownArrow() {
		if (!active)
			return;

		int prev = cursorY.value;
		cursorY.value = Mathf.Max(cursorY.value -1, 0);
		if (prev != cursorY.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
	}

	public override void OnLeftArrow() {
		if (!active)
			return;

		int prev = cursorX.value;
		cursorX.value = Mathf.Max(cursorX.value -1, 0);
		if (prev != cursorX.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
	}

	public override void OnRightArrow() {
		if (!active)
			return;

		int prev = cursorX.value;
		cursorX.value = Mathf.Min(cursorX.value + 1, map.sizeX -1);
		if (prev != cursorX.value)
			menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
	}

	public override void OnOkButton() {
		if (!active)
			return;
		
		targetIndex.value = 0;
		actionMenuPosition.value = -1;
		bool res = clicker.CursorClick(false);
		if (!res)
			return;

		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.MOVE) {
			clicker.CursorBack();
		}
		else {
			currentMenuMode.value = (int)MenuMode.PREP;
			menuModeChangedEvent.Invoke();
		}
	}

    public override void OnXButton() {
		if (!active)
			return;

		clicker.DangerAreaToggle(true);
	}

    public override void OnRButton() {
		if (!active)
			return;

		clicker.JumpCursor();
		menuMoveEvent.Invoke();

		cursorMovedEvent.Invoke();
	}

    public override void OnStartButton() {
		if (!active)
			return;

		if (clicker.ShowIngameMenu())
			menuAcceptEvent.Invoke();
	}

    public override void OnLButton() {
		if (!active || clicker.selectCharacter.value == null)
			return;

		currentMenuMode.value = (int)MenuMode.TOOL;
		StartCoroutine(MenuChangeDelay());
	}


	/// <summary>
	/// Shows the in-game menu with end turn and options.
	/// </summary>
	public void ShowIngameMenu() {
		currentMenuMode.value = (int)MenuMode.PREP;
		StartCoroutine(MenuChangeDelay());
	}

    public override void OnYButton() { }
}
