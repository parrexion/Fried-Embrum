using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassType { NONE, INFANTRY, ARMORED, CAVALRY, FLYING }

[CreateAssetMenu]
public class CharClass : ScriptableObject {

	public string id;
	public int movespeed = 2;
	public ClassType classType;

	[Header("Bases")]
	public int hp;
	public int atk;
	public int spd;
	public int skl;
	public int lck;
	public int def;
	public int res;
	public int con;

	[Header("Growths")]
	public float gHp;
	public float gAtk;
	public float gSpd;
	public float gSkl;
	public float gLck;
	public float gDef;
	public float gRes;

	[Header("Skills")]
	public ItemType[] weaponSkills;
	public SkillTuple[] skillGains;
	

	public int[] GenerateBaseWpnSkill() {
		int[] res = new int[StatsContainer.WPN_SKILLS];
		for (int i = 0; i < weaponSkills.Length; i++) {
			res[(int)weaponSkills[i]] = 1;
		}
		return res;
	}

	public CharacterSkill AwardSkills(int level) {
		for (int i = 0; i < skillGains.Length; i++) {
			if (skillGains[i].level == level) {
				return skillGains[i].skill;
			}
		}
		return null;
	}
}

[System.Serializable]
public class SkillTuple {
	public int level;
	public CharacterSkill skill;
}
