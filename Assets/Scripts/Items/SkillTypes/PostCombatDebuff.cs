using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/PostCombatDebuff")]
public class PostCombatDebuff : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
        enemy.ReceiveBuff(boost, false, true);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }
}
