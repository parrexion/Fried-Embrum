using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AggroType { WAIT, CHARGE, GUARD, BOSS, HUNT, PATROL, ESCAPE }

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

	private static List<TacticsMove> enemies = new List<TacticsMove>();
	private static List<TacticsMove> friends = new List<TacticsMove>();

	public ActionModeVariable currentMode;
	public AggroType aggroType;
	public MapTileVariable moveTile;
	public SpriteRenderer bossCrest;
	public MapTile huntTile;
	public List<MapTile> patrolTiles = new List<MapTile>();
	private int patrolIndex;

	public UnityEvent finishedMovingEvent;
	public UnityEvent destroyedTileEvent;
	public UnityEvent escapeEvent;


	/// <summary>
	/// Additional setup for enemy characters.
	/// </summary>
	protected override void ExtraSetup() {
		bossCrest.enabled = (aggroType == AggroType.BOSS);
		if (aggroType == AggroType.BOSS) {
			fatigueCap = 0;
		}

		if (faction == Faction.ENEMY) {
			enemyList.values.Add(this);
			GetComponent<SpriteRenderer>().sprite = stats.charData.enemySprite;
		}
		else {
			allyList.values.Add(this);
			GetComponent<SpriteRenderer>().sprite = stats.charData.allySprite;
		}

		if (GetComponent<SpriteRenderer>().sprite == null) {
			Debug.LogError("Battle sprite is null!", this);
		}
		//Debug.Log("Spawned  " + stats.charData.entryName);
	}

	/// <summary>
	/// Removes the character from the NPC list.
	/// </summary>
	public override void RemoveFromList() {
		if (faction == Faction.ENEMY)
			enemyList.values.Remove(this);
		else {
			allyList.values.Remove(this);
		}
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

		//Hunting or Escaping AI
		if (aggroType == AggroType.HUNT || aggroType == AggroType.ESCAPE) {
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
		else if (goodTile && aggroType != AggroType.CHARGE && aggroType != AggroType.PATROL) {
			//Have no weapons that can be used
			currentMode.value = ActionMode.NONE;
			tileBest = null;
			tileGood = null;
		}
		else if (aggroType == AggroType.PATROL) {
			tileBest = null;
			tileGood = patrolTiles[patrolIndex];
			while (tileGood != null && (tileGood.distance > moveSpeed || !tileGood.selectable)) {
				tileGood = tileGood.parent;
			}
			if (tileGood != null) {
				tileGood.PrintPos();
				currentMode.value = ActionMode.MOVE;
				if (tileGood == patrolTiles[patrolIndex])
					patrolIndex = OPMath.FullLoop(0, patrolTiles.Count, patrolIndex + 1);
				return;
			}
			else {
				currentMode.value = ActionMode.NONE;
				tileGood = null;
				return;
			}
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

		//Sort characters
		enemies.Clear();
		friends.Clear();
		if (faction == Faction.ALLY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (enemyList.values[i].IsAlive() && !enemyList.values[i].hasEscaped)
					enemies.Add(enemyList.values[i]);
			}
			for (int i = 0; i < playerList.Count; i++) {
				if (playerList.values[i].IsAlive() && !playerList.values[i].hasEscaped)
					friends.Add(playerList.values[i]);
			}
			for (int i = 0; i < allyList.Count; i++) {
				if (this != allyList.values[i] && allyList.values[i].IsAlive() && !allyList.values[i].hasEscaped)
					friends.Add(allyList.values[i]);
			}
		}
		else if (faction == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this != enemyList.values[i] && enemyList.values[i].IsAlive() && !enemyList.values[i].hasEscaped)
					friends.Add(enemyList.values[i]);
			}
			for (int i = 0; i < playerList.Count; i++) {
				if (playerList.values[i].IsAlive() && !playerList.values[i].hasEscaped)
					enemies.Add(playerList.values[i]);
			}
			for (int i = 0; i < allyList.Count; i++) {
				if (allyList.values[i].IsAlive() && !allyList.values[i].hasEscaped)
					enemies.Add(allyList.values[i]);
			}
		}

		//Calculate range

		//Generate attack/support tiles
		if (weapons[0].itemCategory == ItemCategory.WEAPON) {
			int damage = BattleCalc.CalculateDamage(weapons[0], stats);
			WeaponRange reach = inventory.GetReach(ItemCategory.WEAPON);
			for (int i = 0; i < enemies.Count; i++) {
				int defense = (weapons[0].attackType == AttackType.PHYSICAL) ? enemies[i].stats.def : enemies[i].stats.mnd;
				enemies[i].ShowAttackTiles(reach, damage - defense);
			}
		}
		else {
			WeaponRange reach = inventory.GetReach(ItemCategory.SUPPORT);
			for (int i = 0; i < friends.Count; i++) {
				bool isBuff = (weapons[0].weaponType == WeaponType.BARRIER);
				if (isBuff || friends[i].IsInjured())
					friends[i].ShowSupportTiles(reach, isBuff);
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

		currentTile.CheckTile(currentTile, 0, info);
		currentTile.parent = null;

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

		if (challenger.terrain.avoid + challenger.terrain.defense * 10 != current.terrain.avoid + current.terrain.defense * 10)
			return challenger.terrain.avoid + challenger.terrain.defense * 10 > current.terrain.avoid + current.terrain.defense * 10;

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
		else if (aggroType == AggroType.ESCAPE && currentTile == huntTile) {
			escapeEvent.Invoke();
			Escape();
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
			targetTile.value = FindRandomTarget(enemies, ItemCategory.WEAPON, false);
			int distance = BattleMap.DistanceTo(this, targetTile.value);
			inventory.EquipFirstInRangeItem(ItemCategory.WEAPON, distance);
			Attack(targetTile.value);
		}
		else {
			targetTile.value = FindRandomTarget(friends, ItemCategory.SUPPORT, true);
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
	private MapTile FindRandomTarget(List<TacticsMove> list, ItemCategory category, bool checkInjured) {
		List<MapTile> hits = new List<MapTile>();
		WeaponRange range = inventory.GetReach(category);

		for (int i = 0; i < list.Count; i++) {
			if (list[i] == this || !list[i].IsAlive() || (checkInjured && !list[i].IsInjured()))
				continue;
			int distance = BattleMap.DistanceTo(this, list[i]);
			if (range.InRange(distance)) {
				hits.Add(list[i].currentTile);
			}
		}

		hits.Shuffle();
		return hits[0];
	}
}
