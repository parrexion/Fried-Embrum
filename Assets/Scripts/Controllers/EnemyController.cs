using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
	
	[Header("References")]
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable selectTile;
	public IntVariable battleWeaponIndex;
	public IntVariable currentPage;
	public IntVariable cursorX;
	public IntVariable cursorY;

	[Header("Settings")]
	public BoolVariable selectMainCharacter;
	public IntVariable slowGameSpeed;
	public IntVariable currentGameSpeed;

	[Header("Spinner")]
	public MySpinner spinner;
	public SfxEntry destroyedTileSfx;

	[Header("Events")]
	public UnityEvent charClicked;
	public UnityEvent cursorMovedEvent;
	public UnityEvent nextStateEvent;
	
	private NPCMove enemy;
	private bool isRunning;
	private bool waitForNextAction;


	/// <summary>
	/// Starts the loop which runs the enemies' turns.
	/// </summary>
	public void RunEnemies() {
		if (!isRunning)
			StartCoroutine(RunEnemyTurn());
	}

	/// <summary>
	/// Takes each enemy that is alive and runs their turn.
	/// </summary>
	/// <returns></returns>
	private IEnumerator RunEnemyTurn() {
		isRunning = true;

		battleWeaponIndex.value = 0;
		currentPage.value = 0;

		for (int i = 0; i < enemyList.values.Count; i++) {
			if (!enemyList.values[i].IsAlive())
				continue;

			// Select the next enemy and show its movement
			Debug.Log(enemyList.values[i].gameObject.name + " turn");
			selectCharacter.value = enemyList.values[i];
			selectTile.value = selectCharacter.value.currentTile;
			enemy = (NPCMove)enemyList.values[i];
			// enemy.FindAllMoveTiles(false);
				cursorX.value = enemy.posx;
				cursorY.value = enemy.posy;
				cursorMovedEvent.Invoke();

			// Calculate the tile to move towards and wait for the character to move there if any
			MapTile moveTile = enemy.CalculateMovement();
			if (moveTile == null) {
				Debug.Log("No tiles!");
				enemy.EndMovement();
				waitForNextAction = false;
			}
			else {
				enemy.ShowMove(moveTile);
				waitForNextAction = true;
			}

			if (waitForNextAction) {
				yield return new WaitForSeconds(1f * slowGameSpeed.value / currentGameSpeed.value);
				enemy.StartMove();
			}
			while (waitForNextAction)
				yield return null;

			// Calculate which character to attack/support and waits for the battle scene to finish
			// Debug.Log("Attack time");
			waitForNextAction = enemy.CalculateAttacksHeals();
			while (waitForNextAction)
				yield return null;

			// Finish the turn
			// Debug.Log("End turn");
			enemy.End();
		}

		if (selectMainCharacter.value) {
			cursorX.value = playerList.values[0].posx;
			cursorY.value = playerList.values[0].posy;
			cursorMovedEvent.Invoke();
		}
		isRunning = false;
		nextStateEvent.Invoke();
	}

	/// <summary>
	/// Sets the wait bool to false which means the enemy loop will continue.
	/// </summary>
	public void ActionFinishedQue() {
		waitForNextAction = false;
	}

	/// <summary>
	/// Called when an enemy is going to destroy a tile and pauses for that to happen.
	/// </summary>
	public void IncomingDestruction() {
		if (enemy != null) {
			StartCoroutine(DestroyTile());
		}
	}
	
	/// <summary>
	/// Runs the action of destroying the tile and showing a message to the player.
	/// </summary>
	/// <returns></returns>
	private IEnumerator DestroyTile() {
		string destroyType = (enemy.currentTile.interactType == InteractType.VILLAGE) ? "Village" : "???";
		MySpinnerData data = new MySpinnerData(){
			 icon = null,
			 sfx = destroyedTileSfx,
			 text = destroyType + " was destroyed!"
		};
		yield return StartCoroutine(spinner.ShowSpinner(data));
		waitForNextAction = false;
	}
}
