using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : TacticsMove {

	public IntVariable currentMenuMode;
	public int squad = 0;

	public UnityEvent playerFinishedMoveEvent;
	public UnityEvent prepMoveEndEvent;


	/// <summary>
	/// Additional setup for player characters.
	/// </summary>
	protected override void ExtraSetup() {
		playerList.values.Add(this);
		GetComponent<SpriteRenderer>().sprite = stats.currentClass.playerSprite;
		if(stats.currentClass.playerSprite == null) {
			Debug.LogError("Battle sprite is null!", this);
		}
		//Debug.Log("Spawned " + stats.charData.entryName);
	}

	/// <summary>
	/// Additional functions which run when the player ends their turn.
	/// </summary>
	public override void EndMovement() {
		//Debug.Log("Finished move");
		isMoving = false;
		lockControls.value = false;
		if(currentMenuMode.value == (int)MenuMode.FORMATION) {
			prepMoveEndEvent.Invoke();
		}
		else {
			playerFinishedMoveEvent.Invoke();
		}
	}

	/// <summary>
	/// Removes the character from the NPC list.
	/// </summary>
	public override void RemoveFromList() {
		playerList.values.Remove(this);
	}




	/// <summary>
	/// Finds all the allies around the character which can be supported with the staff.
	/// </summary>
	/// <returns></returns>
	public List<MapTile> FindSupportablesInRange() {
		InventoryTuple staff = inventory.GetFirstUsableItemTuple(ItemCategory.SUPPORT);
		if(string.IsNullOrEmpty(staff.uuid))
			return new List<MapTile>();

		List<MapTile> supportables = new List<MapTile>();
		for(int i = 0; i < playerList.values.Count; i++) {
			if(this == playerList.values[i] || !playerList.values[i].IsInjured() || !playerList.values[i].IsAlive())
				continue;

			int tempDist = BattleMap.DistanceTo(this, playerList.values[i]);
			if(staff.InRange(tempDist))
				supportables.Add(playerList.values[i].currentTile);
		}
		for(int i = 0; i < allyList.values.Count; i++) {
			if(this == allyList.values[i] || !allyList.values[i].IsInjured() || !allyList.values[i].IsAlive())
				continue;

			int tempDist = BattleMap.DistanceTo(this, allyList.values[i]);
			if(staff.InRange(tempDist))
				supportables.Add(allyList.values[i].currentTile);
		}
		return supportables;
	}


	// Functions for checking if the character can do stuff.
	#region Can do stuff

	/// <summary>
	/// Takes the range of the character's weapons and checks if any enemy is in range.
	/// </summary>
	/// <returns></returns>
	public bool CanAttack() {
		WeaponRange range = inventory.GetReach(ItemCategory.WEAPON);
		if(range.max == 0)
			return false;
		for(int i = 0; i < enemyList.values.Count; i++) {
			if(!enemyList.values[i].IsAlive() || enemyList.values[i].hasEscaped)
				continue;
			int distance = BattleMap.DistanceTo(this, enemyList.values[i]);
			if(range.InRange(distance)) {
				return true;
			}
		}
		for(int i = 0; i < battleMap.breakables.Count; i++) {
			if(!battleMap.breakables[i].blockMove.IsAlive())
				continue;
			int distance = BattleMap.DistanceTo(this, battleMap.breakables[i]);
			if(range.InRange(distance)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Takes the range of the character's staffs and checks if any supportable is in range.
	/// </summary>
	/// <returns></returns>
	public bool CanSupport() {
		WeaponRange range = inventory.GetReach(ItemCategory.SUPPORT);
		if(range.max == 0)
			return false;

		List<InventoryTuple> staffs = inventory.GetAllUsableItemTuple(ItemCategory.SUPPORT);
		bool isBuff = false;
		for(int i = 0; i < staffs.Count; i++) {
			if(staffs[i].weaponType == WeaponType.BARRIER) {
				isBuff = true;
				break;
			}
		}

		for(int i = 0; i < playerList.values.Count; i++) {
			bool usable = (playerList.values[i].IsAlive() && playerList.values[i].IsInjured() || isBuff);
			if(!usable || playerList.values[i] == this)
				continue;
			int distance = BattleMap.DistanceTo(this, playerList.values[i]);
			if(range.InRange(distance)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the character can visit a village or other buildings.
	/// </summary>
	/// <returns></returns>
	public bool CanVisit() {
		return (currentTile.interactType == InteractType.VILLAGE && !currentTile.interacted);
	}

	/// <summary>
	/// Checks if the character can capture the command point.
	/// </summary>
	/// <returns></returns>
	public bool CanCapture() {
		return (currentTile.interactType == InteractType.CAPTURE);
	}

	/// <summary>
	/// Checks if the character can escape from the tile.
	/// </summary>
	/// <returns></returns>
	public bool CanEscape() {
		return (currentTile.interactType == InteractType.ESCAPE);
	}

	/// <summary>
	/// Checks if the character can trade with anyone.
	/// </summary>
	/// <returns></returns>
	public bool CanTrade() {
		if(!canUndoMove)
			return false;
		List<MapTile> traders = FindAdjacentCharacters(true, false, false);
		return (traders.Count > 0);
	}

	/// <summary>
	/// Checks if the character can talk to anyone.
	/// </summary>
	/// <returns></returns>
	public bool CanTalk() {
		List<MapTile> talkers = FindAdjacentCharacters(false, true, true);
		for(int i = 0; i < talkers.Count; i++) {
			for(int talk = 0; talk < talkers[i].currentCharacter.talkQuotes.Count; talk++) {
				FightQuote fq = talkers[i].currentCharacter.talkQuotes[talk];
				if(!fq.activated && (fq.triggerer == null || fq.triggerer.uuid == stats.charData.uuid)) {
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the character can open a door.
	/// </summary>
	/// <returns></returns>
	public bool CanOpenDoor() {
		List<MapTile> tiles = GetAdjacentTiles();
		for(int i = 0; i < tiles.Count; i++) {
			if(tiles[i] == null)
				continue;

			if(tiles[i].interactType == InteractType.DOOR && !tiles[i].interacted && inventory.HasKey(tiles[i].doorKeyType)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the character can hack on the current tile.
	/// </summary>
	/// <returns></returns>
	public bool CanHack() {
		return (currentTile.interactType == InteractType.DATABASE && !currentTile.interacted && stats.currentClass.lockTouch);
	}

	#endregion

}
