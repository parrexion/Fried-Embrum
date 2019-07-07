using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapCursor : MonoBehaviour {

	public BattleMap battleMap;
	public IntVariable currentMenumode;
	public ActionModeVariable currentActionMode;
	public FactionVariable currentFaction;

	[Header("Targets")]
	private MapTile startTile;
	public MapTileVariable moveTile;
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable selectTile;
	public MapTileVariable target;
	public CharacterListVariable playerCharacters;
	public CharacterListVariable enemyCharacters;

	[Header("Cursor")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public float zHeight = -0.75f;
	public SpriteRenderer cursorSprite;

	[Header("Events")]
	public UnityEvent updateCharacterUI;
	public UnityEvent cursorMovedEvent;
	public UnityEvent returnToPrepEvent;

	[Header("Settings")]
	public BoolVariable alwaysShowMovement;

	private bool _dangerAreaActive;


	/// <summary>
	/// Initialization
	/// </summary>	
	private void Start() {
		currentActionMode.value = ActionMode.NONE;
		ResetTargets();
		DangerAreaToggle(false);
	}

	public bool Move(int xDir, int yDir) {
		int prevX = cursorX.value;
		int prevY = cursorY.value;
		cursorX.value = Mathf.Clamp(cursorX.value + xDir, 0, battleMap.SizeX() - 1);
		cursorY.value = Mathf.Clamp(cursorY.value + yDir, 0, battleMap.SizeY() - 1);
		cursorMovedEvent.Invoke();
		UpdateCursor();
		CursorHover();

		return (prevX != cursorX.value || prevY != cursorY.value);
	}

	/// <summary>
	/// Makes the cursor jump to the next character in line.
	/// </summary>
	public void JumpCursor() {
		if (selectCharacter.value == null) {
			// No selected character - move to next character to move
			for (int i = 0; i < playerCharacters.values.Count; i++) {
				if (!playerCharacters.values[i].hasMoved) {
					cursorX.value = playerCharacters.values[i].posx;
					cursorY.value = playerCharacters.values[i].posy;
					cursorMovedEvent.Invoke();
					break;
				}
			}
		}
		else if (selectCharacter.value.faction == Faction.PLAYER) {
			// Player selected - Jump to next player that hasn't moved
			int pos = 0;
			for (int i = 0; i < playerCharacters.values.Count; i++) {
				pos = i;
				TacticsMove tactics = playerCharacters.values[i];
				if (tactics.posx == cursorX.value && tactics.posy == cursorY.value)
					break;
			}
			int startPos = pos;
			do {
				pos = OPMath.FullLoop(0, playerCharacters.values.Count, pos + 1);
			} while (startPos != pos && playerCharacters.values[pos].hasMoved);

			cursorX.value = playerCharacters.values[pos].posx;
			cursorY.value = playerCharacters.values[pos].posy;
			cursorMovedEvent.Invoke();
		}
		else if (selectCharacter.value.faction == Faction.ENEMY) {
			// Enemy selected - Jump to next enemy
			int pos = 0;
			for (int i = 0; i < enemyCharacters.values.Count; i++) {
				pos = i;
				TacticsMove tactics = enemyCharacters.values[i];
				if (tactics.posx == cursorX.value && tactics.posy == cursorY.value)
					break;
			}

			pos = OPMath.FullLoop(0, enemyCharacters.values.Count, pos + 1);
			cursorX.value = enemyCharacters.values[pos].posx;
			cursorY.value = enemyCharacters.values[pos].posy;
			cursorMovedEvent.Invoke();
		}
		UpdateCursor();
		CursorHover();
	}

	/// <summary>
	/// Called whenever the cursor position is updated.
	/// Handles both normal cursor movement and target selection.
	/// </summary>
	private void CursorHover() {
		UpdateCursor();
		if (currentActionMode.value == ActionMode.NONE)
			SelectHover();
		else if (currentActionMode.value == ActionMode.MOVE || currentActionMode.value == ActionMode.ACTION)
			MoveHover();
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL || currentActionMode.value == ActionMode.TRADE) {
			updateCharacterUI.Invoke();
		}
	}

	/// <summary>
	/// Executes a cursor click on the location depending on the current action mode.
	/// </summary>
	/// <returns></returns>
	public bool CursorClick(bool real) {
		if (currentActionMode.value == ActionMode.NONE)
			return SelectCharacter(real);
		else if (currentActionMode.value == ActionMode.MOVE)
			return SelectMoveTile(real);
		return false;
	}

	/// <summary>
	/// Backs out of the current state depending on the current state.
	/// </summary>
	public void CursorBack() {
		if (currentActionMode.value == ActionMode.MOVE) {
			cursorX.value = startTile.posx;
			cursorY.value = startTile.posy;
			currentActionMode.value = ActionMode.NONE;
			ResetTargets();
			cursorMovedEvent.Invoke();
		}
		UpdateCursor();
	}

	/// <summary>
	/// Used when hovering around with the cursor.
	/// Shows the movement if a character is hovered.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private void SelectHover() {
		// Debug.Log("Normal hover:  "+ currentFaction.value);
		MapTile tile = battleMap.GetTile(cursorX.value, cursorY.value);
		startTile = tile;
		selectTile.value = tile;
		selectCharacter.value = (tile) ? tile.currentCharacter : null;
		if (currentFaction.value == Faction.PLAYER && selectCharacter.value != null && (alwaysShowMovement.value || !selectCharacter.value.hasMoved)) {
			selectCharacter.value.FindAllMoveTiles(false);
		}
		else {
			battleMap.ResetMap();
		}
		updateCharacterUI.Invoke();
	}

	/// <summary>
	/// Used when selecting a tile to move to.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private void MoveHover() {
		MapTile tile = battleMap.GetTile(cursorX.value, cursorY.value);
		if (currentMenumode.value == (int)MenuMode.FORMATION) {
			battleMap.ClearTargets();
			moveTile.value = tile;
			target.value = tile;
			selectCharacter.value.currentTile.target = true;
			tile.target = true;
			updateCharacterUI.Invoke();
		}
		else {
			if (tile.selectable) {
				moveTile.value = tile;
				selectCharacter.value.ShowMove(tile);
				updateCharacterUI.Invoke();
			}
			else {
				moveTile.value = null;
				battleMap.ClearMovement();
			}
		}

		// Add features to allow the play to attack and heal target with movement.
	}

	/// <summary>
	/// Selects the currently hovered character if not doing any other actions.
	/// If no character is hovered, show the in-game menu instead.
	/// </summary>
	private bool SelectCharacter(bool playing) {
		if (selectCharacter.value == null || selectCharacter.value.hasMoved || selectCharacter.value.faction != Faction.PLAYER) {
			if (playing)
				InputDelegateController.instance.TriggerMenuChange(MenuMode.INGAME);
			return playing;
		}
		else {
			if (!playing && !selectCharacter.value.currentTile.deployable)
				return false;
			currentActionMode.value = ActionMode.MOVE;
			moveTile.value = battleMap.GetTile(cursorX.value, cursorY.value);
			moveTile.value.current = true;
			selectCharacter.value.path.Clear();
			return true;
		}
	}

	/// <summary>
	/// Moves the player to the currently selected move tile.
	/// </summary>
	private bool SelectMoveTile(bool playing) {
		if (!playing) {
			if (moveTile.value.deployable) {
				TacticsMove dual = moveTile.value.currentCharacter;
				MapTile startTile = selectCharacter.value.currentTile;
				selectCharacter.value.MoveDirectSwap(moveTile.value);
				if (dual) {
					dual.MoveDirectSwap(startTile);
				}
				else {
					startTile.currentCharacter = null;
				}
				return true;
			}
		}
		else {
			if (moveTile.value != null) {
				selectCharacter.value.StartMove();
				return true;
			}
		}

		return false;

		// Add features to allow the player to attack and heal target with movement.
	}

	/// <summary>
	/// Updates the position of the cursor depending on the menu mode.
	/// </summary>
	private void UpdateCursor() {
		if (currentFaction.value == Faction.ENEMY) {
			transform.position = new Vector3(cursorX.value, cursorY.value, zHeight);
		}
		else if (currentActionMode.value == ActionMode.ATTACK || currentActionMode.value == ActionMode.HEAL || currentActionMode.value == ActionMode.TRADE) {
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
		selectCharacter.value.UndoMove();
		moveTile.value = null;
		MoveHover();
		cursorMovedEvent.Invoke();
	}

	/// <summary>
	/// Resets all the targets.
	/// </summary>
	public void ResetTargets() {
		selectTile.value = null;
		selectCharacter.value = null;
		moveTile.value = null;
		battleMap.ResetMap();
	}

	/// <summary>
	/// Toggles the danger area if doToggle is true. Always disables it otherwise.
	/// </summary>
	/// <param name="doToggle"></param>
	public void DangerAreaToggle(bool doToggle) {
		_dangerAreaActive = (doToggle) ? !_dangerAreaActive : false;

		battleMap.ClearDangerous();
		if (_dangerAreaActive) {
			for (int i = 0; i < enemyCharacters.values.Count; i++) {
				if (!enemyCharacters.values[i].IsAlive())
					continue;
				enemyCharacters.values[i].FindAllMoveTiles(true);
			}
		}
		if (doToggle) {
			CursorHover();
		}
	}

	/// <summary>
	/// Updates the reach of the danger area.
	/// </summary>
	public void UpdateDangerArea() {
		if (!_dangerAreaActive)
			return;

		battleMap.ClearDangerous();
		if (_dangerAreaActive) {
			for (int i = 0; i < enemyCharacters.values.Count; i++) {
				if (!enemyCharacters.values[i].IsAlive())
					continue;
				enemyCharacters.values[i].FindAllMoveTiles(true);
			}
		}
		CursorHover();
	}

	/// <summary>
	/// Resets the targets after swapping places during preparations.
	/// </summary>
	public void PrepSwapped() {
		currentActionMode.value = ActionMode.NONE;
		ResetTargets();
		SelectHover();
		cursorMovedEvent.Invoke();
	}

}
