using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { WEAPON, STAFF, CONSUME }
public enum ItemType {NONE, SWORD, LANCE, AXE, MAGIC, THROW, BOW, HEAL, BUFF, CHEAL, CSTATS}

[CreateAssetMenu(menuName = "LibraryEntries/ItemEntry")]
public class ItemEntry : ScrObjLibraryEntry {

    public Sprite icon;
    public string description;

    public ItemCategory itemCategory = ItemCategory.WEAPON;
    public ItemType itemType = ItemType.NONE;

    public int power = 5;
    public int maxCharge;
    public int hitRate;
    public int critRate;
    public WeaponRange range = new WeaponRange(1, 1);

	public int skillReq;
    public int weight;
    public int cost;
    
    [Space(10)]
    
    public List<ClassType> advantageType = new List<ClassType>();
    public Boost boost;


	/// <summary>
	/// Resets the values to default.
	/// </summary>
	public override void ResetValues() {
		base.ResetValues();

		icon = null;
        description = "";

        itemCategory = ItemCategory.WEAPON;
        itemType = ItemType.NONE;

		cost = 0;
        maxCharge = 0;
        skillReq = 0;
        weight = 0;

        power = 0;
        hitRate = 0;
        critRate = 0;
        range = new WeaponRange(1,1);
    
        advantageType = new List<ClassType>();
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
        itemType = item.itemType;

		cost = item.cost;
        maxCharge = item.maxCharge;
        skillReq = item.skillReq;
        weight = item.weight;

        power = item.power;
        hitRate = item.hitRate;
        critRate = item.critRate;
        range.min = item.range.min;
        range.max = item.range.max;

        advantageType = new List<ClassType>();
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
    public bool CanUse(int skill) {
        if (itemCategory != ItemCategory.CONSUME) {
            return (skill != 0 && skill >= skillReq);
        }

        return true;
    }

    /// <summary>
    /// Compares this weapon to the otherWeapon for weapon triangle advantage.
    /// Returns 1 if this weapon has advantage, -1 for otherWeapon and 0 for neutral.
    /// </summary>
    /// <param name="otherWeapon"></param>
    /// <returns></returns>
    public int GetAdvantage(ItemEntry otherWeapon) {
        if (otherWeapon == null)
            return 0;
        switch(itemType) 
        {
            case ItemType.SWORD:
            case ItemType.MAGIC:
                if (otherWeapon.itemType == ItemType.AXE || otherWeapon.itemType == ItemType.BOW)
                    return 1;
                else if (otherWeapon.itemType == ItemType.LANCE || otherWeapon.itemType == ItemType.THROW)
                    return -1;
                break;
            case ItemType.LANCE:
            case ItemType.THROW:
                if (otherWeapon.itemType == ItemType.SWORD || otherWeapon.itemType == ItemType.MAGIC)
                    return 1;
                else if (otherWeapon.itemType == ItemType.AXE || otherWeapon.itemType == ItemType.BOW)
                    return -1;
                break;
            case ItemType.AXE:
            case ItemType.BOW:
                if (otherWeapon.itemType == ItemType.LANCE || otherWeapon.itemType == ItemType.THROW)
                    return 1;
                else if (otherWeapon.itemType == ItemType.SWORD || otherWeapon.itemType == ItemType.MAGIC)
                    return -1;
                break;
            default:
                return 0;
        }

        return 0;
    }

    /// <summary>
    /// Returns the type's color of the weapon for weapon triangle clarity.
    /// </summary>
    /// <returns></returns>
    public Color GetTypeColor() {
        switch (itemType)
        {
            case ItemType.SWORD:
            case ItemType.MAGIC:
                return Color.red;
            case ItemType.LANCE: 
            case ItemType.THROW:
                return Color.blue;
            case ItemType.AXE: 
            case ItemType.BOW:
                return Color.green;
            default: return Color.white;
        }
    }

    /// <summary>
    /// Takes the weaponskill value and converts it into a rank letter.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static string GetRankLetter(int level) {
        if (level == -1) {
            return "-";
        }
        else if (level == 0) {
            Debug.LogWarning("Weapon skill is 0");
                return "yo man!";
        }
        else if (level >= 400) {
            return "A";
        }
        else if (level >= 300) {
            return "B";
        }
        else if (level >= 200) {
            return "C";
        }
        else if (level >= 100) {
            return "D";
        }
        else {
            return "E";
        }
    }

    public string GetDescription() {
        string desc = description;
        string rangeStr = (range.min != range.max) ? range.min + "-" + range.max : range.min.ToString();
        if (itemCategory == ItemCategory.WEAPON) {
            desc += "\nRange: " + rangeStr + ", Power: " + power + 
                    "\nHit: " + hitRate + ", Crit: " + critRate +
                    "\nWeight: " + weight + ", Req: " + ItemEntry.GetRankLetter(skillReq);
        }
        else if (itemCategory == ItemCategory.STAFF) {
            desc += "\nRange: " + rangeStr + ", Power: " + power +
                    "\nReq: " + ItemEntry.GetRankLetter(skillReq);
        }
        return desc;
    }
}
