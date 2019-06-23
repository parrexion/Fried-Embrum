﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Initiate")]
public class InitiateBoost : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
		Debug.Log(boost.ToString(), this);
        user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) {
        user.ReceiveBuff(boost.InvertStats(), true, false);
    }
}
