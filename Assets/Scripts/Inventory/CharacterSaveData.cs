using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData {

	public string id;
	public string classID;
	public int level;
	public int currentExp;

	[Header("Inventory and skills")]
	public string[] inventory;
	public string[] skills;
	
	[Header("IV values")]
	public float iHp;
	public float iAtk;
	public float iSpd;
	public float iDef;
	public float iRes;

	[Header("EV values")]
	public float eHp;
	public float eAtk;
	public float eSpd;
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

		inventory = new string[cont.inventory.Length];
		skills = new string[cont.skills.Length];
		for (int i = 0; i < cont.inventory.Length; i++) {
			inventory[i] = cont.inventory[i].id;
		}
		for (int i = 0; i < cont.skills.Length; i++) {
			skills[i] = cont.skills[i].id;
		}

		iHp = cont.iHp;
		iAtk = cont.iAtk;
		iSpd = cont.iSpd;
		iDef = cont.iDef;
		iRes = cont.iRes;
		
		eHp = cont.eHp;
		eAtk = cont.eAtk;
		eSpd = cont.eSpd;
		eDef = cont.eDef;
		eRes = cont.eRes;
	}

	public void GenerateIV() {
		iHp = Random.Range(0.01f,0.99f);
		iAtk = Random.Range(0.01f,0.99f);
		iSpd = Random.Range(0.01f,0.99f);
		iDef = Random.Range(0.01f,0.99f);
		iRes = Random.Range(0.01f,0.99f);
	}
}