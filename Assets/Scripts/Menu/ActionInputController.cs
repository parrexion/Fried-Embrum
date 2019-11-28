using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ActionInputType { CAPTURE, ESCAPE, TALK, DOOR, ATTACK, HEAL, VISIT, HACK, TRADE, ITEM, WAIT }

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

	[Header("Joining characters")]
	public MapSpawner mapSpawner;
	private PlayerMove joiningCharacter;
	private bool willJoin;

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
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		switch ((ActionInputType)actionButtons.GetValue()) {
			case ActionInputType.CAPTURE:
				triggeredWin.value = true;
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				player.End();
				break;
			case ActionInputType.ESCAPE:
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				player.Escape();
				player.End();
				break;
			case ActionInputType.TALK:
				FindTalkers();
				currentActionMode.value = ActionMode.TALK;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.DOOR:
				FindDoors();
				currentActionMode.value = ActionMode.DOOR;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.ATTACK:
				targetList.values = player.GetAttackablesInRange();
				currentActionMode.value = ActionMode.ATTACK;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.HEAL: // HEAL
				targetList.values = player.FindSupportablesInRange();
				currentActionMode.value = ActionMode.HEAL;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				break;
			case ActionInputType.VISIT: // VISIT
				player.currentTile.interacted = true;
				player.canUndoMove = false;
				currentActionMode.value = ActionMode.NONE;
				dialogueMode.value = (int)DialogueMode.VISIT;
				dialogueEntry.value = player.currentTile.dialogue;
				villageVisitor1.value = player.stats.charData.portraitSet;
				startDialogue.Invoke();
				break;
			case ActionInputType.HACK:
				MapTile tile = player.currentTile;
				tile.interacted = true;
				tile.SetTerrain(tile.alternativeTerrain);
				currentActionMode.value = ActionMode.NONE;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				StartCoroutine(WaitForItemGain());
				break;
			case ActionInputType.TRADE: // TRADE
				targetList.values = player.FindAdjacentCharacters(true, false, false);
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
				player.End();
				break;
		}
	}

	private void FindDoors() {
		targetList.values.Clear();
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		List<MapTile> tiles = player.GetAdjacentTiles();
		for(int i = 0; i < tiles.Count; i++) {
			if (tiles[i].interactType == InteractType.DOOR && !tiles[i].interacted && player.inventory.HasKey(tiles[i].doorKeyType))
				targetList.values.Add(tiles[i]);
		}
	}

	private void FindTalkers() {
		targetList.values.Clear();
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		List<MapTile> talkers = player.FindAdjacentCharacters(false, true, true);
		for (int i = 0; i < talkers.Count; i++) {
			for (int talk = 0; talk < talkers[i].currentCharacter.talkQuotes.Count; talk++) {
				FightQuote fq = talkers[i].currentCharacter.talkQuotes[talk];
				if (!fq.activated && (fq.triggerer == null || fq.triggerer.uuid == player.stats.charData.uuid)) {
					targetList.values.Add(talkers[i]);
					break;
				}
			}
		}
	}

	public void TalkToCharacter(MapTile targetTile) {
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		TacticsMove other = targetTile.currentCharacter;
		currentActionMode.value = ActionMode.NONE;
		dialogueMode.value = (int)DialogueMode.TALK;

		FightQuote quote = null;
		for (int i = 0; i < other.talkQuotes.Count; i++) {
			FightQuote fq = other.talkQuotes[i];
			if (!fq.activated && (fq.triggerer == null || fq.triggerer.uuid == player.stats.charData.uuid)) {
				quote = fq;
				break;

			}
		}

		dialogueEntry.value = quote.quote;
		willJoin = quote.willJoin;
		villageVisitor1.value = player.stats.charData.portraitSet;
		startDialogue.Invoke();

		if (willJoin) {
			SpawnData joinData = new SpawnData(other){ joiningSquad = player.squad};
			joiningCharacter = mapSpawner.SpawnPlayerCharacter(joinData, false, false, false);
			joiningCharacter.currentTile = targetTile;
			other.RemoveFromList();
			Destroy(other.gameObject);
		}
	}

	public void UnlockDoor(MapTile targetTile) {
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		for(int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple tuple = player.inventory.GetItem(i);
			if (tuple.attackType == AttackType.KEY && tuple.keyType == targetTile.doorKeyType) {
				currentActionMode.value = ActionMode.NONE;
				player.inventory.UseItem(i, player);
				targetTile.interacted = true;
				targetTile.SetTerrain(targetTile.alternativeTerrain);
				break;
			}
		}
	}

	public void ReturnFromVisit() {
		currentActionMode.value = ActionMode.NONE;
		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
		if (dialogueMode.value == (int)DialogueMode.VISIT) {
			if (!selectedCharacter.value.currentTile.gift.IsEmpty()) {
				StartCoroutine(WaitForItemGain());
			}
			else if (selectedCharacter.value.currentTile.ally != null) {
				PlayerMove tactics = (PlayerMove)selectedCharacter.value.currentTile.ally;
				MapTile closest = selectedCharacter.value.battleMap.GetClosestEmptyTile(selectedCharacter.value.currentTile);
				int recruitSquad = ((PlayerMove)selectedCharacter.value).squad;
				StartCoroutine(WaitForAllyToJoin(tactics, closest, recruitSquad));
			}
		}
		else if (dialogueMode.value == (int)DialogueMode.TALK) {
			if (willJoin) {
				willJoin = false;
				int recruitSquad = ((PlayerMove)selectedCharacter.value).squad;
				StartCoroutine(WaitForAllyToJoin(joiningCharacter, joiningCharacter.currentTile, recruitSquad));
			}
		}

		selectedCharacter.value.End();
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

	private IEnumerator WaitForAllyToJoin(PlayerMove tactics, MapTile closest, int recruitSquad) {
		string message = tactics.stats.charData.entryName + " has joined you!";
		tactics.posx = closest.posx;
		tactics.posy = closest.posy;
		tactics.Setup();
		playerData.AddNewPlayer(tactics);
		if (recruitSquad == 1) {
			tactics.squad = 1;
			squad1.values.Add(new PrepCharacter(playerData.stats.Count - 1));
		}
		else if (recruitSquad == 2) {
			tactics.squad = 2;
			squad2.values.Add(new PrepCharacter(playerData.stats.Count - 1));
		}
		else { Debug.LogError("Squad is invalid!"); }
		yield return StartCoroutine(spinner.ShowSpinner(tactics.stats.charData.portraitSet.small, message, gainItemSfx));
		joiningCharacter = null;
	}

	private void ButtonSetup() {
		PlayerMove player = (PlayerMove)selectedCharacter.value;
		actionButtons.ResetButtons();
		bool seizeWin = ((MapEntry)currentMap.value).winCondition == WinCondition.CAPTURE;
		bool escapeWin = ((MapEntry)currentMap.value).winCondition == WinCondition.ESCAPE;
		if (seizeWin && player.CanCapture())
			actionButtons.AddButton("CAPTURE", (int)ActionInputType.CAPTURE);
		if (escapeWin && player.CanEscape())
			actionButtons.AddButton("ESCAPE", (int)ActionInputType.ESCAPE);
		if (player.CanTalk())
			actionButtons.AddButton("TALK", (int)ActionInputType.TALK);
		if (player.CanOpenDoor())
			actionButtons.AddButton("DOOR", (int)ActionInputType.DOOR);
		if (player.CanAttack())
			actionButtons.AddButton("ATTACK", (int)ActionInputType.ATTACK);
		if (player.CanSupport())
			actionButtons.AddButton("HEAL", (int)ActionInputType.HEAL);
		if (player.CanVisit())
			actionButtons.AddButton("VISIT", (int)ActionInputType.VISIT);
		if (player.CanHack())
			actionButtons.AddButton("HACK", (int)ActionInputType.HACK);
		if (player.CanTrade())
			actionButtons.AddButton("TRADE", (int)ActionInputType.TRADE);

		actionButtons.AddButton("ITEM", (int)ActionInputType.ITEM);
		actionButtons.AddButton("WAIT", (int)ActionInputType.WAIT);
	}

}
