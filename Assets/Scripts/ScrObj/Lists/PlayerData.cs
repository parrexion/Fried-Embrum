using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Save List")]
public class PlayerData : ScriptableObject {

	// Characters
	public List<StatsContainer> stats = new List<StatsContainer>();
	public List<InventoryContainer> inventory = new List<InventoryContainer>();
	public List<SkillsContainer> skills = new List<SkillsContainer>();
	public List<SupportContainer> baseInfo = new List<SupportContainer>();

	// Item storage
	public List<InventoryItem> items = new List<InventoryItem>();

	// Science
	public UpgradeCalculator upgrader = new UpgradeCalculator();

	// Missions
	public List<MissionProgress> missions = new List<MissionProgress>();


	public void ResetData() {
		stats = new List<StatsContainer>();
		inventory = new List<InventoryContainer>();
		skills = new List<SkillsContainer>();
		baseInfo = new List<SupportContainer>();
		
		items = new List<InventoryItem>();

		upgrader = new UpgradeCalculator();

		missions = new List<MissionProgress>();
	}

	public void UpdateUpgrades() {
		for (int i = 0; i < inventory.Count; i++) {
			inventory[i].RefreshUpgrades(upgrader);
		}
	}

	public void AddNewPlayer(TacticsMove player) {
		stats.Add(player.stats);
		inventory.Add(player.inventory);
		skills.Add(player.skills);
		baseInfo.Add(new SupportContainer(null));
	}

	public MissionProgress GetMissionProgress(string uuid) {
		for (int i = 0; i < missions.Count; i++) {
			if (missions[i].uuid == uuid) {
				return missions[i];
			}
		}
		MissionProgress progress = new MissionProgress(uuid);
		missions.Add(progress);
		return progress;
	}
} 
