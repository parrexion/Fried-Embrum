using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Save List")]
public class SaveListVariable : ScriptableObject {

	// Characters
	public List<StatsContainer> stats = new List<StatsContainer>();
	public List<InventoryContainer> inventory = new List<InventoryContainer>();
	public List<SkillsContainer> skills = new List<SkillsContainer>();

	// Item storage
	public List<InventoryItem> items = new List<InventoryItem>();

	// Science
	public UpgradeCalculator upgrader = new UpgradeCalculator();


	public void ResetData() {
		stats = new List<StatsContainer>();
		inventory = new List<InventoryContainer>();
		skills = new List<SkillsContainer>();
		
		items = new List<InventoryItem>();

		upgrader = new UpgradeCalculator();
	}
} 
