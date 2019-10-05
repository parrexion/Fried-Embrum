﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapCreator : MonoBehaviour {

	public ScrObjEntryReference currentMap;
	public PlayerData playerData;
	public ClassWheel playerClassWheel;
	public ClassWheel enemyClassWheel;
	public PrepListVariable prepList1;
	public PrepListVariable prepList2;
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
						StatsContainer stats = new StatsContainer(interPos.ally);
						InventoryContainer inventory = new InventoryContainer(playerClassWheel.GetWpnSkillFromLevel(interPos.ally.charData.startClassLevels), interPos.ally.inventory);
						SkillsContainer skills = new SkillsContainer(playerClassWheel.GetSkillsFromLevel(interPos.ally.charData.startClassLevels, interPos.ally.charData.startClass, interPos.ally.level));
						tempTile.ally = SpawnPlayerCharacter(interPos.x, interPos.y, stats, inventory, skills, 0, false);
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
			StatsContainer stats = playerData.stats[index];
			InventoryContainer inventory = playerData.inventory[index];
			SkillsContainer skills = playerData.skills[index];
			prepPos++;

			SpawnPlayerCharacter(pos.x, pos.y, stats, inventory, skills, 1, true);
		}

		prepPos = 0;
		for (int i = 0; i < map.spawnPoints2.Count; i++) {
			Position pos = map.spawnPoints2[i];

			if (prepPos >= prepList2.values.Count) {
				battleMap.GetTile(pos).selectable = true;
				continue;
			}

			int index = prepList2.values[prepPos].index;
			StatsContainer stats = playerData.stats[index];
			InventoryContainer inventory = playerData.inventory[index];
			SkillsContainer skills = playerData.skills[index];
			prepPos++;

			SpawnPlayerCharacter(pos.x, pos.y, stats, inventory, skills, 2, true);
		}
	}

	/// <summary>
	/// Takes the map information and spawns all the enemy characters on their given positions.
	/// </summary>
	private void SpawnEnemies() {
		MapEntry map = (MapEntry)currentMap.value;

		//Enemies
		for (int i = 0; i < map.enemies.Count; i++) {
			ReinforcementPosition pos = map.enemies[i];

			StatsContainer stats = new StatsContainer(pos);
			InventoryContainer inventory = new InventoryContainer(enemyClassWheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
			SkillsContainer skills = new SkillsContainer(enemyClassWheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
			List<FightQuote> quotes = new List<FightQuote>();
			for (int q = 0; q < pos.quotes.Count; q++) {
				FightQuote fight = new FightQuote();
				fight.triggerer = pos.quotes[q].triggerer;
				fight.quote = pos.quotes[q].quote;
				fight.activated = false;
				quotes.Add(fight);
			}

			SpawnEnemyCharacter(pos, stats, inventory, skills);
		}
	}

	/// <summary>
	/// Takes the map information and spawns all the ally characters on their given positions.
	/// </summary>
	private void SpawnAllies() {
		MapEntry map = (MapEntry)currentMap.value;

		//Allies
		for (int i = 0; i < map.allies.Count; i++) {
			ReinforcementPosition pos = map.allies[i];

			StatsContainer stats = new StatsContainer(pos);
			InventoryContainer inventory = new InventoryContainer(playerClassWheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
			SkillsContainer skills = new SkillsContainer(playerClassWheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
			List<FightQuote> quotes = new List<FightQuote>();
			for (int q = 0; q < pos.quotes.Count; q++) {
				FightQuote fight = new FightQuote();
				fight.triggerer = pos.quotes[q].triggerer;
				fight.quote = pos.quotes[q].quote;
				fight.activated = false;
				quotes.Add(fight);
			}

			SpawnAllyCharacter(pos, stats, inventory, skills);
		}
	}

	public void RespawnPlayers() {
		for (int i = 0; i < battleMap.playerList.values.Count; i++) {
			Destroy(battleMap.playerList.values[i].gameObject);
		}
		battleMap.playerList.values.Clear();

		SpawnPlayers();
	}

	/// <summary>
	/// Spawns a player character on the map.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	public TacticsMove SpawnPlayerCharacter(int x, int y, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills, int squad, bool active) {
		Transform playerTransform = Instantiate(playerPrefab, battleMap.playerParent);
		PlayerMove tactics = playerTransform.GetComponent<PlayerMove>();
		tactics.battleMap = battleMap;
		tactics.posx = x;
		tactics.posy = y;
		tactics.stats = stats;
		tactics.inventory = inventory;
		tactics.skills = skills;
		tactics.squad = squad;
		if (active)
			tactics.Setup();
		else
			playerTransform.gameObject.SetActive(false);

		return tactics;
	}

	/// <summary>
	/// Spawns an enemy character on the map.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	private void SpawnEnemyCharacter(ReinforcementPosition pos, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills) {
		Transform enemyTransform = Instantiate(enemyPrefab, battleMap.enemyParent);
		NPCMove tactics = enemyTransform.GetComponent<NPCMove>();
		tactics.battleMap = battleMap;
		tactics.posx = pos.x;
		tactics.posy = pos.y;
		tactics.stats = stats;
		tactics.faction = Faction.ENEMY;
		tactics.inventory = inventory;
		tactics.skills = skills;
		tactics.fightQuotes = pos.quotes;
		tactics.talkQuotes = pos.talks;
		tactics.aggroType = pos.aggroType;
		tactics.huntTile = battleMap.GetTile(pos.huntX, pos.huntY);
		tactics.patrolTiles.Clear();
		for (int i = 0; i < pos.patrolPositions.Count; i++) {
			tactics.patrolTiles.Add(battleMap.GetTile(pos.patrolPositions[i]));
		}
		tactics.Setup();
	}

	/// <summary>
	/// Spawns an enemy character on the map.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	private void SpawnAllyCharacter(ReinforcementPosition pos, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills) {
		Transform allyTransform = Instantiate(enemyPrefab, battleMap.enemyParent);
		NPCMove tactics = allyTransform.GetComponent<NPCMove>();
		tactics.battleMap = battleMap;
		tactics.posx = pos.x;
		tactics.posy = pos.y;
		tactics.stats = stats;
		tactics.faction = Faction.ALLY;
		tactics.inventory = inventory;
		tactics.skills = skills;
		tactics.fightQuotes = pos.quotes;
		tactics.talkQuotes = pos.talks;
		tactics.aggroType = pos.aggroType;
		tactics.huntTile = battleMap.GetTile(pos.huntX, pos.huntY);
		tactics.patrolTiles.Clear();
		for (int i = 0; i < pos.patrolPositions.Count; i++) {
			tactics.patrolTiles.Add(battleMap.GetTile(pos.patrolPositions[i]));
		}
		tactics.Setup();
	}

	/// <summary>
	/// Is triggered by event and checks the map to see if any reinforcements should be spawned.
	/// </summary>
	public void SpawnReinforcements() {
		StartCoroutine(SpawnReinforcementsLoop());
	}

	/// <summary>
	/// Loop which goes through each reinforcement event and spawns the ones which are available.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SpawnReinforcementsLoop() {
		for (int i = 0; i < battleMap.reinforcementEvents.Count; i++) {
			ReinforcementPosition pos = battleMap.reinforcementEvents.GetEvent(i);
			if (battleMap.reinforcementEvents.IsActivated(i) || currentFaction.value != pos.faction)
				continue;

			bool activate = false;
			if (pos.triggerType == TriggerType.TURN)
				activate = (currentTurn.value >= pos.spawnTurn);
			else if (pos.triggerType == TriggerType.TRIGGER) {
				activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
			}
			else if (pos.triggerType == TriggerType.PLAYER_COUNT) {
				activate = battleMap.playerList.Count <= pos.spawnTurn;
			}

			if (!activate) {
				continue;
			}

			MapTile tile = battleMap.GetTile(pos.x, pos.y);
			if (tile.currentCharacter == null) {
				battleMap.reinforcementEvents.Activate(i);
				StatsContainer stats = new StatsContainer(pos);
				ClassWheel wheel = (stats.charData.faction == Faction.PLAYER) ? playerClassWheel : enemyClassWheel;
				InventoryContainer inventory = new InventoryContainer(wheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
				SkillsContainer skills = new SkillsContainer(wheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
				if (pos.charData.faction == Faction.PLAYER) {
					TacticsMove tm = SpawnPlayerCharacter(pos.x, pos.y, stats, inventory, skills, pos.joiningSquad, true);
					playerData.AddNewPlayer(tm);
					PrepCharacter prep = new PrepCharacter(playerData.stats.Count - 1);
					if (pos.joiningSquad == 2) {
						prepList2.values.Add(prep);
					}
					else {
						prepList1.values.Add(prep);
					}
				}
				else if (pos.charData.faction == Faction.ENEMY) {
					SpawnEnemyCharacter(pos, stats, inventory, skills);
				}
				else {
					Debug.LogError("Unimplemented faction  " + pos.faction);
				}
				cursorX.value = pos.x;
				cursorY.value = pos.y;
				cursorMoveEvent.Invoke();
				// Debug.Log("Hello there!     " + (reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value));
				yield return new WaitForSeconds(reinforcementDelay * slowGameSpeed.value / currentGameSpeed.value);
			}
		}
		nextTurnStateEvent.Invoke();
		yield break;
	}

	/// <summary>
	/// Checks if there are any dialogues that should be shown.
	/// </summary>
	public void CheckDialogueEvents() {
		for (int i = 0; i < battleMap.dialogueEvents.Count; i++) {
			TurnEvent pos = battleMap.dialogueEvents.GetEvent(i);
			if (battleMap.dialogueEvents.IsActivated(i) || currentFaction.value != pos.factionTurn)
				continue;

			bool activate = false;
			switch (pos.triggerType) {
				case TriggerType.TURN:
					activate = (currentTurn.value == pos.turn);
					break;
				case TriggerType.TRIGGER:
					activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
					break;
				case TriggerType.PLAYER_COUNT:
					activate = (battleMap.playerList.values.Count <= pos.turn);
					break;
				case TriggerType.ALLY_COUNT:
					activate = (battleMap.allyList.values.Count <= pos.turn);
					break;
				case TriggerType.ENEMY_COUNT:
					activate = (battleMap.enemyList.AliveCount() <= pos.turn);
					break;
			}

			if (activate) {
				battleMap.dialogueEvents.Activate(i);
				currentDialogue.value = pos.dialogue;
				currentDialogueMode.value = (int)DialogueMode.EVENT;
				startDialogueEvent.Invoke();
				return;
			}
		}
		//If no dialogues were triggered
		nextTurnStateEvent.Invoke();
	}

	/// <summary>
	/// Checks if there are any events that should be triggered.
	/// </summary>
	public void CheckOtherEvents() {
		for (int i = 0; i < battleMap.otherEvents.Count; i++) {
			TurnEvent pos = battleMap.otherEvents.GetEvent(i);
			if (battleMap.otherEvents.IsActivated(i) || currentFaction.value != pos.factionTurn)
				continue;

			bool activate = false;
			switch (pos.triggerType) {
				case TriggerType.TURN:
					activate = (currentTurn.value == pos.turn);
					break;
				case TriggerType.TRIGGER:
					activate = battleMap.triggerList.IsTriggered(pos.triggerIndex);
					break;
				case TriggerType.PLAYER_COUNT:
					activate = (battleMap.playerList.values.Count <= pos.turn);
					break;
				case TriggerType.ALLY_COUNT:
					activate = (battleMap.allyList.values.Count <= pos.turn);
					break;
				case TriggerType.ENEMY_COUNT:
					activate = (battleMap.enemyList.AliveCount() <= pos.turn);
					break;
			}

			if (activate) {
				battleMap.otherEvents.Activate(i);
				switch (pos.type) {
					case TurnEventType.MAPCHANGE:
						MapTile tile = battleMap.GetTile(pos.x, pos.y);
						tile.SetTerrain(pos.changeTerrain);
						break;
					case TurnEventType.MONEY:
						Debug.Log("Gained money:  " + pos.value);
						break;
					case TurnEventType.SCRAP:
						Debug.Log("Gained scrap:  " + pos.value);
						break;
				}
			}
		}
		nextTurnStateEvent.Invoke();
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

		return terrain;
	}
}
