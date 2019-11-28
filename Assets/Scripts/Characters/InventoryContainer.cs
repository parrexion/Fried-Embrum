using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { NONE = -1, SHOTGUN, SNIPER, RIFLE, BAZOOKA, MACHINEGUN, PISTOL, ROCKET, PSI_BLAST, PSI_BLADE, MEDKIT, BARRIER, DEBUFF, C_HEAL, C_BOOST, CLAW }
public enum WeaponRank { NONE = 0, C = 1, B = 2, A = 3, S = 4 }

[System.Serializable]
public class InventoryContainer {

	public const int INVENTORY_SIZE = 5;
	public const int WPN_SKILLS = 15;

	public WeaponRank[] wpnSkills = new WeaponRank[WPN_SKILLS];
	[SerializeField] private List<InventoryTuple> inventory = new List<InventoryTuple>();


	public InventoryContainer(ScrObjLibraryVariable iLib, CharacterSaveData saveData, UpgradeCalculator calculator) {
		wpnSkills = new WeaponRank[WPN_SKILLS];
		for (int i = 0; i < saveData.wpnSkills.Length; i++) {
			wpnSkills[i] = (WeaponRank)saveData.wpnSkills[i];
		}
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (i >= saveData.inventory.Count) {
				inventory.Add(new InventoryTuple(null, i));
			}
			else {
				ItemEntry item = (ItemEntry)iLib.GetEntry(saveData.inventory[i]);
				inventory.Add(new InventoryTuple(item, i) {
					currentCharges = saveData.invCharges[i]
				});
				inventory[i].UpdateUpgrades(calculator);
			}
		}
	}

	public InventoryContainer(WeaponRank[] ranks, List<WeaponTuple> presetInventory) {
		wpnSkills = ranks;
		inventory = new List<InventoryTuple>();
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (i < presetInventory.Count && presetInventory[i].item != null) {
				inventory.Add(new InventoryTuple(presetInventory[i].item, i) { droppable = presetInventory[i].droppable });
			}
			else {
				inventory.Add(new InventoryTuple(null, i));
			}
		}
	}

	/// <summary>
	/// Refreshes the bonus values for all the items in the inventory.
	/// </summary>
	/// <param name="calculator"></param>
	public void RefreshUpgrades(UpgradeCalculator calculator) {
		for (int i = 0; i < inventory.Count; i++) {
			inventory[i].UpdateUpgrades(calculator);
		}
	}

	public InventoryTuple GetItem(int index) {
		return inventory[index];
	}

	/// <summary>
	/// Returns the current weapon skill level for the weapon.
	/// </summary>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public virtual WeaponRank GetWpnSkill(InventoryTuple weapon) {
		if (weapon == null || weapon.itemCategory == ItemCategory.CONSUME)
			return WeaponRank.NONE;
		return wpnSkills[(int)weapon.weaponType];
	}

	/// <summary>
	/// Checks if the character can equip the target item.
	/// </summary>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public bool CanEquip(InventoryTuple weapon) {
		WeaponRank rank = GetWpnSkill(weapon);
		return (rank != WeaponRank.NONE && weapon.skillReq <= rank);
	}

	/// <summary>
	/// Increases the weapon rank of all the weapon types in the list.
	/// </summary>
	/// <param name="weapons"></param>
	public void IncreaseWpnSkill(List<WeaponType> weapons) {
		for (int i = 0; i < weapons.Count; i++) {
			wpnSkills[(int)weapons[i]]++;
		}
	}

	/// <summary>
	/// Takes all items of the given category and returns the available ranges for them.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public WeaponRange GetReach(ItemCategory category) {
		int close = 99, far = 0;
		List<InventoryTuple> useables = GetAllUsableItemTuple(category);
		for (int i = 0; i < useables.Count; i++) {
			close = Mathf.Min(close, useables[i].range.min);
			far = Mathf.Max(far, useables[i].range.max);
		}

		return new WeaponRange(close, far);
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the item category.
	/// Returns an empty tuple if there is no item that can be used.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public InventoryTuple GetFirstUsableItemTuple(ItemCategory category) {
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid) || inventory[i].itemCategory != category)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i]);
			if (inventory[i].CanUse(skill) && inventory[i].currentCharges > 0)
				return inventory[i];
		}
		return new InventoryTuple(null);
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the weapon type.
	/// Returns an empty tuple if there is no item that can be used.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public InventoryTuple GetFirstUsableItemTuple(WeaponType type) {
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid) || inventory[i].weaponType != type)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i]);
			if (inventory[i].CanUse(skill) && inventory[i].currentCharges > 0)
				return inventory[i];
		}
		return new InventoryTuple(null);
	}

	/// <summary>
	/// Returns the first empty InventoryTuple in the inventory.
	/// Returns null if the inventory is already full.
	/// </summary>
	/// <returns></returns>
	public InventoryTuple GetFirstEmptyItemTuple() {
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid)) {
				return inventory[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the item category and have
	/// enough range to be used.
	/// </summary>
	/// <param name="category"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	public void EquipFirstInRangeItem(ItemCategory category, int range) {
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid))
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i]);
			if (inventory[i].itemCategory == category && inventory[i].CanEquip(skill) && inventory[i].InRange(range)) {
				EquipItem(i);
				return;
			}
		}
	}

	/// <summary>
	/// Returns a list of all usable items for the given category and the player's skills.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public List<InventoryTuple> GetAllUsableItemTuple(ItemCategory category) {
		List<InventoryTuple> list = new List<InventoryTuple>();
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid))
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i]);
			if (inventory[i].itemCategory == category && inventory[i].CanUse(skill) && inventory[i].currentCharges > 0)
				list.Add(inventory[i]);
		}
		return list;
	}

	/// <summary>
	/// Reduces the weapon charges for the first item for the category.
	/// </summary>
	/// <param name="category"></param>
	public void ReduceItemCharge(ItemCategory category) {
		for (int i = 0; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid))
				continue;
			if (inventory[i].itemCategory == category) {
				inventory[i].currentCharges--;
				return;
			}
		}
	}

	/// <summary>
	/// Removes all broken and dropped weapons without any charges.
	/// Also moves the items upward to fill out the gaps in the inventory.
	/// </summary>
	public void CleanupInventory() {
		int pos = 0;

		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (string.IsNullOrEmpty(inventory[pos].uuid)) {
				InventoryTuple tup = inventory[pos];
				inventory.RemoveAt(pos);
				inventory.Add(tup);
			}
			else {
				pos++;
			}
		}
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			inventory[i].index = i;
		}

		if (!string.IsNullOrEmpty(inventory[0].uuid)) {
			WeaponRank skill = GetWpnSkill(inventory[0]);
			if (!inventory[0].CanEquip(skill) || inventory[0].currentCharges <= 0) {
				InventoryTuple tup = GetFirstUsableItemTuple(ItemCategory.WEAPON);
				if (!string.IsNullOrEmpty(tup.uuid)) {
					inventory.RemoveAt(tup.index);
					inventory.Insert(0, tup);
				}
				else if (!inventory[0].CanEquip(skill)) {
					tup = GetFirstEmptyItemTuple();
					if (tup != null) {
						inventory.RemoveAt(tup.index);
						inventory.Insert(0, tup);
					}
				}
			}
		}
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			inventory[i].index = i;
		}
	}

	/// <summary>
	/// Sets the value of the given inventory tuple.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public void SetTuple(int index, InventoryTuple tuple) {
		inventory[index] = tuple;
	}

	/// <summary>
	/// Returns the inventory tuple for the given index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public InventoryTuple GetTuple(int index) {
		return inventory[index];
	}

	public bool HasKey(KeyType keyType) {
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (inventory[i].itemCategory == ItemCategory.CONSUME && inventory[i].attackType == AttackType.KEY && inventory[i].keyType == keyType)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Equips the item at the given index and moves it to the top.
	/// </summary>
	/// <param name="index"></param>
	public void EquipItem(int index) {
		if (index == 0 || string.IsNullOrEmpty(inventory[index].uuid))
			return;

		InventoryTuple equip = inventory[index];
		inventory.RemoveAt(index);
		int startIndex = (equip.itemCategory == ItemCategory.WEAPON) ? 0 : 1;
		inventory.Insert(startIndex, equip);
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			inventory[i].index = i;
		}
		Debug.Log("Equipped item index " + index);
	}

	/// <summary>
	/// Uses the item at the given index and removes it if there are no more charges left.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="player"></param>
	public void UseItem(int index, TacticsMove player) {
		InventoryTuple useItem = inventory[index];
		if (useItem.itemCategory == ItemCategory.CONSUME) {
			if (useItem.weaponType == WeaponType.C_HEAL) {
				player.TakeHeals(useItem.power);
			}
			else if(useItem.attackType == AttackType.KEY) {
				//Door is done in another place
			}
			else {
				Boost boost = useItem.boost;
				player.stats.BoostBaseStats(boost);
			}
		}
		else {
			Debug.LogWarning("WTF!?");
			return;
		}

		useItem.currentCharges--;
		if (useItem.currentCharges <= 0) {
			inventory[index] = new InventoryTuple(null);
			CleanupInventory();
		}

		player.End();
	}

	/// <summary>
	/// Drops the item at the given index.
	/// </summary>
	/// <param name="index"></param>
	public void DropItem(int index) {
		inventory[index] = new InventoryTuple(null);
		CleanupInventory();
	}

	/// <summary>
	/// Adds the InventoryItem to the first available slot in the inventory.
	/// Returns true if successful, false if there was no room.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool AddItem(InventoryItem item) {
		return AddItem(new InventoryTuple(item.item, -1, item.charges));
	}

	/// <summary>
	/// Adds the selected inventory tuple to the inventory if there's room.
	/// Returns true if successful, false if there was no room.
	/// </summary>
	/// <param name="pickup"></param>
	/// <returns></returns>
	public bool AddItem(InventoryTuple pickup) {
		if (string.IsNullOrEmpty(inventory[0].uuid)) {
			WeaponRank rank = GetWpnSkill(pickup);
			if (rank != WeaponRank.NONE && rank >= pickup.skillReq) {
				inventory[0] = pickup;
				Debug.Log("Equipped  " + rank);
				return true;
			}
		}
		for (int i = 1; i < inventory.Count; i++) {
			if (string.IsNullOrEmpty(inventory[i].uuid)) {
				inventory[i] = pickup;
				pickup.index = i;
				Debug.Log("Added the item to position " + i);
				return true;
			}
		}
		return false;
	}



	public static string GetWeaponTypeName(WeaponType type) {
		switch (type) {
			case WeaponType.C_HEAL:
				return "Consumable";
			case WeaponType.C_BOOST:
				return "Statbooster";
			default:
				return type.ToString();
		}
	}
}
