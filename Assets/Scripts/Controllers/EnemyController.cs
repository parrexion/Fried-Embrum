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

	[Header("Events")]
	public UnityEvent charClicked;
	public UnityEvent cursorMovedEvent;
	public UnityEvent endTurnEvent;
	
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
			NPCMove enemy = (NPCMove)enemyList.values[i];
			enemy.FindAllMoveTiles(false);
			cursorX.value = enemy.posx;
			cursorY.value = enemy.posy;
			cursorMovedEvent.Invoke();

			yield return new WaitForSeconds(1f);

			// Calculate the tile to move towards and wait for the character to move there
			Debug.Log("Move time");
			enemy.CalculateMovement();
			while (enemy.isMoving)
				yield return null;

			// Calculate which character to attack/support and waits for the battle scene to finish
			Debug.Log("Attack time");
			waitForNextAction = enemy.CalculateAttacksHeals();
			while (waitForNextAction)
				yield return null;

			// Finish the turn
			Debug.Log("End turn");
			enemy.End();
		}

		cursorX.value = playerList.values[0].posx;
		cursorY.value = playerList.values[0].posy;
		cursorMovedEvent.Invoke();
		isRunning = false;
		endTurnEvent.Invoke();
	}

	/// <summary>
	/// Sets the wait bool to false which means the enemy loop will continue.
	/// </summary>
	public void BattleFinishedQue() {
		waitForNextAction = false;
	}
}
