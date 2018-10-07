using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ActionInputType { SEIZE, ATTACK, HEAL, VISIT, TRADE, ITEM, WAIT }

public class ActionInputController : InputReceiver {

	[Header("References")]
	public TacticsMoveVariable selectedCharacter;
	public ActionModeVariable currentActionMode;
	public MapTileListVariable targetList;
	public IntVariable inventoryIndex;
	public BoolVariable triggeredWin;

	[Header("Unit Action Menu")]
	public GameObject actionMenu;
	public IntVariable actionMenuPosition;
	private Image[] actionButtons= new Image[0];

	[Header("Dialogues")]
	public IntVariable dialogueMode;
	public ScrObjEntryReference dialogueEntry;
	public PopupController popup;
	public SfxEntry gainItemSfx;
	public UnityEvent startDialogue;


	private void Start() {
		actionButtons = actionMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.UNIT);
		actionMenu.SetActive(mode == MenuMode.UNIT);
		// Debug.Log("Active:  " + active + " , " + mode);
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
		if (!active) {
			return;
		}

		switch ((ActionInputType)actionMenuPosition.value)
		{
			case ActionInputType.SEIZE:
				triggeredWin.value = true;
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.NONE;
				StartCoroutine(MenuChangeDelay());
				selectedCharacter.value.End();
				break;
			case ActionInputType.ATTACK:
				// Debug.Log("Attack!");
				targetList.values = selectedCharacter.value.GetAttackablesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.ATTACK;
				StartCoroutine(MenuChangeDelay());
				break;
			case ActionInputType.HEAL: // HEAL
				// Debug.Log("Heal!");
				targetList.values = selectedCharacter.value.FindSupportablesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.HEAL;
				StartCoroutine(MenuChangeDelay());
				break;
			case ActionInputType.VISIT: // VISIT
				selectedCharacter.value.currentTile.interacted = true;
				active = false;
				StartCoroutine(MenuChangeDelay());
				currentActionMode.value = ActionMode.NONE;
				dialogueMode.value = (int)DialogueMode.VISIT;
				dialogueEntry.value = selectedCharacter.value.currentTile.dialogue;
				startDialogue.Invoke();
				break;
			case ActionInputType.TRADE: // TRADE
				// Debug.Log("Trade!");
				targetList.values = selectedCharacter.value.FindAdjacentCharacters(Faction.PLAYER);
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.TRADE;
				StartCoroutine(MenuChangeDelay());
				break;
			case ActionInputType.ITEM: // ITEM
				// Debug.Log("Item!");
				currentMenuMode.value = (int)MenuMode.STATS;
				inventoryIndex.value = 0;
				StartCoroutine(MenuChangeDelay());
				break;
			case ActionInputType.WAIT: // WAIT
				// Debug.Log("Wait!");
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.NONE;
				StartCoroutine(MenuChangeDelay());
				selectedCharacter.value.End();
				break;
		}
		menuAcceptEvent.Invoke();
    }
	
	public void ResumeBattle() {
		if (dialogueMode.value == (int)DialogueMode.VISIT) {
			currentMenuMode.value = (int)MenuMode.MAP;
			currentActionMode.value = ActionMode.NONE;
			StartCoroutine(MenuChangeDelay());
			if (selectedCharacter.value.currentTile.gift != null) {
				StartCoroutine(WaitForItemGain());
			}
			else if (selectedCharacter.value.currentTile.ally != null) {
				MapTile closest = selectedCharacter.value.battleMap.GetClosestEmptyTile(selectedCharacter.value.currentTile);
				TacticsMove tactics = selectedCharacter.value.currentTile.ally;
				tactics.posx = closest.posx;
				tactics.posy = closest.posy;
				tactics.Setup();
				selectedCharacter.value.End();
			}
			else {
				Debug.Log("OK");
				selectedCharacter.value.End();
			}
		}

		dialogueMode.value = (int)DialogueMode.NONE;
	}

	private IEnumerator WaitForItemGain() {
		WeaponItem item = selectedCharacter.value.currentTile.gift;
		yield return StartCoroutine(popup.ShowPopup(item.icon, item.entryName, gainItemSfx, 2f, 0f));
		selectedCharacter.value.End();
	}

	private void ButtonSetup() {
		if (!active)
			return;
			
		actionButtons[(int)ActionInputType.SEIZE].gameObject.SetActive(selectedCharacter.value.CanSeize());
		actionButtons[(int)ActionInputType.ATTACK].gameObject.SetActive(selectedCharacter.value.CanAttack());
		actionButtons[(int)ActionInputType.HEAL].gameObject.SetActive(selectedCharacter.value.CanSupport());
		actionButtons[(int)ActionInputType.VISIT].gameObject.SetActive(selectedCharacter.value.CanVisit());
		actionButtons[(int)ActionInputType.TRADE].gameObject.SetActive(selectedCharacter.value.CanTrade());
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
    public override void OnLButton() {}
    public override void OnRButton() {}
    public override void OnXButton() {}
    public override void OnYButton() {}
    public override void OnStartButton() {}
}
