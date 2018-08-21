using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { WILD, WEAPON, HEAD, BODY, VISION }

[CreateAssetMenu (menuName = "LibraryEntries/ItemEquip")]
public class ItemEquip : ItemEntry {

	public int healthModifier;
	public int attackModifier;
	public int defenseModifier;
	public int sAttackModifier;
	public int sDefenseModifier;

	/// <summary>
	/// Resets the values to default.
	/// </summary>
	public override void ResetValues() {
		base.ResetValues();
		healthModifier = 0;
		attackModifier = 0;
		defenseModifier = 0;
		sAttackModifier = 0;
		sDefenseModifier = 0;
	}

	/// <summary>
	/// Copies the values from another entry.
	/// </summary>
	/// <param name="other"></param>
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		ItemEquip item = (ItemEquip)other;

		healthModifier = item.healthModifier;
		attackModifier = item.attackModifier;
		defenseModifier = item.defenseModifier;
		sAttackModifier = item.sAttackModifier;
		sDefenseModifier = item.sDefenseModifier;
	}
}
