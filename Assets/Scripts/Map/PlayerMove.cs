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
		Debug.Log("Spawned " + stats.charData.charName);
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
	public void ShowAttackTiles(bool range1, bool range2) {
		if (!IsAlive())
			return;
			
		if (range1) {
			MapTile tile = mapCreator.GetTile(posx+1,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy-1);
			if (tile != null) tile.attackable = true;
		}
		if (range2) {
			MapTile tile = mapCreator.GetTile(posx+2,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy+2);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-2,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy-2);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx+1,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy-1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx+1,posy-1);
			if (tile != null) tile.attackable = true;
		}
	}
}
