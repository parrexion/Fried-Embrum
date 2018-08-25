using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapCursor : MonoBehaviour {

	public MapCreator mapCreator;
	public ActionModeVariable currentActionMode;

	[Header("Targets")]
	private MapTile startTile;
	public MapTileVariable moveTile;
	public MapTileVariable attackTile;
	public TacticsMoveVariable selectTarget;
	public TacticsMoveVariable target;
	public CharacterListVariable enemyCharacters;

	[Header("Cursor")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public float zHeight = -0.75f;
	public SpriteRenderer cursorSprite;

	[Header("Events")]
	public UnityEvent updateCharacterUI;
	public UnityEvent hideTooltipEvent;
	public UnityEvent cursorMovedEvent;
	public UnityEvent showIngameMenuEvent;

	[Header("Settings")]
	public BoolVariable alwaysShowMovement;

	private bool _dangerAreaActive;
	

	/// <summary>
	/// Initialization
	/// </summary>	
	private void Start() {
		ResetTargets();
		DangerAreaToggle(false);
	}

	/// <summary>
	/// Called whenever the cursor position is updated.
	/// Handles both normal cursor movement and target selection.
	/// </summary>
	public void CursorHover() {
		MoveCursor();
		if (currentActionMode.value == ActionMode.NONE)
			NormalHover(cursorX.value, cursorY.value);
		else if (currentActionMode.value == ActionMode.MOVE)
			MoveHover(cursorX.value, cursorY.value);
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL || currentActionMode.value == ActionMode.TRADE) {
			updateCharacterUI.Invoke();
		}
	}

	public void CursorClick() {
		if (currentActionMode.value == ActionMode.NONE)
			SelectCharacter();
		else if (currentActionMode.value == ActionMode.MOVE)
			SelectMoveTile();
	}

	public void CursorBack() {
		if (currentActionMode.value == ActionMode.MOVE) {
			cursorX.value = startTile.posx;
			cursorY.value = startTile.posy;
			currentActionMode.value = ActionMode.NONE;
			ResetTargets();
			cursorMovedEvent.Invoke();
			Debug.Log("Go back to the shadows!");
		}
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL || currentActionMode.value == ActionMode.TRADE) {
			currentActionMode.value = ActionMode.MOVE;
			MoveCursor();
			Debug.Log("Let's do something else");
		}
	}

	/// <summary>
	/// Used when hovering around with the cursor.
	/// Shows the movement if a character is hovered.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private void NormalHover(int x, int y) {
		MapTile tile = mapCreator.GetTile(x, y);
		startTile = tile;
		selectTarget.value = tile.currentCharacter;
		if (selectTarget.value != null && (alwaysShowMovement.value || !selectTarget.value.hasMoved)) {
			selectTarget.value.FindAllMoveTiles(false);
		}
		else {
			mapCreator.ResetMap();
		}
		updateCharacterUI.Invoke();
		hideTooltipEvent.Invoke();
	}

	/// <summary>
	/// Used when selecting a tile to move to.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private void MoveHover(int x, int y) {
		MapTile tile = mapCreator.GetTile(x, y);
		if (tile.selectable) {
			moveTile.value = tile;
			selectTarget.value.ShowMove(tile);
		}
		else {
			moveTile.value = null;
			mapCreator.ClearMovement();
		}

		// Add features to allow the play to attack and heal target with movement.
	}

	/// <summary>
	/// Selects the currently hovered character if not doing any other actions.
	/// If no character is hovered, show the in-game menu instead.
	/// </summary>
	private void SelectCharacter() {
		if (selectTarget.value != null) {
			if (selectTarget.value.faction == Faction.PLAYER && !selectTarget.value.hasMoved) {
				currentActionMode.value = ActionMode.MOVE;
				moveTile.value = mapCreator.GetTile(cursorX.value, cursorY.value);
				moveTile.value.current = true;
				Debug.Log("Click!");
			}
		}
		else {
			Debug.Log("Show other menu. End turn etc.");
			showIngameMenuEvent.Invoke();
		}
	}

	/// <summary>
	/// Moves the player to the currently selected move tile.
	/// </summary>
	private void SelectMoveTile() {
		if (moveTile.value != null) {
			selectTarget.value.StartMove();
			Debug.Log("OK move");
		}

		// Add features to allow the play to attack and heal target with movement.
	}

	/// <summary>
	/// Updates the position of the cursor depending on the menu mode.
	/// </summary>
	private void MoveCursor() {
		if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL || currentActionMode.value == ActionMode.TRADE) {
			transform.position = new Vector3(target.value.posx, target.value.posy, zHeight);
		}
		else {
			transform.position = new Vector3(cursorX.value, cursorY.value, zHeight);
		}
	}

	/// <summary>
	/// Undos the last movement and resets the cursor back to the start tile.
	/// </summary>
	public void UndoMove() {
		cursorX.value = startTile.posx;
		cursorY.value = startTile.posy;
		selectTarget.value.UndoMove(startTile);
		moveTile.value = null;
		MoveHover(cursorX.value, cursorY.value);
		cursorMovedEvent.Invoke();
	}

	/// <summary>
	/// Resets all the targets.
	/// </summary>
	public void ResetTargets() {
		
		Debug.Log("Actionmode");
		selectTarget.value = null;
		moveTile.value = null;
		attackTile.value = null;
		mapCreator.ResetMap();
	}

	/// <summary>
	/// Toggles the danger area if toggle is true. Disables it otherwise.
	/// </summary>
	/// <param name="toggle"></param>
	public void DangerAreaToggle(bool toggle) {
		_dangerAreaActive = (toggle) ? !_dangerAreaActive : false;
		
		mapCreator.ClearReachable();
		if (_dangerAreaActive) {
			for (int i = 0; i < enemyCharacters.values.Count; i++) {
				enemyCharacters.values[i].FindAllMoveTiles(true);
			}
		}
	}

}
