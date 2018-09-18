using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : TacticsMove {

	public IntVariable currentMenuMode;
	public UnityEvent menuModeChangedEvent;


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
	protected override void EndMovement() {
		Debug.Log("Finished move");
		isMoving = false;
		lockControls.value = false;
		currentMenuMode.value = (int)MenuMode.UNIT;
		menuModeChangedEvent.Invoke();
	}
	
	/// <summary>
	/// Adds attackable to all tiles surrounding the character depending on range.
	/// </summary>
	/// <param name="range1"></param>
	/// <param name="range2"></param>
	public void ShowAttackTiles(WeaponRange range) {
		if (!IsAlive())
			return;

		for (int i = 0; i < mapCreator.tiles.Length; i++) {			
			if (range.InRange(MapCreator.DistanceTo(this, mapCreator.tiles[i])))
				mapCreator.tiles[i].attackable = true;
		}
	}
}
