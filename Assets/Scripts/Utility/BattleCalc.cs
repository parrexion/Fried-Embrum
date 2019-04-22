using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleCalc {


	// Flat calculations

	public static int CalculateDamage(ItemEntry weaponAtk, StatsContainer attacker) {
		if (weaponAtk == null)
			return -1;
		return weaponAtk.power + attacker.atk;
	}

	public static int CalculateHeals(ItemEntry weapon, StatsContainer attacker) {
		if (weapon == null)
			return -1;
		return weapon.power + attacker.atk;
	}

	public static int GetAttackSpeed(ItemEntry weaponAtk, StatsContainer attacker) {
		return attacker.spd - attacker.GetConPenalty(weaponAtk);
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
	public static int GetCritRate(ItemEntry weaponAtk, StatsContainer attacker) {
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

	public static int CalculateDamageBattle(ItemEntry weaponAtk, ItemEntry weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int pwr = weaponAtk.power + attacker.atk;
		int def = defender.def + terrain.defense;
		int bonus = GetDamageAdvantage(weaponAtk, weaponDef);
		float weakness = GetWeaknessBonus(weaponAtk, defender);

		int damage = Mathf.Max(0, Mathf.FloorToInt(pwr * weakness) + bonus - def);
		return damage;
	}

	/// <summary>
	/// Returns the hit rate for the character given the opponent's weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <param name="attacker"></param>
	/// <param name="defender"></param>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public static int GetHitRateBattle(ItemEntry weaponAtk, ItemEntry weaponDef, StatsContainer attacker, StatsContainer defender, TerrainTile terrain) {
		int bonus = GetHitAdvantage(weaponAtk, weaponDef);
		int avoid = GetAvoid(defender) + terrain.avoid;
		return Mathf.Clamp(GetHitRate(weaponAtk, attacker) + bonus - avoid, 0, 100);
	}

	public static int GetCritRateBattle(ItemEntry weaponAtk, StatsContainer attacker, StatsContainer defender) {
		int trueSkl = attacker.skl - attacker.GetConPenalty(weaponAtk);
		int calcCrit = weaponAtk.critRate + (int)(trueSkl * 0.5f);
		return Mathf.Clamp(calcCrit - GetCritAvoid(defender), 0, 100);
	}


	// Different types of advantages

	/// <summary>
	/// Compares the weapon advantage between the two weapons.
	/// 1 means attack is at advantage, -1 defender, and 0 is neutral.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <returns></returns>
	public static int GetWeaponAdvantage(ItemEntry weaponAtk, ItemEntry weaponDef) {
		if (weaponAtk == null)
			return 0;
		return weaponAtk.GetAdvantage(weaponDef);
	}

	/// <summary>
	/// Returns the damage bonus for the weapon given the opponent's weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <returns></returns>
	public static int GetDamageAdvantage(ItemEntry weaponAtk, ItemEntry weaponDef) {
		int adv = GetWeaponAdvantage(weaponAtk, weaponDef);
		return (adv == 1) ? 1 : (adv == -1) ? -1 : 0;
	}

	/// <summary>
	/// Calculates the weapon's hit rate given the opponent's weapon.
	/// </summary>
	/// <param name="weaponAtk"></param>
	/// <param name="weaponDef"></param>
	/// <returns></returns>
	public static int GetHitAdvantage(ItemEntry weaponAtk, ItemEntry weaponDef) {
		int adv = GetWeaponAdvantage(weaponAtk, weaponDef);
		return (adv == 1) ? 15 : (adv == -1) ? -15 : 0;
	}

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
			if (weaponAtk.advantageType[i] == defender.classData.classType)
				return true;
		}
		return false;
	}

	public static float GetWeaknessBonus(ItemEntry weaponAtk, StatsContainer defender) {
		for (int i = 0; i < weaponAtk.advantageType.Count; i++) {
			if (weaponAtk.advantageType[i] == defender.classData.classType)
				return 2.5f;
		}
		return 1f;
	}


	// Experience calculations

	public static int GetExperienceDamage(StatsContainer player, StatsContainer enemy, bool isKill, bool isBoss) {
		int ld = player.level - enemy.level;
		int killExp = (isBoss) ? 50 : 20;
		if (ld < 0) {
			ld = Mathf.Min(0,ld+2);
		}

		int gainedExp = (int)((30 + ld) / 3.0f);
		if (isKill) {
			gainedExp += killExp + (ld * 3);
		}
		
		return gainedExp;
	}

	public static int GetExperienceSupport(ItemEntry weapon, StatsContainer player) {
		int ld = Mathf.Max(0, (int)(player.level * 0.5f) - 3);
		int baseExp = (weapon.itemType == ItemType.HEAL) ? 15 : 30;
		return baseExp - ld;
	}
}
