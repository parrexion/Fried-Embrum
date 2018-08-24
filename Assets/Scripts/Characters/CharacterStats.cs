using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStats : ScriptableObject {

	public string id;

	[Header("Character Info")]
	public string charName;
	public Sprite bigPortrait;
	public Sprite portrait;
	public Sprite battleSprite;
	public CharClass charClass;

	[Header("Skills")]
	public CharacterSkill personalSkill;

	[Header("Personal Base Stats")]
	public int hp;
	public int atk;
	public int skl;
	public int spd;
	public int lck;
	public int def;
	public int res;
	public int con;

	[Header("Personal Growths")]
	public float gHp;
	public float gAtk;
	public float gSkl;
	public float gSpd;
	public float gLck;
	public float gDef;
	public float gRes;

	[Header("Other Data")]
	public bool mustSurvive;
}
