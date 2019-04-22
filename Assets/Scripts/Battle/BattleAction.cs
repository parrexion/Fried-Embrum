using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleAction {

	public enum Type { DAMAGE, HEAL, BUFF }

	public bool leftSide;
	public Type type;
	public TacticsMove attacker;
	public TacticsMove defender;
	public InventoryTuple weaponAtk;
	public InventoryTuple weaponDef;
	public InventoryTuple staffAtk;
	public TerrainTile terrainDef;


	public BattleAction(bool leftSide, Type type, TacticsMove atk, TacticsMove def) {
		this.leftSide = leftSide;
		this.type = type;
		attacker = atk;
		defender = def;
		weaponAtk = attacker.inventory.GetFirstUsableItemTuple(ItemCategory.WEAPON, attacker.stats);
		weaponDef = defender.inventory.GetFirstUsableItemTuple(ItemCategory.WEAPON, defender.stats);
		staffAtk = attacker.inventory.GetFirstUsableItemTuple(ItemCategory.STAFF, attacker.stats);
		if (defender.currentTile)
			terrainDef = defender.currentTile.terrain;
	}

	/// <summary>
	/// Returns the speed difference between the battling characters.
	/// Positive values mean the attacker is faster, negative for the defender.
	/// </summary>
	/// <returns></returns>
	public int GetSpeedDifference() {
		if (defender.faction == Faction.WORLD)
			return 0;
		ItemEntry wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetAttackSpeed(weaponAtk.item, attacker.stats) - BattleCalc.GetAttackSpeed(wpn, defender.stats);
	}

	/// <summary>
	/// Returns the weapon advatage for the battle.
	/// 1 means advatage for the attacker, -1 for the defender, and 0 is neutral.
	/// </summary>
	/// <returns></returns>
	public int GetAdvantage() {
		if (defender.faction == Faction.WORLD)
			return 0;
		ItemEntry wpn1 = (weaponAtk != null) ? weaponAtk.item : null;
		ItemEntry wpn2 = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetWeaponAdvantage(wpn1, wpn2);
	}

	public int GetDamage() {
		ItemEntry wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.CalculateDamageBattle(weaponAtk.item, wpn, attacker.stats, defender.stats, terrainDef);
	}

	public int GetHeals() {
		return BattleCalc.CalculateHeals(staffAtk.item, attacker.stats);
	}

	public int GetHitRate() {
		if (defender.faction == Faction.WORLD)
			return 100;
		ItemEntry wpn = (weaponDef != null) ? weaponDef.item : null;
		return BattleCalc.GetHitRateBattle(weaponAtk.item, wpn, attacker.stats, defender.stats, terrainDef);
	}

	public int GetCritRate() {
		if (defender.faction == Faction.WORLD)
			return 0;
		return BattleCalc.GetCritRateBattle(weaponAtk.item, attacker.stats, defender.stats);
	}

	public bool CheckWeaponWeakness() {
		ItemEntry wpn = (weaponAtk != null) ? weaponAtk.item : null;
		return BattleCalc.CheckWeaponWeakness(wpn, defender.stats);
	}

	public int GetExperience() {
		if (defender.faction == Faction.WORLD)
			return 0;

		//Exp for support skills
		if (type != Type.DAMAGE) {
			return (attacker.faction == Faction.PLAYER) ? BattleCalc.GetExperienceSupport(staffAtk.item, attacker.stats) : 0;
		}

		//Exp for fights
		TacticsMove player = (attacker.faction == Faction.PLAYER) ? attacker : defender;
		TacticsMove enemy = (attacker.faction == Faction.PLAYER) ? defender : attacker;
		if (!player.IsAlive())
			return 0;

		bool killed = (!enemy.IsAlive());
		bool isBoss = (enemy is NPCMove && ((NPCMove)enemy).aggroType == AggroType.BOSS);
		return BattleCalc.GetExperienceDamage(player.stats, enemy.stats, killed, isBoss);
	}

	/// <summary>
	/// Checks if the defender is in range to retaliate.
	/// </summary>
	/// <param name="distance"></param>
	/// <returns></returns>
	public bool DefenderInRange(int distance) {
		if (weaponDef.item == null || weaponDef.charge <= 0)
			return false;

		return weaponDef.item.InRange(distance);
	}

	/// <summary>
	/// Attempts to attack. If the attacks hit, the damage dealt is returned.
	/// If the attack misses, -1 is returned.
	/// </summary>
	/// <param name="useTrueHit"></param>
	/// <returns></returns>
	public int AttemptAttack(bool useTrueHit) {
		return (GenerateHitNumber(GetHitRate(), useTrueHit)) ? GetDamage() : -1;
	}

	/// <summary>
	/// Attempts to make a crit. If the crit rate is enough than true is returned.
	/// </summary>
	/// <returns></returns>
	public bool AttemptCrit() {
		return SingleNumberCheck(GetCritRate());
	}
	
	/// <summary>
	/// Generates two hit numbers from 100 and averages them.
	/// Is a hit if it's below the hit number.
	/// </summary>
	/// <param name="hit"></param>
	/// <returns></returns>
	private bool GenerateHitNumber(int hit, bool useTrueHit) {
		int nr = Random.Range(0, 100);
		if (useTrueHit) {
			nr += Random.Range(0, 100);
			nr /= 2;
		}
		// Debug.Log("HIT:  " + nr + " -> " + hit);
		return (nr < hit);
	}

	/// <summary>
	/// Generates a single number out of 100 and hits if it's below the hit value.
	/// </summary>
	/// <param name="hit"></param>
	/// <returns></returns>
	private bool SingleNumberCheck(int hit) {
		int nr = Random.Range(0, 100);
		// Debug.Log("SINGLE:  " + nr + " -> " + hit);
		return (nr < hit);
	}
}
