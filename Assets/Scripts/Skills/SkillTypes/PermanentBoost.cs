using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Permanent")]
public class PermanentBoost : CharacterSkill {
    
    public override void UseSkill(TacticsMove user, TacticsMove enemy) {
		boost.boostType = BoostType.PASSIVE;
        user.ReceiveBuff(boost, true, false);
    }
	
}
