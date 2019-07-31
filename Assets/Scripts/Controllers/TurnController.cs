using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum TurnState { INIT, STORY, PREP, INTRO, ACTION, REINFORCE, EVENTS, DIALOGUE, FINISHED }

/// <summary>
/// Class which handles changing turns and triggers functionality when the turn is changed.
/// </summary>
public class TurnController : MonoBehaviour {
	
	[Header("Objects")]
	public EnemyController enemyController;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;

	[Header("References")]
	public BoolVariable lockControls;
	public ScrObjEntryReference currentMap;
	public IntVariable currentTurn;
	public FactionVariable currentFactionTurn;
	public ActionModeVariable currentAction;
	public IntVariable currentMenuMode;
	public BoolVariable triggeredWin;
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;
	public BoolVariable selectMainCharacter;
	public BoolVariable autoEndTurn;
	public IntVariable cursorX, cursorY;

	public TurnState currentState;

	[Header("Rewards")]
	public MySpinner spinner;
	public IntVariable totalMoney;
	public IntVariable totalScrap;
	public PlayerData playerData;
	public IntVariable nextLoadState;
	public SfxEntry droppedItemFanfare;

	[Header("UI")]
	public GameObject notificationObject;
	public Text notificationText;

	[Header("Sound")]
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public BoolVariable musicFocus;
	public AudioQueueVariable sfxQueue;
	public SfxEntry victoryFanfare;
	public SfxEntry gameOverFanfare;
	public SfxEntry turnChangeFanfare;

	[Header("Events")]
	public UnityEvent resetSelections;
	public UnityEvent startDialogueEvent;
	public UnityEvent gameLoseEvent;
	public UnityEvent playBkgMusicEvent;
	public UnityEvent replaceMusicEvent;
	public UnityEvent playSfxEvent;
	public UnityEvent checkReinforcementsEvent;
	public UnityEvent checkDialoguesEvent;
	public UnityEvent checkMapChangeEvent;
	public UnityEvent moveCursorEvent;
	public UnityEvent characterChangedEvent;
	
	private bool gameover;
	public BoolVariable autoWin;


	/// <summary>
	/// Clears character lists and prepares for the player's first turn.
	/// </summary>
	private void Start() {
		currentState = TurnState.INIT;
		currentTurn.value = 0;
		currentFactionTurn.value = Faction.ENEMY;
		triggeredWin.value = false;
		playerList.values.Clear();
		enemyList.values.Clear();
	}

	public void TriggerNextStep() {

		switch (currentState)
		{
		case TurnState.INIT:
			Debug.Log("Show story 1");
			currentState = TurnState.STORY;
			currentDialogueMode.value = (int)DialogueMode.PRELUDE;
			currentDialogue.value = ((MapEntry)currentMap.value).preDialogue;
			startDialogueEvent.Invoke();
			break;
		case TurnState.STORY:
			Debug.Log("Show prep?");
			if (((MapEntry)currentMap.value).skipBattlePrep) {
				currentState = TurnState.INTRO;
				TriggerNextStep();
				break;
			};
			currentState = TurnState.PREP;
			InputDelegateController.instance.TriggerMenuChange(MenuMode.PREP);
			break;
		case TurnState.PREP:
			Debug.Log("Show story 2");
			currentState = TurnState.INTRO;
			currentDialogueMode.value = (int)DialogueMode.INTRO;
			currentDialogue.value = ((MapEntry)currentMap.value).introDialogue;
			startDialogueEvent.Invoke();
			break;
		case TurnState.INTRO:
			Debug.Log("Start game");
			currentState = TurnState.EVENTS;
			StartGameSetup();
			break;
		case TurnState.ACTION:
			EndChangeTurn();
			Debug.Log("Check dialogue");
			currentState = TurnState.DIALOGUE;
			checkDialoguesEvent.Invoke();
			break;
		case TurnState.DIALOGUE:
			Debug.Log("Check reinforcements");
			currentState = TurnState.REINFORCE;
			if (currentFactionTurn.value == Faction.ENEMY) {
				checkReinforcementsEvent.Invoke();
			}
			else {
				TriggerNextStep();
			}
			break;
		case TurnState.REINFORCE:
			Debug.Log("Check events");
			currentState = TurnState.EVENTS;
			checkMapChangeEvent.Invoke();
			break;
		case TurnState.EVENTS:
			Debug.Log("Next turn");
			StartCoroutine(DisplayTurnChange(1.5f));
			break;
		case TurnState.FINISHED:
			Debug.Log("Game finished!");
			break;
		}
	}

	/// <summary>
	/// Starts the game and enables the music and shows the turn change.
	/// </summary>
	private void StartGameSetup() {
		MapEntry map = (MapEntry)currentMap.value;
		musicFocus.value = true;
		mainMusic.value = map.playerMusic.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();
		checkReinforcementsEvent.Invoke();
	}

	/// <summary>
	/// Auto-ends the turn if all the player characters have taken their turn if enabled.
	/// </summary>
	public void CheckEndTurn() {
		if (!autoEndTurn.value || currentFactionTurn.value != Faction.PLAYER || currentState == TurnState.FINISHED)
			return;

		for (int i = 0; i < playerList.values.Count; i++) {
			if (playerList.values[i].IsAlive() && !playerList.values[i].hasMoved) {
				return;
			}
		}
		TriggerNextStep();
	}

	/// <summary>
	/// Changes the turn to the other faction and displays the turn change text box.
	/// </summary>
	private void EndChangeTurn() {
		if (gameover)
			return;
		lockControls.value = true;

		if (currentFactionTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnEndTurn();
			}
		}
		else if (currentFactionTurn.value == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnEndTurn();
			}
		}
	}

	/// <summary>
	/// Checks to see if the win/lose condition has been met, displays a message and ends the game.
	/// </summary>
	public void CheckGameFinished() {
		if (gameover)
			return;

		// Check if any players are alive
		bool gameFinished = false;
		MapEntry map = (MapEntry)currentMap.value;
		// Normal lose condition
		if (map.loseCondition == LoseCondition.NORMAL) {
			for (int i = 0; i < playerList.values.Count; i++) {
				if (!playerList.values[i].IsAlive() && playerList.values[i].stats.charData.mustSurvive) {
					gameFinished = true;
					break;
				}
			}
		}
		else {
			Debug.LogError("Undefined lose condition:   " + map.loseCondition);
		}

		if (gameFinished) {
			Debug.Log("GAME OVER");
			StartCoroutine(EndGameLose());
			gameover = true;
			return;
		}

		// Check if any enemies are alive
		gameFinished = true;
		// Rout win condition
		if (map.winCondition == WinCondition.ROUT) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (enemyList.values[i].IsAlive()) {
					gameFinished = false;
					break;
				}
			}
		}
		else if (map.winCondition == WinCondition.BOSS) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				NPCMove enemy = (NPCMove)enemyList.values[i];
				if (enemy.aggroType == AggroType.BOSS && enemyList.values[i].IsAlive()) {
					gameFinished = false;
					break;
				}
			}
		}
		else if (map.winCondition == WinCondition.SEIZE){
			gameFinished = (triggeredWin.value);
		}
		else {
			Debug.LogError("Undefined win condition:   " + map.winCondition);
		}

		// DEBUG
		if (autoWin.value)
			gameFinished = true;

		if (gameFinished) {
			Debug.Log("BATTLE WON");
			gameover = true;
			StartCoroutine(EndGameWin());
			return;
		}

		gameover = false;
	}

	/// <summary>
	/// Coroutine which locks the controls and shows the turn change display.
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	private IEnumerator DisplayTurnChange(float duration) {
		currentFactionTurn.value = (currentFactionTurn.value == Faction.PLAYER) ? Faction.ENEMY : Faction.PLAYER;
		if (currentFactionTurn.value == Faction.PLAYER)
			currentTurn.value++;

		currentAction.value = ActionMode.LOCK;
		InputDelegateController.instance.TriggerMenuChange(MenuMode.NONE);
		notificationText.text = currentFactionTurn.value + " TURN";

		yield return null;

		notificationObject.SetActive(true);
		resetSelections.Invoke();
		sfxQueue.Enqueue(turnChangeFanfare);
		playSfxEvent.Invoke();
		MapEntry map = (MapEntry)currentMap.value;
		mainMusic.value = (currentFactionTurn.value == Faction.ENEMY) ? map.enemyMusic.clip : map.playerMusic.clip;
		replaceMusicEvent.Invoke();

		yield return new WaitForSeconds(duration);
		
		notificationObject.SetActive(false);
		StartTurn();
	}

	/// <summary>
	/// Runs at the start of each turn.
	/// Activates all the characters on the current faction.
	/// </summary>
	private void StartTurn() {
		if (gameover)
			return;

		Debug.Log("New turn");
		currentState = TurnState.ACTION;
		
		if (currentFactionTurn.value == Faction.ENEMY) {
			float wait = 0;
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (enemyList.values[i].OnStartTurn())
					wait = 2f;
			}
			enemyController.RunEnemies(wait);
		}
		else if (currentFactionTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnStartTurn();
			}
			if (selectMainCharacter.value) {
				cursorX.value = playerList.values[0].posx;
				cursorY.value = playerList.values[0].posy;
				moveCursorEvent.Invoke();
			}
			lockControls.value = false;
			currentAction.value = ActionMode.NONE;
			InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
		}
		else {
			Debug.LogError("Wrong state!");
		}
	}

	public void InstaWin() {
		if (gameover)
			return;
		Debug.Log("BATTLE WON");
		gameover = true;
		StartCoroutine(EndGameWin());
	}
	
	public void GameOver() {
		if (gameover)
			return;
		gameover = true;
		StartCoroutine(EndGameLose());
	}

	/// <summary>
	/// When clearing the map, show win text for a while and then move 
	/// on to post dialogue.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndGameWin() {
		lockControls.value = true;
		currentState = TurnState.FINISHED;
		notificationText.text = "BATTLE WON";
		notificationObject.SetActive(true);
		mainMusic.value = null;
		musicFocus.value = true;
		playBkgMusicEvent.Invoke();
		sfxQueue.Enqueue(victoryFanfare);
		playSfxEvent.Invoke();

		//Remove dead characters
		for (int i = 0; i < playerList.values.Count; i++) {
			if (!playerList.values[i].IsAlive()) {
				playerData.stats.RemoveAt(i);
				playerData.inventory.RemoveAt(i);
				playerData.skills.RemoveAt(i);
				playerData.baseInfo.RemoveAt(i);
				playerList.values.RemoveAt(i);
				i--;
			}
		}

		yield return new WaitForSeconds(4f);

		notificationObject.SetActive(false);
		notificationText.gameObject.SetActive(false);

		// Award all the rewards
		MapEntry map = (MapEntry)currentMap.value;
		if (map.reward.money > 0) {
			totalMoney.value += map.reward.money;
			yield return StartCoroutine(spinner.ShowSpinner(null, "Gained " + map.reward.money + " Money", droppedItemFanfare));
		}
		if (map.reward.scrap > 0) {
			totalScrap.value += map.reward.scrap;
			yield return StartCoroutine(spinner.ShowSpinner(null, "Gained " + map.reward.scrap + " Scrap", droppedItemFanfare));
		}
		for (int i = 0; i < map.reward.items.Count; i++) {
			playerData.items.Add(new InventoryItem(map.reward.items[i]));
			yield return StartCoroutine(spinner.ShowSpinner(map.reward.items[i].icon, "Gained " + map.reward.items[i].entryName, droppedItemFanfare));
		}

		//Move to the ending dialogue
		currentDialogueMode.value = (int)DialogueMode.ENDING;
		currentDialogue.value = ((MapEntry)currentMap.value).endDialogue;
		nextLoadState.value = (int)SaveScreenController.NextState.BASE;
		startDialogueEvent.Invoke();
	}

	/// <summary>
	/// When losing the map, show lose text and then the game over menu.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndGameLose() {
		currentState = TurnState.FINISHED;
		notificationText.text = "GAME OVER";
		notificationObject.SetActive(true);
		sfxQueue.Enqueue(gameOverFanfare);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(2f);
		InputDelegateController.instance.TriggerSceneChange(MenuMode.MAIN_MENU, "MainMenu");
	}
	

	public void DamageAllPlayers() {
		for (int i = 0; i < playerList.values.Count; i++) {
			playerList.values[i].TakeDamage(5, false);
		}
	}

	public void DamageAllEnemies() {
		for (int i = 0; i < enemyList.values.Count; i++) {
			enemyList.values[i].TakeDamage(5, false);
		}
	}

	public void ToggleInfiniteSpeed() {
		TacticsMove.infiniteMove = !TacticsMove.infiniteMove;
	}
}
