using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class StatsContainer {

	[Header("Character Info")]
	public CharData charData;
	public CharClass currentClass;
	public int[] classLevels = new int[0];

	[Header("Player stuff")]
	public int level;
	public int currentExp;

	[Header("Current Stats")]
	public int hp;
	public int dmg;
	public int mnd;
	public int spd;
	public int skl;
	public int def;
	public int hitBoost;
	public int critBoost;
	public int avoidBoost;

	//Base stats
	public int eHp;
	public int eDmg;
	public int eMnd;
	public int eSpd;
	public int eSkl;
	public int eDef;

	[Header("Boost")]
	public int fatigueAmount;
	public List<Boost> boosts = new List<Boost>();
	public Boost supportBoost = new Boost();
	public Boost currentBoost;


	public StatsContainer(CharacterSaveData saveData, CharData cStats, CharClass charClass) {
		SetupValues(saveData, cStats, charClass);
	}

	public StatsContainer(PlayerPosition pos) {
		level = pos.level;
		charData = pos.charData;
		currentClass = pos.charData.startClass;
		classLevels = pos.charData.startClassLevels;
		GenerateStartingStats();
	}

	public StatsContainer(EnemyPosition pos) {
		level = pos.level;
		charData = pos.charData;
		currentClass = pos.charData.startClass;
		classLevels = pos.charData.startClassLevels;
		GenerateStartingStats();
	}

	public StatsContainer(ReinforcementPosition pos) {
		level = pos.level;
		charData = pos.charData;
		currentClass = pos.charData.startClass;
		classLevels = pos.charData.startClassLevels;
		GenerateStartingStats();
	}

	private void SetupValues(CharacterSaveData saveData, CharData cStacDatas, CharClass cClass) {
		charData = cStacDatas;
		currentClass = cClass;

		if (saveData == null) {
			return;
		}

		classLevels = new int[ClassWheel.CLASS_COUNT];
		for (int i = 0; i < ClassWheel.CLASS_COUNT; i++) {
			classLevels[i] = saveData.classLevels[i];
		}
		level = saveData.level;
		if (level == -1)
			return;
		currentExp = saveData.currentExp;

		eHp = saveData.eHp;
		eDmg = saveData.eDmg;
		eMnd = saveData.eMnd;
		eSpd = saveData.eSpd;
		eSkl = saveData.eSkl;
		eDef = saveData.eDef;

		//TODO calculate support boost.
		supportBoost = new Boost();

		CalculateStats();
	}

	public void GenerateStartingStats() {
		eHp = charData.hp + currentClass.hp;
		eDmg = charData.dmg + currentClass.dmg;
		eMnd = charData.mnd + currentClass.mnd;
		eSpd = charData.spd + currentClass.spd;
		eSkl = charData.skl + currentClass.skl;
		eDef = charData.def + currentClass.def;

		for (int i = 1; i < level; i++) {
			GainLevel();
			level--;
		}
	}

	/// <summary>
	/// Sums up the stat boosts the character has active at the moment.
	/// </summary>
	private void GenerateBoosts() {
		currentBoost = new Boost();

		Boost mergeBoost = new Boost();
		mergeBoost.boostType = BoostType.DECREASE;
		for (int i = 0; i < boosts.Count; i++) {
			if (boosts[i].boostType == BoostType.DECREASE) {
				mergeBoost.MergeBoost(boosts[i]);
				boosts.RemoveAt(i);
				i--;
				mergeBoost.ActivateBoost();
			}
		}
		if (mergeBoost.IsActive())
			boosts.Add(mergeBoost);

		for (int i = 0; i < boosts.Count; i++) {
			currentBoost.AddBoost(boosts[i]);
		}
	}

	public void CalculateStats() {
		if (charData == null)
			return;
		GenerateBoosts();
		hp = currentBoost.hp + eHp;
		dmg = currentBoost.dmg + eDmg;
		mnd = currentBoost.mnd + eMnd;
		spd = currentBoost.spd + eSpd;
		skl = currentBoost.skl + eSkl;
		def = currentBoost.def + eDef;
		hitBoost = currentBoost.hit;
		critBoost = currentBoost.crit;
		avoidBoost = currentBoost.avoid;
	}

	public void GainLevel() {
		level++;
		int sum = eHp + eDmg + eMnd + eSpd + eSkl + eDef;
		while (sum == eHp + eDmg + eMnd + eSpd + eSkl + eDef) {
			eHp += (int)(0.01f * (currentClass.gHp + charData.gHp + Random.Range(0, 100)));
			eDmg += (int)(0.01f * (currentClass.gDmg + charData.gDmg + Random.Range(0, 100)));
			eMnd += (int)(0.01f * (currentClass.gMnd + charData.gMnd + Random.Range(0, 100)));
			eSpd += (int)(0.01f * (currentClass.gSpd + charData.gSpd + Random.Range(0, 100)));
			eSkl += (int)(0.01f * (currentClass.gSkl + charData.gSkl + Random.Range(0, 100)));
			eDef += (int)(0.01f * (currentClass.gDef + charData.gDef + Random.Range(0, 100)));
		}

		CalculateStats();
	}

	public void ClassGain(LevelGain gainedClass, int classPosition) {
		eHp += gainedClass.bonusHp;
		eDmg += gainedClass.bonusDmg;
		eMnd += gainedClass.bonusMnd;
		eSpd += gainedClass.bonusSpd;
		eSkl += gainedClass.bonusSkl;
		eDef += gainedClass.bonusDef;
		classLevels[classPosition]++;

		CalculateStats();
	}

	public void ChangeClass(CharClass newClass) {
		eHp += newClass.hp - currentClass.hp;
		eDmg += newClass.dmg - currentClass.dmg;
		eMnd += newClass.mnd - currentClass.mnd;
		eSpd += newClass.spd - currentClass.spd;
		eSkl += newClass.skl - currentClass.skl;
		eDef += newClass.def - currentClass.def;
		currentClass = newClass;

		CalculateStats();
	}

	public bool CanLevelup() {
		int classSum = 0;
		for (int i = 0; i < classLevels.Length; i++) {
			classSum += classLevels[i];
		}
		return (level / 10 + 2 > classSum);
		//return (level / 10 + 1 > classSum);
	}

	/// <summary>
	/// The character's current move speed.
	/// </summary>
	/// <returns></returns>
	public int GetMovespeed() {
		return currentClass.movespeed;
	}

	public void BoostBaseStats(Boost boost) {
		eHp += boost.hp;
		eDmg += boost.dmg;
		eMnd += boost.mnd;
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

}
