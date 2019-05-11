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
	public int currentExp;
	public int[] wpnSkills = new int[WPN_SKILLS];

	[Header("Current Stats")]
	public int hp;
	public int dmg;
	public int mnd;
	public int spd;
	public int skl;
	public int def;

	[Header("EV values")]
	public int eHp;
	public int eDmg;
	public int eMnd;
	public int eSpd;
	public int eSkl;
	public int eDef;

	[Header("Boost values")]
	public int bHp;
	public int bDmg;
	public int bMnd;
	public int bSpd;
	public int bSkl;
	public int bDef;

	[SerializeField]
	public List<Boost> boosts = new List<Boost>();

	[Header("Supports")]
	public int roomNo = -1;
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
		if (level == -1)
			return;
		currentExp = saveData.currentExp;

		wpnSkills = new int[WPN_SKILLS];
		for (int i = 0; i < saveData.wpnSkills.Length; i++) {
			wpnSkills[i] = saveData.wpnSkills[i];
		}
	
		eHp = saveData.eHp;
		eDmg = saveData.eDmg;
		eMnd = saveData.eMnd;
		eSpd = saveData.eSpd;
		eSkl = saveData.eSkl;
		eDef = saveData.eDef;

		roomNo = saveData.roomNo;
		supportValues = new List<SupportValue>();
		// for (int i = 0; i < charData.supports.Count; i++) {
		// 	supportValues.Add(new SupportValue(){uuid = charData.supports[i].partner.uuid});
		// }
		for (int i = 0; i < saveData.supports.Count; i++) {
			supportValues.Add(saveData.supports[i]);
			//Debug.Log("Added support value " + supportValues[i].uuid + " = " + supportValues[i].value);
		}
		
		CalculateStats();
	}

	public void GenerateStartingStats() {
		eHp = charData.hp + classData.hp;
		eDmg = charData.dmg + classData.dmg;
		eMnd = charData.mnd + classData.mnd;
		eSpd = charData.spd + classData.spd;
		eSkl = charData.skl + classData.skl;
		eDef = charData.def + classData.def;

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
		bDmg = 0;
		bMnd = 0;
		bSpd = 0;
		bSkl = 0;
		bDef = 0;

		for (int i = 0; i < boosts.Count; i++) {
			bHp += boosts[i].hp;
			bDmg += boosts[i].atk;
			bMnd += boosts[i].res;
			bSpd += boosts[i].spd;
			bSkl += boosts[i].skl;
			bDef += boosts[i].def;
		}
	}

	public void CalculateStats() {
		if (charData == null)
			return;
		GenerateBoosts();
		hp = bHp + eHp;
		dmg = bDmg + eDmg;
		mnd = bMnd + eMnd;
		spd = bSpd + eSpd;
		skl = bSkl + eSkl;
		def = bDef + eDef;
	}

	public void GainLevel() {
		level++;
		eHp += (int)(0.01f * (classData.gHp+charData.gHp + Random.Range(0,100)));
		eDmg += (int)(0.01f * (classData.gDmg+charData.gDmg + Random.Range(0,100)));
		eMnd += (int)(0.01f * (classData.gMnd+charData.gMnd + Random.Range(0,100)));
		eSpd += (int)(0.01f * (classData.gSpd+charData.gSpd + Random.Range(0,100)));
		eSkl += (int)(0.01f * (classData.gSkl+charData.gSkl + Random.Range(0,100)));
		eDef += (int)(0.01f * (classData.gDef+charData.gDef + Random.Range(0,100)));

		CalculateStats();
	}

	public void ChangeClass(CharClass newClass) {
		eHp += newClass.hp - classData.hp;
		eDmg += newClass.dmg - classData.dmg;
		eMnd += newClass.mnd - classData.mnd;
		eSpd += newClass.spd - classData.spd;
		eSkl += newClass.skl - classData.skl;
		eDef += newClass.def - classData.def;
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

	public int GetConPenalty(ItemEntry item) {
		return (item) ? Mathf.Max(item.weight - GetConstitution(),0) : 0;
	}

	public void GiveWpnExp(ItemEntry usedItem) {
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
	public int GetWpnSkill(ItemEntry weapon) {
		if (weapon == null)
			return 0;
		int skill = Mathf.Clamp((int)weapon.itemType, 0, wpnSkills.Length-1);
		return wpnSkills[skill] * classData.GetWeaponSkill(skill);
	}

	public void BoostBaseStats(Boost boost) {
		eHp += boost.hp;
		eDmg += boost.atk;
		eMnd += boost.res;
		eSpd += boost.spd;
		eSkl += boost.skl;
		eDef += boost.def;
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

	public SupportValue GetSupportValue(CharData other) {
		for (int i = 0; i < supportValues.Count; i++) {
			if (supportValues[i].uuid == other.uuid)
				return supportValues[i];
		}
		return new SupportValue(){ uuid = other.uuid };
	}
}
