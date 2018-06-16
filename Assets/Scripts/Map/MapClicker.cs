using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapClicker : MonoBehaviour, IPointerDownHandler {
	public BoolVariable lockControls;
	public MapCreator mapCreator;

	public TacticsMoveVariable lastSelectedCharacter;
	public MapTileVariable lastTarget;
	public MapTileVariable attackTarget;
	public ActionModeVariable currentMode;
	public FactionVariable currentTurn;
	public CharacterListVariable enemyCharacters;

	public UnityEvent characterClicked;
	public UnityEvent hideTooltipEvent;

	private bool _dangerAreaActive;
	
	
	private void Start() {
		lastSelectedCharacter.value = null;
		lastTarget.value = null;
		currentMode.value = ActionMode.NONE;
	}

	public void CharacterClicked(int x, int y) {
		MapTile tile = mapCreator.GetTile(x, y);
		TacticsMove temp = tile.currentCharacter;

		switch (currentMode.value) {
			case ActionMode.NONE:
				lastSelectedCharacter.value = temp;
				if (lastSelectedCharacter.value != null) {
					lastSelectedCharacter.value.FindAllMoveTiles(false);
					if (lastSelectedCharacter.value.faction == Faction.PLAYER) {
						currentMode.value = ActionMode.MOVE;
					}
				}
				else {
					mapCreator.ResetMap();
				}
				break;
			
			case ActionMode.MOVE:
			case ActionMode.ATTACK:
			case ActionMode.HEAL:
				if (temp == null) {
					attackTarget.value = null;
					if (tile.selectable) {
						if (tile == lastTarget.value) {
							EndDrag();
						}
						else {
							currentMode.value = ActionMode.MOVE;
							lastTarget.value = tile;
							lastSelectedCharacter.value.ShowMove(tile);
						}
					}
					else {
						currentMode.value = ActionMode.NONE;
						currentMode.value = ActionMode.NONE;
						lastSelectedCharacter.value = null;
						lastTarget.value = null;
						mapCreator.ResetMap();
					}
				}
				else if (temp == lastSelectedCharacter.value) {
					lastSelectedCharacter.value = null;
					lastTarget.value = null;
					attackTarget.value = null;
					currentMode.value = ActionMode.NONE;
					mapCreator.ResetMap();
				}
				else if (tile.attackable && temp.faction == Faction.ENEMY) {
					if (tile == attackTarget.value) {
						EndDrag();
					}
					else {
						currentMode.value = ActionMode.ATTACK;
						attackTarget.value = tile;
						lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
						lastSelectedCharacter.value.ShowMove(lastTarget.value);
					}
				}
				else if (tile.supportable && temp.faction == Faction.PLAYER) {
					if (tile == attackTarget.value) {
						EndDrag();
					}
					else {
						currentMode.value = ActionMode.HEAL;
						attackTarget.value = tile;
						lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
						lastSelectedCharacter.value.ShowMove(lastTarget.value);
					}
				}
				break;
			default:
				return;
		}

		characterClicked.Invoke();
	}

	public void BeginDrag(int x, int y) {
		MapTile tile = mapCreator.GetTile(x,y);
		TacticsMove clickedChar = tile.currentCharacter;
		currentMode.value = ActionMode.NONE;
		lastTarget.value = null;
		attackTarget.value = null;
		mapCreator.ResetMap();
		tile.current = true;
		if (clickedChar) {
			lastSelectedCharacter.value = clickedChar;
			if (!clickedChar.hasMoved) {
				lastSelectedCharacter.value.FindAllMoveTiles(false);
				if (clickedChar.faction == currentTurn.value)
					currentMode.value = ActionMode.MOVE;
			}
		}
		else {
			lastSelectedCharacter.value = null;
			Debug.Log("NULL!?");
		}
		characterClicked.Invoke();
		hideTooltipEvent.Invoke();
	}

	public void DuringDrag(int x, int y) {
		MapTile tile = mapCreator.GetTile(x,y);
		if (tile == null)
			return;
		TacticsMove clickedChar = tile.currentCharacter;

		if (clickedChar != null && clickedChar == lastSelectedCharacter.value) {
			mapCreator.ClearMovement();
			lastTarget.value = null;
			attackTarget.value = null;
			currentMode.value = ActionMode.MOVE;
		}
		else if (tile.selectable) {
			mapCreator.ClearMovement();
			tile.target = true;
			lastTarget.value = tile;
			attackTarget.value = null;
			lastSelectedCharacter.value.ShowMove(tile);
			currentMode.value = ActionMode.MOVE;
		}
		else if (tile.attackable && clickedChar != null) {
			tile.target = true;
			attackTarget.value = tile;
			currentMode.value = ActionMode.ATTACK;
			mapCreator.ClearMovement();
			lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
			lastTarget.value.target = true;
			lastSelectedCharacter.value.ShowMove(lastTarget.value);
		}
		else if (tile.supportable && clickedChar != null) {
			tile.target = true;
			attackTarget.value = tile;
			currentMode.value = ActionMode.HEAL;
			mapCreator.ClearMovement();
			lastTarget.value = lastSelectedCharacter.value.CalculateCorrectMoveTile(lastTarget.value, attackTarget.value);
			lastTarget.value.target = true;
			lastSelectedCharacter.value.ShowMove(lastTarget.value);
		}
		characterClicked.Invoke();
	}

	public void EndDrag() {
		if (lastTarget.value != null) {
			lastSelectedCharacter.value.Move();
		}
	}

	public void ResetTargets() {
		currentMode.value = ActionMode.NONE;
		lastSelectedCharacter.value = null;
		lastTarget.value = null;
		attackTarget.value = null;
	}

	public void BattleEnd() {
		currentMode.value = ActionMode.NONE;
		lastTarget.value = null;
		attackTarget.value = null;
		characterClicked.Invoke();
	}

	/// <summary>
	/// Handles the clicks that don't hit any of the characters in the scene.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerDown(PointerEventData eventData) {
		hideTooltipEvent.Invoke();
	
		if (lockControls.value)
			return;
		
		Debug.Log("Click");
		int x = Mathf.FloorToInt(0.5f + eventData.pointerCurrentRaycast.worldPosition.x);
		int y = Mathf.FloorToInt(0.5f + eventData.pointerCurrentRaycast.worldPosition.y);
		CharacterClicked(x,y);
	}

	public void EndButtonClicked() {
		if (lastSelectedCharacter.value != null && !lastSelectedCharacter.value.IsAlive())
			lastSelectedCharacter.value = null;
		lastTarget.value = null;
		attackTarget.value = null;
		currentMode.value = ActionMode.NONE;
		characterClicked.Invoke();
	}

	public void DangerAreaClicked(bool toggle) {
		if (toggle)
			_dangerAreaActive = !_dangerAreaActive;
		
		mapCreator.ClearReachable();
		if (_dangerAreaActive) {
			for (int i = 0; i < enemyCharacters.values.Count; i++) {
				enemyCharacters.values[i].FindAllMoveTiles(true);
			}
		}
	}

//	public void ShowAttack() {
//		if (lastSelectedCharacter.value == null || TurnController.busy)
//			return;
//
//		Debug.Log("Show your moves!");
//		currentMode.value = ActionMode.ATTACK;
//		mapCreator.ResetMap();
//		lastSelectedCharacter.value.FindAllAttackTiles();
//	}
//
//	public void ShowHeal() {
//		if (lastSelectedCharacter.value == null || TurnController.busy)
//			return;
//
//		Debug.Log("Show your heals!");
//		currentMode.value = ActionMode.HEAL;
//		mapCreator.ResetMap();
//		lastSelectedCharacter.value.FindAllHealTiles();
//	}

//	public void WaitCharacter() {
//		if (lastSelectedCharacter.value == null || TurnController.busy)
//			return;
//		
//		Debug.Log("Wait time!");
//		lastSelectedCharacter.value.End();
//	}
}
