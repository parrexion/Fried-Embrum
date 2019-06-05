using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/CombatBoost")]
public class CombatBoost : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetEquippedWeapon(ItemCategory.WEAPON) != null && enemy.GetEquippedWeapon(ItemCategory.WEAPON).item.weaponType == weaponReq)
            user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetEquippedWeapon(ItemCategory.WEAPON) != null && enemy.GetEquippedWeapon(ItemCategory.WEAPON).item.weaponType == weaponReq)
            user.ReceiveBuff(boost.InvertStats(), true, false);
    }
}
