using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ActionInputType { SEIZE, ATTACK, HEAL, VISIT, TRADE, ITEM, WAIT }

public class ActionInputController : MonoBehaviour {

	[Header("References")]
	public TacticsMoveVariable selectedCharacter;
	public IntVariable currentMenuMode;
	public ActionModeVariable currentActionMode;
	public MapTileListVariable targetList;
	public IntVariable inventoryIndex;
	public BoolVariable triggeredWin;

	[Header("Unit Action Menu")]
	public GameObject actionMenu;
	private int menuPosition;
	private Image[] actionButtons = new Image[0];

	[Header("Dialogues")]
	public IntVariable dialogueMode;
	public ScrObjEntryReference dialogueEntry;
	public PopupController popup;
	public SfxEntry gainItemSfx;
	public UnityEvent startDialogue;


	private void Start() {
		actionButtons = actionMenu.GetComponentsInChildren<Image>(true);
	}

    public void ShowMenu(bool active) {
		actionMenu.SetActive(active);
		if (active)
			ButtonSetup();
    }

    public bool MoveVertical(int dir) {
		int startPos = menuPosition;
		do {
			menuPosition = OPMath.FullLoop(0,actionButtons.Length, menuPosition + dir);
		} while (!actionButtons[menuPosition].gameObject.activeSelf);
		UpdateHighlight();
		return (startPos != menuPosition);
    }

    public bool BackButton() {
		if (selectedCharacter.value.canUndoMove) {
			menuPosition = 0;
		}
		return (selectedCharacter.value.canUndoMove);
    }

    public void OkButton() {
		switch ((ActionInputType)menuPosition)
		{
			case ActionInputType.SEIZE:
				triggeredWin.value = true;
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				selectedCharacter.value.End();
				break;
			case ActionInputType.ATTACK:
				targetList.values = selectedCharacter.value.GetAttackablesInRange();
				currentActionMode.value = ActionMode.ATTACK;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.HEAL: // HEAL
				targetList.values = selectedCharacter.value.FindSupportablesInRange();
				currentActionMode.value = ActionMode.HEAL;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.VISIT: // VISIT
				selectedCharacter.value.currentTile.interacted = true;
				//TODO active = false;
				InputDelegateController.instance.TriggerMenuChange((MenuMode)currentMenuMode.value);
				currentActionMode.value = ActionMode.NONE;
				dialogueMode.value = (int)DialogueMode.VISIT;
				dialogueEntry.value = selectedCharacter.value.currentTile.dialogue;
				startDialogue.Invoke();
				break;
			case ActionInputType.TRADE: // TRADE
				targetList.values = selectedCharacter.value.FindAdjacentCharacters(Faction.PLAYER);
				currentActionMode.value = ActionMode.TRADE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.ITEM: // ITEM
				inventoryIndex.value = 0;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.INV);
				break;
			case ActionInputType.WAIT: // WAIT
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				selectedCharacter.value.End();
				break;
		}
    }
	
	public void ResumeBattle() {
		if (dialogueMode.value == (int)DialogueMode.VISIT) {
			currentActionMode.value = ActionMode.NONE;
			InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
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
		ItemEntry item = selectedCharacter.value.currentTile.gift;
		yield return StartCoroutine(popup.ShowPopup(item.icon, item.entryName, gainItemSfx, 2f, 0f));
		selectedCharacter.value.End();
	}

	private void ButtonSetup() {
		actionButtons[(int)ActionInputType.SEIZE].gameObject.SetActive(selectedCharacter.value.CanSeize());
		actionButtons[(int)ActionInputType.ATTACK].gameObject.SetActive(selectedCharacter.value.CanAttack());
		actionButtons[(int)ActionInputType.HEAL].gameObject.SetActive(selectedCharacter.value.CanSupport());
		actionButtons[(int)ActionInputType.VISIT].gameObject.SetActive(selectedCharacter.value.CanVisit());
		actionButtons[(int)ActionInputType.TRADE].gameObject.SetActive(selectedCharacter.value.CanTrade());
		if (menuPosition == -1 || !actionButtons[menuPosition].IsActive()) {
			menuPosition = -1;
			MoveVertical(1);
		}
		UpdateHighlight();
	}

	/// <summary>
	/// Colors the selected button to show the current selection.
	/// </summary>
	private void UpdateHighlight() {
		for (int i = 0; i < actionButtons.Length; i++) {
			actionButtons[i].color = (menuPosition == i) ? Color.cyan : Color.white;
		}
	}

}
