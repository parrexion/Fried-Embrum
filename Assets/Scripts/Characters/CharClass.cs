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
	public int def;
	public int res;

	[Header("Growths")]
	public float gHp;
	public float gAtk;
	public float gSpd;
	public float gDef;
	public float gRes;

	[Header("Skill Gains")]
	public SkillTuple[] skillGains;
}

[System.Serializable]
public class SkillTuple {
	public int level;
	public CharacterSkill skill;
}
