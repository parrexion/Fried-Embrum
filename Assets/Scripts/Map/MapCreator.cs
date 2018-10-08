using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCreator : MonoBehaviour {

	public ScrObjEntryReference currentMap;
	public SaveListVariable availableCharacters;
	public BattleMap battleMap;
	public MapCursor mapClicker;

	public IntVariable cursorX;
	public IntVariable cursorY;
	public IntVariable currentTurn;
	public FactionVariable currentFaction;
	public float reinforcementDelay = 0.75f;
	public IntVariable slowGameSpeed;
	public IntVariable currentGameSpeed;
	
	[Header("Prefabs")]
	public Transform playerPrefab;
	public Transform enemyPrefab;
	public Transform tilePrefab;
	public Transform blockTilePrefab;
	
	[Header("Dialogues")]
	public BoolVariable lockControls;
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;

	[Header("Events")]
	public UnityEvent cursorMoveEvent;
	public UnityEvent nextTurnStateEvent;
	public UnityEvent startDialogueEvent;
	
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
		battleMap.SetupMap(map);
		
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
				tile.position = new Vector3(i,j,0);
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
					tempTile.SetTerrain(tileHouse);
					tempTile.alternativeTerrain = GetTerrainFromPixel(colorData[pos]);
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
				battleMap.GetTile(pos.x, pos.y).selectable = true;
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
			MapTile huntTile = (pos.aggroType == AggroType.HUNT) ? battleMap.GetTile(pos.huntX, pos.huntY) : null; 

			SpawnEnemyCharacter(pos.x, pos.y, stats, inventory, skills, quotes, pos.aggroType, huntTile);
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
		Transform playerTransform = Instantiate(playerPrefab, battleMap.playerParent);
		playerTransform.position = new Vector3(x, y);

		TacticsMove tactics = playerTransform.GetComponent<TacticsMove>();
		tactics.battleMap = battleMap;
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
	private void SpawnEnemyCharacter(int x, int y, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills, List<FightQuote> quotes, AggroType aggro, MapTile huntTile) {
		Transform enemyTransform = Instantiate(enemyPrefab, battleMap.enemyParent);
		enemyTransform.position = new Vector3(x, y);

		NPCMove tactics = enemyTransform.GetComponent<NPCMove>();
		tactics.battleMap = battleMap;
		tactics.posx = x;
		tactics.posy = y;
		tactics.stats = stats;
		tactics.inventory = inventory;
		tactics.skills = skills;
		tactics.fightQuotes = quotes;
		tactics.aggroType = aggro;
		tactics.huntTile = huntTile;
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
				MapTile tile = battleMap.GetTile(pos.x, pos.y);
				if (tile.currentCharacter == null) {
					StatsContainer stats = new StatsContainer(pos.stats, pos.level);
					InventoryContainer inventory = new InventoryContainer(pos.inventory);
					SkillsContainer skills = new SkillsContainer(pos.skills);

					SpawnEnemyCharacter(pos.x, pos.y, stats, inventory, skills, new List<FightQuote>(), AggroType.CHARGE, null);
					cursorX.value = pos.x;
					cursorY.value = pos.y;
					cursorMoveEvent.Invoke();
					// Debug.Log("Hello there!     " + (reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value));
					yield return new WaitForSeconds(reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value);
				}
			}
		}
		Debug.Log("DONE!");
		nextTurnStateEvent.Invoke();
		Debug.Log("DONE2!");
		yield break;
	}

	public void CheckDialogues() {
		MapEntry map = (MapEntry)currentMap.value;
		for (int i = 0; i < map.turnEvents.Count; i++) {
			TurnEvent pos = map.turnEvents[i];
			if (currentTurn.value == pos.turn && currentFaction.value == pos.factionTurn && pos.type == TurnEventType.DIALOGUE) {
				Debug.Log("It's time!");
				currentDialogue.value = pos.dialogue;
				currentDialogueMode.value = (int)DialogueMode.EVENT;
				lockControls.value = false;
				startDialogueEvent.Invoke();
				return;
			}
		}
		nextTurnStateEvent.Invoke();
	}

	public void CheckMapChange() {
		MapEntry map = (MapEntry)currentMap.value;
		for (int i = 0; i < map.turnEvents.Count; i++) {
			TurnEvent pos = map.turnEvents[i];
			if (currentTurn.value == pos.turn && currentFaction.value == pos.factionTurn && pos.type == TurnEventType.MAPCHANGE) {
				Debug.Log("It's time!");
				MapTile tile = battleMap.GetTile(pos.x, pos.y);
				tile.SetTerrain(pos.changeTerrain);
			}
		}
		nextTurnStateEvent.Invoke();
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
