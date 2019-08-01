﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WinCondition { ROUT, SEIZE, BOSS, ESCAPE, DEFEND }
public enum LoseCondition { NORMAL, SEIZE, PROTECT }
public enum MapLocation { UNKNOWN = -1, DEBES, GHART, THARSONIS, VILJIA, WALNIA_REX }

[CreateAssetMenu(menuName = "LibraryEntries/MapEntry")]
public class MapEntry : ScrObjLibraryEntry {

	[Header("Map")]
	public int sizeX;
	public int sizeY;
	public Texture2D mapSprite;

	[Header("Map Info")]
	public MapLocation mapLocation;
	public string mapDescription;

	[Header("Objectives")]
	public WinCondition winCondition;
	public LoseCondition loseCondition;
	public Reward reward = new Reward();

	[Header("Chapter Linking")]
	public bool skipBattlePrep;
	public MapEntry autoNextChapter;
	public int mapDuration = 1;
	public int unlockDay = 1;

	[Header("Dialogues")]
	public DialogueEntry preDialogue;
	public DialogueEntry introDialogue;
	public DialogueEntry endDialogue;

	[Header("Music")]
	public MusicEntry playerMusic;
	public MusicEntry enemyMusic;
	public MusicEntry battleMusic;
	public MusicEntry healMusic;

	[Header("Players")]
	public List<Position> spawnPoints = new List<Position>();
	public List<CharData> forcedCharacters = new List<CharData>();
	public List<CharData> lockedCharacters = new List<CharData>();
	
	[Header("Enemies")]
	public List<EnemyPosition> enemies = new List<EnemyPosition>();
	public List<ReinforcementPosition> reinforcements = new List<ReinforcementPosition>();
	
	[Header("Interactions")]
	public List<InteractPosition> interactions = new List<InteractPosition>();
	
	[Header("Turn Events")]
	public List<TurnEvent> turnEvents = new List<TurnEvent>();


	public override void ResetValues() {
		base.ResetValues();
		sizeX = 0;
		sizeY = 0;
		mapSprite = null;

		mapLocation = MapLocation.UNKNOWN;
		mapDescription = "";

		winCondition = WinCondition.ROUT;
		loseCondition = LoseCondition.NORMAL;
		reward = new Reward();

		skipBattlePrep = false;
		autoNextChapter = null;
		mapDuration = 1;
		unlockDay = 1;

		preDialogue = null;
		introDialogue = null;
		endDialogue = null;

		playerMusic = null;
		enemyMusic = null;
		battleMusic = null;
		healMusic = null;

		spawnPoints = new List<Position>();
		forcedCharacters = new List<CharData>();
		lockedCharacters = new List<CharData>();
		enemies = new List<EnemyPosition>();
		interactions = new List<InteractPosition>();
		turnEvents = new List<TurnEvent>();
		reinforcements = new List<ReinforcementPosition>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		MapEntry map = (MapEntry)other;

		sizeX = map.sizeX;
		sizeY = map.sizeY;
		mapSprite = map.mapSprite;

		mapLocation = map.mapLocation;
		mapDescription = map.mapDescription;

		winCondition = map.winCondition;
		loseCondition = map.loseCondition;
		reward = map.reward;

		skipBattlePrep = map.skipBattlePrep;
		autoNextChapter = map.autoNextChapter;
		mapDuration = map.mapDuration;
		unlockDay = map.unlockDay;

		preDialogue = map.preDialogue;
		introDialogue = map.introDialogue;
		endDialogue = map.endDialogue;

		playerMusic = map.playerMusic;
		enemyMusic = map.enemyMusic;
		battleMusic = map.battleMusic;
		healMusic = map.healMusic;

		spawnPoints = new List<Position>();
		for (int i = 0; i < map.spawnPoints.Count; i++) {
			spawnPoints.Add(map.spawnPoints[i]);
		}
		forcedCharacters = new List<CharData>();
		for (int i = 0; i < map.forcedCharacters.Count; i++) {
			forcedCharacters.Add(map.forcedCharacters[i]);
		}
		lockedCharacters = new List<CharData>();
		for (int i = 0; i < map.lockedCharacters.Count; i++) {
			lockedCharacters.Add(map.lockedCharacters[i]);
		}
		enemies = new List<EnemyPosition>();
		for (int i = 0; i < map.enemies.Count; i++) {
			enemies.Add(map.enemies[i]);
		}
		interactions = new List<InteractPosition>();
		for (int i = 0; i < map.interactions.Count; i++) {
			interactions.Add(map.interactions[i]);
		}
		turnEvents = new List<TurnEvent>();
		for (int i = 0; i < map.turnEvents.Count; i++) {
			turnEvents.Add(map.turnEvents[i]);
		}
		reinforcements = new List<ReinforcementPosition>();
		for (int i = 0; i < map.reinforcements.Count; i++) {
			reinforcements.Add(map.reinforcements[i]);
		}
	}

	/// <summary>
	/// Checks the forced character list to see if the given character appears.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public bool IsForced(CharData data) {
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
	public bool IsLocked(CharData data) {
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
public class PlayerPosition {
	public int x;
	public int y;
	public int level = 1;
	public CharData charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
}

[System.Serializable]
public class WeaponTuple {
	public ItemEntry item;
	public bool droppable;
}

[System.Serializable]
public class EnemyPosition {
	public int spawnTurn;
	public int x;
	public int y;
	public int level;
	public CharData charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	public AggroType aggroType;
	public bool hasQuotes;
	public List<FightQuote> quotes = new List<FightQuote>();
	public int huntX, huntY;

	public void Copy(EnemyPosition other) {
		spawnTurn = other.spawnTurn;
		x = other.x;
		y = other.y;
		level = other.level;
		charData = other.charData;
		for (int i = 0; i < other.inventory.Count; i++) {
			inventory.Add(new WeaponTuple(){
				item = other.inventory[i].item, droppable = other.inventory[i].droppable
			});
		}
		aggroType = other.aggroType;
		hasQuotes = other.hasQuotes;
		for (int i = 0; i < other.quotes.Count; i++) {
			quotes.Add(new FightQuote(){
				triggerer = other.quotes[i].triggerer, quote = other.quotes[i].quote, activated = other.quotes[i].activated
			});
		}
		huntX = other.huntX;
		huntY = other.huntY;
	}
}

[System.Serializable]
public class ReinforcementPosition {
	public int spawnTurn;
	public Faction faction;
	public int x;
	public int y;
	public int level;
	public CharData charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	// Enemy only
	public AggroType aggroType;
	public bool hasQuotes;
	public List<FightQuote> quotes = new List<FightQuote>();
	public int huntX, huntY;

	public void Copy(ReinforcementPosition other) {
		spawnTurn = other.spawnTurn;
		faction = other.faction;
		x = other.x;
		y = other.y;
		level = other.level;
		charData = other.charData;
		for (int i = 0; i < other.inventory.Count; i++) {
			inventory.Add(new WeaponTuple(){
				item = other.inventory[i].item, droppable = other.inventory[i].droppable
			});
		}
		aggroType = other.aggroType;
		hasQuotes = other.hasQuotes;
		for (int i = 0; i < other.quotes.Count; i++) {
			quotes.Add(new FightQuote(){
				triggerer = other.quotes[i].triggerer, quote = other.quotes[i].quote, activated = other.quotes[i].activated
			});
		}
		huntX = other.huntX;
		huntY = other.huntY;
	}
}

public enum InteractType { NONE, BLOCK, DIALOGUE, VILLAGE, SEIZE, CHEST }

[System.Serializable]
public class InteractPosition {
	public int x;
	public int y;
	public InteractType interactType;
	public int health;
	public DialogueEntry dialogue;
	public ItemEntry gift;
	public PlayerPosition ally = new PlayerPosition();
}

public enum TurnEventType { NONE, DIALOGUE, MAPCHANGE }
[System.Serializable]
public class TurnEvent {
	public int turn;
	public Faction factionTurn;
	public TurnEventType type;
	public DialogueEntry dialogue;
	public int x;
	public int y;
	public TerrainTile changeTerrain;

	public override string ToString() {
		return "Type: " + type + ", Turn: " + turn + ", faction: " + factionTurn;
	}
}

[System.Serializable]
public class FightQuote {
	public CharData triggerer;
	public DialogueEntry quote;
	public bool activated;
}

[System.Serializable]
public class Reward {
	public int money;
	public int scrap;
	public List<ItemEntry> items = new List<ItemEntry>();
}