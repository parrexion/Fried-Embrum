using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public int GetSpeedDifference() {
		return BattleCalc.GetAttackSpeed(weaponAtk.item, attacker.stats) - BattleCalc.GetAttackSpeed(weaponDef.item, defender.stats);
	}

	public int GetAdvantage() {
		return BattleCalc.GetWeaponAdvantage(weaponAtk.item, weaponDef.item);
	}

	public int GetDamage() {
		return BattleCalc.CalculateDamageBattle(weaponAtk.item, weaponDef.item, attacker.stats, defender.stats, terrainDef);
	}

	public int GetHeals() {
		return BattleCalc.CalculateHeals(staffAtk.item, attacker.stats);
	}

	public int GetHitRate() {
		return BattleCalc.GetHitRateBattle(weaponAtk.item, weaponDef.item, attacker.stats, defender.stats, terrainDef);
	}

	public int GetCritRate() {
		return BattleCalc.GetCritRateBattle(weaponAtk.item, attacker.stats, defender.stats);
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

}
