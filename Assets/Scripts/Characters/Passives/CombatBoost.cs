using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/CombatBoost")]
public class CombatBoost : PassiveSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetWeapon() != null && enemy.GetWeapon().weaponType == weaponType)
            user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetWeapon() != null && enemy.GetWeapon().weaponType == weaponType)
            user.ReceiveBuff(boost.InvertStats(), true, false);
    }
}
