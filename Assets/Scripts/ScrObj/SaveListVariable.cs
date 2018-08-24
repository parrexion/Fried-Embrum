using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/SaveList")]
public class SaveListVariable : ScriptableObject {

	public StatsContainer[] stats = new StatsContainer[0];
	public InventoryContainer[] inventory = new InventoryContainer[0];
	public SkillsContainer[] skills = new SkillsContainer[0];

}
