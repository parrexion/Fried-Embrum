using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleAction {

	public bool leftSide;
	public bool isDamage;
	public TacticsMove attacker;
	public TacticsMove defender;
	public InventoryTuple weaponAtk;
	public InventoryTuple weaponDef;
	public InventoryTuple staffAtk;
	public TerrainTile terrainDef;


	public BattleAction(bool leftSide, bool damage, TacticsMove atk, TacticsMove def) {
		this.leftSide = leftSide;
		isDamage = damage;
		attacker = atk;
		defender = def;
		weaponAtk = attacker.GetFirstUsableInventoryTuple(ItemCategory.WEAPON);
		weaponDef = defender.GetFirstUsableInventoryTuple(ItemCategory.WEAPON);
		staffAtk = attacker.GetFirstUsableInventoryTuple(ItemCategory.STAFF);
		terrainDef = defender.GetTerrain();
	}

	/// <summary>
	/// Returns the speed difference between the battling characters.
	/// Positive values mean the attacker is faster, negative for the defender.
	/// </summary>
	/// <returns></returns>
	public int GetSpeedDifference() {
		WeaponItem wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetAttackSpeed(weaponAtk.item, attacker.stats) - BattleCalc.GetAttackSpeed(wpn, defender.stats);
	}

	/// <summary>
	/// Returns the weapon advatage for the battle.
	/// 1 means advatage for the attacker, -1 for the defender, and 0 is neutral.
	/// </summary>
	/// <returns></returns>
	public int GetAdvantage() {
		WeaponItem wpn1 = (weaponAtk != null) ? weaponAtk.item : null;
		WeaponItem wpn2 = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetWeaponAdvantage(wpn1, wpn2);
	}

	public int GetDamage() {
		WeaponItem wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.CalculateDamageBattle(weaponAtk.item, wpn, attacker.stats, defender.stats, terrainDef);
	}

	public int GetHeals() {
		return BattleCalc.CalculateHeals(staffAtk.item, attacker.stats);
	}

	public int GetHitRate() {
		WeaponItem wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetHitRateBattle(weaponAtk.item, wpn, attacker.stats, defender.stats, terrainDef);
	}

	public int GetCritRate() {
		return BattleCalc.GetCritRateBattle(weaponAtk.item, attacker.stats, defender.stats);
	}

	public bool CheckWeaponWeakness() {
		WeaponItem wpn = (weaponAtk != null) ? weaponAtk.item : null;
		return BattleCalc.CheckWeaponWeakness(wpn, defender.stats);
	}

	public int GetExperience() {

		//Exp for support skills
		if (!isDamage) {
			return (attacker.faction == Faction.PLAYER) ? BattleCalc.GetExperienceSupport(staffAtk.item, attacker.stats) : 0;
		}

		//Exp for fights
		TacticsMove player = (attacker.faction == Faction.PLAYER) ? attacker : defender;
		TacticsMove enemy = (attacker.faction == Faction.PLAYER) ? defender : attacker;
		if (!player.IsAlive())
			return 0;

		bool killed = (!enemy.IsAlive());
		return BattleCalc.GetExperienceDamage(player.stats, enemy.stats, killed);
	}

	/// <summary>
	/// Checks if the defender is in range to retaliate.
	/// </summary>
	/// <param name="distance"></param>
	/// <returns></returns>
	public bool DefenderInRange(int distance) {
		if (weaponDef == null)
			return false;

		return weaponDef.item.InRange(distance);
	}
}
