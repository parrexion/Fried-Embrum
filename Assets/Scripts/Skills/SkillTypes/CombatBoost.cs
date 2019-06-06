﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/CombatBoost")]
public class CombatBoost : CharacterSkill {
    
    protected override void UseSkill(TacticsMove user, TacticsMove enemy) {
		if (CheckReq(user, enemy))
            user.ReceiveBuff(boost, true, false);
    }

    protected override void RemoveEffect(TacticsMove user, TacticsMove enemy) {
        if (CheckReq(user, enemy))
            user.ReceiveBuff(boost.InvertStats(), true, false);
    }

	private bool CheckReq(TacticsMove user, TacticsMove enemy) {
		bool terrainOk = false;
		for (int i = 0; i < terrainReq.Count; i++) {
			if (user.currentTile.terrain == terrainReq[i]) {
				terrainOk = true;
				break;
			}
		}

		int dist = BattleMap.DistanceTo(user, enemy);
		bool rangeOk = (range <= dist && dist <= rangeMax);

		bool retaliateOk = (enemyCanAttack == EnemyCanAttack.BOTH);
		if (enemyCanAttack != EnemyCanAttack.BOTH) {
			InventoryTuple tuple = enemy.GetEquippedWeapon(ItemCategory.WEAPON);
			bool inRange = (tuple.item != null && tuple.item.InRange(range));
			retaliateOk = ((inRange && enemyCanAttack == EnemyCanAttack.ATTACK) || 
				(!inRange && enemyCanAttack == EnemyCanAttack.NO_ATTACK));
		}

		return (retaliateOk && (terrainOk || rangeOk));
	}
}
