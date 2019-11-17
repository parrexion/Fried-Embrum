using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

	[Header("References")]
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;
	public CharacterListVariable allyList;
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable selectTile;
	public IntVariable battleWeaponIndex;
	public IntVariable currentPage;
	public IntVariable cursorX;
	public IntVariable cursorY;

	[Header("Settings")]
	public FloatVariable currentGameSpeed;

	[Header("Spinner")]
	public MySpinner spinner;
	public SfxEntry destroyedTileSfx;

	[Header("Events")]
	public UnityEvent charClicked;
	public UnityEvent cursorMovedEvent;
	public UnityEvent nextStateEvent;

	private NPCMove tactics;
	private bool isRunning;
	private bool waitForNextAction;


	/// <summary>
	/// Starts the loop which runs the enemies' turns.
	/// </summary>
	public void RunEnemies(float startDelay) {
		if (!isRunning)
			StartCoroutine(RunNPCTurn(enemyList, startDelay));
	}

	/// <summary>
	/// Starts the loop which runs the allies' turns.
	/// </summary>
	/// <param name="startDelay"></param>
	public void RunAllies(float startDelay) {
		if (!isRunning)
			StartCoroutine(RunNPCTurn(allyList, startDelay));
	}

	/// <summary>
	/// Takes each enemy that is alive and runs their turn.
	/// </summary>
	/// <returns></returns>
	private IEnumerator RunNPCTurn(CharacterListVariable list, float startDelay) {
		isRunning = true;
		yield return new WaitForSeconds(startDelay * currentGameSpeed.value);

		battleWeaponIndex.value = 0;
		currentPage.value = 0;

		yield return new WaitForSeconds(2f * currentGameSpeed.value);

		for (int i = 0; i < list.Count; i++) {
			if (!list.values[i].IsAlive() || list.values[i].hasEscaped)
				continue;

			// Select the next enemy and show its movement
			//Debug.Log(list.values[i].gameObject.name + " turn");
			selectCharacter.value = list.values[i];
			selectTile.value = selectCharacter.value.currentTile;
			tactics = (NPCMove)list.values[i];
			// enemy.FindAllMoveTiles(false);
			cursorX.value = tactics.posx;
			cursorY.value = tactics.posy;
			cursorMovedEvent.Invoke();

			// Calculate the tile to move towards and wait for the character to move there if any
			MapTile moveTile = tactics.CalculateMovement();
			if (moveTile == null) {
				tactics.EndMovement();
				waitForNextAction = false;
			}
			else {
				tactics.ShowMove(moveTile);
				waitForNextAction = true;
			}

			if (waitForNextAction) {
				yield return new WaitForSeconds(1.5f * currentGameSpeed.value);
				tactics.StartMove();
			}
			while (waitForNextAction)
				yield return null;

			// Calculate which character to attack/support and waits for the battle scene to finish
			// Debug.Log("Attack time");
			bool res = waitForNextAction = tactics.CalculateAttacksHeals();
			while (waitForNextAction)
				yield return null;

			if (res) {
				yield return new WaitForSeconds(1f * currentGameSpeed.value);
			}
			// Finish the turn
			// Debug.Log("End turn");
			tactics.End();
			cursorX.value = tactics.posx;
			cursorY.value = tactics.posy;
			cursorMovedEvent.Invoke();
		}

		yield return new WaitForSeconds(3f * currentGameSpeed.value);

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
		if (tactics != null) {
			StartCoroutine(DestroyTile());
		}
	}

	/// <summary>
	/// Runs the action of destroying the tile and showing a message to the player.
	/// </summary>
	/// <returns></returns>
	private IEnumerator DestroyTile() {
		string destroyType = (tactics.currentTile.interactType == InteractType.VILLAGE) ? "Village" : "???";
		MySpinnerData data = new MySpinnerData() {
			icon = null,
			sfx = destroyedTileSfx,
			text = destroyType + " was destroyed!"
		};
		yield return StartCoroutine(spinner.ShowSpinner(data));
		waitForNextAction = false;
	}

	/// <summary>
	/// Called when an enemy is going to destroy a tile and pauses for that to happen.
	/// </summary>
	public void IncomingEscape() {
		if (tactics != null) {
			StartCoroutine(EscapeCharacter());
		}
	}

	/// <summary>
	/// Runs the action of escaping with an NPC character.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EscapeCharacter() {
		MySpinnerData data = new MySpinnerData() {
			icon = tactics.stats.charData.portraitSet.small,
			sfx = null,
			text = tactics.stats.charData.entryName + " escaped!"
		};
		yield return StartCoroutine(spinner.ShowSpinner(data));
		waitForNextAction = false;
	}

	/// <summary>
	/// Calculates how many enemies has been killed.
	/// </summary>
	/// <returns></returns>
	public int NumberOfKilledEnemies() {
		int sum = 0;

		for (int i = 0; i < enemyList.Count; i++) {
			if (!enemyList.values[i].IsAlive()) {
				sum++;
			}
		}

		return sum;
	}
}
