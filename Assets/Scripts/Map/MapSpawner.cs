using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour {

	public BattleMap battleMap;

	[Header("Prefabs")]
	public Transform playerPrefab;
	public Transform enemyPrefab;


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
		if(active)
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
	public void SpawnEnemyCharacter(ReinforcementPosition pos, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills) {
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
		for(int i = 0; i < pos.patrolPositions.Count; i++) {
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
	public void SpawnAllyCharacter(ReinforcementPosition pos, StatsContainer stats, InventoryContainer inventory, SkillsContainer skills) {
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
		for(int i = 0; i < pos.patrolPositions.Count; i++) {
			tactics.patrolTiles.Add(battleMap.GetTile(pos.patrolPositions[i]));
		}
		tactics.Setup();
	}
}
