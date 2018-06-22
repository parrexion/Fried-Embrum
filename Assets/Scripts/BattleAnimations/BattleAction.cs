using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction {

	public bool leftSide;
	public bool isDamage;
	public TacticsMove attacker;
	public TacticsMove defender;


	public BattleAction(bool leftSide, bool damage, TacticsMove atk, TacticsMove def) {
		this.leftSide = leftSide;
		isDamage = damage;
		attacker = atk;
		defender = def;
	}

	public int GetDamage() {
		int wpn = attacker.GetWeapon(ItemCategory.WEAPON).power;
		int atk = attacker.stats.atk;
		int def = defender.stats.def;
		int adv = GetAdvantage();
		float bonus = (adv == 1) ? 1.2f : (adv == -1) ? 0.8f : 1f;
		float weakness = GetWeaknessBonus(attacker.GetWeapon(ItemCategory.WEAPON).advantageType, defender.stats.classData.classType);

		int damage = Mathf.Max(0, Mathf.FloorToInt((wpn + atk) * weakness * bonus) - def);
		return damage;
	}

	public int GetHeals() {
		return attacker.GetWeapon(ItemCategory.STAFF).power;
	}

	public int GetHitRate() {
		float statsBoost = attacker.stats.skl * 1.5f + attacker.stats.lck * 0.5f;
		return attacker.GetWeapon(ItemCategory.WEAPON).hitRate + (int)statsBoost - GetAvoid();
	}

	private int GetAvoid() {
		return (int)(defender.stats.spd * 1.5f + defender.stats.lck * 0.5f);
	}

	public int GetCritRate() {
		return attacker.GetWeapon(ItemCategory.WEAPON).critRate + (int)(attacker.stats.skl * 0.5f) - GetCritAvoid();
	}

	private int GetCritAvoid() {
		return defender.stats.lck;
	}

	public int GetAdvantage() {
		return attacker.GetWeapon(ItemCategory.WEAPON).GetAdvantage(defender.GetWeapon(ItemCategory.WEAPON));
	}

	public float GetWeaknessBonus(ClassType[] atkType, ClassType defType) {
		for (int i = 0; i < atkType.Length; i++) {
			if (atkType[i] == defType)
				return 1.5f;
		}
		return 1f;
	}

	public int GetExperience() {
		TacticsMove player = (attacker.faction == Faction.PLAYER) ? attacker : defender;
		TacticsMove enemy = (attacker.faction == Faction.PLAYER) ? defender : attacker;
		if (!player.IsAlive())
			return 0;

		//Exp for support skills
		if (!isDamage) {
			return (player.GetWeapon(ItemCategory.STAFF).itemType == ItemType.HEAL) ? 10 : 0;
		}

		//Exp for fights
		bool killed = (!enemy.IsAlive());
		int attackerLevel = attacker.stats.level;
		int defenderLevel = defender.stats.level;

		int ld = attackerLevel - defenderLevel;
		if (ld < 0) {
			ld = Mathf.Min(0,ld+2);
		}

		int gainedExp = (int)((30 + ld) / 3.0f);
		if (killed) {
			gainedExp += 20 + (ld * 3);
		}
		
		return gainedExp;
	}

}
