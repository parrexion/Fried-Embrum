using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { WEAPON, STAFF, CONSUME }
public enum ItemType {NONE, SWORD, LANCE, AXE, MAGIC, THROW, BOW, HEAL, BUFF, CHEAL, CSTATS}

[CreateAssetMenu]
public class WeaponItem : Item {

    public ItemCategory itemCategory = ItemCategory.WEAPON;
    public ItemType itemType = ItemType.NONE;
    public int power = 5;
    public int maxCharge;
    public int skillReq;
    public int hitRate;
    public int critRate;
    public int range;
    public bool variableRange;
    public int cost;
    
    [Space(10)]
    
    public ClassType[] advantageType;
    public Boost boost;


    public bool InRange(int distance) {
        return (distance == range || (distance < range && variableRange));
    }

    public bool CanUse(StatsContainer stats) {
        if (itemCategory == ItemCategory.WEAPON) {
            int skill = stats.wpnSkills[(int)itemType];
            return (skill != 0 && skill >= skillReq);
        }
        else if (itemCategory == ItemCategory.STAFF) {
            int skill = stats.wpnSkills[stats.wpnSkills.Length-1];
            return (skill != 0 && skill >= skillReq);
        }

        return true;
    }

    public int GetAdvantage(WeaponItem otherWeapon) {
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

    public static Color GetTypeColor(ItemType itemType) {
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
}
