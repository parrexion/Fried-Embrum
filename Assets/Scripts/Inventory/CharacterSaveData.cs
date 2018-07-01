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
	

	public CharacterSaveData() {
		id = "";
		level = -1;
	}

	public void StoreData(StatsContainer cont) {
		id = cont.charData.id;
		classID = cont.classData.id;
		
		level = cont.level;
		currentExp = cont.currentExp;

		wpnSkills = new int[StatsContainer.WPN_SKILLS];
		for (int i = 0; i < cont.wpnSkills.Length; i++) {
			wpnSkills[i] = cont.wpnSkills[i];
		}
		inventory = new List<string>();
		invCharges = new List<int>();
		skills = new List<string>();
		for (int i = 0; i < cont.inventory.Length; i++) {
			if (cont.inventory[i] == null)
				continue;
			inventory.Add(cont.inventory[i].item.id);
			invCharges.Add(cont.inventory[i].charge);
		}
		for (int i = 0; i < cont.skills.Length; i++) {
			if (!cont.skills[i])
				continue;
			skills.Add(cont.skills[i].id);
		}

		iHp = cont.iHp;
		iAtk = cont.iAtk;
		iSpd = cont.iSpd;
		iSkl = cont.iSkl;
		iLck = cont.iLck;
		iDef = cont.iDef;
		iRes = cont.iRes;
		
		eHp = cont.eHp;
		eAtk = cont.eAtk;
		eSpd = cont.eSpd;
		eSkl = cont.eSkl;
		eLck = cont.eLck;
		eDef = cont.eDef;
		eRes = cont.eRes;
	}

	public void GenerateIV() {
		iHp = Random.Range(0.01f,0.99f);
		iAtk = Random.Range(0.01f,0.99f);
		iSpd = Random.Range(0.01f,0.99f);
		iSkl = Random.Range(0.01f,0.99f);
		iLck = Random.Range(0.01f,0.99f);
		iDef = Random.Range(0.01f,0.99f);
		iRes = Random.Range(0.01f,0.99f);
	}
}