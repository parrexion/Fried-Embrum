using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/CombatBoost")]
public class CombatBoost : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetWeapon(ItemCategory.WEAPON) != null && enemy.GetWeapon(ItemCategory.WEAPON).itemType == activationItemType)
            user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) {
        if (enemy.GetWeapon(ItemCategory.WEAPON) != null && enemy.GetWeapon(ItemCategory.WEAPON).itemType == activationItemType)
            user.ReceiveBuff(boost.InvertStats(), true, false);
    }
}
