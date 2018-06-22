using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnController : MonoBehaviour {
	
	[Header("Objects")]
	public MapCreator mapCreator;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;

	public BoolVariable lockControls;
	public FactionVariable currentTurn;
	public ActionModeVariable currentMode;
	public TacticsMoveVariable selectCharacter;

	[Header("UI")]
	public GameObject turnChangeDisplay;
	public Text turnChangeText;

	[Header("Game Finished")]
	public GameObject gameFinishObject;
	public Text gameFinishText;

	public UnityEvent charClicked;
	public UnityEvent saveGameEvent;
	public UnityEvent loadGameEvent;
	public UnityEvent returnToMain;
	

	// Use this for initialization
	private void Awake() {
		currentTurn.value = Faction.PLAYER;
		playerList.values.Clear();
		enemyList.values.Clear();
		StartCoroutine(DisplayTurnChange(1.5f));
	}

	public void ButtonClickChangeTurn() {
		if (currentTurn.value == Faction.PLAYER)
			ChangeTurn();
	}

	private void ChangeTurn() {
		mapCreator.ResetMap();
		selectCharacter.value = null;
		if (currentTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnEndTurn();
			}
			currentTurn.value = Faction.ENEMY;
		}
		else if (currentTurn.value == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnEndTurn();
			}
			currentTurn.value = Faction.PLAYER;
		}
		else {
			Debug.LogError("Wrong state!");
		}
		StartCoroutine(DisplayTurnChange(1.5f));
	}

	private void StartTurn() {
		currentMode.value = ActionMode.NONE;
		selectCharacter.value = null;
		charClicked.Invoke();
		if (currentTurn.value == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				enemyList.values[i].OnStartTurn();
			}
			StartCoroutine(RunEnemyTurn());
		}
		else if (currentTurn.value == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].OnStartTurn();
			}
		}
		else {
			Debug.LogError("Wrong state!");
		}
	}

	public void CheckEndTurn() {
		for (int i = 0; i < playerList.values.Count; i++) {
			if (playerList.values[i].IsAlive() && !playerList.values[i].hasMoved) {
				lockControls.value = false;
				return;
			}
		}
		ChangeTurn();
		lockControls.value = true;
	}

	public void CheckGameFinished() {
		bool gameFinished = true;
		for (int i = 0; i < playerList.values.Count; i++) {
			if (playerList.values[i].IsAlive()) {
				gameFinished = false;
				break;
			}
		}
		if (gameFinished) {
			Debug.Log("GAME OVER");
			gameFinishText.text = "GAME OVER";
			gameFinishText.gameObject.SetActive(false);
			gameFinishObject.SetActive(true);
			loadGameEvent.Invoke();
			StartCoroutine(EndGame());
			return;
		}

		gameFinished = true;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (enemyList.values[i].IsAlive()) {
				gameFinished = false;
				break;
			}
		}
		if (gameFinished) {
			Debug.Log("BATTLE WON");
			gameFinishText.text = "BATTLE WON";
			gameFinishText.gameObject.SetActive(true);
			gameFinishObject.SetActive(true);
			saveGameEvent.Invoke();
			StartCoroutine(EndGame());
		}
	}

	private IEnumerator EndGame() {
		yield return new WaitForSeconds(2f);
		returnToMain.Invoke();
	}

	public bool CheckRange(TacticsMove player, bool isAttack) {
		if (isAttack) {
			WeaponItem weapon = player.GetWeapon(ItemCategory.WEAPON);
			if (weapon == null)
				return false;
			for (int i = 0; i < enemyList.values.Count; i++) {
				int distance = MapCreator.DistanceTo(player, enemyList.values[i]);
				if (weapon.InRange(distance)) {
					return true;
				}
			}
		}
		else {
			WeaponItem support = player.GetWeapon(ItemCategory.STAFF);
			if (support == null)
				return false;
			for (int i = 0; i < playerList.values.Count; i++) {
				if (!playerList.values[i].IsInjured())
					continue;
				int distance = MapCreator.DistanceTo(player, playerList.values[i]);
				if (support.InRange(distance)) {
					return true;
				}
			}
		}
		return false;
	}

	private IEnumerator RunEnemyTurn() {
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (!enemyList.values[i].IsAlive())
				continue;

			selectCharacter.value = enemyList.values[i];
			charClicked.Invoke();
			Debug.Log(enemyList.values[i].gameObject.name);
			mapCreator.ResetMap();
			enemyList.values[i].FindAllMoveTiles(false);
			yield return new WaitForSeconds(1f);
			lockControls.value = true;
			enemyList.values[i].CalculateMovement();
			Debug.Log("Move");
			while (lockControls.value)
				yield return null;
			lockControls.value = true;
			enemyList.values[i].CalculateAttacks();
			Debug.Log("Attack");
			while (lockControls.value)
				yield return null;
			enemyList.values[i].End();
			Debug.Log("End");
		}

		ChangeTurn();
	}

	private IEnumerator DisplayTurnChange(float duration) {
		lockControls.value = true;
		currentMode.value = ActionMode.NONE;
		selectCharacter.value = null;
		turnChangeText.text = currentTurn.value + " TURN";
		turnChangeDisplay.SetActive(true);
		charClicked.Invoke();
		yield return new WaitForSeconds(duration);
		turnChangeDisplay.SetActive(false);
		StartTurn();
		lockControls.value = false;
		charClicked.Invoke();
	}

	public void SetBusy(bool bbusy) {
		lockControls.value = bbusy;
	}
}
