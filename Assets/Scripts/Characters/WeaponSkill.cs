using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {NONE, SWORD, LANCE, AXE, RED, BLUE, GREEN, BOW}

[System.Serializable]
public class WeaponSkill : BaseSkill {

	public WeaponType weaponType = WeaponType.NONE;
	public int power = 5;
	public int range = 1;
	public bool variableRange = false;
	public ClassType advantageType;


	public bool InRange(int distance) {
		return (distance == range || (distance < range && variableRange));
	}

	public int GetAdvantage(WeaponSkill otherWeapon) {
		if (otherWeapon == null)
			return 0;
		switch(weaponType) 
		{
			case WeaponType.SWORD:
			case WeaponType.RED:
				if (otherWeapon.weaponType == WeaponType.AXE || otherWeapon.weaponType == WeaponType.GREEN)
					return 1;
				else if (otherWeapon.weaponType == WeaponType.LANCE || otherWeapon.weaponType == WeaponType.BLUE)
					return -1;
				break;
			case WeaponType.LANCE:
			case WeaponType.BLUE:
				if (otherWeapon.weaponType == WeaponType.SWORD || otherWeapon.weaponType == WeaponType.RED)
					return 1;
				else if (otherWeapon.weaponType == WeaponType.AXE || otherWeapon.weaponType == WeaponType.GREEN)
					return -1;
				break;
			case WeaponType.AXE:
			case WeaponType.GREEN:
				if (otherWeapon.weaponType == WeaponType.LANCE || otherWeapon.weaponType == WeaponType.BLUE)
					return 1;
				else if (otherWeapon.weaponType == WeaponType.SWORD || otherWeapon.weaponType == WeaponType.RED)
					return -1;
				break;
			default:
				return 0;
		}

		return 0;
	}

	public Color GetTypeColor() {
		switch (weaponType)
		{
			case WeaponType.SWORD:
			case WeaponType.RED:
				return Color.red;
			case WeaponType.LANCE: 
			case WeaponType.BLUE:
				return Color.blue;
			case WeaponType.AXE: 
			case WeaponType.GREEN:
				return Color.green;
			default: return Color.white;
		}
	}
}
