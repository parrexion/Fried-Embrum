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
    public int cost;
    public int hitRate;
    public int critRate;
    public int range;
    public int maxCharge;
    public bool variableRange;
    public bool droppable;
    
    [Space(10)]
    
    public ClassType[] advantageType;
    public Boost boost;


    public bool InRange(int distance) {
        return (distance == range || (distance < range && variableRange));
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
}
