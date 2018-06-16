using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonMenu : MonoBehaviour {

	public BoolVariable lockControls;
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable targetTile;
	public ActionModeVariable currentMode;
	public FactionVariable currentTurn;

	[Header("Button Menu")]
	public GameObject buttonMenu;
	public GameObject endTurnMenu;
	public Button attackButton;
	public Button supportButton;
	public Button waitButton;
	public Button endButton;
	
	
	private void Start() {
		ActiveButtons();
	}

	public void ActiveButtons() {
		if (selectCharacter.value == null) {
			attackButton.interactable = false;
			supportButton.interactable = false;
		}
		else {
			bool canAttack = selectCharacter.value.CanAttack();
			bool canSupport = selectCharacter.value.CanSupport();
			attackButton.interactable = !lockControls.value && canAttack;
			supportButton.interactable = !lockControls.value && canSupport;
		}
		waitButton.interactable = !lockControls.value;
		endButton.interactable = !lockControls.value;
		
		if (selectCharacter.value == null) {
			buttonMenu.SetActive(false);
			endTurnMenu.SetActive(currentTurn.value == Faction.PLAYER && !lockControls.value);
		}
		else {
			bool activeTurn = (currentTurn.value == Faction.PLAYER);
			bool activeChar = (currentMode.value != ActionMode.NONE);
			buttonMenu.SetActive(activeTurn && activeChar);
			endTurnMenu.SetActive(activeTurn && !activeChar && !lockControls.value);
		}
	}
}
