using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnEventController : MonoBehaviour {

	public MapSpawner mapSpawner;

	[Header("Game state")]
	public BattleMap battleMap;
	public IntVariable currentTurn;
	public FactionVariable currentFaction;

	[Header("Spawning")]
	public float reinforcementDelay = 0.75f;
	public FloatVariable currentGameSpeed;

	[Header("Dialogues")]
	public BoolVariable lockControls;
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;

	[Header("Events")]
	public UnityEvent nextTurnStateEvent;
	public UnityEvent startDialogueEvent;


	/// <summary>
	/// Is triggered by event and checks the map to see if any reinforcements should be spawned.
	/// </summary>
	public void SpawnReinforcements() {
		StartCoroutine(SpawnReinforcementsLoop());
	}

	/// <summary>
	/// Loop which goes through each reinforcement event and spawns the ones which are available.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SpawnReinforcementsLoop() {
		for(int i = 0; i < battleMap.reinforcementEvents.Count; i++) {
			SpawnData pos = battleMap.reinforcementEvents.GetEvent(i);
			if(battleMap.reinforcementEvents.IsActivated(i) || currentFaction.value != pos.faction)
				continue;

			bool activate = false;
			if(pos.triggerType == TriggerType.TURN)
				activate = (currentTurn.value >= pos.spawnTurn);
			else if(pos.triggerType == TriggerType.TRIGGER) {
				activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
			} else if(pos.triggerType == TriggerType.PLAYER_COUNT) {
				activate = battleMap.playerList.Count <= pos.spawnTurn;
			}

			if(!activate) {
				continue;
			}

			MapTile tile = battleMap.GetTile(pos.x, pos.y);
			if(tile.currentCharacter == null) {
				mapSpawner.SpawnPlayerCharacter(pos, true, true, true);
				battleMap.reinforcementEvents.Activate(i);
				yield return new WaitForSeconds(reinforcementDelay * currentGameSpeed.value);
			}
		}
		nextTurnStateEvent.Invoke();
		yield break;
	}


	/// <summary>
	/// Checks if there are any dialogues that should be shown.
	/// </summary>
	public void CheckDialogueEvents() {
		for(int i = 0; i < battleMap.dialogueEvents.Count; i++) {
			TurnEvent pos = battleMap.dialogueEvents.GetEvent(i);
			if(battleMap.dialogueEvents.IsActivated(i) || currentFaction.value != pos.factionTurn)
				continue;

			bool activate = false;
			switch(pos.triggerType) {
				case TriggerType.TURN:
					activate = (currentTurn.value == pos.turn);
					break;
				case TriggerType.TRIGGER:
					activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
					break;
				case TriggerType.PLAYER_COUNT:
					activate = (battleMap.playerList.values.Count <= pos.turn);
					break;
				case TriggerType.ALLY_COUNT:
					activate = (battleMap.allyList.values.Count <= pos.turn);
					break;
				case TriggerType.ENEMY_COUNT:
					activate = (battleMap.enemyList.AliveCount() <= pos.turn);
					break;
			}

			if(activate) {
				battleMap.dialogueEvents.Activate(i);
				currentDialogue.value = pos.dialogue;
				currentDialogueMode.value = (int)DialogueMode.EVENT;
				startDialogueEvent.Invoke();
				return;
			}
		}
		//If no dialogues were triggered
		nextTurnStateEvent.Invoke();
	}

	/// <summary>
	/// Checks if there are any events that should be triggered.
	/// </summary>
	public void CheckMapEvents() {
		for(int i = 0; i < battleMap.mapEvents.Count; i++) {
			TurnEvent pos = battleMap.mapEvents.GetEvent(i);
			if(battleMap.mapEvents.IsActivated(i) || currentFaction.value != pos.factionTurn)
				continue;

			bool activate = false;
			switch(pos.triggerType) {
				case TriggerType.TURN:
					activate = (currentTurn.value == pos.turn);
					break;
				case TriggerType.TRIGGER:
					activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
					break;
				case TriggerType.PLAYER_COUNT:
					activate = (battleMap.playerList.values.Count <= pos.turn);
					break;
				case TriggerType.ALLY_COUNT:
					activate = (battleMap.allyList.values.Count <= pos.turn);
					break;
				case TriggerType.ENEMY_COUNT:
					activate = (battleMap.enemyList.AliveCount() <= pos.turn);
					break;
			}

			if(activate) {
				battleMap.mapEvents.Activate(i);
				switch(pos.type) {
					case TurnEventType.MAPCHANGE:
						MapTile tile = battleMap.GetTile(pos.x, pos.y);
						tile.SetTerrain(pos.changeTerrain);
						break;
					case TurnEventType.MONEY:
						Debug.Log("Gained money:  " + pos.value);
						break;
					case TurnEventType.SCRAP:
						Debug.Log("Gained scrap:  " + pos.value);
						break;
				}
			}
		}
		nextTurnStateEvent.Invoke();
	}
}
