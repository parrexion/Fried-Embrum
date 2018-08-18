using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleCalc {


	// Flat calculations

	public static int CalculateDamage(WeaponItem weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		return weaponAtk.power + attacker.atk;
	}

	public static int CalculateHeals(WeaponItem weapon, StatsContainer attacker) {
		if (weapon == null)
			return -1;
		return weapon.power + attacker.atk;
	}

	public static int GetAttackSpeed(WeaponItem weaponAtk, StatsContainer attacker) {
		return attacker.spd - attacker.GetConPenalty(weaponAtk);
	}

	/// <summary>
	/// Hit rate for the character with the given weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="attacker"></param>
	/// <returns></returns>
	public static int GetHitRate(WeaponItem weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		int trueSkl = attacker.skl - attacker.GetConPenalty(weaponAtk);
		// int statsBoost = (int)(trueSkl * 1.5f + attacker.lck * 0.5f);
		int statsBoost = (int)(trueSkl * 2 + attacker.lck * 0.5f);
		return statsBoost + weaponAtk.hitRate;
	}

	/// <summary>
	/// Critical hit rate for the character with the given weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="attacker"></param>
	/// <returns></returns>
	public static int GetCritRate(WeaponItem weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		int trueSkl = attacker.skl - attacker.GetConPenalty(weaponAtk);
		int calcCrit = weaponAtk.critRate + (int)(trueSkl * 0.5f);
		return calcCrit;
	}
	
	/// <summary>
	/// Avoid rate for the character.
	/// </summary>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static int GetAvoid(StatsContainer defender) {
		// return (int)(defender.spd * 1.5f + defender.lck * 0.5f);
		return defender.spd * 2 + defender.lck;
	}

	/// <summary>
	/// Critical avoid rate for the character.
	/// </summary>
	/// <param name="defender"></param>
	/// <returns></returns>
	public static int GetCritAvoid(StatsContainer defender) {
		return defender.lck;
	}


	// Against opponent calculations

	public static int CalculateDamageBattle(WeaponItem weaponAtk, WeaponItem weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int pwr = weaponAtk.power + attacker.atk;
		int def = defender.def + terrain.defense;
		int bonus = GetDamageAdvantage(weaponAtk, weaponDef);
		float weakness = GetWeaknessBonus(weaponAtk, defender);

		int damage = Mathf.Max(0, Mathf.FloorToInt(pwr * weakness) + bonus - def);
		return damage;
	}

	public static int GetHitRateBattle(WeaponItem weaponAtk, WeaponItem weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int bonus = GetHitAdvantage(weaponAtk, weaponDef);
		int avoid = GetAvoid(defender) + terrain.avoid;
		return Mathf.Clamp(GetHitRate(weaponAtk, attacker) + bonus - avoid, 0, 100);
	}

	public static int GetCritRateBattle(WeaponItem weaponAtk, StatsContainer attacker, StatsContainer defender) {
		int trueSkl = attacker.skl - attacker.GetConPenalty(weaponAtk);
		int calcCrit = weaponAtk.critRate + (int)(trueSkl * 0.5f);
		return Mathf.Clamp(calcCrit - GetCritAvoid(defender), 0, 100);
	}


	// Different types of advantages

	public static int GetWeaponAdvantage(WeaponItem weaponAtk, WeaponItem weaponDef) {
		return weaponAtk.GetAdvantage(weaponDef);
	}

	public static int GetDamageAdvantage(WeaponItem weaponAtk, WeaponItem weaponDef) {
		int adv = GetWeaponAdvantage(weaponAtk, weaponDef);
		return (adv == 1) ? 1 : (adv == -1) ? -1 : 0;
	}

	public static int GetHitAdvantage(WeaponItem weaponAtk, WeaponItem weaponDef) {
		int adv = GetWeaponAdvantage(weaponAtk, weaponDef);
		return (adv == 1) ? 15 : (adv == -1) ? -15 : 0;
	}

	public static bool CheckWeaponWeakness(WeaponItem weaponAtk, StatsContainer defender) {
		for (int i = 0; i < weaponAtk.advantageType.Length; i++) {
			if (weaponAtk.advantageType[i] == defender.classData.classType)
				return true;
		}
		return false;
	}

	public static float GetWeaknessBonus(WeaponItem weaponAtk, StatsContainer defender) {
		for (int i = 0; i < weaponAtk.advantageType.Length; i++) {
			if (weaponAtk.advantageType[i] == defender.classData.classType)
				return 2.5f;
		}
		return 1f;
	}


	// Experience calculations

	public static int GetExperienceDamage(StatsContainer player, StatsContainer enemy, bool isKill) {

		int ld = player.level - enemy.level;
		if (ld < 0) {
			ld = Mathf.Min(0,ld+2);
		}

		int gainedExp = (int)((30 + ld) / 3.0f);
		if (isKill) {
			gainedExp += 20 + (ld * 3);
		}
		
		return gainedExp;
	}

	public static int GetExperienceSupport(WeaponItem weapon, StatsContainer player) {

		int ld = Mathf.Max(0, (int)(player.level * 0.5f) - 3);
		int baseExp = (weapon.itemType == ItemType.HEAL) ? 15 : 30;
		return baseExp - ld;
	}
}
