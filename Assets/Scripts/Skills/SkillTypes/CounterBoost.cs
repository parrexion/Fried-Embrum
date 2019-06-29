using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Counter")]
public class CounterBoost : CharacterSkill {
    
    public override void UseSkill(TacticsMove user, TacticsMove enemy) {
		boost.boostType = BoostType.SINGLE;
        user.ReceiveBuff(boost, true, false);
    }

    public override void EndSkill(TacticsMove user, TacticsMove enemy) {
        user.ReceiveBuff(boost.InvertStats(), true, false);
    }
}
