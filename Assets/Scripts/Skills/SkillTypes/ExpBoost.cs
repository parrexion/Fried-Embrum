using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Exp")]
public class ExpBoost : CharacterSkill {

    protected override void UseSkill(TacticsMove user, TacticsMove enemy) { }
    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }

    protected override int EditValue(int value, TacticsMove user) {
        if (user.GetEquippedWeapon(ItemCategory.WEAPON) != null && user.GetEquippedWeapon(ItemCategory.WEAPON).item.weaponType == weaponReq)
            value = Mathf.FloorToInt(value * percent);
        return value;
    }
}
