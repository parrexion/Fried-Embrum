using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TurnState { INIT, ACTION, REINFORCE, EVENTS, FINISHED }

/// <summary>
/// Class which handles changing turns and triggers functionality when the turn is changed.
/// </summary>
public class TurnController : MonoBehaviour {
	
	[Header("Objects")]
	public EnemyController enemyController;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;

	public BoolVariable lockControls;
	public ScrObjEntryReference currentMap;
	public IntVariable currentTurn;
	public FactionVariable currentFactionTurn;
	public ActionModeVariable currentMode;
	public IntVariable currentMenuMode;
	public BoolVariable triggeredWin;
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;
	public BoolVariable autoEndTurn;

	public TurnState currentState;

	[Header("UI")]
	public GameObject turnChangeDisplay;
	public Text turnChangeText;

	[Header("Sound")]
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public BoolVariable musicFocus;
	public AudioQueueVariable sfxQueue;
	public SfxEntry victoryFanfare;
	public SfxEntry gameOverFanfare;
	public SfxEntry turnChangeFanfare;

	[Header("Game Finished")]
	public GameObject gameFinishObject;
	public Text gameFinishText;

	[Header("Events")]
	public UnityEvent menuModeChangedEvent;
	public UnityEvent resetSelections;
	public UnityEvent startDialogueEvent;
	public UnityEvent gameLoseEvent;
	public UnityEvent playBkgMusicEvent;
	public UnityEvent playSfxEvent;
	public UnityEvent checkReinforcementsEvent;
	public UnityEvent checkDialoguesEvent;
	
	private bool gameover;
	public BoolVariable autoWin;


	/// <summary>
	/// Clears character lists and prepares for the player's first turn.
	/// </summary>
	private void Start() {
		currentState = TurnState.INIT;
		currentTurn.value = 1;
		currentFactionTurn.value = Faction.PLAYER;
		triggeredWin.value = false;
		playerList.values.Clear();
		enemyList.values.Clear();
	}

	public void TriggerNextStep() {
		switch (currentState)
		{
			case TurnState.INIT:
				StartGame();
				currentState = TurnState.ACTION;
				break;
			case TurnState.ACTION:
				EndChangeTurn();
				break;
			case TurnState.REINFORCE:
				currentState = TurnState.EVENTS;
				checkDialoguesEvent.Invoke();
				break;
			case TurnState.EVENTS:
				currentDialogueMode.value = (int)DialogueMode.NONE;
				currentState = TurnState.ACTION;
				StartCoroutine(DisplayTurnChange(1.5f));
				break;
			case TurnState.FINISHED:
				break;
		}
	}

	/// <summary>
	/// Starts the game and enables the music and shows the turn change.
	/// </summary>
	private void StartGame() {
		currentMenuMode.value = (int)MenuMode.MAP;
		menuModeChangedEvent.Invoke();

		MapEntry map = (MapEntry)currentMap.value;
		musicFocus.value = true;
		mainMusic.value = map.owMusic.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();
		
		StartCoroutine(DisplayTurnChange(1.5f));
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
		EndChangeTurn();
	}

	/// <summary>
	/// Changes the turn to the other faction and displays the turn change text box.
	/// </summary>
	public void EndChangeTurn() {
		if (gameover)
			return;
		lockControls.value = true;
		if (currentFactionTurn.value == Faction.PLAYER) {
			currentFactionTurn.value = Faction.ENEMY;
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnEndTurn();
			}
			currentState = TurnState.EVENTS;
			checkDialoguesEvent.Invoke();
		}
		else if (currentFactionTurn.value == Faction.ENEMY) {
			currentFactionTurn.value = Faction.PLAYER;
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnEndTurn();
			}
			currentTurn.value++;
			currentState = TurnState.REINFORCE;
			checkReinforcementsEvent.Invoke();
		}
		else {
			Debug.LogError("Wrong state!");
		}
	}

	/// <summary>
	/// Checks to see if the win/lose condition has been met, displays a message and ends the game.
	/// </summary>
	public void CheckGameFinished() {
		// Check if any players are alive
		bool gameFinished = true;
		MapEntry map = (MapEntry)currentMap.value;
		// Normal lose condition
		if (map.loseCondition == LoseCondition.NORMAL) {
			for (int i = 0; i < playerList.values.Count; i++) {
				if (playerList.values[i].IsAlive()) {
					gameFinished = false;
				}
				else if (playerList.values[i].stats.charData.mustSurvive) {
					gameFinished = true;
					Debug.Log("Uh oh!");
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
	/// When clearing the map, show win text for a while and then move 
	/// on to post dialogue.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndGameWin() {
		gameFinishText.text = "BATTLE WON";
		gameFinishText.gameObject.SetActive(true);
		gameFinishObject.SetActive(true);
		sfxQueue.Enqueue(victoryFanfare);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(4f);
		gameFinishObject.SetActive(false);
		gameFinishText.gameObject.SetActive(false);
		currentDialogueMode.value = (int)DialogueMode.POST;
		currentDialogue.value = ((MapEntry)currentMap.value).postDialogue;
		startDialogueEvent.Invoke();
	}

	/// <summary>
	/// When losing the map, show lose text and then the game over menu.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndGameLose() {
		currentState = TurnState.FINISHED;
		gameFinishText.text = "GAME OVER";
		gameFinishText.gameObject.SetActive(true);
		gameFinishObject.SetActive(true);
		sfxQueue.Enqueue(gameOverFanfare);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(2f);
		gameLoseEvent.Invoke();
	}

	/// <summary>
	/// Coroutine which locks the controls and shows the turn change display.
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	private IEnumerator DisplayTurnChange(float duration) {
		lockControls.value = true;
		currentMode.value = ActionMode.NONE;
		currentMenuMode.value = (int)MenuMode.NONE;
		turnChangeText.text = currentFactionTurn.value + " TURN";

		yield return null;

		turnChangeDisplay.SetActive(true);
		resetSelections.Invoke();
		menuModeChangedEvent.Invoke();
		sfxQueue.Enqueue(turnChangeFanfare);
		playSfxEvent.Invoke();

		yield return new WaitForSeconds(duration);

		turnChangeDisplay.SetActive(false);
		StartTurn();
	}

	/// <summary>
	/// Runs at the start of each turn.
	/// Activates all the characters on the current faction.
	/// </summary>
	private void StartTurn() {
		if (gameover)
			return;
		
		currentMode.value = ActionMode.NONE;
		if (currentFactionTurn.value == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnStartTurn();
			}
			enemyController.RunEnemies();
			currentMenuMode.value = (int)MenuMode.NONE;
		}
		else if (currentFactionTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnStartTurn();
			}
			lockControls.value = false;
			currentMenuMode.value = (int)MenuMode.MAP;
		}
		else {
			Debug.LogError("Wrong state!");
		}
		menuModeChangedEvent.Invoke();
	}

	public void MoveToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}
