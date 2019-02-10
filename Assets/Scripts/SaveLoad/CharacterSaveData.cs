using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData {

	public string id;
	public string classID;
	public int level;
	public int currentLevel;
	public int currentExp;

	[Header("Inventory and skills")]
	public int[] wpnSkills;
	public List<string> inventory;
	public List<int> invCharges;
	public List<string> skills;

	[Header("EV values")]
	public int eHp;
	public int eAtk;
	public int eSpd;
	public int eSkl;
	public int eLck;
	public int eDef;
	public int eRes;

	[Header("Supports")]
	public List<SupportValue> supports;
	

	public CharacterSaveData() {
		id = "";
		level = -1;
		currentLevel = -1;
	}

	public void StoreData(StatsContainer stats, InventoryContainer invCont, SkillsContainer skillCont) {
		id = stats.charData.uuid;
		classID = stats.classData.uuid;
		
		level = stats.level;
		currentLevel = stats.currentLevel;
		currentExp = stats.currentExp;

		wpnSkills = new int[StatsContainer.WPN_SKILLS];
		for (int i = 0; i < stats.wpnSkills.Length; i++) {
			wpnSkills[i] = stats.wpnSkills[i];
		}
		
		inventory = new List<string>();
		invCharges = new List<int>();
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			if (invCont.GetTuple(i).item == null)
				continue;
			inventory.Add(invCont.GetTuple(i).item.uuid);
			invCharges.Add(invCont.GetTuple(i).charge);
		}

		skills = new List<string>();
		for (int i = 0; i < skillCont.skills.Length; i++) {
			if (!skillCont.skills[i])
				continue;
			skills.Add(skillCont.skills[i].uuid);
		}

		stats.GenerateStartingStats();

		eHp = stats.eHp;
		eAtk = stats.eAtk;
		eSpd = stats.eSpd;
		eSkl = stats.eSkl;
		eLck = stats.eLck;
		eDef = stats.eDef;
		eRes = stats.eRes;

		supports = new List<SupportValue>();
		for (int i = 0; i < stats.supportValues.Count; i++) {
			supports.Add(stats.supportValues[i]);
		}
	}

}