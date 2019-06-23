﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Exp")]
public class ExpBoost : CharacterSkill {

    protected override void UseSkill(TacticsMove user, TacticsMove enemy) { }
    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }

    protected override int EditValue(int value, TacticsMove user) {
		Debug.Log(percent.ToString(), this);
        return Mathf.FloorToInt(value * percent);
    }
}
