using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class StatsContainer {

	public const int WPN_SKILLS = 8;
	public const int SKILL_SIZE = 5;
	
	[Header("Character Info")]
	public CharacterStats charData;
	public CharClass classData;
	
	[Header("Player stuff")]
	public int level;
	public int currentExp;
	public int[] wpnSkills = new int[WPN_SKILLS];
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

		wpnSkills = new int[WPN_SKILLS];
		for (int i = 0; i < saveData.wpnSkills.Length; i++) {
			wpnSkills[i] = saveData.wpnSkills[i];
		}
		skills = new CharacterSkill[SKILL_SIZE];
		for (int i = 0; i < saveData.skills.Count; i++) {
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

	/// <summary>
	/// Generates new IV values. Overwrites the previous ones.
	/// </summary>
	public void GenerateIV() {
		iHp = Random.Range(0f,1f);
		iAtk = Random.Range(0f,1f);
		iSkl = Random.Range(0f,1f);
		iSpd = Random.Range(0f,1f);
		iLck = Random.Range(0f,1f);
		iDef = Random.Range(0f,1f);
		iRes = Random.Range(0f,1f);
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

	/// <summary>
	/// The character's current move speed.
	/// </summary>
	/// <returns></returns>
	public int GetMovespeed() {
		return classData.movespeed;
	}

	public int GetConstitution() {
		return charData.con + classData.con;
	}

	public int GetConPenalty(WeaponItem item) {
		return (item) ? Mathf.Max(item.weight - GetConstitution(),0) : 0;
	}

	public void GiveWpnExp(WeaponItem usedItem) {
		if (usedItem.itemCategory == ItemCategory.WEAPON) {
            wpnSkills[(int)usedItem.itemType] += 3;
		}
		else if (usedItem.itemCategory == ItemCategory.STAFF) {
            wpnSkills[(int)ItemType.HEAL] += 3;
		}
	}

	/// <summary>
	/// Returns the current weapon skill level for the weapon.
	/// </summary>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public int GetWpnSkill(WeaponItem weapon) {
		if (weapon == null)
			return 0;
		int skill = Mathf.Clamp((int)weapon.itemType, 0, wpnSkills.Length-1);
		return wpnSkills[skill] * classData.GetWeaponSkill(skill);
	}

	public void BoostBaseStats(Boost boost) {
		iHp += boost.hp;
		iAtk += boost.atk;
		iSpd += boost.spd;
		iSkl += boost.skl;
		iLck += boost.lck;
		iDef += boost.def;
		iRes += boost.res;
		CalculateStats();
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

	public bool GainSkill(CharacterSkill skill) {
		
		Debug.Log("Adding skill " + skill.itemName);
		for (int i = 0; i < skills.Length; i++) {
			if (skills[i] == null) {
				skills[i] = skill;
				Debug.Log("Added the skill to position " + i);
				return true;
			}
		}
		return false;
	}

	//Skill activations

	public void ActivateSkills(Activation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].ActivateSkill(activation, user, enemy);
		}
	}

	public void EndSkills(Activation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].EndSkill(activation, user, enemy);
		}
	}

	public int EditValueSkills(Activation activation, TacticsMove user, int value) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].EditValue(activation, value, user);
		}
		return value;
	}

	public void ForEachSkills(Activation activation, TacticsMove user, CharacterListVariable list) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].ActivateForEach(activation, user, list);
		}
	}

}


[System.Serializable]
public class InventoryTuple {
	public int index;
	public WeaponItem item;
	public int charge;
	public bool droppable;
}