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
	public List<MissionContainer> missions = new List<MissionContainer>();


	public void ResetData() {
		stats = new List<StatsContainer>();
		inventory = new List<InventoryContainer>();
		skills = new List<SkillsContainer>();
		baseInfo = new List<SupportContainer>();
		
		items = new List<InventoryItem>();

		upgrader = new UpgradeCalculator();

		missions = new List<MissionContainer>();
	}
} 
