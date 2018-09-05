using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Activation { PRECOMBAT, INITCOMBAT, STARTTURN, PASSIVE, POSTCOMBAT, EXP } 

public abstract class CharacterSkill : ScrObjLibraryEntry {

    public Sprite icon;
    public string description;

    public Activation activation;
    public int range;

    [Space(10)]
    
    public float multiplier;
    public ItemType activationItemType;
    public Boost boost;
    

    public void ActivateSkill(Activation act, TacticsMove user, TacticsMove enemy) {
        if (act == activation)
            UseSkill(user, enemy);
    }

    public void EndSkill(Activation act, TacticsMove user, TacticsMove enemy) {
        if (act == activation)
            RemoveEffect(user, enemy);
    }

    public int EditValue(Activation act, int value, TacticsMove user) {
        return (act == activation) ? EditValue(value, user) : value;
    }

    public void ActivateForEach(Activation act, TacticsMove user, CharacterListVariable list) {
        if (act == activation) {
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
