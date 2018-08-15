using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Exp")]
public class ExpBoost : CharacterSkill {

    protected override void UseSkill(TacticsMove user, TacticsMove enemy) { }
    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }

    protected override int EditValue(int value, TacticsMove user) {
        if (user.GetEquippedWeapon(ItemCategory.WEAPON) != null && user.GetEquippedWeapon(ItemCategory.WEAPON).itemType == activationItemType)
            value = Mathf.FloorToInt(value * multiplier);
        return value;
    }
}
