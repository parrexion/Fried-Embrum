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
		int pwr = attacker.stats.GetAttackPower();
		int def = defender.stats.def;
		int adv = GetAdvantage();
		int bonus = (adv == 1) ? 2 : (adv == -1) ? -2 : 0;
		float weakness = GetWeaknessBonus(attacker.GetWeapon(ItemCategory.WEAPON).advantageType, defender.stats.classData.classType);

		int damage = Mathf.Max(0, Mathf.FloorToInt(pwr * weakness) + bonus - def);
		return damage;
	}

	public int GetHeals() {
		return attacker.GetWeapon(ItemCategory.STAFF).power + attacker.stats.atk;
	}

	public int GetHitRate() {
		int adv = GetAdvantage();
		int bonus = (adv == 1) ? 15 : (adv == -1) ? -15 : 0;
		return Mathf.Clamp(attacker.stats.GetHitRate() - defender.stats.GetAvoid() + bonus, 0, 100);
	}

	public int GetCritRate() {
		return Mathf.Clamp(attacker.stats.GetCriticalRate() - defender.stats.GetCritAvoid(), 0, 100);
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
