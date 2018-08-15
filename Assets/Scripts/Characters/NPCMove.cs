using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove {

	public bool aggressive;
	public MapTileVariable targetTile;


	protected override void SetupLists() {
		enemyList.values.Add(this);
		Debug.Log(stats.charData.charName);
	}


	
	///////////   MOVEMENT

	/// <summary>
	/// Generates possible moves and selects and moves the AI to the best one found.
	/// </summary>
	public void CalculateMovement() {
		MapTile tileBestWpn = null, tileGoodWpn = null, tileBestStf = null, tileGoodStf = null;
		List<InventoryTuple> weapon = inventory.GetAllUsableItemTuple(ItemCategory.WEAPON, stats);
		List<InventoryTuple> staff = inventory.GetAllUsableItemTuple(ItemCategory.STAFF, stats);
		if (weapon.Count > 0) {
			FindBestTile(weapon, out tileBestWpn, out tileGoodWpn);
		}
		if (tileBestWpn == null && staff.Count > 0) {
			FindBestTile(staff, out tileBestStf, out tileGoodStf);
		}

		targetTile.value = (tileBestWpn != null) ? tileBestWpn :
					(tileBestStf != null) ? tileBestStf :
					(tileGoodWpn != null) ? tileGoodWpn : tileGoodStf;

		//Nowhere to move
		if (targetTile.value == null) {
			EndMovement();
		}
		else {
			ShowMove(targetTile.value);
			StartMove();
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

		int moveSpeed = stats.GetMovespeed();
		MapTile bestTile = null; // Reachable this turn
		MapTile goodTile = null; // Reachable in multiple turns

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
		}
		else if (!goodTile || !aggressive) {
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
		bool range1 = false;
		bool range2 = false; 
		for (int w = 0; w < weapons.Count; w++) {
			range1 = (weapons[w].item.InRange(1)) ? true : range1;
			range2 = (weapons[w].item.InRange(2)) ? true : range2;
		}

		//Generate attack/support tiles
		if (weapons[0].item.itemCategory == ItemCategory.WEAPON) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].ShowAttackTiles(range1, range2);
			}
		}
		else {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this == enemyList.values[i])
					continue;
				bool isBuff = (weapons[0].item.itemType == ItemType.BUFF);
				if (isBuff || enemyList.values[i].IsInjured())
					enemyList.values[i].ShowSupportTiles(range1, range2);
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

		WeaponItem weapon = GetEquippedWeapon(ItemCategory.WEAPON);
		WeaponItem staff = GetEquippedWeapon(ItemCategory.STAFF);

		while(process.Count > 0) {
			MapTile tile = process.Dequeue();
			tile.FindNeighbours(process, tile.distance, this, 1000, weapon, staff, false, false);
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
	}


	///////////   ATTACKS

	/// <summary>
	/// Calculates which character to attack/support from the current position.
	/// </summary>
	public bool CalculateAttacksHeals() {
		if (currentMode.value != ActionMode.ATTACK && currentMode.value != ActionMode.HEAL)
			return false;

		if (currentMode.value == ActionMode.ATTACK) {
			targetCharacter.value = FindRandomTarget(playerList, ItemCategory.WEAPON, false);
			int distance = MapCreator.DistanceTo(this, targetCharacter.value);
			inventory.EquipFirstInRangeItem(ItemCategory.WEAPON, stats, distance);
			Attack(targetCharacter.value);
		}
		else {
			targetCharacter.value = FindRandomTarget(enemyList, ItemCategory.STAFF, true);
			int distance = MapCreator.DistanceTo(this, targetCharacter.value);
			inventory.EquipFirstInRangeItem(ItemCategory.STAFF, stats, distance);
			Heal(targetCharacter.value);
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
	private TacticsMove FindRandomTarget(CharacterListVariable list, ItemCategory category, bool checkInjured) {
		List<TacticsMove> hits = new List<TacticsMove>();
		WeaponRange range = inventory.GetReach(category);

		for (int i = 0; i < list.values.Count; i++) {
			if (list.values[i] == this || !list.values[i].IsAlive() || (checkInjured && !list.values[i].IsInjured()))
				continue;
			int distance = MapCreator.DistanceTo(this, list.values[i]);
			if (range.InRange(distance)) {
				hits.Add(list.values[i]);
			}
		}

		hits.Shuffle();
		Debug.Log("Target is at " + hits[0].posx + " , " + hits[0].posy);
		return hits[0];
	}
}
