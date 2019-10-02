using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType { TURN, TRIGGER }
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
	public List<CharData> forcedCharacters = new List<CharData>();
	public List<CharData> lockedCharacters = new List<CharData>();
	
	[Header("Other characters")]
	public List<ReinforcementPosition> enemies = new List<ReinforcementPosition>();
	public List<ReinforcementPosition> allies = new List<ReinforcementPosition>();
	public List<ReinforcementPosition> reinforcements = new List<ReinforcementPosition>();
	
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
		forcedCharacters = new List<CharData>();
		lockedCharacters = new List<CharData>();

		enemies = new List<ReinforcementPosition>();
		allies = new List<ReinforcementPosition>();
		reinforcements = new List<ReinforcementPosition>();

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
		forcedCharacters = new List<CharData>();
		for (int i = 0; i < map.forcedCharacters.Count; i++) {
			forcedCharacters.Add(map.forcedCharacters[i]);
		}
		lockedCharacters = new List<CharData>();
		for (int i = 0; i < map.lockedCharacters.Count; i++) {
			lockedCharacters.Add(map.lockedCharacters[i]);
		}

		enemies = new List<ReinforcementPosition>();
		for (int i = 0; i < map.enemies.Count; i++) {
			enemies.Add(map.enemies[i]);
		}
		allies = new List<ReinforcementPosition>();
		for (int i = 0; i < map.allies.Count; i++) {
			allies.Add(map.allies[i]);
		}
		reinforcements = new List<ReinforcementPosition>();
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
	public List<FightQuote> talks = new List<FightQuote>();
	public int huntX, huntY;
	public List<Position> patrolPositions = new List<Position>();


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
		quotes.Clear();
		for (int i = 0; i < other.quotes.Count; i++) {
			quotes.Add(new FightQuote(){
				triggerer = other.quotes[i].triggerer, quote = other.quotes[i].quote, activated = other.quotes[i].activated
			});
		}
		talks.Clear();
		for (int i = 0; i < other.talks.Count; i++) {
			talks.Add(new FightQuote(){
				triggerer = other.talks[i].triggerer, quote = other.talks[i].quote, willJoin = other.talks[i].willJoin, activated = other.talks[i].activated
			});
		}
		huntX = other.huntX;
		huntY = other.huntY;
		patrolPositions = new List<Position>();
		for(int i = 0; i < other.patrolPositions.Count; i++) {
			patrolPositions.Add(new Position() { x = other.patrolPositions[i].x, y = other.patrolPositions[i].y });
		}
	}
}

[System.Serializable]
public class ReinforcementPosition {
	public TriggerType triggerType;
	public int spawnTurn;
	public string triggerIndex;
	public Faction faction;
	public int x;
	public int y;
	public int level;
	public CharData charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	//Player only
	public int joiningSquad;
	// Enemy only
	public AggroType aggroType;
	public bool hasQuotes;
	public List<FightQuote> quotes = new List<FightQuote>();
	public List<FightQuote> talks = new List<FightQuote>();
	public int huntX, huntY;
	public List<Position> patrolPositions = new List<Position>();


	public ReinforcementPosition(){}

	public void Copy(ReinforcementPosition other) {
		triggerType = other.triggerType;
		spawnTurn = other.spawnTurn;
		triggerIndex = other.triggerIndex;
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
		joiningSquad = other.joiningSquad;
		aggroType = other.aggroType;
		hasQuotes = other.hasQuotes;
		quotes.Clear();
		for (int i = 0; i < other.quotes.Count; i++) {
			quotes.Add(new FightQuote(){
				triggerer = other.quotes[i].triggerer, quote = other.quotes[i].quote, activated = other.quotes[i].activated
			});
		}
		talks.Clear();
		for(int i = 0; i < other.talks.Count; i++) {
			talks.Add(new FightQuote() {
				triggerer = other.talks[i].triggerer,
				quote = other.talks[i].quote,
				willJoin = other.talks[i].willJoin,
				activated = other.talks[i].activated
			});
		}
		huntX = other.huntX;
		huntY = other.huntY;
		patrolPositions = new List<Position>();
		for (int i = 0; i < other.patrolPositions.Count; i++) {
			patrolPositions.Add(new Position() { x = other.patrolPositions[i].x, y = other.patrolPositions[i].y });
		}
	}
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
	public PlayerPosition ally = new PlayerPosition();
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
	public CharData triggerer;
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