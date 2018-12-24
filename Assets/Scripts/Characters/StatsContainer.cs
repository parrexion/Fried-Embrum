using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class StatsContainer {

	public const int WPN_SKILLS = 8;
	
	[Header("Character Info")]
	public CharData charData;
	public CharClass classData;
	
	[Header("Player stuff")]
	public int level;
	public int currentLevel;
	public int currentExp;
	public int[] wpnSkills = new int[WPN_SKILLS];

	[Header("Current Stats")]
	public int hp;
	public int atk;
	public int spd;
	public int skl;
	public int lck;
	public int def;
	public int res;

	[Header("EV values")]
	public int eHp;
	public int eAtk;
	public int eSpd;
	public int eSkl;
	public int eLck;
	public int eDef;
	public int eRes;

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

	[Header("Supports")]
	public List<SupportValue> supportValues = new List<SupportValue>();


	public StatsContainer(CharacterSaveData saveData, CharData cStats, CharClass charClass) {
		SetupValues(saveData, cStats, charClass);
	}

	public StatsContainer(CharData cStats, CharClass cClass, int level) {
		this.level = level;
		charData = cStats;
		classData = cClass;
		GenerateStartingStats();
		wpnSkills = classData.GenerateBaseWpnSkill();
	}

	private void SetupValues(CharacterSaveData saveData, CharData cStats, CharClass charClass) {
		charData = cStats;
		classData = charClass;
		
		if (saveData == null) {
			return;
		}
// Fixa level och current level överallt
		level = saveData.level;
		currentLevel = saveData.currentLevel;
		if (level == -1)
			return;
		currentExp = saveData.currentExp;

		wpnSkills = new int[WPN_SKILLS];
		for (int i = 0; i < saveData.wpnSkills.Length; i++) {
			wpnSkills[i] = saveData.wpnSkills[i];
		}
	
		eHp = saveData.eHp;
		eAtk = saveData.eAtk;
		eSpd = saveData.eSpd;
		eSkl = saveData.eSkl;
		eLck = saveData.eLck;
		eDef = saveData.eDef;
		eRes = saveData.eRes;

		supportValues = new List<SupportValue>();
		// for (int i = 0; i < charData.supports.Count; i++) {
		// 	supportValues.Add(new SupportValue(){uuid = charData.supports[i].partner.uuid});
		// }
		for (int i = 0; i < saveData.supports.Count; i++) {
			supportValues.Add(saveData.supports[i]);
			Debug.Log("Added support value " + supportValues[i].uuid + " = " + supportValues[i].value);
		}
		
		CalculateStats();
	}

	public void GenerateStartingStats() {
		eHp = charData.hp + classData.hp;
		eAtk = charData.atk + classData.atk;
		eSpd = charData.spd + classData.spd;
		eSkl = charData.skl + classData.skl;
		eLck = charData.lck + classData.lck;
		eDef = charData.def + classData.def;
		eRes = charData.res + classData.res;

		for (int i = 1; i < level; i++) {
			GainLevel();
			level--;
		}
	}

	/// <summary>
	/// Sums up the stat boosts the character has active at the moment.
	/// </summary>
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
		hp = bHp + eHp;
		atk = bAtk + eAtk;
		spd = bSpd + eSpd;
		skl = bSkl + eSkl;
		lck = bLck + eLck;
		def = bDef + eDef;
		res = bRes + eRes;
	}

	public void GainLevel() {
		level++;
		currentLevel++;
		eHp += (int)(0.01f * (classData.gHp+charData.gHp + Random.Range(0,100)));
		eAtk += (int)(0.01f * (classData.gAtk+charData.gAtk + Random.Range(0,100)));
		eSpd += (int)(0.01f * (classData.gSpd+charData.gSpd + Random.Range(0,100)));
		eSkl += (int)(0.01f * (classData.gSkl+charData.gSkl + Random.Range(0,100)));
		eLck += (int)(0.01f * (classData.gLck+charData.gLck + Random.Range(0,100)));
		eDef += (int)(0.01f * (classData.gDef+charData.gDef + Random.Range(0,100)));
		eRes += (int)(0.01f * (classData.gRes+charData.gRes + Random.Range(0,100)));

		CalculateStats();
	}

	public void ChangeClass(CharClass newClass) {
		while(currentLevel <= 20) {
			level++;
			currentLevel++;
		}
		currentLevel = 1;
		eHp += newClass.hp - classData.hp;
		eAtk += newClass.atk - classData.atk;
		eSpd += newClass.spd - classData.spd;
		eSkl += newClass.skl - classData.skl;
		eLck += newClass.lck - classData.lck;
		eDef += newClass.def - classData.def;
		eRes += newClass.res - classData.res;
		classData = newClass;
		CalculateStats();
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
		eHp += boost.hp;
		eAtk += boost.atk;
		eSpd += boost.spd;
		eSkl += boost.skl;
		eLck += boost.lck;
		eDef += boost.def;
		eRes += boost.res;
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

	public int GetSupportValue(string id) {
		for (int i = 0; i < supportValues.Count; i++) {
			if (supportValues[i].uuid == id)
				return supportValues[i].value;
		}
		return 0;
	}
}
