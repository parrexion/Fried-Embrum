using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { WEAPON, SUPPORT, CONSUME }
public enum AttackType { PHYSICAL, MENTAL, HEAL, STATS }

[CreateAssetMenu(menuName = "LibraryEntries/Item")]
public class ItemEntry : ScrObjLibraryEntry {

    public Sprite icon;
    public string description;

    public ItemCategory itemCategory = ItemCategory.WEAPON;
    public WeaponType weaponType = WeaponType.NONE;
	public AttackType attackType = AttackType.PHYSICAL;

    public int cost;
    public int maxCharge;
	public bool researchNeeded;
	public WeaponRank skillReq;

    public int power;
    public int hitRate;
    public int critRate;
    public WeaponRange range = new WeaponRange(1, 1);
    
    [Space(10)]
    
    public List<MovementType> advantageType = new List<MovementType>();
    public Boost boost;


	/// <summary>
	/// Resets the values to default.
	/// </summary>
	public override void ResetValues() {
		base.ResetValues();

		icon = null;
        description = "";

        itemCategory = ItemCategory.WEAPON;
		weaponType = WeaponType.NONE;
		attackType = AttackType.PHYSICAL;

		cost = 0;
        maxCharge = 0;
		researchNeeded = false;
        skillReq = WeaponRank.NONE;

        power = 0;
        hitRate = 0;
        critRate = 0;
        range = new WeaponRange(1,1);
    
        advantageType = new List<MovementType>();
        boost = new Boost();
	}

	/// <summary>
	/// Copies the values from another entry.
	/// </summary>
	/// <param name="other"></param>
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		ItemEntry item = (ItemEntry)other;

		icon = item.icon;
        description = item.description;

        itemCategory = item.itemCategory;
        weaponType = item.weaponType;
        attackType = item.attackType;

		cost = item.cost;
        maxCharge = item.maxCharge;
		researchNeeded = item.researchNeeded;
        skillReq = item.skillReq;

        power = item.power;
        hitRate = item.hitRate;
        critRate = item.critRate;
        range.min = item.range.min;
        range.max = item.range.max;

        advantageType = new List<MovementType>();
        for (int i = 0; i < item.advantageType.Count; i++) {
            advantageType.Add(item.advantageType[i]);
        }
        boost = item.boost;
	}


    /// <summary>
    /// Returns true if the given distance is within the range of the weapon.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public bool InRange(int distance) {
        return (distance == -1 || range.InRange(distance));
    }

    /// <summary>
    /// Checks if an item can be used with the given skill value.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool CanUse(WeaponRank skill) {
		if (itemCategory == ItemCategory.CONSUME) {
			return true;
		}
		else if (skill == WeaponRank.NONE) {
			return false;
		}
        else {
            return (skill >= skillReq);
        }
    }

    /// <summary>
    /// Checks if an item can be used with the given skill value.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool CanEquip(WeaponRank skill) {
        if (itemCategory == ItemCategory.CONSUME || skill == 0) {
			return false;
        }

        return (skill >= skillReq);
    }

	/// <summary>
	/// Generates a description string containing the stats of the item.
	/// </summary>
	/// <returns></returns>
    public string GetDescription() {
        string desc = description;
        string rangeStr = (range.min != range.max) ? range.min + "-" + range.max : range.min.ToString();
        if (itemCategory == ItemCategory.WEAPON) {
            desc += "\nRange: " + rangeStr + ", Power: " + power + 
                    "\nHit: " + hitRate + ", Crit: " + critRate +
                    ", Req: " + skillReq.ToString();
        }
        else if (itemCategory == ItemCategory.SUPPORT) {
            desc += "\nRange: " + rangeStr + ", Power: " + power +
                    "\nReq: " + skillReq.ToString();
        }
        return desc;
    }
}
