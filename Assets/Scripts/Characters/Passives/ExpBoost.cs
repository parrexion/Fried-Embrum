using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Exp")]
public class ExpBoost : PassiveSkill {

    protected override void UseSkill(TacticsMove user, TacticsMove enemy) { }
    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }

    protected override int EditValue(int value, TacticsMove user) {
        if (user.GetWeapon() != null && user.GetWeapon().weaponType == weaponType)
            value = Mathf.FloorToInt(value * multiplier);
        return value;
    }
}
