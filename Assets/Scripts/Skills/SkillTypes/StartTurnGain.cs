using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/StartTurn")]
public class StartTurnGain : CharacterSkill {

	protected override void UseSkill(TacticsMove user, TacticsMove enemy) { }
	protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) { }

	protected override void ForEachBoost(CharacterListVariable list, TacticsMove user) {
		if (includeSelf) {
			user.TakeHeals((int)(user.stats.hp * percent));
		}
		for (int i = 0; i < list.values.Count; i++) {
			if (BattleMap.DistanceTo(list.values[i], user) <= range) {
				list.values[i].TakeHeals((int)(list.values[i].stats.hp * percent));
			}
		}
	}
}
