using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WinCondition { ROUT, SEIZE, BOSS, ESCAPE, DEFEND }
public enum LoseCondition { NORMAL, SEIZE, PROTECT }
public enum MapLocation { UNKNOWN, RYERDE, KHATHELET, THRIA, BADON, TASCANA }

[CreateAssetMenu(menuName = "LibraryEntries/MapEntry")]
public class MapEntry : ScrObjLibraryEntry {

	[Header("Map")]
	public int sizeX;
	public int sizeY;
	public Texture2D mapSprite;
	public MapLocation mapLocation;

	[Header("Objectives")]
	public WinCondition winCondition;
	public LoseCondition loseCondition;
	public Reward reward = new Reward();

	[Header("Dialogues")]
	public bool skipBattlePrep;
	public DialogueEntry preDialogue;
	public DialogueEntry postDialogue;

	[Header("Music")]
	public MusicEntry owMusic;
	public MusicEntry battleMusic;
	public MusicEntry healMusic;

	[Header("Players")]
	public List<PlayerPosition> spawnPoints = new List<PlayerPosition>();
	
	[Header("Enemies")]
	public List<EnemyPosition> enemies = new List<EnemyPosition>();
	public List<EnemyPosition> reinforcements = new List<EnemyPosition>();
	
	[Header("Interactions")]
	public List<InteractPosition> interactions = new List<InteractPosition>();
	
	[Header("Turn Events")]
	public List<TurnEvent> turnEvents = new List<TurnEvent>();


	public override void ResetValues() {
		sizeX = 0;
		sizeY = 0;
		mapSprite = null;
		mapLocation = MapLocation.UNKNOWN;

		winCondition = WinCondition.ROUT;
		loseCondition = LoseCondition.NORMAL;
		reward = new Reward();

		base.ResetValues();
		preDialogue = null;
		postDialogue = null;

		owMusic = null;
		battleMusic = null;
		healMusic = null;

		spawnPoints = new List<PlayerPosition>();
		enemies = new List<EnemyPosition>();
		interactions = new List<InteractPosition>();
		turnEvents = new List<TurnEvent>();
		reinforcements = new List<EnemyPosition>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		MapEntry map = (MapEntry)other;

		sizeX = map.sizeX;
		sizeY = map.sizeY;
		mapSprite = map.mapSprite;
		mapLocation = map.mapLocation;

		winCondition = map.winCondition;
		loseCondition = map.loseCondition;
		reward = map.reward;

		preDialogue = map.preDialogue;
		postDialogue = map.postDialogue;

		owMusic = map.owMusic;
		battleMusic = map.battleMusic;
		healMusic = map.healMusic;

		spawnPoints = new List<PlayerPosition>();
		for (int i = 0; i < map.spawnPoints.Count; i++) {
			spawnPoints.Add(map.spawnPoints[i]);
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
		reinforcements = new List<EnemyPosition>();
		for (int i = 0; i < map.reinforcements.Count; i++) {
			reinforcements.Add(map.reinforcements[i]);
		}
	}
}

[System.Serializable]
public class PlayerPosition {
	public int x;
	public int y;
	public int level;
	public CharData stats;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	public CharacterSkill[] skills;
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
	public CharData stats;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	public CharacterSkill[] skills;
	public AggroType aggroType;
	public bool hasQuotes;
	public List<FightQuote> quotes = new List<FightQuote>();
	public int huntX, huntY;
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