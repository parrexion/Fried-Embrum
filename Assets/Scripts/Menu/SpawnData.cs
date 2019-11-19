using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SpawnData {
	public TriggerType triggerType;
	public int spawnTurn;
	public int triggerIndex;
	public Faction faction;
	//Character info
	public int x;
	public int y;
	public int level;
	public CharEntry charData;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	public StatsContainer stats;
	public InventoryContainer inventoryContainer;
	public SkillsContainer skills;
	//Player only
	public int joiningSquad;
	// Enemy only
	public AggroType aggroType;
	public bool hasQuotes;
	public List<FightQuote> quotes = new List<FightQuote>();
	public List<FightQuote> talks = new List<FightQuote>();
	public int huntX, huntY;
	public List<Position> patrolPositions = new List<Position>();


	public SpawnData() { }

	public SpawnData(TacticsMove tactics) {
		x = tactics.posx;
		y = tactics.posy;
		charData = tactics.stats.charData;
		level = tactics.stats.level;
		stats = tactics.stats;
		inventoryContainer = tactics.inventory;
		skills = tactics.skills;
	}

	public void Copy(SpawnData other) {
		triggerType = other.triggerType;
		spawnTurn = other.spawnTurn;
		triggerIndex = other.triggerIndex;
		faction = other.faction;
		x = other.x;
		y = other.y;
		level = other.level;
		charData = other.charData;
		for (int i = 0; i < other.inventory.Count; i++) {
			inventory.Add(new WeaponTuple() {
				item = other.inventory[i].item,
				droppable = other.inventory[i].droppable
			});
		}
		joiningSquad = other.joiningSquad;
		aggroType = other.aggroType;
		hasQuotes = other.hasQuotes;
		quotes.Clear();
		for (int i = 0; i < other.quotes.Count; i++) {
			quotes.Add(new FightQuote() {
				triggerer = other.quotes[i].triggerer,
				quote = other.quotes[i].quote,
				activated = other.quotes[i].activated
			});
		}
		talks.Clear();
		for (int i = 0; i < other.talks.Count; i++) {
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
