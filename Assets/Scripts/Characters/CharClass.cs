using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassType { NONE, INFANTRY, ARMORED, CAVALRY, FLYING }

[CreateAssetMenu(menuName = "LibraryEntries/CharClass")]
public class CharClass : ScrObjLibraryEntry {

	public ClassType classType;
	public int movespeed = 2;
	public int con;

	[Header("Bases")]
	public int hp;
	public int dmg;
	public int mnd;
	public int skl;
	public int spd;
	public int def;

	[Header("Growths")]
	public int gHp;
	public int gDmg;
	public int gMnd;
	public int gSkl;
	public int gSpd;
	public int gDef;

	[Header("Skills")]
	public List<ItemType> weaponSkills = new List<ItemType>();
	public List<int> weaponLevels = new List<int>();
	public List<SkillTuple> skillGains = new List<SkillTuple>();

	[Header("Upgrade class")]
	public List<CharClass> nextClass = new List<CharClass>();
	

	public override void ResetValues() {
		base.ResetValues();
		classType = ClassType.NONE;
		movespeed = 5;
		con = 0;

		hp = 1;
		dmg = 0;
		mnd = 0;
		skl = 0;
		spd = 0;
		def = 0;

		gHp = 0;
		gDmg = 0;
		gMnd = 0;
		gSkl = 0;
		gSpd = 0;
		gDef = 0;

		weaponSkills = new List<ItemType>();
		weaponLevels = new List<int>();
		skillGains = new List<SkillTuple>();

		nextClass = new List<CharClass>();
	}
	
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharClass cc = (CharClass)other;

		classType = cc.classType;
		movespeed = cc.movespeed;
		con = cc.con;

		hp = cc.hp;
		dmg = cc.dmg;
		mnd = cc.mnd;
		skl = cc.skl;
		spd = cc.spd;
		def = cc.def;

		gHp = cc.gHp;
		gDmg = cc.gDmg;
		gMnd = cc.gMnd;
		gSkl = cc.gSkl;
		gSpd = cc.gSpd;
		gDef = cc.gDef;

		weaponSkills = new List<ItemType>();
		weaponLevels = new List<int>();
		for (int i = 0; i < cc.weaponSkills.Count; i++) {
			weaponSkills.Add(cc.weaponSkills[i]);
			weaponLevels.Add(cc.weaponLevels[i]);
		}
		skillGains = new List<SkillTuple>();
		for (int i = 0; i < cc.skillGains.Count; i++) {
			skillGains.Add(cc.skillGains[i]);
		}
		
		nextClass = new List<CharClass>();
		for (int i = 0; i < cc.nextClass.Count; i++) {
			nextClass.Add(cc.nextClass[i]);
		}
	}

	/// <summary>
	/// Creates an array with the basic weapon skills this class can use.
	/// </summary>
	/// <returns></returns>
	public int[] GenerateBaseWpnSkill() {
		int[] res = new int[StatsContainer.WPN_SKILLS];
		for (int i = 0; i < weaponSkills.Count; i++) {
			res[(int)weaponSkills[i]] = weaponLevels[i];
		}
		return res;
	}

	public CharacterSkill AwardSkills(int level) {
		for (int i = 0; i < skillGains.Count; i++) {
			if (skillGains[i].level == level) {
				return skillGains[i].skill;
			}
		}
		return null;
	}

	public int GetWeaponSkill(int skill) {
		for (int i = 0; i < weaponSkills.Count; i++) {
			if ((int)weaponSkills[i] == skill)
				return 1;
		}
		return 0;
	}
}

[System.Serializable]
public class SkillTuple {
	public int level;
	public CharacterSkill skill;
}
