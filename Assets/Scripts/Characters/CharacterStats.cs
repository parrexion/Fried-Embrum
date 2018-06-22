using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStats : ScriptableObject {

	public string id;

	[Header("Character Info")]
	public string charName;
	public Sprite portrait;
	public Sprite battleSprite;
	public CharClass charClass;

	[Header("Skills")]
	public CharacterSkill personalSkill;

	[Header("Base Stats")]
	public int hp = 10;
	public int atk = 6;
	public int spd = 6;
	public int skl = 5;
	public int lck = 4;
	public int def = 3;
	public int res = 3;

	[Header("Personal Growths")]
	public float gHp = 0.1f;
	public float gAtk = 0.1f;
	public float gSpd = 0.1f;
	public float gSkl = 0.1f;
	public float gLck = 0.1f;
	public float gDef = 0.1f;
	public float gRes = 0.1f;

}
