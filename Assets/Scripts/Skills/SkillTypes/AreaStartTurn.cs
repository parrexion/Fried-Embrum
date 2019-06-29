using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AreaStartTurn")]
public class AreaStartTurn : CharacterSkill {


    [Space(10)]
    public bool useOnSelf;
    
    public override void UseSkill(TacticsMove user, TacticsMove enemy) { }
    public override void EndSkill(TacticsMove user, TacticsMove enemy) { }

    public override void ForEachBoost(CharacterListVariable list, TacticsMove user) {
        for (int i = 0; i < list.values.Count; i++) {
            if (list.values[i] == user && !useOnSelf)
                continue;
            int distance = BattleMap.DistanceTo(user, list.values[i]);
            if (distance <= range)
                list.values[i].ReceiveBuff(boost, true, true);
        }
    }
}
