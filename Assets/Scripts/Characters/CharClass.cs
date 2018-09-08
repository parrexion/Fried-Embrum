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
	public int atk;
	public int skl;
	public int spd;
	public int lck;
	public int def;
	public int res;

	[Header("Growths")]
	public int gHp;
	public int gAtk;
	public int gSkl;
	public int gSpd;
	public int gLck;
	public int gDef;
	public int gRes;

	[Header("Skills")]
	public List<ItemType> weaponSkills = new List<ItemType>();
	public List<int> weaponLevels = new List<int>();
	public List<SkillTuple> skillGains = new List<SkillTuple>();
	

	public override void ResetValues() {
		base.ResetValues();
		classType = ClassType.NONE;
		movespeed = 5;
		con = 0;

		hp = 1;
		atk = 0;
		skl = 0;
		spd = 0;
		lck = 0;
		def = 0;
		res = 0;

		gHp = 0;
		gAtk = 0;
		gSkl = 0;
		gSpd = 0;
		gLck = 0;
		gDef = 0;
		gRes = 0;

		weaponSkills = new List<ItemType>();
		weaponLevels = new List<int>();
		skillGains = new List<SkillTuple>();
	}
	
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharClass cc = (CharClass)other;

		classType = cc.classType;
		movespeed = cc.movespeed;
		con = cc.con;

		hp = cc.hp;
		atk = cc.atk;
		skl = cc.skl;
		spd = cc.spd;
		lck = cc.lck;
		def = cc.def;
		res = cc.res;

		gHp = cc.gHp;
		gAtk = cc.gAtk;
		gSkl = cc.gSkl;
		gSpd = cc.gSpd;
		gLck = cc.gLck;
		gDef = cc.gDef;
		gRes = cc.gRes;

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
