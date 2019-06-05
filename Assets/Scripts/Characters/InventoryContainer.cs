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


	public InventoryContainer(ScrObjLibraryVariable iLib, CharacterSaveData saveData) {
		SetupValues(iLib, saveData);
	}

	public InventoryContainer(WeaponRank[] ranks, List<WeaponTuple> presetInventory) {
		wpnSkills = ranks;
		inventory = new List<InventoryTuple>();
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (i < presetInventory.Count && presetInventory[i].item != null) {
				inventory.Add(new InventoryTuple {
					index = i,
					item = presetInventory[i].item,
					charge = presetInventory[i].item.maxCharge,
					droppable = presetInventory[i].droppable
				});
			}
			else {
				inventory.Add(new InventoryTuple {
					index = i,
					charge = 0,
					droppable = false
				});
			}
		}
	}

	private void SetupValues(ScrObjLibraryVariable iLib, CharacterSaveData saveData) {
		wpnSkills = new WeaponRank[WPN_SKILLS];
		for (int i = 0; i < saveData.wpnSkills.Length; i++) {
			wpnSkills[i] = (WeaponRank)saveData.wpnSkills[i];
		}

		inventory = new List<InventoryTuple>();
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (i < saveData.inventory.Count) {
				inventory.Add(new InventoryTuple {
					index = i,
					item = (ItemEntry)iLib.GetEntry(saveData.inventory[i]),
					charge = saveData.invCharges[i]
				});
			}
			else {
				inventory.Add(new InventoryTuple {
					index = i,
					charge = 0
				});
			}
		}
	}

	public bool HasRoom() {
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (!inventory[i].item)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Returns the current weapon skill level for the weapon.
	/// </summary>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public virtual WeaponRank GetWpnSkill(ItemEntry weapon) {
		if (weapon == null || weapon.itemCategory == ItemCategory.CONSUME)
			return WeaponRank.NONE;
		return wpnSkills[(int)weapon.weaponType];
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
			close = Mathf.Min(close, useables[i].item.range.min);
			far = Mathf.Max(far, useables[i].item.range.max);
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
			if (inventory[i].item == null || inventory[i].item.itemCategory != category)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i].item);
			if (inventory[i].item.CanUse(skill) && inventory[i].charge > 0)
				return inventory[i];
		}
		return new InventoryTuple();
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the weapon type.
	/// Returns an empty tuple if there is no item that can be used.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public InventoryTuple GetFirstUsableItemTuple(WeaponType type) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null || inventory[i].item.weaponType != type)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i].item);
			if (inventory[i].item.CanUse(skill) && inventory[i].charge > 0)
				return inventory[i];
		}
		return new InventoryTuple();
	}

	/// <summary>
	/// Returns the first empty InventoryTuple in the inventory.
	/// Returns null if the inventory is already full.
	/// </summary>
	/// <returns></returns>
	public InventoryTuple GetFirstEmptyItemTuple() {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null) {
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
			if (inventory[i].item == null)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanEquip(skill) && inventory[i].item.InRange(range)) {
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
			if (inventory[i].item == null)
				continue;
			WeaponRank skill = GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill))
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
			if (inventory[i].item == null)
				continue;
			if (inventory[i].item.itemCategory == category)
				inventory[i].charge--;
			return;
		}
	}

	/// <summary>
	/// Removes all broken and dropped weapons without any charges.
	/// Also moves the items upward to fill out the gaps in the inventory.
	/// </summary>
	public void CleanupInventory() {
		int pos = 0;

		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (inventory[pos].item == null) {
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

		if (inventory[0].item != null) {
			WeaponRank skill = GetWpnSkill(inventory[0].item);
			if (!inventory[0].item.CanEquip(skill) || inventory[0].charge <= 0) {
				InventoryTuple tup = GetFirstUsableItemTuple(ItemCategory.WEAPON);
				if (tup.item != null) {
					inventory.RemoveAt(tup.index);
					inventory.Insert(0, tup);
				}
				else if (!inventory[0].item.CanEquip(skill)) {
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
	/// Returns the inventory tuple for the given index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public InventoryTuple GetTuple(int index) {
		return inventory[index];
	}

	/// <summary>
	/// Equips the item at the given index and moves it to the top.
	/// </summary>
	/// <param name="index"></param>
	public void EquipItem(int index) {
		if (index == 0 || inventory[index].item == null)
			return;

		InventoryTuple equip = inventory[index];
		inventory.RemoveAt(index);
		inventory.Insert(0, equip);
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
		if (useItem.item.itemCategory == ItemCategory.CONSUME) {
			if (useItem.item.weaponType == WeaponType.C_HEAL) {
				player.TakeHeals(useItem.item.power);
			}
			else {
				Boost boost = useItem.item.boost;
				player.stats.BoostBaseStats(boost);
			}
		}
		else {
			Debug.LogWarning("WTF!?");
			return;
		}

		useItem.charge--;
		if (useItem.charge <= 0) {
			inventory[index].item = null;
			CleanupInventory();
		}

		player.End();
	}

	/// <summary>
	/// Drops the item at the given index.
	/// </summary>
	/// <param name="index"></param>
	public void DropItem(int index, StatsContainer stats) {
		inventory[index].item = null;
		inventory[index].charge = 0;
		CleanupInventory();
	}

	/// <summary>
	/// Adds the InventoryItem to the first available slot in the inventory.
	/// Returns true if successful, false if there was no room.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool AddItem(InventoryItem item) {
		return AddItem(new InventoryTuple() {
			item = item.item,
			charge = item.charges
		});
	}

	/// <summary>
	/// Adds the selected inventory tuple to the inventory if there's room.
	/// Returns true if successful, false if there was no room.
	/// </summary>
	/// <param name="pickup"></param>
	/// <returns></returns>
	public bool AddItem(InventoryTuple pickup) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null) {
				inventory[i] = pickup;
				pickup.index = i;
				Debug.Log("Added the item to position " + i);
				return true;
			}
		}
		return false;
	}
}
