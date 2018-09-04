﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCreator : MonoBehaviour {

	public MapInfoVariable mapInfo;
	public BoxCollider2D cameraBox;
	public MapCursor mapClicker;

	public IntVariable cursorX;
	public IntVariable cursorY;
	
	public Transform tilePrefab;
	
	[Header("Characters")]
	public Transform enemyParent;
	public Transform enemyPrefab;

	public Transform playerParent;
	public Transform playerPrefab;
	public SaveListVariable availableCharacters;

	[Header("Music")]
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public BoolVariable musicFocus;

	[Header("Events")]
	public UnityEvent cursorMoveEvent;
	public UnityEvent playBkgMusicEvent;

	[HideInInspector] public MapTile[] tiles;

	
	//Map Info
	private int _sizeX;
	private int _sizeY;
	private TerrainTile _tNormal;
	private TerrainTile _tForest;
	private TerrainTile _tMountain;
	private TerrainTile _tBridge;
	private TerrainTile _tLedge;
	private TerrainTile _tHouse;
	private TerrainTile _tFort;
	private TerrainTile _tRiver;
	private TerrainTile _tBreakable;
	private TerrainTile _tWall;
	private TerrainTile _tThrone;
	private TerrainTile _tPillar;


	private void Start() {
		CreateMap();
	}

	public void CreateMap() {
		_sizeX = mapInfo.value.sizeX;
		_sizeY = mapInfo.value.sizeY;
		
		_tNormal = mapInfo.value.normal;
		_tForest = mapInfo.value.forest;
		_tRiver = mapInfo.value.river;
		_tMountain = mapInfo.value.mountain;
		_tBridge = mapInfo.value.bridge;
		_tLedge = mapInfo.value.ledge;
		_tFort = mapInfo.value.fort;
		_tWall = mapInfo.value.wall;
		_tBreakable = mapInfo.value.breakable;
		_tHouse = mapInfo.value.house;
		_tThrone = mapInfo.value.throne;
		_tPillar = mapInfo.value.pillar;
		
		cameraBox.size = new Vector2(_sizeX+1, _sizeY+1);
		cameraBox.transform.position = new Vector3((_sizeX-1)/2.0f, (_sizeY-1)/2.0f, 0);
		
		GenerateMap(mapInfo.value.mapSprite);
		cursorMoveEvent.Invoke();
		SpawnCharacters();
		SetupMusic();
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
				tiles[i].reachable = true;
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


	// public void GenerateLinks() {
	// 	Debug.Log("Generate maP!");

	// 	tiles = new MapTile[_sizeX * _sizeY];
	// 	MapTile[] objects = GetComponentsInChildren<MapTile>();
	// 	for (int i = 0; i < objects.Length; i++) {
	// 		MapTile t = objects[i];
	// 		t.mapCreator = this;
	// 		t.posx = (int)t.transform.position.x;
	// 		t.posy = (int)t.transform.position.y;
	// 		t.Reset();
	// 		tiles[TilePosition(t.posx, t.posy)] = t;
	// 	}
	// }

	// public void RemoveOldMap() {
	// 	for (int i = 0; i < tiles.Length; i++) {
	// 		DestroyImmediate(tiles[i].gameObject);
	// 	}
	// 	tiles = new MapTile[0];
	// 	Debug.Log("Removed old map.");
	// }
	
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
			StatsContainer stats;
			InventoryContainer inventory;
			SkillsContainer skills;

			if (pos.stats != null) {
				stats = new StatsContainer(pos.stats, pos.level);
				inventory = new InventoryContainer(pos.inventory);
				skills = new SkillsContainer(pos.skills);
				availableCharacters.stats.Add(stats);
				availableCharacters.inventory.Add(inventory);
				availableCharacters.skills.Add(skills);
			}
			else if (i >= availableCharacters.stats.Count) {
				GetTile(pos.x, pos.y).selectable = true;
				continue;
			}
			else {
				stats = availableCharacters.stats[i];
				inventory = availableCharacters.inventory[i];
				skills = availableCharacters.skills[i];
			}

			Transform playerTransform = Instantiate(playerPrefab, playerParent);
			playerTransform.position = new Vector3(pos.x, pos.y);

			TacticsMove tactics = playerTransform.GetComponent<TacticsMove>();
			tactics.mapCreator = this;
			tactics.posx = pos.x;
			tactics.posy = pos.y;
			tactics.stats = stats;
			tactics.inventory = inventory;
			tactics.skills = skills;
			tactics.Setup();
		}
		cursorX.value = mapInfo.value.spawnPoints[0].x;
		cursorY.value = mapInfo.value.spawnPoints[0].y;
		
		//Enemies
		for (int i = 0; i < mapInfo.value.enemies.Length; i++) {
			EnemyPosition pos = mapInfo.value.enemies[i];
			Transform enemyTransform = Instantiate(enemyPrefab, enemyParent);
			enemyTransform.position = new Vector3(pos.x, pos.y);

			TacticsMove tactics = enemyTransform.GetComponent<TacticsMove>();
			tactics.mapCreator = this;
			tactics.posx = pos.x;
			tactics.posy = pos.y;
			tactics.stats = new StatsContainer(pos.stats, pos.level);
			tactics.inventory = new InventoryContainer(pos.inventory);
			tactics.skills = new SkillsContainer(pos.skills);
			((NPCMove)tactics).aggroType = pos.aggroType;
			tactics.Setup();
		}
	}

	private TerrainTile GetTerrainFromPixel(Color32 pixelColor) {
		TerrainTile terrain = _tNormal;

		if (pixelColor.r == 255 && pixelColor.g == 255 && pixelColor.b == 255) {
			//Normal empty space
		}
		else if (pixelColor == new Color(0f,0f,0f,1f)) {
			terrain = _tWall;
		}
		else if (pixelColor == new Color(0f,1f,0f,1f)) {
			terrain = _tForest;
		}
		else if (pixelColor == new Color(0f,0f,1f,1f)) {
			terrain = _tRiver;
		}
		else if (pixelColor == new Color(1f,0f,0f,1f)) {
			terrain = _tMountain;
		}
		else if (pixelColor == new Color(1f,1f,0f,1f)) {
			terrain = _tBridge;
		}
		else if (pixelColor == new Color(1f,0f,1f,1f)) {
			terrain = _tLedge;
		}
		else if (pixelColor == new Color(0f,1f,1f,1f)) {
			terrain = _tFort;
		}
		else if (pixelColor.r == 255 && pixelColor.g == 128 && pixelColor.b == 0) {
			terrain = _tHouse;
		}
		else if (pixelColor.r == 0 && pixelColor.g == 128 && pixelColor.b == 255) {
			terrain = _tBreakable;
		}
		else if (pixelColor.r == 128 && pixelColor.g == 128 && pixelColor.b == 0) {
			terrain = _tThrone;
		}
		else if (pixelColor.r == 128 && pixelColor.g == 0 && pixelColor.b == 255) {
			terrain = _tPillar;
		}

		return terrain;
	}

	private void SetupMusic() {
		musicFocus.value = true;
		mainMusic.value = mapInfo.value.owMusic.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();
	}
}
