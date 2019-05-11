using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/CharData")]
public class CharData : ScrObjLibraryEntry {

	[Header("Character Info")]
	public Sprite bigPortrait;
	public Sprite portrait;
	public Sprite battleSprite;
	public PortraitEntry portraitSet;
	public CharClass charClass;

	[Header("Skills")]
	public CharacterSkill personalSkill;

	[Header("Personal Base Stats")]
	public int hp;
	public int dmg;
	public int skl;
	public int spd;
	public int def;
	public int mnd;
	public int con;

	[Header("Personal Growths")]
	public int gHp;
	public int gDmg;
	public int gMnd;
	public int gSkl;
	public int gSpd;
	public int gDef;

	[Header("Supports")]
	public List<SupportTuple> supports = new List<SupportTuple>();

	[Header("Other Data")]
	public bool mustSurvive;
	public DialogueEntry deathQuote;

	
	public override void ResetValues() {
		base.ResetValues();

		bigPortrait = null;
		portrait = null;
		battleSprite = null;
		portraitSet = null;
		charClass = null;

		personalSkill = null;

		hp = 0;
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

		supports = new List<SupportTuple>();

		mustSurvive = false;
		deathQuote = null;
	}
	
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharData cd = (CharData)other;

		bigPortrait = cd.bigPortrait;
		portrait = cd.portrait;
		battleSprite = cd.battleSprite;
		portraitSet = cd.portraitSet;
		charClass = cd.charClass;

		personalSkill = cd.personalSkill;

		hp = cd.hp;
		dmg = cd.dmg;
		mnd = cd.mnd;
		skl = cd.skl;
		spd = cd.spd;
		def = cd.def;

		gHp = cd.gHp;
		gDmg = cd.gDmg;
		gMnd = cd.gMnd;
		gSkl = cd.gSkl;
		gSpd = cd.gSpd;
		gDef = cd.gDef;

		supports = new List<SupportTuple>();
		for (int i = 0; i < cd.supports.Count; i++) {
			supports.Add(cd.supports[i]);
		}

		mustSurvive = cd.mustSurvive;
		deathQuote = cd.deathQuote;
	}

	public SupportTuple GetSupport(CharData partner) {
		for (int i = 0; i < supports.Count; i++) {
			if (supports[i].partner.uuid == partner.uuid)
				return supports[i];
		}
		return null;
	}

	public SupportTuple GetSupport(string uuid) {
		for (int i = 0; i < supports.Count; i++) {
			if (supports[i].partner.uuid == uuid)
				return supports[i];
		}
		return null;
	}
}
