using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCursor : InputReceiver {

	public MapClicker clicker;
	public MapInfoVariable selectedMap;
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


    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.MAP);
		clicker.cursorSprite.enabled = (active || mode != MenuMode.UNIT);
		if (!active)
			return;
		
		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			target.value = targetList.values[targetIndex.value];
			Debug.Log("Show attack or heal");
		}
		else if (currentMode.value == ActionMode.MOVE) {
			clicker.UndoMove();
		}
		cursorMovedEvent.Invoke();
    }

	public override void OnUpArrow() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			targetIndex.value = (targetIndex.value + 1) % targetList.values.Count;
			target.value = targetList.values[targetIndex.value];
		}
		else {
			cursorY.value = Mathf.Min(cursorY.value + 1, selectedMap.value.sizeY -1);
		}

		cursorMovedEvent.Invoke();
	}

	public override void OnDownArrow() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			targetIndex.value = (targetIndex.value -1 < 0) ? (targetList.values.Count-1) : targetIndex.value -1;
			target.value = targetList.values[targetIndex.value];
		}
		else {
			cursorY.value = Mathf.Max(cursorY.value -1, 0);
		}

		cursorMovedEvent.Invoke();
	}

	public override void OnLeftArrow() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			targetIndex.value = (targetIndex.value -1 < 0) ? (targetList.values.Count-1) : targetIndex.value -1;
			target.value = targetList.values[targetIndex.value];
		}
		else {
			cursorX.value = Mathf.Max(cursorX.value -1, 0);
		}
		cursorMovedEvent.Invoke();
	}

	public override void OnRightArrow() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			targetIndex.value = (targetIndex.value + 1) % targetList.values.Count;
			target.value = targetList.values[targetIndex.value];
		}
		else {
			cursorX.value = Mathf.Min(cursorX.value + 1, selectedMap.value.sizeX -1);
		}
		cursorMovedEvent.Invoke();
	}

	public override void OnOkButton() {
		if (!active)
			return;
		
		if (currentMode.value == ActionMode.ATTACK) {
			currentMenuMode.value = (int)MenuMode.ATTACK;
			Debug.Log("GO to weapons");
			StartCoroutine(MenuChangeDelay());
		}
		else if (currentMode.value == ActionMode.HEAL) {
			currentMenuMode.value = (int)MenuMode.HEAL;
			Debug.Log("GO to Staffs");
			StartCoroutine(MenuChangeDelay());
		}
		else {
			targetIndex.value = 0;
			buttonMenuPosition.value = 0;
			clicker.CursorClick();
		}
	}

	public override void OnBackButton() {
		if (!active)
			return;

		if (currentMode.value == ActionMode.ATTACK || currentMode.value == ActionMode.HEAL) {
			currentMenuMode.value = (int)MenuMode.UNIT;
			StartCoroutine(MenuChangeDelay());
			Debug.Log("ohoyo?");
		}
		clicker.CursorBack();
	}


	/// <summary>
	/// Triggered when battles end and updates the current menu mode.
	/// </summary>
	public void BattleEnd() {
		currentMenuMode.value = (int)MenuMode.MAP;
		currentMode.value = ActionMode.NONE;
		menuModeChangedEvent.Invoke();
		clicker.ResetTargets();
	}


    public override void OnSp1Button() {}
    public override void OnSp2Button() {}
    public override void OnStartButton() {}
}
