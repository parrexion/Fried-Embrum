using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove {

	public bool aggressive;
	public MapTileVariable targetTile;


	// Use this for initialization
	protected override void SetupLists() {
		stats = enemyChars.values[id];
		enemyList.values.Add(this);
		Debug.Log(stats.charName);
	}
	
	protected override void EndMovement() {
		Debug.Log("Finished move");
		isMoving = false;
		mapCreator.ResetMap();
		currentTile.current = true;
		if (faction == Faction.PLAYER)
			currentMode.value = ActionMode.ATTACK;
		TurnController.busy = false;
		characterClicked.Invoke();
	}

	public override void CalculateMovement() {
		MapTile tileA = null, tileB = null, tileC = null, tileD = null;
		if (GetWeapon() != null) {
			FindBestTile(true, out tileA, out tileB);
		}
		if (tileA == null && GetSupport() != null) {
			FindBestTile(false, out tileC, out tileD);
		}

		targetTile.value = (tileA != null) ? tileA :
					(tileC != null) ? tileC :
					(tileB != null) ? tileB : tileD;

		if (targetTile.value == null) {
			EndMovement();
		}
		else {
			ShowMove(targetTile.value);
			Move();
		}
	}

	private void FindBestTile(bool isAttack, out MapTile tileBest, out MapTile tileGood) {
		GenerateHitTiles(isAttack);
		BFS();

		int moveSpeed = stats.GetMove();
		MapTile bestTile = null;
		MapTile goodTile = null;

		if (currentTile.attackable || currentTile.supportable) {
			bestTile = currentTile;
		}
		else {
			for (int i = 0; i < ConstValues.MAP_SIZE_X; i++) {
				for (int j = 0; j < ConstValues.MAP_SIZE_Y; j++) {
					MapTile tempTile = mapCreator.GetTile(i,j);
					if (tempTile.selectable && (tempTile.attackable || tempTile.supportable)) {
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
				}
			}
		}

		if (bestTile) {
			bestTile.current = true;
			currentMode.value = (isAttack) ? ActionMode.ATTACK : ActionMode.HEAL;
			Debug.Log("That's the best");
			tileBest = bestTile;
			tileGood = null;
		}
		else if (!goodTile || !aggressive) {
			Debug.Log("Nothing is good!!");
			currentMode.value = ActionMode.NONE;
			tileBest = null;
			tileGood = null;
		}
		else {
			while (goodTile.distance > moveSpeed || goodTile.currentCharacter != null) {
				goodTile = goodTile.parent;
			}

			goodTile.current = true;
			currentMode.value = ActionMode.NONE;
			Debug.Log("That's good enough");
			tileBest = null;
			tileGood = goodTile;
		}
	}

	private void GenerateHitTiles(bool isAttack) {
		mapCreator.ResetMap();
		bool range1 = (isAttack) ? GetWeapon().InRange(1) : GetSupport().InRange(1);
		bool range2 = (isAttack) ? GetWeapon().InRange(2) : GetSupport().InRange(2);
		if (isAttack) {
			for (int i = 0; i < playerList.values.Count; i++) {
				playerList.values[i].ShowAttackTiles(range1, range2);
			}
		}
		else {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this == enemyList.values[i])
					continue;
				bool isBuff = (GetSupport().supportType == SupportType.BUFF);
				if (isBuff || enemyList.values[i].IsInjured())
					enemyList.values[i].ShowSupportTiles(range1, range2);
			}
		}
	}

	public override void CalculateAttacks() {
		if (currentMode.value != ActionMode.ATTACK && currentMode.value != ActionMode.HEAL) {
			TurnController.busy = false;
			return;
		}

		if (currentMode.value == ActionMode.ATTACK) {
			attackTarget.value = FindNearestTarget(playerList);
			int distance = MapCreator.DistanceTo(this, attackTarget.value);
			if (GetWeapon().InRange(distance)) {
				mapCreator.GetTile(posx,posy).current = true;
				Attack(attackTarget.value.currentCharacter);
			}
			else
				TurnController.busy = false;
		}
		else {
			attackTarget.value = FindNearestTarget(enemyList);
			int distance = MapCreator.DistanceTo(this, attackTarget.value.currentCharacter);
			if (GetSupport().InRange(distance)) {
				mapCreator.GetTile(posx,posy).current = true;
				Heal(attackTarget.value.currentCharacter);
			}
			else
				TurnController.busy = false;
		}
	}

	private MapTile FindNearestTarget(CharacterListVariable list) {
		TacticsMove closest = null;
		int distance = 1000;

		for (int i = 0; i < list.values.Count; i++) {
			if (list.values[i] == this || !list.values[i].IsAlive())
				continue;
			int tempDist = MapCreator.DistanceTo(this, list.values[i]);
			if (tempDist < distance) {
				distance = tempDist;
				closest = list.values[i];
			}
		}

		if (closest != null)
			Debug.Log("Closest is at " + closest.posx + " , " + closest.posy);
			
		return (closest != null) ? mapCreator.GetTile(closest.posx, closest.posy) : null;
	}

	private bool IsBetterTile(MapTile current, MapTile challenger) {
		if (current == null)
			return true;
		return challenger.distance < current.distance;
	}
	
	private void BFS() {
		Queue<MapTile> process = new Queue<MapTile>();
		process.Enqueue(currentTile);
		currentTile.distance = 0;
		currentTile.parent = null;
		while(process.Count > 0) {
			MapTile tile = process.Dequeue();
			tile.FindNeighbours(process, tile.distance, this, 1000, false, false);
		}
	}
}
