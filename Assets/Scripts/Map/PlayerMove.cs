﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : TacticsMove {

	public IntVariable currentMenuMode;
	public UnityEvent playerFinishedMoveEvent;
	public UnityEvent prepMoveEndEvent;


	/// <summary>
	/// Additional setup for player characters.
	/// </summary>
	protected override void SetupLists() {
		playerList.values.Add(this);
		Debug.Log("Spawned " + stats.charData.entryName);
	}
	
	/// <summary>
	/// Additional functions which run when the player ends their turn.
	/// </summary>
	public override void EndMovement() {
		//Debug.Log("Finished move");
		isMoving = false;
		lockControls.value = false;
		if (currentMenuMode.value == (int)MenuMode.FORMATION) {
			prepMoveEndEvent.Invoke();
		}
		else {
			playerFinishedMoveEvent.Invoke();
		}
	}
	
	/// <summary>
	/// Adds attackable to all tiles surrounding the character depending on range.
	/// </summary>
	/// <param name="range1"></param>
	/// <param name="range2"></param>
	public void ShowAttackTiles(WeaponRange range, int damage) {
		if (!IsAlive())
			return;

		for (int i = 0; i < battleMap.tiles.Length; i++) {			
			if (range.InRange(BattleMap.DistanceTo(this, battleMap.tiles[i]))) {
				battleMap.tiles[i].attackable = true;
				if (damage >= currentHealth) {
					damage += 100;
				}
				battleMap.tiles[i].value = damage;
			}
		}
	}
}
