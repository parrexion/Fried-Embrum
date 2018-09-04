﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInputController : InputReceiver {

	[Header("References")]
	public TacticsMoveVariable selectedCharacter;
	public ActionModeVariable currentActionMode;
	public CharacterListVariable targetList;
	public IntVariable inventoryIndex;

	[Header("Unit Action Menu")]
	public GameObject actionMenu;
	public IntVariable actionMenuPosition;
	private Image[] actionButtons= new Image[0];


	private void Start() {
		actionButtons = actionMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.UNIT);
		actionMenu.SetActive(mode == MenuMode.UNIT);
		ButtonSetup();
    }

    public override void OnDownArrow() {
		if (!active)
			return;
		
		do {
			actionMenuPosition.value++;
			if (actionMenuPosition.value >= actionButtons.Length)
				actionMenuPosition.value = 0;
		} while (!actionButtons[actionMenuPosition.value].gameObject.activeSelf);
		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnUpArrow() {
		if (!active)
			return;

		do {
			actionMenuPosition.value--;
			if (actionMenuPosition.value < 0)
				actionMenuPosition.value += actionButtons.Length;
		} while (!actionButtons[actionMenuPosition.value].gameObject.activeSelf);
		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnBackButton() {
		if (!active)
			return;
		
		if (selectedCharacter.value.canUndoMove) {
			currentMenuMode.value = (int)MenuMode.MAP;
			currentActionMode.value = ActionMode.MOVE;
			StartCoroutine(MenuChangeDelay());
			menuBackEvent.Invoke();
		}
    }

    public override void OnOkButton() {
		if (!active)
			return;

		switch (actionMenuPosition.value)
		{
			case 0: // ATTACK
				// Debug.Log("Attack!");
				targetList.values = selectedCharacter.value.GetEnemiesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.ATTACK;
				StartCoroutine(MenuChangeDelay());
				break;
			case 1: // HEAL
				// Debug.Log("Heal!");
				targetList.values = selectedCharacter.value.FindSupportablesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.HEAL;
				StartCoroutine(MenuChangeDelay());
				break;
			case 2: // TRADE
				// Debug.Log("Trade!");
				targetList.values = selectedCharacter.value.FindAdjacentCharacters(Faction.PLAYER);
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.TRADE;
				StartCoroutine(MenuChangeDelay());
				break;
			case 3: // ITEM
				// Debug.Log("Item!");
				currentMenuMode.value = (int)MenuMode.STATS;
				inventoryIndex.value = 0;
				StartCoroutine(MenuChangeDelay());
				break;
			case 4: // WAIT
				// Debug.Log("Wait!");
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.NONE;
				StartCoroutine(MenuChangeDelay());
				selectedCharacter.value.End();
				break;
		}
		menuAcceptEvent.Invoke();
    }

	private void ButtonSetup() {
		if (!active)
			return;
			
		actionButtons[0].gameObject.SetActive(selectedCharacter.value.CanAttack());
		actionButtons[1].gameObject.SetActive(selectedCharacter.value.CanSupport());
		actionButtons[2].gameObject.SetActive(selectedCharacter.value.CanTrade());
		if (actionMenuPosition.value == -1 || !actionButtons[actionMenuPosition.value].IsActive()) {
			actionMenuPosition.value = -1;
			OnDownArrow();
		}
		ButtonHighlighting();
	}

	/// <summary>
	/// Colors the selected button to show the current selection.
	/// </summary>
	private void ButtonHighlighting() {
		for (int i = 0; i < actionButtons.Length; i++) {
			actionButtons[i].color = (actionMenuPosition.value == i) ? Color.cyan : Color.white;
		}
	}



    public override void OnLeftArrow() {}
    public override void OnRightArrow() {}
    public override void OnSp1Button() {}
    public override void OnSp2Button() {}
    public override void OnStartButton() {}

}
