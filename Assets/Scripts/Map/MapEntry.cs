using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType { TURN, TRIGGER, PLAYER_COUNT, ALLY_COUNT, ENEMY_COUNT }
public enum WinCondition { ROUT, CAPTURE, BOSS, ESCAPE, DEFEND }
public enum LoseCondition { NONE, TIME }
public enum MapLocation { UNKNOWN = -1, DEBES, GHART, THARSONIS, VILJIA, WALNIA_REX }

[CreateAssetMenu(menuName = "LibraryEntries/Map")]
public class MapEntry : ScrObjLibraryEntry {

	[Header("Map")]
	public int sizeX;
	public int sizeY;
	public Texture2D mapSprite;

	[Header("Map Info")]
	public WinCondition winCondition;
	public LoseCondition loseCondition;
	public string mapDescription;
	public int turnLimit;

	[Header("Dialogues")]
	public bool skipBattlePrep;
	public DialogueEntry preDialogue;
	public DialogueEntry introDialogue;
	public DialogueEntry endDialogue;

	[Header("Music")]
	public MusicSetEntry mapMusic;

	[Header("Players")]
	public List<Position> spawnPoints1 = new List<Position>();
	public List<Position> spawnPoints2 = new List<Position>();
	public List<CharEntry> forcedCharacters = new List<CharEntry>();
	public List<CharEntry> lockedCharacters = new List<CharEntry>();
	
	[Header("Other characters")]
	public List<SpawnData> enemies = new List<SpawnData>();
	public List<SpawnData> allies = new List<SpawnData>();
	public List<SpawnData> reinforcements = new List<SpawnData>();
	
	[Header("Interactions")]
	public List<InteractPosition> interactions = new List<InteractPosition>();
	
	[Header("Events")]
	public List<TriggerTuple> triggerIds = new List<TriggerTuple>();
	public List<TriggerArea> triggerAreas = new List<TriggerArea>();
	public List<TurnEvent> turnEvents = new List<TurnEvent>();


	public override void ResetValues() {
		base.ResetValues();
		sizeX = 0;
		sizeY = 0;
		mapSprite = null;
		
		mapDescription = "";

		winCondition = WinCondition.ROUT;
		loseCondition = LoseCondition.NONE;
		turnLimit = 0;
		
		skipBattlePrep = false;
		preDialogue = null;
		introDialogue = null;
		endDialogue = null;

		mapMusic = null;

		spawnPoints1 = new List<Position>();
		spawnPoints2 = new List<Position>();
		forcedCharacters = new List<CharEntry>();
		lockedCharacters = new List<CharEntry>();

		enemies = new List<SpawnData>();
		allies = new List<SpawnData>();
		reinforcements = new List<SpawnData>();

		interactions = new List<InteractPosition>();

		triggerIds = new List<TriggerTuple>();
		triggerAreas = new List<TriggerArea>();
		turnEvents = new List<TurnEvent>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		MapEntry map = (MapEntry)other;

		sizeX = map.sizeX;
		sizeY = map.sizeY;
		mapSprite = map.mapSprite;
		
		mapDescription = map.mapDescription;

		winCondition = map.winCondition;
		loseCondition = map.loseCondition;
		turnLimit = map.turnLimit;
		
		skipBattlePrep = map.skipBattlePrep;
		preDialogue = map.preDialogue;
		introDialogue = map.introDialogue;
		endDialogue = map.endDialogue;

		mapMusic = map.mapMusic;

		spawnPoints1 = new List<Position>();
		for (int i = 0; i < map.spawnPoints1.Count; i++) {
			spawnPoints1.Add(map.spawnPoints1[i]);
		}
		spawnPoints2 = new List<Position>();
		for (int i = 0; i < map.spawnPoints2.Count; i++) {
			spawnPoints2.Add(map.spawnPoints2[i]);
		}
		forcedCharacters = new List<CharEntry>();
		for (int i = 0; i < map.forcedCharacters.Count; i++) {
			forcedCharacters.Add(map.forcedCharacters[i]);
		}
		lockedCharacters = new List<CharEntry>();
		for (int i = 0; i < map.lockedCharacters.Count; i++) {
			lockedCharacters.Add(map.lockedCharacters[i]);
		}

		enemies = new List<SpawnData>();
		for (int i = 0; i < map.enemies.Count; i++) {
			enemies.Add(map.enemies[i]);
		}
		allies = new List<SpawnData>();
		for (int i = 0; i < map.allies.Count; i++) {
			allies.Add(map.allies[i]);
		}
		reinforcements = new List<SpawnData>();
		for (int i = 0; i < map.reinforcements.Count; i++) {
			reinforcements.Add(map.reinforcements[i]);
		}

		interactions = new List<InteractPosition>();
		for (int i = 0; i < map.interactions.Count; i++) {
			interactions.Add(map.interactions[i]);
		}

		triggerIds = new List<TriggerTuple>();
		for (int i = 0; i < map.triggerIds.Count; i++) {
			triggerIds.Add(map.triggerIds[i]);
		}
		triggerAreas = new List<TriggerArea>();
		for (int i = 0; i < map.triggerAreas.Count; i++) {
			triggerAreas.Add(map.triggerAreas[i]);
		}
		turnEvents = new List<TurnEvent>();
		for (int i = 0; i < map.turnEvents.Count; i++) {
			turnEvents.Add(map.turnEvents[i]);
		}
	}

	/// <summary>
	/// Checks the forced character list to see if the given character appears.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public bool IsForced(CharEntry data) {
		for (int i = 0; i < forcedCharacters.Count; i++) {
			if (forcedCharacters[i].entryName == data.entryName)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Checks the locked character list to see if the given character appears.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public bool IsLocked(CharEntry data) {
		for (int i = 0; i < lockedCharacters.Count; i++) {
			if (lockedCharacters[i].entryName == data.entryName)
				return true;
		}
		return false;
	}
}

[System.Serializable]
public class Position {
	public int x;
	public int y;
}

[System.Serializable]
public class CharacterSpawnData {
	public int x;
	public int y;
	public int level = 1;
	public CharEntry charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
}

[System.Serializable]
public class WeaponTuple {
	public ItemEntry item;
	public bool droppable;
}

public enum InteractType { NONE, BLOCK, DIALOGUE, VILLAGE, CAPTURE, DATABASE, ESCAPE }

[System.Serializable]
public class InteractPosition {
	public int x;
	public int y;
	public InteractType interactType;
	public int health;
	public DialogueEntry dialogue;
	public Reward gift = new Reward();
	public CharacterSpawnData ally = new CharacterSpawnData();
}

public enum TurnEventType { NONE, DIALOGUE, MAPCHANGE, MONEY, SCRAP }
[System.Serializable]
public class TurnEvent {
	public TriggerType triggerType;
	public int turn;
	public int triggerIndex;
	public Faction factionTurn;
	public TurnEventType type;
	public DialogueEntry dialogue;
	public int x;
	public int y;
	public TerrainTile changeTerrain;
	public int value;

	public override string ToString() {
		return "Type: " + type + ", Turn: " + turn + ", faction: " + factionTurn;
	}
}

[System.Serializable]
public class FightQuote {
	public CharEntry triggerer;
	public DialogueEntry quote;
	public bool willJoin;
	public bool activated;
}

[System.Serializable]
public class Reward {
	public int money;
	public int scrap;
	public List<ItemEntry> items = new List<ItemEntry>();


	public bool IsEmpty() {
		if (money != 0 || scrap != 0)
			return false;

		for (int i = 0; i < items.Count; i++) {
			if (items[i] != null)
				return false;
		}

		return true;
	}
}

[System.Serializable]
public class TriggerArea {
	public int idIndex;
	public Faction faction;
	public int xMin;
	public int xMax;
	public int yMin;
	public int yMax;
}