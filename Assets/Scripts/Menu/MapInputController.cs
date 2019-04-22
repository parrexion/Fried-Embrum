using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapInputController : InputReceiverDelegate {

	public ScrObjEntryReference currentMap;
	public ActionModeVariable currentAction;
	public IntVariable dialogueMode;
	public FactionVariable currentFaction;

	[Header("Controllers")]
	public MapCursor cursor;
	public ActionInputController actionController;
	public TargetController targetController;
	
	[Header("Events")]
	public UnityEvent changeStatsEvent;


	private bool IsTargetMode() {
		return (currentAction.value == ActionMode.ATTACK || currentAction.value == ActionMode.HEAL || currentAction.value == ActionMode.TRADE);
	}
	

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.MAP);
		actionController.ShowMenu(currentMenuMode.value == (int)MenuMode.MAP && currentAction.value == ActionMode.ACTION, false);
		if (!active)
			return;
		
		if (IsTargetMode())
			targetController.UpdateSelection();

		if (dialogueMode.value == (int)DialogueMode.VISIT)
			actionController.ReturnFromVisit();

		cursor.Move(0,0);
    }

	public override void OnUpArrow() {
		if (IsTargetMode()) {
			targetController.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {
			if (actionController.MoveVertical(-1))
				menuMoveEvent.Invoke();
		}
		else {
			if (cursor.Move(0, 1))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (IsTargetMode()) {
			targetController.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {
			if (actionController.MoveVertical(1))
				menuMoveEvent.Invoke();
		}
		else {
			if (cursor.Move(0, -1))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (IsTargetMode()) {
			targetController.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {

		}
		else {
			if (cursor.Move(-1, 0))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (IsTargetMode()) {
			targetController.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {

		}
		else {
			if (cursor.Move(1, 0))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (currentAction.value == ActionMode.ATTACK || currentAction.value == ActionMode.HEAL) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.WEAPON);
			menuAcceptEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.TRADE) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.TRADE);
			menuAcceptEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {
			actionController.OkButton();
			menuAcceptEvent.Invoke();
		}
		else {
			if (cursor.CursorClick(true))
				menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (IsTargetMode()) {
			targetController.Clear();
			currentAction.value = ActionMode.ACTION;
			actionController.ShowMenu(true, false);
			cursor.Move(0,0);
			menuBackEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.ACTION) {
			if (actionController.BackButton()) {
				currentAction.value = ActionMode.MOVE;
				cursor.UndoMove();
				actionController.ShowMenu(false, false);
				menuBackEvent.Invoke();
			}
		}
		else if (currentAction.value == ActionMode.MOVE) {
			cursor.CursorBack();
			menuBackEvent.Invoke();
		}
	}

    public override void OnXButton() {
		cursor.DangerAreaToggle(true);
	}

    public override void OnYButton() {
		changeStatsEvent.Invoke();
	}

    public override void OnRButton() {
		if (IsTargetMode())
			return;

		cursor.JumpCursor();
		menuMoveEvent.Invoke();
	}

    public override void OnStartButton() {
		if (currentAction.value == ActionMode.NONE) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.INGAME);
			menuAcceptEvent.Invoke();
		}
	}

    public override void OnLButton() {
		if (cursor.selectCharacter.value == null)
			return;
		
		InputDelegateController.instance.TriggerMenuChange(MenuMode.TOOLTIP);
	}

	public void FinishedMove() {
		currentAction.value = ActionMode.ACTION;
		actionController.ShowMenu(true, true);
	}

	/// <summary>
	/// Triggered when battles end and updates the current menu mode.
	/// </summary>
	public void BattleEnd() {
		if (currentFaction.value != Faction.PLAYER)
			return;

		actionController.selectedCharacter.value.End();
		currentAction.value = ActionMode.NONE;
		cursor.ResetTargets();
		MenuChangeDelay(MenuMode.MAP);
	}
}
