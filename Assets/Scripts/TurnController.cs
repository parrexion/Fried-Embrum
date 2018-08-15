using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class which handles changing turns and triggers functionality when the turn is changed.
/// </summary>
public class TurnController : MonoBehaviour {
	
	[Header("Objects")]
	public EnemyController enemyController;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;

	public BoolVariable lockControls;
	public FactionVariable currentTurn;
	public ActionModeVariable currentMode;
	public IntVariable currentMenuMode;
	public BoolVariable autoEndTurn;

	[Header("UI")]
	public GameObject turnChangeDisplay;
	public Text turnChangeText;

	[Header("Game Finished")]
	public GameObject gameFinishObject;
	public Text gameFinishText;

	[Header("Events")]
	public UnityEvent menuModeChangedEvent;
	public UnityEvent resetSelections;
	public UnityEvent returnToMain;
	

	/// <summary>
	/// Clears character lists and starts the player's first turn.
	/// </summary>
	private void Awake() {
		currentTurn.value = Faction.PLAYER;
		playerList.values.Clear();
		enemyList.values.Clear();
		StartCoroutine(DisplayTurnChange(1.5f));
	}

	/// <summary>
	/// Auto-ends the turn if all the player characters have taken their turn if enabled.
	/// </summary>
	public void CheckEndTurn() {
		if (!autoEndTurn.value || currentTurn.value != Faction.PLAYER)
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
		if (currentTurn.value == Faction.PLAYER) {
			currentTurn.value = Faction.ENEMY;
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnEndTurn();
			}
		}
		else if (currentTurn.value == Faction.ENEMY) {
			currentTurn.value = Faction.PLAYER;
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnEndTurn();
			}
		}
		else {
			Debug.LogError("Wrong state!");
		}
		StartCoroutine(DisplayTurnChange(1.5f));
	}

	/// <summary>
	/// Checks to see if the win/lose condition has been met, displays a message and ends the game.
	/// </summary>
	public void CheckGameFinished() {
		// Check if any players are alive
		bool gameFinished = true;
		for (int i = 0; i < playerList.values.Count; i++) {
			if (playerList.values[i].IsAlive()) {
				gameFinished = false;
				break;
			}
		}
		if (gameFinished) {
			Debug.Log("GAME OVER");
			StartCoroutine(EndGame("GAME OVER"));
			return;
		}

		// Check if any enemies are alive
		gameFinished = true;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (enemyList.values[i].IsAlive()) {
				gameFinished = false;
				break;
			}
		}
		if (gameFinished) {
			Debug.Log("BATTLE WON");
			StartCoroutine(EndGame("BATTLE WON"));
			// return;
		}

		// Add more win/lose conditions
	}

	/// <summary>
	/// Short delay before return to the main menu after the map is finished.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndGame(string message) {
		gameFinishText.text = message;
		gameFinishText.gameObject.SetActive(true);
		gameFinishObject.SetActive(true);
		yield return new WaitForSeconds(2f);
		returnToMain.Invoke();
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
		turnChangeText.text = currentTurn.value + " TURN";

		yield return null;

		turnChangeDisplay.SetActive(true);
		resetSelections.Invoke();
		menuModeChangedEvent.Invoke();

		yield return new WaitForSeconds(duration);

		turnChangeDisplay.SetActive(false);
		StartTurn();
	}

	/// <summary>
	/// Runs at the start of each turn.
	/// Activates all the characters on the current faction.
	/// </summary>
	private void StartTurn() {
		currentMode.value = ActionMode.NONE;
		currentMenuMode.value = (int)MenuMode.MAP;
		menuModeChangedEvent.Invoke();
		if (currentTurn.value == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnStartTurn();
			}
			enemyController.RunEnemies();
		}
		else if (currentTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnStartTurn();
			}
			lockControls.value = false;
		}
		else {
			Debug.LogError("Wrong state!");
		}
	}

}
