using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/CharacterStats")]
public class CharacterStatsVariable : ScriptableObject {

	public StatsContainer stats;
	public InventoryContainer inventory;
	public SkillsContainer skills;
}
