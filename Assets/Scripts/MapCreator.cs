using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {
	private enum TerrainName { NORMAL, WALL, FOREST, MOUNTAIN }

	public MapTile[] tiles;

	[Header("Map Creation")]
	public TerrainTile[] terrains;


	public void ResetMap() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].Reset();
		}
	}

	public void ClearTargets() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].target = false;
		}
	}

	public void ClearMovement() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].target = false;
			tiles[i].pathable = false;
		}
	}
	
	public void ClearReachable() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].reachable = false;
		}
	}

	private int TilePosition(int x, int y) {
		return x + y * ConstValues.MAP_SIZE_X;
	}

	public MapTile GetTile(int x, int y) {
		if (x < 0 || y < 0 || x >= ConstValues.MAP_SIZE_X || y >= ConstValues.MAP_SIZE_Y)
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

	public void ShowAttackTiles(MapTile startTile, WeaponSkill weapon, Faction faction, bool isDanger) {
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (weapon.InRange(tempDist)) {
				if (isDanger) {
					tiles[i].reachable = true;
				}
				else if (tiles[i].currentCharacter == null || tiles[i].currentCharacter.faction != faction) {
					tiles[i].attackable = true;
				}
			}
		}
	}
	
	public void ShowSupportTiles(MapTile startTile, SupportSkill support, Faction faction, bool isDanger) {
		if (isDanger)
			return;
		
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (!support.InRange(tempDist))
				continue;

			if (tiles[i].currentCharacter == null) {
				tiles[i].supportable = true;
			}
			else if(tiles[i].currentCharacter.faction == faction) {
				if (support.supportType == SupportType.BUFF || (support.supportType == SupportType.HEAL && tiles[i].currentCharacter.IsInjured()))
					tiles[i].supportable = true;
			}
		}
	}
	
	

	//////////
	// EDITOR STUFF


	


	public void GenerateLinks() {
		Debug.Log("Generate maP!");

		tiles = new MapTile[ConstValues.MAP_SIZE_X * ConstValues.MAP_SIZE_Y];
		MapTile[] objects = GetComponentsInChildren<MapTile>();
		for (int i = 0; i < objects.Length; i++) {
			MapTile t = objects[i];
			t.mapCreator = this;
			t.posx = (int)t.transform.position.x;
			t.posy = (int)t.transform.position.y;
			t.Reset();
			tiles[TilePosition(t.posx, t.posy)] = t;
		}
	}

	public void GenerateMap(Texture2D texMap) {
		Color32[] colorData = texMap.GetPixels32();
		int pos = 0;

		for (int j = 0; j < ConstValues.MAP_SIZE_Y; j++) {
			for (int i = 0; i < ConstValues.MAP_SIZE_X; i++) {
				MapTile tempTile = GetTile(i,j);
				TerrainTile terrain = GetTerrainFromPixel(colorData[pos]);
				tempTile.SetTerrain(terrain);
				pos++;
			}
		}
		Debug.Log("Data read");
	}

	private TerrainTile GetTerrainFromPixel(Color32 pixelColor) {
		TerrainTile terrain = terrains[(int)TerrainName.NORMAL];

		if (pixelColor.a == 0 || (pixelColor.r == 255 && pixelColor.g == 255 && pixelColor.b == 255)) {
			//Empty space
		}
		else if (pixelColor.r + pixelColor.g + pixelColor.b == 0) {
			terrain = terrains[(int)TerrainName.WALL];
		}
		else if (pixelColor.g == 255) {
			terrain = terrains[(int)TerrainName.FOREST];
		}
		else if (pixelColor.b == 255) {
			terrain = terrains[(int)TerrainName.MOUNTAIN];
		}

		return terrain;
	}
}
