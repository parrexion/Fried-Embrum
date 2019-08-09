using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Exp")]
public class ExpBoost : CharacterSkill {

    public override int EditValue(int value, TacticsMove user) {
        return Mathf.FloorToInt(value * percent);
    }
}
