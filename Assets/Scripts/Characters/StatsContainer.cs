using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class StatsContainer {

	[SerializeField]
	private CharacterStats _stats;
	public List<Boost> boosts = new List<Boost>();

	public int id;
	public int statsID;

	[Header("Character Info")]
	public string charName;
	public Sprite portrait;
	public Sprite battleSprite;
	public CharClass charClass;
	
	[Header("Player stuff")]
	public int level;
	public int currentExp;
	public int currentSp;
	public int weaponLevel;
	public int supportLevel;
	public int skillLevel;
	public int skillALevel = -1;
	public int skillBLevel = -1;
	public int skillCLevel = -1;

	[Header("Current Stats")]
	public int hp;
	public int atk;
	public int spd;
	public int def;
	public int res;

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

	[Header("Boost values")]
	public int bHp;
	public int bAtk;
	public int bSpd;
	public int bDef;
	public int bRes;


	public StatsContainer(CharacterSave save, CharacterStats stats) {
		if (save == null) {
			id = -1;
			statsID = -1;
			level = -1;
		}
		else
			SetupValues(save, stats);
	}

	private void SetupValues(CharacterSave save, CharacterStats stats) {
		id = save.id;
		statsID = save.statsID;
		_stats = stats;
		level = save.level;
		if (level == -1)
			return;
		currentExp = save.currentExp;
		currentSp = save.currentSp;
		weaponLevel = save.weaponLevel;
		supportLevel = save.supportLevel;
		skillLevel = save.skillLevel;
		skillALevel = save.skillALevel;
		skillBLevel = save.skillBLevel;
		skillCLevel = save.skillCLevel;
		
		iHp = save.iHp;
		iAtk = save.iAtk;
		iSpd = save.iSpd;
		iDef = save.iDef;
		iRes = save.iRes;
	
		eHp = save.eHp;
		eAtk = save.eAtk;
		eSpd = save.eSpd;
		eDef = save.eDef;
		eRes = save.eRes;
		
		charName = stats.charName;
		portrait = stats.portrait;
		battleSprite = stats.battleSprite;
		charClass = stats.charClass;
		
		CalculateStats();
	}

	public void ReadCharValues() {
		charName = _stats.charName;
		portrait = _stats.portrait;
		battleSprite = _stats.battleSprite;
		charClass = _stats.charClass;
	}

	private void GenerateBoosts() {
		bHp = 0;
		bAtk = 0;
		bSpd = 0;
		bDef = 0;
		bRes = 0;

		for (int i = 0; i < boosts.Count; i++) {
			bHp += boosts[i].hp;
			bAtk += boosts[i].atk;
			bSpd += boosts[i].spd;
			bDef += boosts[i].def;
			bRes += boosts[i].res;
		}
	}

	public void CalculateStats() {
		if (_stats == null)
			return;
		GenerateBoosts();
		int calcLevel = level - 1;
		hp = (int)(_stats.hp + iHp + calcLevel * _stats.gHp + bHp + eHp);
		atk = (int)(_stats.atk + iAtk + calcLevel * _stats.gAtk + bAtk + eAtk);
		spd = (int)(_stats.spd + iSpd + calcLevel * _stats.gSpd + bSpd + eSpd);
		def = (int)(_stats.def + iDef + calcLevel * _stats.gDef + bDef + eDef);
		res = (int)(_stats.res + iRes + calcLevel * _stats.gRes + bRes + eRes);
	}

	public int GetMove() {
		return _stats.charClass.movespeed;
	}

	public WeaponSkill GetWeapon() {
		return (weaponLevel != -1 && weaponLevel < _stats.weapons.Length) ? _stats.weapons[weaponLevel] : null;
	}

	public SupportSkill GetSupport() {
		return (supportLevel != -1 && supportLevel < _stats.supports.Length) ? _stats.supports[supportLevel] : null;
	}

	public SkillSkill GetSkill() {
		return (skillLevel != -1 && skillLevel < _stats.skills.Length) ? _stats.skills[skillLevel] : null;
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
		if (skillALevel >= 0) _stats.skillsA[skillALevel].ActivateSkill(activation, user, enemy);
		if (skillBLevel >= 0) _stats.skillsB[skillBLevel].ActivateSkill(activation, user, enemy);
		if (skillCLevel >= 0) _stats.skillsC[skillCLevel].ActivateSkill(activation, user, enemy);
	}

	public void EndSkills(Activation activation, TacticsMove user, TacticsMove enemy) {
		if (skillALevel >= 0) _stats.skillsA[skillALevel].EndSkill(activation, user, enemy);
		if (skillBLevel >= 0) _stats.skillsB[skillBLevel].EndSkill(activation, user, enemy);
		if (skillCLevel >= 0) _stats.skillsC[skillCLevel].EndSkill(activation, user, enemy);
	}

	public int EditValueSkills(Activation activation, TacticsMove user, int value) {
		if (skillALevel >= 0) value = _stats.skillsA[skillALevel].EditValue(activation, value, user);
		if (skillBLevel >= 0) value = _stats.skillsB[skillBLevel].EditValue(activation, value, user);
		if (skillCLevel >= 0) value = _stats.skillsC[skillCLevel].EditValue(activation, value, user);
		return value;
	}

	public void ForEachSkills(Activation activation, TacticsMove user, CharacterListVariable list) {
		if (skillALevel >= 0) _stats.skillsA[skillALevel].ActivateForEach(activation, user, list);
		if (skillBLevel >= 0) _stats.skillsB[skillBLevel].ActivateForEach(activation, user, list);
		if (skillCLevel >= 0) _stats.skillsC[skillCLevel].ActivateForEach(activation, user, list);
	}

	public PassiveSkill GetPassive(SkillSlot slot) {
		switch (slot)
		{
			case SkillSlot.SLOTA:
				return (skillALevel >= 0) ? _stats.skillsA[skillALevel] : null;
			case SkillSlot.SLOTB:
				return (skillBLevel >= 0) ? _stats.skillsB[skillBLevel] : null;
			case SkillSlot.SLOTC:
				return (skillCLevel >= 0) ? _stats.skillsC[skillCLevel] : null;
				
			default:
				return null;
		}
	}

	public bool IsWeakAgainst(WeaponSkill weapon) {
		if (weapon == null)
			return false;
		return (weapon.advantageType == charClass.classType);
	}
}
