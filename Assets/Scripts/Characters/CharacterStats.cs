using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStats : ScriptableObject {

	public int statsID;

	[Header("Character Info")]
	public string charName;
	public Sprite portrait;
	public Sprite battleSprite;
	public CharClass charClass;

	[Header("Skills")]
	public WeaponSkill[] weapons;
	public SupportSkill[] supports;
	public SkillSkill[] skills;
	public PassiveSkill[] skillsA;
	public PassiveSkill[] skillsB;
	public PassiveSkill[] skillsC;

	[Header("Base Stats")]
	public int hp = 10;
	public int atk = 6;
	public int spd = 6;
	public int def = 3;
	public int res = 3;

	[Header("Growths")]
	public float gHp = 0.5f;
	public float gAtk = 0.35f;
	public float gSpd = 0.35f;
	public float gDef = 0.35f;
	public float gRes = 0.35f;

}
