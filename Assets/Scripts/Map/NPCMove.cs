﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AggroType { WAIT, CHARGE, GUARD, BOSS, HUNT }

public class NPCMove : TacticsMove {

	public ActionModeVariable currentMode;
	public AggroType aggroType;
	public MapTileVariable moveTile;
	public SpriteRenderer bossCrest;
	public MapTile huntTile;

	public UnityEvent finishedMovingEvent;
	public UnityEvent destroyedTileEvent;


	/// <summary>
	/// Additional setup for enemy characters.
	/// </summary>
	protected override void SetupLists() {
		bossCrest.enabled = (aggroType == AggroType.BOSS);

		enemyList.values.Add(this);
		Debug.Log("Spawned  " + stats.charData.entryName);
	}


	
	///////////   MOVEMENT

	/// <summary>
	/// Generates possible moves and selects and moves the AI to the best one found.
	/// </summary>
	public bool CalculateMovement() {
		MapTile tileBestWpn = null, tileGoodWpn = null, tileBestStf = null, tileGoodStf = null;
		List<InventoryTuple> weapon = inventory.GetAllUsableItemTuple(ItemCategory.WEAPON, stats);
		List<InventoryTuple> staff = inventory.GetAllUsableItemTuple(ItemCategory.STAFF, stats);
		if (weapon.Count > 0) {
			FindBestTile(weapon, out tileBestWpn, out tileGoodWpn);
		}
		if (tileBestWpn == null && staff.Count > 0) {
			FindBestTile(staff, out tileBestStf, out tileGoodStf);
		}

		moveTile.value = (tileBestWpn != null) ? tileBestWpn :
					(tileBestStf != null) ? tileBestStf :
					(tileGoodWpn != null) ? tileGoodWpn : tileGoodStf;

		//Nowhere to move
		if (moveTile.value == null) {
			EndMovement();
			return false;
		}
		else {
			ShowMove(moveTile.value);
			StartMove();
			return true;
		}
	}

	/// <summary>
	/// Searches for the best tile to move to with the currently equipped weapon.
	/// If no tile can be reached within 1 turn, a good tile is picked instead and the 
	/// AI will be able to move towards that tile instead.
	/// </summary>
	/// <param name="weapons"></param>
	/// <param name="tileBest"></param>
	/// <param name="tileGood"></param>
	private void FindBestTile(List<InventoryTuple> weapons, out MapTile tileBest, out MapTile tileGood) {
		// Generate map links
		GenerateHitTiles(weapons);
		BFS();

		int moveSpeed = (aggroType == AggroType.GUARD || aggroType == AggroType.BOSS) ? 0 : stats.GetMovespeed();
		MapTile bestTile = null; // Reachable this turn
		MapTile goodTile = null; // Reachable in multiple turns

		//Hunting AI
		if (aggroType == AggroType.HUNT) {
			tileBest = huntTile;
			tileGood = null;
			while(tileBest.distance > moveSpeed) {
				tileBest = tileBest.parent;
			}
			tileBest.PrintPos();
			return;
		}

		// Go through all tiles and find the best one to move to or towards
		for (int i = 0; i < mapCreator.tiles.Length; i++) {
			MapTile tempTile = mapCreator.tiles[i];
			if ((!tempTile.attackable && !tempTile.supportable) || !tempTile.selectable)
				continue;
			
			tempTile.target = true;
			if (tempTile.distance <= moveSpeed) {
				if (IsBetterTile(bestTile, tempTile))
					bestTile = tempTile;
			}
			else {
				if (IsBetterTile(goodTile, tempTile))
					goodTile = tempTile;
			}
		}

		if (bestTile) {
			// Found a best tile to move to
			bestTile.current = true;
			currentMode.value = (weapons[0].item.itemCategory == ItemCategory.WEAPON) ? ActionMode.ATTACK : ActionMode.HEAL;
			Debug.Log("That's the best");
			tileBest = bestTile;
			tileGood = null;
			if (aggroType == AggroType.WAIT)
				aggroType = AggroType.CHARGE;
		}
		else if (!goodTile || aggroType != AggroType.CHARGE) {
			//Have no weapons that can be used
			Debug.Log("Nothing is good!!");
			currentMode.value = ActionMode.NONE;
			tileBest = null;
			tileGood = null;
		}
		else {
			//The finds the tile which takes the character towards the good tile
			while (goodTile.distance > moveSpeed || !goodTile.IsEmpty()) {
				goodTile = goodTile.parent;
			}

			goodTile.current = true;
			currentMode.value = ActionMode.MOVE;
			Debug.Log("That's good enough");
			tileBest = null;
			tileGood = goodTile;
		}
	}

	/// <summary>
	/// Takes a weapon and shows the possible tiles it can be used on to define where the AI 
	/// can do things. Each opponent or each ally depending on weapon type.
	/// </summary>
	/// <param name="weapon"></param>
	private void GenerateHitTiles(List<InventoryTuple> weapons) {
		mapCreator.ResetMap();

		//Calculate range

		//Generate attack/support tiles
		if (weapons[0].item.itemCategory == ItemCategory.WEAPON) {
			WeaponRange reach = inventory.GetReach(ItemCategory.WEAPON);
			for (int i = 0; i < playerList.values.Count; i++) {
				((PlayerMove)playerList.values[i]).ShowAttackTiles(reach);
			}
		}
		else {
			WeaponRange reach = inventory.GetReach(ItemCategory.STAFF);
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this == enemyList.values[i])
					continue;
				bool isBuff = (weapons[0].item.itemType == ItemType.BUFF);
				if (isBuff || enemyList.values[i].IsInjured())
					((NPCMove)enemyList.values[i]).ShowSupportTiles(reach);
			}
		}
	}
	
	/// <summary>
	/// Does a BFS for the whole map, creating paths for the character to use when moving towards  
	/// the target tiles at the other characters.
	/// </summary>
	private void BFS() {
		Queue<MapTile> process = new Queue<MapTile>();
		process.Enqueue(currentTile);
		currentTile.distance = 0;
		currentTile.parent = null;
		
		WeaponRange weapon = inventory.GetReach(ItemCategory.WEAPON);
		WeaponRange staff = inventory.GetReach(ItemCategory.STAFF);
		
		bool isBuff = false;
		if (staff.max > 0)
			isBuff = (inventory.GetFirstUsableItem(ItemType.BUFF, stats) != null);

		while(process.Count > 0) {
			MapTile tile = process.Dequeue();
			tile.FindNeighbours(process, tile.distance, this, 1000, weapon, staff, false, false, isBuff);
		}
	}

	/// <summary>
	/// Compares two different tiles to see which one yields the better result for the AI.
	/// </summary>
	/// <param name="current"></param>
	/// <param name="challenger"></param>
	/// <returns></returns>
	private bool IsBetterTile(MapTile current, MapTile challenger) {
		if (current == null)
			return true;
		return challenger.distance < current.distance;
	}
	
	/// <summary>
	/// Ends the character's movement and clears the map.
	/// </summary>
	protected override void EndMovement() {
		Debug.Log("Finished move");
		isMoving = false;
		mapCreator.ResetMap();
		currentTile.current = true;

		if (aggroType == AggroType.HUNT && currentTile == huntTile) {
			aggroType = AggroType.CHARGE;
			huntTile.SetTerrain(huntTile.alternativeTerrain);
			destroyedTileEvent.Invoke();
		}
		else {
			Debug.Log("FALSE");
			finishedMovingEvent.Invoke();
		}
	}


	///////////   ATTACKS

	/// <summary>
	/// Calculates which character to attack/support from the current position.
	/// </summary>
	public bool CalculateAttacksHeals() {
		if (currentMode.value != ActionMode.ATTACK && currentMode.value != ActionMode.HEAL)
			return false;

		if (currentMode.value == ActionMode.ATTACK) {
			targetTile.value = FindRandomTarget(playerList, ItemCategory.WEAPON, false);
			int distance = MapCreator.DistanceTo(this, targetTile.value);
			inventory.EquipFirstInRangeItem(ItemCategory.WEAPON, stats, distance);
            Attack(targetTile.value);
		}
		else {
			targetTile.value = FindRandomTarget(enemyList, ItemCategory.STAFF, true);
			int distance = MapCreator.DistanceTo(this, targetTile.value);
			inventory.EquipFirstInRangeItem(ItemCategory.STAFF, stats, distance);
            Heal(targetTile.value);
		}
		return true;
	}

	/// <summary>
	/// Takes a list of characters and finds all which are in range and picks one randomly.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="category"></param>
	/// <param name="checkInjured"></param>
	/// <returns></returns>
	private MapTile FindRandomTarget(CharacterListVariable list, ItemCategory category, bool checkInjured) {
		List<MapTile> hits = new List<MapTile>();
		WeaponRange range = inventory.GetReach(category);

		for (int i = 0; i < list.values.Count; i++) {
			if (list.values[i] == this || !list.values[i].IsAlive() || (checkInjured && !list.values[i].IsInjured()))
				continue;
			int distance = MapCreator.DistanceTo(this, list.values[i]);
			if (range.InRange(distance)) {
				hits.Add(list.values[i].currentTile);
			}
		}

		hits.Shuffle();
		return hits[0];
	}
	
	/// <summary>
	/// Adds supportable to all tiles surrounding the character depending on range.
	/// </summary>
	/// <param name="range1"></param>
	/// <param name="range2"></param>
	public void ShowSupportTiles(WeaponRange range) {
		if (!IsAlive())
			return;

		for (int i = 0; i < mapCreator.tiles.Length; i++) {			
			if (range.InRange(MapCreator.DistanceTo(this, mapCreator.tiles[i])))
				mapCreator.tiles[i].supportable = true;
		}
	}
}
