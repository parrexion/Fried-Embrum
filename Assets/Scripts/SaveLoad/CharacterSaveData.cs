﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData {

	public string id;
	public int level;
	public int currentExp;

	[Header("Class")]
	public int[] classLevels;
	public string currentClass;
	public int[] wpnSkills;
	public List<string> skills;

	[Header("Inventory")]
	public List<string> inventory;
	public List<int> invCharges;

	[Header("Current stats")]
	public int eHp;
	public int eDmg;
	public int eMnd;
	public int eSpd;
	public int eSkl;
	public int eDef;

	[Header("Supports")]
	public int roomNo;
	public List<SupportValue> supports;
	

	public CharacterSaveData() {
		id = "";
		level = -1;
	}

	public void StoreData(StatsContainer stats, InventoryContainer invCon, SkillsContainer skillCont) {
		id = stats.charData.uuid;
		currentClass = stats.currentClass.uuid;
		classLevels = new int[ClassWheel.CLASS_COUNT];
		for (int i = 0; i < ClassWheel.CLASS_COUNT; i++) {
			classLevels[i] = stats.classLevels[i];
		}
		
		level = stats.level;
		currentExp = stats.currentExp;

		wpnSkills = new int[InventoryContainer.WPN_SKILLS];
		for (int i = 0; i < invCon.wpnSkills.Length; i++) {
			wpnSkills[i] = (int)invCon.wpnSkills[i];
		}
		
		inventory = new List<string>();
		invCharges = new List<int>();
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			if (invCon.GetTuple(i).item == null)
				continue;
			inventory.Add(invCon.GetTuple(i).item.uuid);
			invCharges.Add(invCon.GetTuple(i).charge);
		}

		skills = new List<string>();
		for (int i = 0; i < skillCont.skills.Length; i++) {
			if (!skillCont.skills[i])
				continue;
			skills.Add(skillCont.skills[i].uuid);
		}

		stats.GenerateStartingStats();

		eHp = stats.eHp;
		eDmg = stats.eDmg;
		eMnd = stats.eMnd;
		eSpd = stats.eSpd;
		eSkl = stats.eSkl;
		eDef = stats.eDef;

		//roomNo = stats.roomNo;
		//supports = new List<SupportValue>();
		//for (int i = 0; i < stats.supportValues.Count; i++) {
		//	supports.Add(stats.supportValues[i]);
		//}
	}

}