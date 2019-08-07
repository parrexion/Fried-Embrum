using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType { NONE, INFANTRY, ARMORED, CAVALRY, FLYING }

[CreateAssetMenu(menuName = "LibraryEntries/CharClass")]
public class CharClass : ScrObjLibraryEntry {
	
	public PlayerClassName className = PlayerClassName.NONE;
	public Sprite icon;

	[Header("Movement")]
	public MovementType classType;
	public int movespeed = 2;

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
	public bool lockTouch;
	public List<WeaponType> weaponSkills = new List<WeaponType>();
	public List<CharacterSkill> skills = new List<CharacterSkill>();
	public int bonusHp;
	public int bonusDmg;
	public int bonusMnd;
	public int bonusSkl;
	public int bonusSpd;
	public int bonusDef;
	

	public override void ResetValues() {
		base.ResetValues();
		className = PlayerClassName.NONE;
		icon = null;

		classType = MovementType.NONE;
		movespeed = 5;

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

		lockTouch = false;
		weaponSkills = new List<WeaponType>();
		skills = new List<CharacterSkill>();

		bonusHp = 0;
		bonusDmg = 0;
		bonusMnd = 0;
		bonusSkl = 0;
		bonusSpd = 0;
		bonusDef = 0;
	}
	
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharClass cc = (CharClass)other;

		className = cc.className;
		icon = cc.icon;
		classType = cc.classType;
		movespeed = cc.movespeed;

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

		lockTouch = cc.lockTouch;
		weaponSkills = new List<WeaponType>();
		for (int i = 0; i < cc.weaponSkills.Count; i++) {
			weaponSkills.Add(cc.weaponSkills[i]);
		}
		skills = new List<CharacterSkill>();
		for (int i = 0; i < cc.skills.Count; i++) {
			skills.Add(cc.skills[i]);
		}
		bonusHp = cc.bonusHp;
		bonusDmg = cc.bonusDmg;
		bonusMnd = cc.bonusMnd;
		bonusSkl = cc.bonusSkl;
		bonusSpd = cc.bonusSpd;
		bonusDef = cc.bonusDef;
	}

	/// <summary>
	/// Creates an array with the basic weapon skills this class can use.
	/// </summary>
	/// <returns></returns>
	public WeaponRank[] GetWeaponSkill(int classLevel) {
		WeaponRank[] res = new WeaponRank[InventoryContainer.WPN_SKILLS];
		for (int i = 0; i < weaponSkills.Count; i++) {
			res[(int)weaponSkills[i]] = (WeaponRank)Mathf.Min((int)WeaponRank.S, classLevel);
		}
		return res;
	}
}
