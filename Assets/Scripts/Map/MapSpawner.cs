using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapSpawner : MonoBehaviour {

	public BattleMap battleMap;
	public ClassWheel playerClassWheel;
	public ClassWheel enemyClassWheel;

	[Header("Prefabs")]
	public Transform playerPrefab;
	public Transform enemyPrefab;

	[Header("Save data")]
	public PlayerData playerData;
	public PrepListVariable prepList1;
	public PrepListVariable prepList2;

	[Header("Cursor")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public UnityEvent cursorMoveEvent;



	/// <summary>
	/// Spawns a village character behind the scenes which can later be recruited by the player.
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public TacticsMove SpawnVillageCharacter(InteractPosition pos) {
		SpawnData reinforcement = new SpawnData() {
			x = pos.x,
			y = pos.y,
			level = pos.ally.level,
			charData = pos.ally.charData,
			inventory = pos.ally.inventory
		};
		return SpawnPlayerCharacter(reinforcement, false, false, false);
	}

	/// <summary>
	/// Spawns a player character on the map.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	public PlayerMove SpawnPlayerCharacter(SpawnData pos, bool addToPrep, bool active, bool cursorHover) {
		Transform playerTransform = Instantiate(playerPrefab, battleMap.playerParent);
		PlayerMove tactics = playerTransform.GetComponent<PlayerMove>();
		tactics.battleMap = battleMap;
		tactics.posx = pos.x;
		tactics.posy = pos.y;
		tactics.stats = pos.stats ?? new StatsContainer(pos);
		tactics.inventory = pos.inventoryContainer ?? new InventoryContainer(playerClassWheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
		tactics.skills = pos.skills ?? new SkillsContainer(playerClassWheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
		tactics.squad = pos.joiningSquad;

		if (addToPrep) {
			playerData.AddNewPlayer(tactics);
			PrepCharacter prep = new PrepCharacter(playerData.stats.Count - 1);
			if (pos.joiningSquad == 2) {
				prepList2.values.Add(prep);
			}
			else {
				prepList1.values.Add(prep);
			}
		}

		if (active)
			tactics.Setup();
		else
			playerTransform.gameObject.SetActive(false);

		if (cursorHover) {
			cursorX.value = pos.x;
			cursorY.value = pos.y;
			cursorMoveEvent.Invoke();
		}

		return tactics;
	}

	/// <summary>
	/// Spawns an enemy character on the map.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="stats"></param>
	/// <param name="inventory"></param>
	/// <param name="skills"></param>
	public void SpawnEnemyCharacter(SpawnData pos) {
		Transform enemyTransform = Instantiate(enemyPrefab, battleMap.enemyParent);
		NPCMove tactics = enemyTransform.GetComponent<NPCMove>();
		tactics.battleMap = battleMap;
		tactics.posx = pos.x;
		tactics.posy = pos.y;
		tactics.stats = new StatsContainer(pos);
		tactics.faction = Faction.ENEMY;
		tactics.inventory = new InventoryContainer(enemyClassWheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
		tactics.skills = new SkillsContainer(enemyClassWheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
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
	public void SpawnAllyCharacter(SpawnData pos) {
		Transform allyTransform = Instantiate(enemyPrefab, battleMap.enemyParent);
		NPCMove tactics = allyTransform.GetComponent<NPCMove>();
		tactics.battleMap = battleMap;
		tactics.posx = pos.x;
		tactics.posy = pos.y;
		tactics.faction = Faction.ALLY;
		tactics.stats = new StatsContainer(pos);
		tactics.inventory = new InventoryContainer(playerClassWheel.GetWpnSkillFromLevel(pos.charData.startClassLevels), pos.inventory);
		tactics.skills = new SkillsContainer(playerClassWheel.GetSkillsFromLevel(pos.charData.startClassLevels, pos.charData.startClass, pos.level));
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
