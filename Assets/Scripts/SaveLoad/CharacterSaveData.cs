using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData {

	public string id;
	public string classID;
	public int level;
	public int currentExp;

	[Header("Inventory and skills")]
	public int[] wpnSkills;
	public List<string> inventory;
	public List<int> invCharges;
	public List<string> skills;

	[Header("IV values")]
	public float iHp;
	public float iAtk;
	public float iSpd;
	public float iSkl;
	public float iLck;
	public float iDef;
	public float iRes;

	[Header("EV values")]
	public float eHp;
	public float eAtk;
	public float eSpd;
	public float eSkl;
	public float eLck;
	public float eDef;
	public float eRes;

	[Header("Supports")]
	public List<SupportValue> supports;
	

	public CharacterSaveData() {
		id = "";
		level = -1;
	}

	public void StoreData(StatsContainer stats, InventoryContainer invCont, SkillsContainer skillCont) {
		id = stats.charData.uuid;
		classID = stats.classData.uuid;
		
		level = stats.level;
		currentExp = stats.currentExp;

		wpnSkills = new int[StatsContainer.WPN_SKILLS];
		for (int i = 0; i < stats.wpnSkills.Length; i++) {
			wpnSkills[i] = stats.wpnSkills[i];
		}
		
		inventory = new List<string>();
		invCharges = new List<int>();
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			if (invCont.GetItem(i).item == null)
				continue;
			inventory.Add(invCont.GetItem(i).item.uuid);
			invCharges.Add(invCont.GetItem(i).charge);
		}

		skills = new List<string>();
		for (int i = 0; i < skillCont.skills.Length; i++) {
			if (!skillCont.skills[i])
				continue;
			skills.Add(skillCont.skills[i].uuid);
		}

		iHp = stats.iHp;
		iAtk = stats.iAtk;
		iSpd = stats.iSpd;
		iSkl = stats.iSkl;
		iLck = stats.iLck;
		iDef = stats.iDef;
		iRes = stats.iRes;
		
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