using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillActivation { NONE = -1, PRECOMBAT, INITCOMBAT, STARTTURN, PASSIVE, POSTCOMBAT, EXP } 

public abstract class CharacterSkill : ScrObjLibraryEntry {

    public Sprite icon;
    public string description;
    public SkillActivation activationType;

    [Space(10)]
    
	public bool includeSelf;
    public int range;
    public int rangeMax;
    public float percent;
    public WeaponType weaponReq;
    public Boost boost;


	public override void ResetValues() {
		base.ResetValues();
		icon = null;
		description = "";
		activationType = SkillActivation.NONE;

		includeSelf = false;
		range = 0;
		rangeMax = 0;
		percent = 0;
		weaponReq = WeaponType.NONE;
		boost = new Boost();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharacterSkill cs = (CharacterSkill)other;

		icon = cs.icon;
		description = cs.description;
		activationType = cs.activationType;

		includeSelf = cs.includeSelf;
		range = cs.range;
		rangeMax = cs.rangeMax;
		percent = cs.percent;
		weaponReq = cs.weaponReq;
		boost = new Boost();
		boost.AddBoost(cs.boost);
	}

	public void ActivateSkill(SkillActivation act, TacticsMove user, TacticsMove enemy) {
        if (act == activationType)
            UseSkill(user, enemy);
    }

    public void EndSkill(SkillActivation act, TacticsMove user, TacticsMove enemy) {
        if (act == activationType)
            RemoveEffect(user, enemy);
    }

    public int EditValue(SkillActivation act, int value, TacticsMove user) {
        return (act == activationType) ? EditValue(value, user) : value;
    }

    public void ActivateForEach(SkillActivation act, TacticsMove user, CharacterListVariable list) {
        if (act == activationType) {
            ForEachBoost(list, user);
        }
    }
    
    protected abstract void UseSkill(TacticsMove user, TacticsMove enemy);
    protected abstract void RemoveEffect(TacticsMove user, TacticsMove enemy);

    protected virtual int EditValue(int value, TacticsMove user) {
        return value;
    }

    protected virtual void ForEachBoost(CharacterListVariable list, TacticsMove user) { }
}
