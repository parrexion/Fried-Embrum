using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AggroType { WAIT, CHARGE, GUARD, BOSS, HUNT }

public class SearchInfo {
	public TacticsMove tactics;
	public int moveSpeed;

	public WeaponRange wpnRange;
	public WeaponRange staff;

	public bool showAttack;
	public bool isDanger;
	public bool isBuff;
}

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
	protected override void ExtraSetup() {
		bossCrest.enabled = (aggroType == AggroType.BOSS);

		enemyList.values.Add(this);
		//Debug.Log("Spawned  " + stats.charData.entryName);
	}



	///////////   MOVEMENT	


	/// <summary>
	/// Returns the current movement speed. Virtual so that other classes
	/// can change the movement speed.
	/// </summary>
	/// <returns></returns>
	protected override int GetMoveSpeed() {
		return (aggroType == AggroType.BOSS || aggroType == AggroType.GUARD) ? 0 : stats.GetMovespeed();
	}

	/// <summary>
	/// Generates possible moves and selects and moves the AI to the best one found.
	/// </summary>
	public MapTile CalculateMovement() {
		MapTile tileBestWpn = null, tileGoodWpn = null, tileBestStf = null, tileGoodStf = null;
		List<InventoryTuple> weapon = inventory.GetAllUsableItemTuple(ItemCategory.WEAPON);
		List<InventoryTuple> staff = inventory.GetAllUsableItemTuple(ItemCategory.SUPPORT);
		if (weapon.Count > 0) {
			FindBestTile(weapon, out tileBestWpn, out tileGoodWpn);
		}
		if (tileBestWpn == null && staff.Count > 0) {
			FindBestTile(staff, out tileBestStf, out tileGoodStf);
		}

		moveTile.value = (tileBestWpn != null) ? tileBestWpn :
					(tileBestStf != null) ? tileBestStf :
					(tileGoodWpn != null) ? tileGoodWpn : tileGoodStf;

		battleMap.ClearMovable();
		return moveTile.value;
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

		// Skip move if guarding type of enemy
		if (aggroType == AggroType.GUARD || aggroType == AggroType.BOSS) {
			if (currentTile.attackable) {
				//currentMode.value = (weapons[0].item.itemCategory == ItemCategory.WEAPON) ? ActionMode.ATTACK : ActionMode.HEAL;
				currentMode.value = ActionMode.MOVE;
				//tileBest = currentTile;
				tileBest = null;
				tileGood = null;
			}
			else {
				tileBest = null;
				tileGood = null;
				//tileGood = currentTile;
				currentMode.value = ActionMode.NONE;
			}
			return;
		}

		BFS();

		int moveSpeed = GetMoveSpeed();
		MapTile bestTile = null; // Reachable this turn
		MapTile goodTile = null; // Reachable in multiple turns

		if (aggroType == AggroType.HUNT && huntTile.interacted)
			aggroType = AggroType.CHARGE;

		//Hunting AI
		if (aggroType == AggroType.HUNT) {
			tileBest = huntTile;
			tileGood = null;
			while (tileBest != null && (tileBest.distance > moveSpeed || !tileBest.selectable)) {
				tileBest = tileBest.parent;
			}
			if (tileBest != null) {
				tileBest.PrintPos();
				currentMode.value = ActionMode.MOVE;
				return;
			}
		}

		// Go through all tiles and find the best one to move to or towards
		for (int i = 0; i < battleMap.tiles.Length; i++) {
			MapTile tempTile = battleMap.tiles[i];
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
			currentMode.value = (weapons[0].itemCategory == ItemCategory.WEAPON) ? ActionMode.ATTACK : ActionMode.HEAL;
			tileBest = bestTile;
			tileGood = null;
			if (aggroType == AggroType.WAIT)
				aggroType = AggroType.CHARGE;
		}
		else if (!goodTile || aggroType != AggroType.CHARGE) {
			//Have no weapons that can be used
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
		battleMap.ResetMap();

		//Calculate range

		//Generate attack/support tiles
		if (weapons[0].itemCategory == ItemCategory.WEAPON) {
			int damage = BattleCalc.CalculateDamage(weapons[0], stats);
			WeaponRange reach = inventory.GetReach(ItemCategory.WEAPON);
			for (int i = 0; i < playerList.values.Count; i++) {
				int defense = (weapons[0].attackType == AttackType.PHYSICAL) ? playerList.values[i].stats.def : playerList.values[i].stats.mnd;
				((PlayerMove)playerList.values[i]).ShowAttackTiles(reach, damage - defense);
			}
		}
		else {
			WeaponRange reach = inventory.GetReach(ItemCategory.SUPPORT);
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this == enemyList.values[i])
					continue;
				bool isBuff = (weapons[0].weaponType == WeaponType.BARRIER);
				if (isBuff || enemyList.values[i].IsInjured())
					((NPCMove)enemyList.values[i]).ShowSupportTiles(reach, isBuff);
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
		currentTile.selectable = true;

		SearchInfo info = new SearchInfo() {
			tactics = this,
			moveSpeed = 1000,

			wpnRange = inventory.GetReach(ItemCategory.WEAPON),
			staff = inventory.GetReach(ItemCategory.SUPPORT),

			//showAttack = false,
			//isDanger = false,
			//isBuff = false
		};

		if (info.staff.max > 0)
			info.isBuff = (inventory.GetFirstUsableItemTuple(WeaponType.BARRIER) != null);

		while (process.Count > 0) {
			MapTile tile = process.Dequeue();
			tile.FindNeighbours(process, tile.distance, info);
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

		if (challenger.value != current.value)
			return challenger.value > current.value;

		return challenger.distance < current.distance;
	}

	/// <summary>
	/// Ends the character's movement and clears the map of the selection.
	/// </summary>
	public override void EndMovement() {
		//Debug.Log("Finished move");
		isMoving = false;
		battleMap.ResetMap();
		currentTile.current = true;

		if (aggroType == AggroType.HUNT && currentTile == huntTile) {
			aggroType = AggroType.CHARGE;
			huntTile.SetTerrain(huntTile.alternativeTerrain);
			destroyedTileEvent.Invoke();
		}
		else {
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
			int distance = BattleMap.DistanceTo(this, targetTile.value);
			inventory.EquipFirstInRangeItem(ItemCategory.WEAPON, distance);
			Attack(targetTile.value);
		}
		else {
			targetTile.value = FindRandomTarget(enemyList, ItemCategory.SUPPORT, true);
			int distance = BattleMap.DistanceTo(this, targetTile.value);
			inventory.EquipFirstInRangeItem(ItemCategory.SUPPORT, distance);
			Heal(targetTile.value);
		}
		InputDelegateController.instance.TriggerMenuChange(MenuMode.BATTLE);
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
			int distance = BattleMap.DistanceTo(this, list.values[i]);
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
	public void ShowSupportTiles(WeaponRange range, bool isBuff) {
		if (!IsAlive())
			return;

		for (int i = 0; i < battleMap.tiles.Length; i++) {
			if (range.InRange(BattleMap.DistanceTo(this, battleMap.tiles[i]))) {
				battleMap.tiles[i].supportable = true;
				if (isBuff) {
					battleMap.tiles[i].value = 5;
				}
				else {
					battleMap.tiles[i].value = stats.hp - currentHealth;
				}
			}
		}
	}
}
