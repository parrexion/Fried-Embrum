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

	// Graveyard
	public List<string> graveyard = new List<string>();


	public void ResetData() {
		stats = new List<StatsContainer>();
		inventory = new List<InventoryContainer>();
		skills = new List<SkillsContainer>();
		baseInfo = new List<SupportContainer>();
		
		items = new List<InventoryItem>();

		upgrader = new UpgradeCalculator();

		missions = new List<MissionProgress>();

		graveyard = new List<string>();
	}

	/// <summary>
	/// Refreshes the inventory items with the current set of upgrades.
	/// </summary>
	public void UpdateUpgrades() {
		for (int i = 0; i < inventory.Count; i++) {
			inventory[i].RefreshUpgrades(upgrader);
		}
	}

	/// <summary>
	/// Adds a new player with the information in the TacticsMove and default values for the rest.
	/// </summary>
	/// <param name="player"></param>
	public void AddNewPlayer(TacticsMove player) {
		stats.Add(player.stats);
		inventory.Add(player.inventory);
		skills.Add(player.skills);
		baseInfo.Add(new SupportContainer(null));
	}

	/// <summary>
	/// Returns true if the character is recruited and alive.
	/// </summary>
	/// <param name="uuid"></param>
	/// <returns></returns>
	public bool HasCharacter(string uuid) {
		for (int i = 0; i < stats.Count; i++) {
			if (stats[i].charData.uuid == uuid)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Returns true if the character has died.
	/// </summary>
	/// <param name="uuid"></param>
	/// <returns></returns>
	public bool IsDead(string uuid) {
		return graveyard.Contains(uuid);
	}

	/// <summary>
	/// Returns the progress state of the given mission.
	/// </summary>
	/// <param name="uuid"></param>
	/// <returns></returns>
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
