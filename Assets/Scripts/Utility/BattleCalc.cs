using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleCalc {

	const int FLAT_CRITRATE = 5;
	public const float CRIT_MODIFIER = 2;

	// Flat calculations

	public static int CalculateDamage(ItemEntry weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		if (weaponAtk.attackType == AttackType.PHYSICAL) {
			return weaponAtk.power + attacker.dmg;
		}
		else if (weaponAtk.attackType == AttackType.MENTAL) {
			return weaponAtk.power + attacker.mnd;
		}
		return -1;
	}

	public static int CalculateHeals(ItemEntry weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		return weaponAtk.power + attacker.skl;
	}

	public static int GetAttackSpeed(ItemEntry weaponAtk, StatsContainer attacker) {
		return attacker.spd;
	}

	/// <summary>
	/// Hit rate for the character with the given weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="attacker"></param>
	/// <returns></returns>
	public static int GetHitRate(ItemEntry weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		return weaponAtk.hitRate + attacker.skl * 2 + attacker.hitBoost;
	}

	/// <summary>
	/// Critical hit rate for the character with the given weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="attacker"></param>
	/// <returns></returns>
	public static int GetCritRate(ItemEntry weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		return FLAT_CRITRATE + weaponAtk.critRate + attacker.critBoost;
	}

	/// <summary>
	/// Avoid rate for the character.
	/// </summary>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static int GetAvoid(StatsContainer defender) {
		return defender.spd * 2 + defender.avoidBoost;
	}

	/// <summary>
	/// Critical avoid rate for the character.
	/// </summary>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static int GetCritAvoid(ItemEntry weaponDef, StatsContainer defender) {
		return 0;
	}


	// Against opponent calculations

	public static int CalculateDamageBattle(ItemEntry weaponAtk, ItemEntry weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int pwr = (weaponAtk.attackType == AttackType.PHYSICAL) ? weaponAtk.power + attacker.dmg : weaponAtk.power + attacker.mnd;
		int def = (weaponAtk.attackType == AttackType.PHYSICAL) ? defender.def + terrain.defense : defender.mnd + terrain.defense;
		float weakness = GetWeaknessBonus(weaponAtk, defender);

		int damage = Mathf.Max(0, Mathf.FloorToInt(pwr * weakness) - def);
		return damage;
	}

	/// <summary>
	/// Returns the hit rate for the character given the opponent's stats.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <param name="attacker"></param>
	/// <param name="defender"></param>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public static int GetHitRateBattle(ItemEntry weaponAtk, ItemEntry weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int avoid = GetAvoid(defender) + terrain.avoid;
		return Mathf.Clamp(GetHitRate(weaponAtk, attacker) - avoid, 0, 100);
	}

	/// <summary>
	/// Returns the crit rate for the character given the opponent's stats.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <param name="attacker"></param>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static int GetCritRateBattle(ItemEntry weaponAtk, ItemEntry weaponDef, StatsContainer attacker, StatsContainer defender) {
		return Mathf.Clamp(GetCritRate(weaponAtk, attacker) - GetCritAvoid(weaponDef, defender), 0, 100);
	}


	// Different types of advantages

	/// <summary>
	/// Checks if the attacking weapon has advantage against the defender.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static bool CheckWeaponWeakness(ItemEntry weaponAtk, StatsContainer defender) {
		if (weaponAtk == null)
			return false;

		for (int i = 0; i < weaponAtk.advantageType.Count; i++) {
			if (weaponAtk.advantageType[i] == defender.currentClass.classType)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Adds a damage bonus if the weapon exploits a weakness.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static float GetWeaknessBonus(ItemEntry weaponAtk, StatsContainer defender) {
		for (int i = 0; i < weaponAtk.advantageType.Count; i++) {
			if (weaponAtk.advantageType[i] == defender.currentClass.classType)
				return 2f;
		}
		return 1f;
	}


	// Experience calculations

	public static int GetExperienceDamage(StatsContainer player, StatsContainer enemy, bool isKill, bool isBoss) {
		int ld = player.level - enemy.level;
		int killExp = (isBoss) ? 50 : 20;
		if (ld < 0) {
			ld = Mathf.Min(0, ld + 2);
		}

		int gainedExp = (int)((30 + ld) / 3.0f);
		if (isKill) {
			gainedExp += killExp + (ld * 3);
		}

		return gainedExp;
	}

	public static int GetExperienceSupport(ItemEntry weapon, StatsContainer player) {
		int ld = Mathf.Max(0, (int)(player.level * 0.5f) - 3);
		int baseExp = (weapon.weaponType == WeaponType.MEDKIT) ? 15 : 30;
		return baseExp - ld;
	}
}
