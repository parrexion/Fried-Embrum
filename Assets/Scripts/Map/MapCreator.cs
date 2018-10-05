using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCreator : MonoBehaviour {

	public ScrObjEntryReference currentMap;
	public MapCursor mapClicker;

	public IntVariable cursorX;
	public IntVariable cursorY;
	public IntVariable currentTurn;
	public float reinforcementDelay = 0.75f;
	public IntVariable slowGameSpeed;
	public IntVariable currentGameSpeed;
	
	public Transform tilePrefab;
	public Transform blockTilePrefab;
	
	[Header("Characters")]
	public Transform enemyParent;
	public Transform enemyPrefab;

	public Transform playerParent;
	public Transform playerPrefab;
	public SaveListVariable availableCharacters;
	
	[Header("Dialogues")]
	public BoolVariable lockControls;
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;

	[Header("Events")]
	public UnityEvent cursorMoveEvent;
	public UnityEvent finishedReinforcementEvent;
	public UnityEvent startDialogueEvent;
	public UnityEvent noDialogueEvent;

	[HideInInspector] public MapTile[] tiles;
	[HideInInspector] public List<MapTile> breakables = new List<MapTile>();
	
	[Header("Terrain Tiles")]
	public TerrainTile tileNormal;
	public TerrainTile tileForest;
	public TerrainTile tileMountain;
	public TerrainTile tileBridge;
	public TerrainTile tileLedge;
	public TerrainTile tileHouse;
	public TerrainTile tileFort;
	public TerrainTile tileRiver;
	public TerrainTile tileBreakable;
	public TerrainTile tileWall;
	public TerrainTile tileThrone;
	public TerrainTile tilePillar;

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
		
		TacticsCamera.boxCollider.size = new Vector2(_sizeX+1, _sizeY+1);
		TacticsCamera.boxCollider.center = new Vector3((_sizeX-1)/2.0f, (_sizeY-1)/2.0f, 0);
		TacticsCamera.boxActive = true;
		
		GenerateMap(map.mapSprite);
		cursorX.value = map.spawnPoints[0].x;
		cursorY.value = map.spawnPoints[0].y;
		cursorMoveEvent.Invoke();
		SpawnCharacters();
		Debug.Log("Finished creating map");
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
	
	public void ClearDangerous() {
		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].dangerous = false;
		}
	}

	private int TilePosition(int x, int y) {
		return x + y * _sizeX;
	}

	/// <summary>
	/// Returns the tile for the given coordinates.
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

	/// <summary>
	/// Takes a texture representing the map and generates tiles from the pixels' color values.
	/// </summary>
	/// <param name="texMap"></param>
	public void GenerateMap(Texture2D texMap) {
		MapEntry map = (MapEntry)currentMap.value;
		Color32[] colorData = texMap.GetPixels32();
		int pos = 0;
		List<MapTile> mappus = new List<MapTile>();
		breakables.Clear();
		
		for (int j = 0; j < _sizeY; j++) {
			for (int i = 0; i < _sizeX; i++) {
				InteractPosition interPos = GetInteractable(map, i, j);
				Transform tile = (interPos != null && interPos.interactType == InteractType.BLOCK) ? Instantiate(blockTilePrefab) : Instantiate(tilePrefab);
				tile.position = new Vector3(i,j,0);
				tile.parent = transform;

				MapTile tempTile = tile.GetComponent<MapTile>();
				tempTile.mapCreator = this;
				tempTile.posx = i;
				tempTile.posy = j;
				if (interPos == null) {
					tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
				}
				else if (interPos.interactType == InteractType.BLOCK) {
					tempTile.interactType = InteractType.BLOCK;
					tempTile.SetTerrain(tileBreakable);
					tempTile.alternativeTerrain = GetTerrainFromPixel(colorData[pos]);
					breakables.Add(tempTile);
					
					BlockMove block = tempTile.GetComponent<BlockMove>();
					block.currentTile = tempTile;
					block.stats.hp = interPos.health;
					block.currentHealth = interPos.health;
				}
				else if (interPos.interactType == InteractType.VILLAGE) {
					tempTile.interactType = InteractType.VILLAGE;
					tempTile.SetTerrain(tileHouse);
					tempTile.dialogue = interPos.dialogue;
					tempTile.gift = interPos.gift;
					if (interPos.ally.stats != null) {
						StatsContainer stats = new StatsContainer(interPos.ally.stats, interPos.ally.level);
						InventoryContainer inventory = new InventoryContainer(interPos.ally.inventory);
						SkillsContainer skills = new SkillsContainer(interPos.ally.skills);
						tempTile.ally = SpawnPlayerCharacter(interPos.x, interPos.y, stats, inventory, skills, false);
					}
				}
				else if (interPos.interactType == InteractType.SEIZE) {
					tempTile.SetTerrain(GetTerrainFromPixel(colorData[pos]));
					tempTile.interactType = InteractType.SEIZE;
				}
				else {
					Debug.LogError("Unimplemented interact type");
				}
				mappus.Add(tempTile);

				pos++;
			}
		}

		tiles = mappus.ToArray();
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

	/// <summary>
	/// Takes the map information and spawns all the characters on their given positions.
	/// </summary>
	private void SpawnCharacters() {
		MapEntry map = (MapEntry)currentMap.value;
		
		//Players
		for (int i = 0; i < map.spawnPoints.Count; i++) {
			PlayerPosition pos = map.spawnPoints[i];
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

			SpawnPlayerCharacter(pos.x, pos.y, stats, inventory, skills, true);
		}
		
		//Enemies
		for (int i = 0; i < map.enemies.Count; i++) {
			EnemyPosition pos = map.enemies[i];

			StatsContainer stats = new StatsContainer(pos.stats, pos.level);
			InventoryContainer inventory = new InventoryContainer(pos.inventory);
			SkillsContainer skills = new SkillsContainer(pos.skills);
			List<FightQuote> quotes = new List<FightQuote>();
			for (int q = 0; q < pos.quotes.Count; q++) {
				FightQuote fight = new FightQuote();
				fight.triggerer = pos.quotes[q].triggerer;
				fight.quote = pos.quotes[q].quote;
				fight.activated = false;
				quotes.Add(fight);
			}

			SpawnEnemyCharacter(pos.x, pos.y, stats, inventory, skills, quotes, pos.aggroType);
		}
	}

	/// <summary>
	/// Spawns a player character on the map.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	private TacticsMove SpawnPlayerCharacter(int x, int y, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills, bool active) {
		Transform playerTransform = Instantiate(playerPrefab, playerParent);
		playerTransform.position = new Vector3(x, y);

		TacticsMove tactics = playerTransform.GetComponent<TacticsMove>();
		tactics.mapCreator = this;
		tactics.posx = x;
		tactics.posy = y;
		tactics.stats = stats;
		tactics.inventory = inventory;
		tactics.skills = skills;
		if (active)
			tactics.Setup();
		else 
			playerTransform.gameObject.SetActive(false);

		return tactics;
	}

	/// <summary>
	/// Spawns an enemy character on the map.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	/// <param name="quotes"></param>
	/// <param name="aggro"></param>
	private void SpawnEnemyCharacter(int x, int y, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills, List<FightQuote> quotes, AggroType aggro) {
		Transform enemyTransform = Instantiate(enemyPrefab, enemyParent);
		enemyTransform.position = new Vector3(x, y);

		NPCMove tactics = enemyTransform.GetComponent<NPCMove>();
		tactics.mapCreator = this;
		tactics.posx = x;
		tactics.posy = y;
		tactics.stats = stats;
		tactics.inventory = inventory;
		tactics.skills = skills;
		tactics.fightQuotes = quotes;
		tactics.aggroType = aggro;
		tactics.Setup();
	}
	
	/// <summary>
	/// Is triggered by event and checks the map to see if any reinforcements should be spawned.
	/// </summary>
	public void SpawnReinforcements() {
		Debug.Log("Show some reinforcements!");
		StartCoroutine(SpawnReinforcementsLoop());
	}

	/// <summary>
	/// Loop which goes through each reinforcement event and spawns the ones which are available.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SpawnReinforcementsLoop() {
		MapEntry map = (MapEntry)currentMap.value;
		for (int i = 0; i < map.reinforcements.Count; i++) {
			EnemyPosition pos = map.reinforcements[i];
			if (currentTurn.value == pos.spawnTurn) {
				MapTile tile = GetTile(pos.x, pos.y);
				if (tile.currentCharacter == null) {
					StatsContainer stats = new StatsContainer(pos.stats, pos.level);
					InventoryContainer inventory = new InventoryContainer(pos.inventory);
					SkillsContainer skills = new SkillsContainer(pos.skills);

					SpawnEnemyCharacter(pos.x, pos.y, stats, inventory, skills, new List<FightQuote>(), AggroType.CHARGE);
					Debug.Log("Hello there!     " + (reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value));
					yield return new WaitForSeconds(reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value);
				}
			}
		}
		Debug.Log("DONE!");
		finishedReinforcementEvent.Invoke();
		Debug.Log("DONE2!");
		yield break;
	}

	public void CheckDialogues() {
		MapEntry map = (MapEntry)currentMap.value;
		for (int i = 0; i < map.turnEvents.Count; i++) {
			TurnEvent pos = map.turnEvents[i];
			if (currentTurn.value == pos.turn) {
				Debug.Log("It's time!");
				currentDialogue.value = pos.dialogue;
				currentDialogueMode.value = (int)DialogueMode.EVENT;
				lockControls.value = false;
				startDialogueEvent.Invoke();
				return;
			}
		}
		noDialogueEvent.Invoke();
	}

	private TerrainTile GetTerrainFromPixel(Color32 pixelColor) {
		TerrainTile terrain = tileNormal;

		if (pixelColor.r == 255 && pixelColor.g == 255 && pixelColor.b == 255) {
			//Normal empty space
		}
		else if (pixelColor == new Color(0f,0f,0f,1f)) {
			terrain = tileWall;
		}
		else if (pixelColor == new Color(0f,1f,0f,1f)) {
			terrain = tileForest;
		}
		else if (pixelColor == new Color(0f,0f,1f,1f)) {
			terrain = tileRiver;
		}
		else if (pixelColor == new Color(1f,0f,0f,1f)) {
			terrain = tileMountain;
		}
		else if (pixelColor == new Color(1f,1f,0f,1f)) {
			terrain = tileBridge;
		}
		else if (pixelColor == new Color(1f,0f,1f,1f)) {
			terrain = tileLedge;
		}
		else if (pixelColor == new Color(0f,1f,1f,1f)) {
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

		return terrain;
	}
}
