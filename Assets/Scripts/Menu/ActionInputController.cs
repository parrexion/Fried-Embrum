using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ActionInputType { SEIZE, ATTACK, HEAL, VISIT, HACK, TRADE, ITEM, WAIT }

public class ActionInputController : MonoBehaviour {

	[Header("References")]
	public PlayerData playerData;
	public IntVariable totalMoney;
	public IntVariable totalScrap;
	public TacticsMoveVariable selectedCharacter;
	public ScrObjEntryReference currentMap;
	public IntVariable currentMenuMode;
	public ActionModeVariable currentActionMode;
	public ScrObjEntryReference villageVisitor1;
	public MapTileListVariable targetList;
	public IntVariable inventoryIndex;
	public BoolVariable triggeredWin;
	public PrepListVariable squad1;
	public PrepListVariable squad2;

	[Header("Unit Action Menu")]
	public GameObject actionMenu;
	public MyButtonList actionButtons;

	[Header("Dialogues")]
	public IntVariable dialogueMode;
	public ScrObjEntryReference dialogueEntry;
	public MySpinner spinner;
	public SfxEntry gainItemSfx;
	public UnityEvent startDialogue;


	public void ShowMenu(bool active, bool reset) {
		actionMenu.SetActive(active);
		if (active) {
			int previousPosition = (reset) ? 0 : actionButtons.GetPosition();
			ButtonSetup();
			actionButtons.ForcePosition(previousPosition);
		}
	}

	public bool MoveVertical(int dir) {
		int startPos = actionButtons.GetPosition();
		int endPos = actionButtons.Move(dir);
		return (startPos != endPos);
	}

	public bool BackButton() {
		if (selectedCharacter.value.canUndoMove) {
			actionButtons.ForcePosition(0);
		}
		return (selectedCharacter.value.canUndoMove);
	}

	public void OkButton() {
		switch ((ActionInputType)actionButtons.GetValue()) {
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
				selectedCharacter.value.canUndoMove = false;
				currentActionMode.value = ActionMode.NONE;
				dialogueMode.value = (int)DialogueMode.VISIT;
				dialogueEntry.value = selectedCharacter.value.currentTile.dialogue;
				villageVisitor1.value = selectedCharacter.value.stats.charData.portraitSet;
				startDialogue.Invoke();
				break;
			case ActionInputType.HACK:
				MapTile tile = selectedCharacter.value.currentTile;
				tile.interacted = true;
				tile.SetTerrain(tile.alternativeTerrain);
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				StartCoroutine(WaitForItemGain());
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

	public void ReturnFromVisit() {
		currentActionMode.value = ActionMode.NONE;
		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
		if (!selectedCharacter.value.currentTile.gift.IsEmpty()) {
			StartCoroutine(WaitForItemGain());
		}
		else if (selectedCharacter.value.currentTile.ally != null) {
			MapTile closest = selectedCharacter.value.battleMap.GetClosestEmptyTile(selectedCharacter.value.currentTile);
			PlayerMove tactics = (PlayerMove)selectedCharacter.value.currentTile.ally;
			tactics.posx = closest.posx;
			tactics.posy = closest.posy;
			tactics.Setup();
			playerData.AddNewPlayer(tactics);
			int recruitSquad = ((PlayerMove)selectedCharacter.value).squad;
			if (recruitSquad == 1) {
				tactics.squad = 1;
				squad1.values.Add(new PrepCharacter(playerData.stats.Count - 1));
			}
			else if (recruitSquad == 2) {
				tactics.squad = 2;
				squad2.values.Add(new PrepCharacter(playerData.stats.Count - 1));
			}
			else { Debug.LogError("Squad is invalid!"); }
			selectedCharacter.value.End();
		}
		else {
			selectedCharacter.value.End();
		}

		dialogueMode.value = (int)DialogueMode.NONE;
	}

	private IEnumerator WaitForItemGain() {

		if (selectedCharacter.value.currentTile.gift.money > 0) {
			string message = "Received " + selectedCharacter.value.currentTile.gift.money + " money";
			yield return StartCoroutine(spinner.ShowSpinner(null, message, gainItemSfx));
			totalMoney.value += selectedCharacter.value.currentTile.gift.money;
		}

		if (selectedCharacter.value.currentTile.gift.scrap > 0) {
			string message = "Received " + selectedCharacter.value.currentTile.gift.scrap + " scrap";
			yield return StartCoroutine(spinner.ShowSpinner(null, message, gainItemSfx));
			totalScrap.value += selectedCharacter.value.currentTile.gift.scrap;
		}

		for (int i = 0; i < selectedCharacter.value.currentTile.gift.items.Count; i++) {
			if (selectedCharacter.value.currentTile.gift.items[i] == null)
				continue;
			InventoryItem item = new InventoryItem(selectedCharacter.value.currentTile.gift.items[i]);
			string message = "Received " + item.item.entryName;
			yield return StartCoroutine(spinner.ShowSpinner(item.item.icon, message, gainItemSfx));
			bool res = selectedCharacter.value.inventory.AddItem(item);
			if (!res) {
				playerData.items.Add(item);
			}
		}

		selectedCharacter.value.End();
	}

	private void ButtonSetup() {
		actionButtons.ResetButtons();
		bool seizeWin = ((MapEntry)currentMap.value).winCondition == WinCondition.SEIZE;
		if (seizeWin && selectedCharacter.value.CanSeize())
			actionButtons.AddButton("SEIZE", (int)ActionInputType.SEIZE);
		if (selectedCharacter.value.CanAttack())
			actionButtons.AddButton("ATTACK", (int)ActionInputType.ATTACK);
		if (selectedCharacter.value.CanSupport())
			actionButtons.AddButton("HEAL", (int)ActionInputType.HEAL);
		if (selectedCharacter.value.CanVisit())
			actionButtons.AddButton("VISIT", (int)ActionInputType.VISIT);
		if (selectedCharacter.value.CanHack())
			actionButtons.AddButton("HACK", (int)ActionInputType.HACK);
		if (selectedCharacter.value.CanTrade())
			actionButtons.AddButton("TRADE", (int)ActionInputType.TRADE);

		actionButtons.AddButton("ITEM", (int)ActionInputType.ITEM);
		actionButtons.AddButton("WAIT", (int)ActionInputType.WAIT);
	}

}
