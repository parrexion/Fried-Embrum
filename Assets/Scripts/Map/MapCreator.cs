using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class MapCreator : MonoBehaviour {

	public MapInfoVariable mapInfo;
	public BoxCollider2D cameraBox;
	public MapClicker mapClicker;
	
	public Transform tilePrefab;
	
	public Transform enemyParent;
	public Transform enemyPrefab;

	public Transform playerParent;
	public Transform playerPrefab;
	public SaveListVariable availableCharacters;

	public MapTile[] tiles;

	
	//Map Info
	private int _sizeX;
	private int _sizeY;
	private TerrainTile _tNormal;
	private TerrainTile _tForest;
	private TerrainTile _tMountain;
	private TerrainTile _tBlocked;


	private void Start() {
		CreateMap();
	}

	public void CreateMap() {
		_sizeX = mapInfo.value.sizeX;
		_sizeY = mapInfo.value.sizeY;
		
		_tNormal = mapInfo.value.normal;
		_tForest = mapInfo.value.forest;
		_tMountain = mapInfo.value.mountain;
		_tBlocked = mapInfo.value.blocked;
		
		cameraBox.size = new Vector2(_sizeX+2, _sizeY+2);
		cameraBox.transform.position = new Vector3((_sizeX-1)/2.0f, (_sizeY-1)/2.0f, 0);
		
		GenerateMap(mapInfo.value.mapSprite);
		SpawnCharacters();
	}
	
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
		return x + y * _sizeX;
	}

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

	public void ShowAttackTiles(MapTile startTile, WeaponItem item, Faction faction, bool isDanger) {
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (item.InRange(tempDist)) {
				if (isDanger) {
					tiles[i].reachable = true;
				}
				else if (tiles[i].IsEmpty() || tiles[i].currentCharacter.faction != faction) {
					tiles[i].attackable = true;
				}
			}
		}
	}
	
	public void ShowSupportTiles(MapTile startTile, WeaponItem item, Faction faction, bool isDanger) {

		if (isDanger)
			return;
		
		for (int i = 0; i < tiles.Length; i++) {
			int tempDist = DistanceTo(startTile, tiles[i]);
			if (!item.InRange(tempDist))
				continue;

			if (tiles[i].IsEmpty()) {
				tiles[i].supportable = true;
			}
			else if(tiles[i].currentCharacter.faction == faction) {
				if (item.itemType == ItemType.BUFF || (item.itemType == ItemType.HEAL && tiles[i].currentCharacter.IsInjured()))
					tiles[i].supportable = true;
			}
		}
	}
	
	
	

	//////////
	// EDITOR STUFF



	public void GenerateLinks() {
		Debug.Log("Generate maP!");

		tiles = new MapTile[_sizeX * _sizeY];
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

	public void RemoveOldMap() {
		for (int i = 0; i < tiles.Length; i++) {
			DestroyImmediate(tiles[i].gameObject);
		}
		tiles = new MapTile[0];
		Debug.Log("Removed old map.");
	}
	
	public void GenerateMap(Texture2D texMap) {
		Color32[] colorData = texMap.GetPixels32();
		int pos = 0;
		List<MapTile> mappus = new List<MapTile>();
		
		for (int j = 0; j < _sizeY; j++) {
			for (int i = 0; i < _sizeX; i++) {
				Transform tile = Instantiate(tilePrefab);
				tile.position = new Vector3(i,j,0);
				tile.parent = transform;

				MapTile tempTile = tile.GetComponent<MapTile>();
				tempTile.mapCreator = this;
				tempTile.posx = i;
				tempTile.posy = j;
				tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
				mappus.Add(tempTile);
				pos++;
			}
		}

		tiles = mappus.ToArray();
		Debug.Log("Data read");
	}

	private void SpawnCharacters() {
		
		//Players
		for (int i = 0; i < mapInfo.value.spawnPoints.Length; i++) {
			PlayerPosition pos = mapInfo.value.spawnPoints[i];
			if (i >= availableCharacters.stats.Length) {
				GetTile(pos.x, pos.y).selectable = true;
				continue;
			}

			Transform playerTransform = Instantiate(playerPrefab, playerParent);
			playerTransform.position = new Vector3(pos.x, pos.y);

			TacticsMove tactics = playerTransform.GetComponent<TacticsMove>();
			tactics.mapCreator = this;
			tactics.posx = pos.x;
			tactics.posy = pos.y;
			tactics.stats = availableCharacters.stats[i];
			tactics.inventory = availableCharacters.inventory[i];
			tactics.Setup();
		}
		
		//Enemies
		for (int i = 0; i < mapInfo.value.enemies.Length; i++) {
			EnemyPosition pos = mapInfo.value.enemies[i];
			Transform enemyTransform = Instantiate(enemyPrefab, enemyParent);
			enemyTransform.position = new Vector3(pos.x, pos.y);

			TacticsMove tactics = enemyTransform.GetComponent<TacticsMove>();
			tactics.mapCreator = this;
			tactics.posx = pos.x;
			tactics.posy = pos.y;
			tactics.stats.level = pos.level;
			tactics.stats.charData = pos.stats;
			tactics.stats.classData = pos.stats.charClass;
			tactics.stats.wpnSkills = pos.stats.charClass.GenerateBaseWpnSkill();
			tactics.inventory.inventory = new InventoryTuple[InventoryContainer.INVENTORY_SIZE];
			for (int j = 0; j < InventoryContainer.INVENTORY_SIZE; j++) {
				if (j < pos.inventory.Length) {
					tactics.inventory.inventory[j] = new InventoryTuple() {
						index = j,
						item = pos.inventory[j].item,
						charge = pos.inventory[j].item.maxCharge,
						droppable = pos.inventory[j].droppable
					};
				}
				else {
					tactics.inventory.inventory[j] = new InventoryTuple() {
						index = j,
						charge = 0,
						droppable = false
					};
				}
			}
			tactics.stats.skills = pos.skills;
			tactics.Setup();
		}
	}

	private TerrainTile GetTerrainFromPixel(Color32 pixelColor) {
		TerrainTile terrain = _tNormal;

		if (pixelColor.r == 255 && pixelColor.g == 255 && pixelColor.b == 255) {
			//Normal empty space
		}
		else if (pixelColor.r + pixelColor.g + pixelColor.b == 0) {
			terrain = _tBlocked;
		}
		else if (pixelColor.g == 255) {
			terrain = _tForest;
		}
		else if (pixelColor.b == 255) {
			terrain = _tMountain;
		}

		return terrain;
	}
}
