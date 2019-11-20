using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCreator : MonoBehaviour {

	public ScrObjEntryReference currentMap;
	public PlayerData playerData;
	public PrepListVariable prepList1;
	public PrepListVariable prepList2;
	public BattleMap battleMap;
	public MapSpawner mapSpawner;

	public IntVariable cursorX;
	public IntVariable cursorY;

	[Header("Prefabs")]
	public Transform tilePrefab;
	public Transform blockTilePrefab;

	[Header("Events")]
	public UnityEvent cursorMoveEvent;

	[Header("Terrain Tiles")]
	public TerrainTile tileNormal;
	public TerrainTile tileDirt;
	public TerrainTile tileFloor;
	public TerrainTile tileForest;
	public TerrainTile tileMountain;
	public TerrainTile tileBridge;
	public TerrainTile tileLedge;
	public TerrainTile tileHouse;
	public TerrainTile tileHouseReward;
	public TerrainTile tileFort;
	public TerrainTile tileRiver;
	public TerrainTile tileBreakable;
	public TerrainTile tileWall;
	public TerrainTile tileThrone;
	public TerrainTile tilePillar;
	public TerrainTile tileChest;
	public TerrainTile tileDoor;

	//Map size
	private int _sizeX;
	private int _sizeY;


	private void Start() {
		CreateMap();
	}

	/// <summary>
	/// Creates the map and spawns all characters and interactions from the map data.
	/// </summary>
	public void CreateMap() {
		MapEntry map = (MapEntry)currentMap.value;
		_sizeX = map.sizeX;
		_sizeY = map.sizeY;
		battleMap.SetupMap(map);

		TacticsCamera.boxCollider.size = new Vector2(_sizeX + 1, _sizeY + 1);
		TacticsCamera.boxCollider.center = new Vector3((_sizeX - 1) / 2.0f, (_sizeY - 1) / 2.0f, 0);
		TacticsCamera.boxActive = true;

		GenerateMap(map.mapSprite);
		if (map.spawnPoints1.Count > 0) {
			cursorX.value = map.spawnPoints1[0].x;
			cursorY.value = map.spawnPoints1[0].y;
		}
		else {
			cursorX.value = map.spawnPoints2[0].x;
			cursorY.value = map.spawnPoints2[0].y;
		}
		cursorMoveEvent.Invoke();
		CreateTriggers(map);
		SpawnPlayers();
		SpawnEnemies();
		SpawnAllies();
		Debug.Log("Finished spawning and creating map");
	}

	/// <summary>
	/// Takes a texture representing the map and generates tiles from the pixels' color values.
	/// </summary>
	/// <param name="texMap"></param>
	public void GenerateMap(Texture2D texMap) {
		MapEntry map = (MapEntry)currentMap.value;
		Color32[] colorData = texMap.GetPixels32();
		int pos = 0;
		List<MapTile> mappus = new List<MapTile>();
		battleMap.breakables.Clear();

		for (int j = 0; j < _sizeY; j++) {
			for (int i = 0; i < _sizeX; i++) {
				InteractPosition interPos = GetInteractable(map, i, j);
				Transform tile = (interPos != null && interPos.interactType == InteractType.BLOCK) ? Instantiate(blockTilePrefab) : Instantiate(tilePrefab);
				tile.position = new Vector3(i, j, 0);
				tile.SetParent(battleMap.tileParent);

				MapTile tempTile = tile.GetComponent<MapTile>();
				tempTile.battlemap = battleMap;
				tempTile.posx = i;
				tempTile.posy = j;
				if (interPos == null) {
					tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
				}
				else if (interPos.interactType == InteractType.BLOCK) {
					tempTile.interactType = InteractType.BLOCK;
					tempTile.SetTerrain(tileBreakable);
					tempTile.alternativeTerrain = GetTerrainFromPixel(colorData[pos]);
					battleMap.breakables.Add(tempTile);

					BlockMove block = tempTile.GetComponent<BlockMove>();
					block.currentTile = tempTile;
					block.stats.hp = interPos.health;
					block.currentHealth = interPos.health;
				}
				else if (interPos.interactType == InteractType.VILLAGE) {
					tempTile.interactType = InteractType.VILLAGE;
					tempTile.alternativeTerrain = GetTerrainFromPixel(colorData[pos]);
					tempTile.dialogue = interPos.dialogue;
					tempTile.gift = interPos.gift;
					if (interPos.ally.charData != null) {
						tempTile.ally = mapSpawner.SpawnVillageCharacter(interPos);
						Debug.Log("Spawned ally:  " + tempTile.ally.name);
					}
					TerrainTile terrain = (interPos.gift == null && interPos.ally == null) ? tileHouse : tileHouseReward;
					tempTile.SetTerrain(terrain);
				}
				else if (interPos.interactType == InteractType.CAPTURE) {
					tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
					tempTile.interactType = InteractType.CAPTURE;
				}
				else if (interPos.interactType == InteractType.ESCAPE) {
					tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
					tempTile.interactType = InteractType.ESCAPE;
				}
				else if (interPos.interactType == InteractType.DATABASE) {
					tempTile.SetTerrain(tileChest);
					tempTile.alternativeTerrain = tileChest.substitueTile;
					tempTile.interactType = InteractType.DATABASE;
					tempTile.gift = interPos.gift;
				}
				else if (interPos.interactType == InteractType.DOOR) {
					tempTile.SetTerrain(tileDoor);
					tempTile.alternativeTerrain = tileDoor.substitueTile;
					tempTile.interactType = InteractType.DOOR;
					tempTile.gift = interPos.gift;
				}
				else {
					Debug.LogError("Unimplemented interact type   " + interPos.interactType);
				}
				mappus.Add(tempTile);

				pos++;
			}
		}

		battleMap.tiles = mappus.ToArray();
		Debug.Log("Data read and map created");
	}

	/// <summary>
	/// Checks the list of interactables and returns the one at the given position 
	/// or null if there is none.
	/// </summary>
	/// <param name="map"></param>
	/// <param name="posx"></param>
	/// <param name="posy"></param>
	/// <returns></returns>
	private InteractPosition GetInteractable(MapEntry map, int posx, int posy) {
		for (int i = 0; i < map.interactions.Count; i++) {
			if (map.interactions[i].x == posx && map.interactions[i].y == posy)
				return map.interactions[i];
		}
		return null;
	}

	private void CreateTriggers(MapEntry map) {
		int triggerCount = 0;
		for (int i = 0; i < map.triggerAreas.Count; i++) {
			TriggerArea area = map.triggerAreas[i];
			for (int x = area.xMin; x < area.xMax + 1; x++) {
				for (int y = area.yMin; y < area.yMax + 1; y++) {
					battleMap.GetTile(x, y).AddTrigger(area);
					triggerCount++;
				}
			}
		}
		Debug.Log("Added triggers to tiles:  " + triggerCount);
	}

	/// <summary>
	/// Spawns the player characters on their designated spawn zones.
	/// </summary>
	private void SpawnPlayers() {
		MapEntry map = (MapEntry)currentMap.value;

		//Players
		int prepPos = 0;
		for (int i = 0; i < map.spawnPoints1.Count; i++) {
			Position pos = map.spawnPoints1[i];

			if (prepPos >= prepList1.values.Count) {
				battleMap.GetTile(pos).selectable = true;
				continue;
			}

			int index = prepList1.values[prepPos].index;
			SpawnData spawn = new SpawnData() {
				x = pos.x,
				y = pos.y,
				stats = playerData.stats[index],
				inventoryContainer = playerData.inventory[index],
				skills = playerData.skills[index],
				joiningSquad = 1
			};
			prepPos++;

			mapSpawner.SpawnPlayerCharacter(spawn, false, true, false);
		}

		prepPos = 0;
		for (int i = 0; i < map.spawnPoints2.Count; i++) {
			Position pos = map.spawnPoints2[i];

			if (prepPos >= prepList2.values.Count) {
				battleMap.GetTile(pos).selectable = true;
				continue;
			}

			int index = prepList2.values[prepPos].index;
			SpawnData spawn = new SpawnData() {
				x = pos.x,
				y = pos.y,
				stats = playerData.stats[index],
				inventoryContainer = playerData.inventory[index],
				skills = playerData.skills[index],
				joiningSquad = 2
			};
			prepPos++;

			mapSpawner.SpawnPlayerCharacter(spawn, false, true, false);
		}
	}

	/// <summary>
	/// Takes the map information and spawns all the enemy characters on their given positions.
	/// </summary>
	private void SpawnEnemies() {
		MapEntry map = (MapEntry)currentMap.value;

		//Enemies
		for (int i = 0; i < map.enemies.Count; i++) {
			mapSpawner.SpawnEnemyCharacter(map.enemies[i]);
		}
	}

	/// <summary>
	/// Takes the map information and spawns all the ally characters on their given positions.
	/// </summary>
	private void SpawnAllies() {
		MapEntry map = (MapEntry)currentMap.value;

		//Allies
		for (int i = 0; i < map.allies.Count; i++) {
			mapSpawner.SpawnAllyCharacter(map.allies[i]);
		}
	}

	public void RespawnPlayers() {
		for (int i = 0; i < battleMap.playerList.values.Count; i++) {
			Destroy(battleMap.playerList.values[i].gameObject);
		}
		battleMap.playerList.values.Clear();

		SpawnPlayers();
	}

	private TerrainTile GetTerrainFromPixel(Color32 pixelColor) {
		TerrainTile terrain = tileNormal;

		if (pixelColor.r == 255 && pixelColor.g == 255 && pixelColor.b == 255) {
			//Normal empty space
		}
		else if (pixelColor.r == 128 && pixelColor.g == 64 && pixelColor.b == 0) {
			terrain = tileDirt;
		}
		else if (pixelColor.r == 128 && pixelColor.g == 128 && pixelColor.b == 128) {
			terrain = tileFloor;
		}
		else if (pixelColor == new Color(0f, 0f, 0f, 1f)) {
			terrain = tileWall;
		}
		else if (pixelColor == new Color(0f, 1f, 0f, 1f)) {
			terrain = tileForest;
		}
		else if (pixelColor == new Color(0f, 0f, 1f, 1f)) {
			terrain = tileRiver;
		}
		else if (pixelColor == new Color(1f, 0f, 0f, 1f)) {
			terrain = tileMountain;
		}
		else if (pixelColor == new Color(1f, 1f, 0f, 1f)) {
			terrain = tileBridge;
		}
		else if (pixelColor == new Color(1f, 0f, 1f, 1f)) {
			terrain = tileLedge;
		}
		else if (pixelColor == new Color(0f, 1f, 1f, 1f)) {
			terrain = tileFort;
		}
		else if (pixelColor.r == 255 && pixelColor.g == 128 && pixelColor.b == 0) {
			terrain = tileHouse;
		}
		else if (pixelColor.r == 0 && pixelColor.g == 128 && pixelColor.b == 255) {
			terrain = tileBreakable;
		}
		else if (pixelColor.r == 128 && pixelColor.g == 128 && pixelColor.b == 0) {
			terrain = tileThrone;
		}
		else if (pixelColor.r == 128 && pixelColor.g == 0 && pixelColor.b == 255) {
			terrain = tilePillar;
		}
		else if (pixelColor.r == 255 && pixelColor.g == 0 && pixelColor.b == 128) {
			terrain = tileDoor;
		}

		return terrain;
	}
}
