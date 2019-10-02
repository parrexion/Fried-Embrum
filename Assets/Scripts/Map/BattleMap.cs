using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleMap : MonoBehaviour {

	[Header("Map state")]
	public TriggerListVariable triggerList;

	[Header("Characters")]
	public Transform playerParent;
	public Transform enemyParent;
	public Transform tileParent;

	[HideInInspector] public MapTile[] tiles;
	[HideInInspector] public List<MapTile> breakables = new List<MapTile>();

	//Map size
	private int _sizeX;
	private int _sizeY;

	
	public void SetupMap(MapEntry map) {
		_sizeX = map.sizeX;
		_sizeY = map.sizeY;
		triggerList.values.Clear();
		for (int i = 0; i < map.triggerIds.Count; i++) {
			triggerList.values.Add(new TriggerTuple(map.triggerIds[i].id));
		}
	}

	public void ResetMap() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].Reset();
		}
	}

	public void ClearDeployment() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].deployable = 0;
		}
	}

	public void ClearTargets() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].target = false;
		}
	}

	public void ClearMovable() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].target = false;
			tiles[i].pathable = false;
			tiles[i].selectable = false;
			tiles[i].attackable = false;
		}
	}

	public void ClearMovement() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].target = false;
			tiles[i].pathable = false;
		}
	}
	
	public void ClearDangerous() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].dangerous = false;
		}
	}

	private int TilePosition(int x, int y) {
		return x + y * _sizeX;
	}

	public int SizeX() {
		return _sizeX;
	}

	public int SizeY() {
		return _sizeY;
	}

	/// <summary>
	/// Returns the map tile for the given position.
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public MapTile GetTile(Position pos) {
		return GetTile(pos.x, pos.y);
	}

	/// <summary>
	/// Returns the map tile for the given coordinates.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public MapTile GetTile(int x, int y) {
		if (x < 0 || y < 0 || x >= _sizeX || y >= _sizeY)
			return null;

		return tiles[TilePosition(x, y)];
	}

	public static int DistanceTo(MapTile startTile, MapTile tile) {
		return Mathf.Abs(startTile.posx - tile.posx) + Mathf.Abs(startTile.posy - tile.posy);
	}

	public static int DistanceTo(TacticsMove character, MapTile tile) {
		return Mathf.Abs(character.posx - tile.posx) + Mathf.Abs(character.posy - tile.posy);
	}

	public static int DistanceTo(TacticsMove character, TacticsMove other) {
		return Mathf.Abs(character.posx - other.posx) + Mathf.Abs(character.posy - other.posy);
	}

	/// <summary>
	/// Checks all tiles to see which closest tile is empty.
	/// </summary>
	/// <param name="startTile"></param>
	/// <returns></returns>
	public MapTile GetClosestEmptyTile(MapTile startTile) {
		int bestRange = 99;
		MapTile bestTile = startTile;
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (tempDist < bestRange && tiles[i].currentCharacter == null) {
				bestRange = tempDist;
				bestTile = tiles[i];
			}
		}
		return bestTile;
	}

	/// <summary>
	/// Shows which tiles are attackable from the startTile given the weapon range.
	/// isDanger is used to show the enemy danger area.
	/// </summary>
	/// <param name="startTile"></param>
	/// <param name="range"></param>
	/// <param name="faction"></param>
	/// <param name="isDanger"></param>
	public void ShowAttackTiles(MapTile startTile, WeaponRange range, Faction faction, bool isDanger) {
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (!range.InRange(tempDist))
				continue;

			if (isDanger) {
				tiles[i].dangerous = true;
			}
			else if (tiles[i].IsEmpty() || tiles[i].currentCharacter.faction != faction) {
				tiles[i].attackable = true;
			}
		}
	}
	
	public void ShowSupportTiles(MapTile startTile, WeaponRange range, Faction faction, bool isDanger, bool isBuff) {
		if (isDanger)
			return;
		
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (!range.InRange(tempDist))
				continue;

			if (tiles[i].IsEmpty()) {
				tiles[i].supportable = true;
			}
			else if(tiles[i].currentCharacter.faction == faction) {
				if (isBuff || tiles[i].currentCharacter.IsInjured())
					tiles[i].supportable = true;
			}
		}
	}

}
