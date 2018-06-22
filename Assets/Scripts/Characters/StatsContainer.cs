using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class StatsContainer {

	[Header("Character Info")]
	public CharacterStats charData;
	public CharClass classData;
	
	[Header("Player stuff")]
	public int level;
	public int currentExp;
	public WeaponItem[] inventory;
	public CharacterSkill[] skills;

	[Header("Current Stats")]
	public int hp;
	public int atk;
	public int spd;
	public int skl;
	public int lck;
	public int def;
	public int res;

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

	[Header("Boost values")]
	public int bHp;
	public int bAtk;
	public int bSpd;
	public int bSkl;
	public int bLck;
	public int bDef;
	public int bRes;

	[SerializeField]
	public List<Boost> boosts = new List<Boost>();


	public StatsContainer(ItemLibrary iLib, CharacterSaveData saveData, CharacterStats cStats, CharClass charClass) {
		SetupValues(iLib, saveData, cStats, charClass);
	}

	private void SetupValues(ItemLibrary iLib, CharacterSaveData saveData, CharacterStats cStats, CharClass charClass) {
		charData = cStats;
		classData = charClass;
		
		level = saveData.level;
		if (level == -1)
			return;
		currentExp = saveData.currentExp;

		inventory = new WeaponItem[saveData.inventory.Length];
		for (int i = 0; i < saveData.inventory.Length; i++) {
			inventory[i] = (WeaponItem) iLib.GetEntry(saveData.inventory[i]);
		}
		skills = new CharacterSkill[saveData.skills.Length];
		for (int i = 0; i < saveData.skills.Length; i++) {
			skills[i] = (CharacterSkill) iLib.GetEntry(saveData.skills[i]);
		}
		
		iHp = saveData.iHp;
		iAtk = saveData.iAtk;
		iSpd = saveData.iSpd;
		iSkl = saveData.iSkl;
		iLck = saveData.iLck;
		iDef = saveData.iDef;
		iRes = saveData.iRes;
	
		eHp = saveData.eHp;
		eAtk = saveData.eAtk;
		eSpd = saveData.eSpd;
		eSkl = saveData.eSkl;
		eLck = saveData.eLck;
		eDef = saveData.eDef;
		eRes = saveData.eRes;
		
		CalculateStats();
	}

	private void GenerateBoosts() {
		bHp = 0;
		bAtk = 0;
		bSpd = 0;
		bSkl = 0;
		bLck = 0;
		bDef = 0;
		bRes = 0;

		for (int i = 0; i < boosts.Count; i++) {
			bHp += boosts[i].hp;
			bAtk += boosts[i].atk;
			bSpd += boosts[i].spd;
			bSkl += boosts[i].skl;
			bLck += boosts[i].lck;
			bDef += boosts[i].def;
			bRes += boosts[i].res;
		}
	}

	public void CalculateStats() {
		if (charData == null)
			return;
		GenerateBoosts();
		int calcLevel = level - 1;
		hp = charData.hp + classData.hp + bHp + (int)(calcLevel * (classData.gHp+charData.gHp) + iHp + eHp);
		atk = charData.atk + classData.atk + bAtk + (int)(calcLevel * (classData.gAtk+charData.gAtk) + iAtk + eAtk);
		spd = charData.spd + classData.spd + bSpd + (int)(calcLevel * (classData.gSpd+charData.gSpd) + iSpd + eSpd);
		skl = charData.skl + classData.skl + bSkl + (int)(calcLevel * (classData.gSkl+charData.gSkl) + iSkl + eSkl);
		lck = charData.lck + classData.lck + bLck + (int)(calcLevel * (classData.gLck+charData.gLck) + iLck + eLck);
		def = charData.def + classData.def + bDef + (int)(calcLevel * (classData.gDef+charData.gDef) + iDef + eDef);
		res = charData.res + classData.res + bRes + (int)(calcLevel * (classData.gRes+charData.gRes) + iRes + eRes);
	}

	public WeaponItem GetItem(ItemCategory category) {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i] == null)
				continue;
			if (inventory[i].itemCategory == category)
				return inventory[i];
		}
		return null;
	}

	public WeaponItem GetItem(int index) {
		return (index >= inventory.Length) ? null : inventory[index];
	}

	public void EquipItem(int index) {
		WeaponItem equip = inventory[index];
		for (int i = 0; i < index+1; i++) {
			WeaponItem temp = inventory[i];
			inventory[i] = equip;
			equip = temp;
		}
	}

	public void UseItem(int index) {
		
	}

	public void ClearBoosts(bool isStartTurn) {
		for (int i = 0; i < boosts.Count; i++) {
			if (isStartTurn)
				boosts[i].StartTurn();
			else
				boosts[i].EndTurn();
		}

		boosts.RemoveAll((b => !b.IsActive()));
		
		CalculateStats();
	}

	public void ActivateSkills(Activation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			skills[i].ActivateSkill(activation, user, enemy);
		}
	}

	public void EndSkills(Activation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			skills[i].EndSkill(activation, user, enemy);
		}
	}

	public int EditValueSkills(Activation activation, TacticsMove user, int value) {
		for (int i = 0; i < skills.Length; i++) {
			skills[i].EditValue(activation, value, user);
		}
		return value;
	}

	public void ForEachSkills(Activation activation, TacticsMove user, CharacterListVariable list) {
		for (int i = 0; i < skills.Length; i++) {
			skills[i].ActivateForEach(activation, user, list);
		}
	}

	public bool IsWeakAgainst(WeaponItem weapon) {
		if (weapon == null)
			return false;
		
		for (int i = 0; i < weapon.advantageType.Length; i++) {
			if (weapon.advantageType[i] == classData.classType)
				return true;
		}

		return false;
	}
}
