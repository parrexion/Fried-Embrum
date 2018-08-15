using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapClicker : MonoBehaviour {

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
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL) {
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
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL) {
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
		if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL) {
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


	// public void CharacterClicked(int x, int y) {
		// MapTile tile = mapCreator.GetTile(x, y);
		// TacticsMove temp = tile.currentCharacter;

		// switch (currentMode.value) {
		// 	case ActionMode.NONE:
		// 		FirstClick(x, y);
		// 		// lastSelectedCharacter.value = temp;
		// 		// if (lastSelectedCharacter.value != null) {
		// 		// 	lastSelectedCharacter.value.FindAllMoveTiles(false);
		// 		// 	if (lastSelectedCharacter.value.faction == Faction.PLAYER && !lastSelectedCharacter.value.hasMoved) {
		// 		// 		currentMode.value = ActionMode.MOVE;
		// 		// 	}
		// 		// }
		// 		// else {
		// 		// 	mapCreator.ResetMap();
		// 		// }
		// 		break;
			
		// 	case ActionMode.MOVE:
		// 	case ActionMode.ATTACK:
		// 	case ActionMode.HEAL:
		// 		// if (temp == null) {
		// 		// 	attackTarget.value = null;
		// 		// 	if (tile.selectable) {
		// 		// 		if (tile == lastTarget.value) {
		// 		// 			EndDrag();
		// 		// 		}
		// 		// 		else {
		// 		// 			currentMode.value = ActionMode.MOVE;
		// 		// 			lastTarget.value = tile;
		// 		// 			lastSelectedCharacter.value.ShowMove(tile);
		// 		// 		}
		// 		// 	}
		// 		// 	else {
		// 		// 		currentMode.value = ActionMode.NONE;
		// 		// 		currentMode.value = ActionMode.NONE;
		// 		// 		lastSelectedCharacter.value = null;
		// 		// 		lastTarget.value = null;
		// 		// 		mapCreator.ResetMap();
		// 		// 	}
		// 		// }
		// 		// else if (temp == lastSelectedCharacter.value) {
		// 		// 	lastSelectedCharacter.value = null;
		// 		// 	lastTarget.value = null;
		// 		// 	attackTarget.value = null;
		// 		// 	currentMode.value = ActionMode.NONE;
		// 		// 	mapCreator.ResetMap();
		// 		// }
		// 		// else if (tile.attackable && temp.faction == Faction.ENEMY) {
		// 		// 	if (tile == attackTarget.value) {
		// 		// 		EndDrag();
		// 		// 	}
		// 		// 	else {
		// 		// 		currentMode.value = ActionMode.ATTACK;
		// 		// 		attackTarget.value = tile;
		// 		// 		lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
		// 		// 		lastSelectedCharacter.value.ShowMove(lastTarget.value);
		// 		// 	}
		// 		// }
		// 		// else if (tile.supportable && temp.faction == Faction.PLAYER) {
		// 		// 	if (tile == attackTarget.value) {
		// 		// 		EndDrag();
		// 		// 	}
		// 		// 	else {
		// 		// 		currentMode.value = ActionMode.HEAL;
		// 		// 		attackTarget.value = tile;
		// 		// 		lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
		// 		// 		lastSelectedCharacter.value.ShowMove(lastTarget.value);
		// 		// 	}
		// 		// }
		// 		break;
		// 	default:
		// 		return;
		// }

		// characterClicked.Invoke();
		// hideTooltipEvent.Invoke();
	// }

	// public void MapClick(int x, int y) {
	// 	MapTile tile = mapCreator.GetTile(x, y);
	// 	TacticsMove temp = tile.currentCharacter;
	// 	if (currentActionMode.value == ActionMode.NONE) {
	// 		if (selectTarget.value != null) {
	// 			if (selectTarget.value.faction == Faction.PLAYER && !selectTarget.value.hasMoved) {
	// 				currentActionMode.value = ActionMode.MOVE;
	// 				Debug.Log("Click!");
	// 			}
	// 		}
	// 	}
	// 	else if (currentActionMode.value == ActionMode.MOVE) {
	// 		if (temp == null) {
	// 			attackTarget.value = null;
	// 			if (tile.selectable) {
	// 				if (moveTarget.value != null) {
	// 					selectTarget.value.Move();
	// 				}
	// 				Debug.Log("OK move");
	// 			}
	// 			else {
	// 				// currentMode.value = ActionMode.NONE;
	// 				// currentMode.value = ActionMode.NONE;
	// 				// lastSelectedCharacter.value = null;
	// 				// lastTarget.value = null;
	// 				// mapCreator.ResetMap();
	// 				Debug.Log("No select");
	// 			}
	// 		}
	// 		else if (temp == selectTarget.value) {
	// 			selectTarget.value = null;
	// 			moveTarget.value = null;
	// 			attackTarget.value = null;
	// 			currentActionMode.value = ActionMode.NONE;
	// 			mapCreator.ResetMap();
	// 			Debug.Log("Same!");
	// 		}
	// 		else {
	// 			Debug.Log("What could it be?");
	// 		}
	// 	}
	// 	else {
	// 		Debug.Log("Hmm :/");
	// 	}
	// }
}
