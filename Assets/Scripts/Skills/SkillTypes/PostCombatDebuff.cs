using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/PostCombatDebuff")]
public class PostCombatDebuff : CharacterSkill {
    
    public override void UseSkill(TacticsMove user, TacticsMove enemy) {
		boost.boostType = BoostType.DECREASE;
        enemy.ReceiveBuff(boost, false, true);
    }
	
}
