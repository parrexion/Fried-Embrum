using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/CharData")]
public class CharData : ScrObjLibraryEntry {

	[Header("Character Info")]
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
	public int gHp;
	public int gAtk;
	public int gSkl;
	public int gSpd;
	public int gLck;
	public int gDef;
	public int gRes;

	[Header("Other Data")]
	public bool mustSurvive;
	public DialogueEntry deathQuote;

	
	public override void ResetValues() {
		base.ResetValues();

		bigPortrait = null;
		portrait = null;
		battleSprite = null;
		charClass = null;

		personalSkill = null;

		hp = 0;
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

		mustSurvive = false;
		deathQuote = null;
	}
	
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharData cd = (CharData)other;

		bigPortrait = cd.bigPortrait;
		portrait = cd.portrait;
		battleSprite = cd.battleSprite;
		charClass = cd.charClass;

		personalSkill = cd.personalSkill;

		hp = cd.hp;
		atk = cd.atk;
		skl = cd.skl;
		spd = cd.spd;
		lck = cd.lck;
		def = cd.def;
		res = cd.res;

		gHp = cd.gHp;
		gAtk = cd.gAtk;
		gSkl = cd.gSkl;
		gSpd = cd.gSpd;
		gLck = cd.gLck;
		gDef = cd.gDef;
		gRes = cd.gRes;

		mustSurvive = cd.mustSurvive;
		deathQuote = cd.deathQuote;
	}
}
