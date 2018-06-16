using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Permanent")]
public class PermanentBoost : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
        user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }
}
