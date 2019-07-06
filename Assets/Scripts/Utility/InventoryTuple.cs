using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryTuple {

	[Header("Item")]
	public string uuid = "";
	private ItemEntry itemz = null;

	[Header("Slot values")]
	public int index = -1;
	public bool droppable;

	[Header("Item stats")]
	public Sprite icon = null;
	public Color repColor = Color.white;
	public string entryName = "";

	public int cost = 0;
	public int power = 0;
	public int hitRate = 0;
	public int critRate = 0;
	public int maxCharge = 0;
	public int currentCharges = 0;
	public WeaponRange range = new WeaponRange(1, 1);

	public ItemCategory itemCategory = ItemCategory.CONSUME;
	public WeaponType weaponType = WeaponType.NONE;
	public AttackType attackType = AttackType.PHYSICAL;
	public List<MovementType> advantageType = new List<MovementType>();
	public WeaponRank skillReq;
	public Boost boost = new Boost();

	[Header("Upgrades")]
	public int bonusPower;
	public int bonusHit;
	public int bonusCrit;
	public int bonusCharges;
	public int bonusCost;


	public InventoryTuple(ItemEntry item, int index = -1, int charges = -1) {
		this.index = index;
		itemz = item;

		if (item == null)
			return;

		uuid = item.uuid;

		icon = item.icon;
		repColor = item.repColor;
		entryName = item.entryName;
		
		UpdateBonus();

		currentCharges = (charges == -1) ? maxCharge : charges;
		range.min = item.range.min;
		range.max = item.range.max;

		skillReq = item.skillReq;
		boost = new Boost();
		boost.AddBoost(item.boost);

		itemCategory = item.itemCategory;
		weaponType = item.weaponType;
		attackType = item.attackType;
		advantageType = new List<MovementType>();
		for (int i = 0; i < item.advantageType.Count; i++) {
			advantageType.Add(item.advantageType[i]);
		}
	}

	public void UpdateUpgrades(UpgradeCalculator calculator) {
		bonusPower = 0;
		bonusHit = 0;
		bonusCrit = 0;
		bonusCharges = 0;
		bonusCost = 0;
		if (itemz == null)
			return;

		List<UpgradeItem> list = calculator.GetItemUpgradeList(itemz.uuid);
		for (int i = 0; i < list.Count; i++) {
			if (!list[i].researched)
				continue;
			bonusPower += list[i].upgrade.power;
			bonusHit += list[i].upgrade.hit;
			bonusCrit += list[i].upgrade.crit;
			bonusCharges += list[i].upgrade.charges;
			bonusCost += list[i].upgrade.costValue;
		}
		UpdateBonus();
	}

	private void UpdateBonus() {
		cost = itemz.cost + bonusCost;
		power = itemz.power + bonusPower;
		hitRate = itemz.hitRate + bonusHit;
		critRate = itemz.critRate + bonusCrit;
		maxCharge = itemz.maxCharge + bonusCharges;
	}

	public InventoryItem StoreData() {
		return new InventoryItem(itemz, currentCharges);
	}

	public string Description() {
		return itemz.description;
	}

	public int GetMissingCharges() {
		return maxCharge - currentCharges;
	}

	public float ChargeCost() {
		return (maxCharge == 0) ? 0 : cost / (float)maxCharge;
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
		if (itemCategory != ItemCategory.WEAPON || skill == 0) {
			return false;
		}

		return (skill >= skillReq);
	}
}