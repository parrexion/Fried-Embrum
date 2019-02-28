using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryContainer {

	public const int INVENTORY_SIZE = 5;
	
	[SerializeField] private List<InventoryTuple> inventory = new List<InventoryTuple>();


	public InventoryContainer(ScrObjLibraryVariable iLib, CharacterSaveData saveData) {
		SetupValues(iLib, saveData);
	}

	public InventoryContainer(List<WeaponTuple> presetInventory) {
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
		inventory = new List<InventoryTuple>();
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			if (i < saveData.inventory.Count) {
				inventory.Add(new InventoryTuple {
					index = i,
					item = (ItemEntry) iLib.GetEntry(saveData.inventory[i]),
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

	/// <summary>
	/// Takes all items of the given category and returns the available ranges for them.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public WeaponRange GetReach(ItemCategory category) {
		int close = 99, far = 0;
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null || inventory[i].item.itemCategory != category)
				continue;
			close = Mathf.Min(close, inventory[i].item.range.min);
			far = Mathf.Max(far, inventory[i].item.range.max);
		}

		return new WeaponRange(close,far);
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the item category.
	/// Returns null if there is not item that can be used.
	/// </summary>
	/// <param name="category"></param>
	/// <param name="player"></param>
	/// <returns></returns>
	public ItemEntry GetFirstUsableItem(ItemCategory category, StatsContainer player) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill))
				return inventory[i].item;
		}
		return null;
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the item type.
	/// Returns null if there is not item that can be used.
	/// </summary>
	/// <param name="category"></param>
	/// <param name="player"></param>
	/// <returns></returns>
	public ItemEntry GetFirstUsableItem(ItemType type, StatsContainer player) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemType == type && inventory[i].item.CanUse(skill))
				return inventory[i].item;
		}
		return null;
	}

	public InventoryTuple GetFirstUsableItemTuple(ItemCategory category, StatsContainer player) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill) && inventory[i].charge > 0)
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
	/// <param name="player"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	public void EquipFirstInRangeItem(ItemCategory category, StatsContainer player, int range) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill) && inventory[i].item.InRange(range)) {
				EquipItem(i);
				return;
			}
		}
	}

	/// <summary>
	/// Returns a list of all usable items for the given category and the player's skills.
	/// </summary>
	/// <param name="category"></param>
	/// <param name="player"></param>
	/// <returns></returns>
	public List<InventoryTuple> GetAllUsableItemTuple(ItemCategory category, StatsContainer player) {
		List<InventoryTuple> list = new List<InventoryTuple>();
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
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
	public void CleanupInventory(StatsContainer stats) {
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

		if (inventory[0].item != null && stats != null) {
			int skill = stats.GetWpnSkill(inventory[0].item);
			if (!inventory[0].item.CanUse(skill) || inventory[0].charge <= 0) {
				InventoryTuple tup = GetFirstUsableItemTuple(ItemCategory.WEAPON, stats);
				if (tup.item != null) {
					inventory.RemoveAt(tup.index);
					inventory.Insert(0, tup);
				}
				else if (!inventory[0].item.CanUse(skill)) {
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
		if (index == 0)
			return;

		InventoryTuple equip = inventory[index];
		for (int i = 0; i < index+1; i++) {
			InventoryTuple temp = inventory[i];
			inventory[i] = equip;
			equip = temp;
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
		if (useItem.item.itemType == ItemType.CHEAL) {
			player.TakeHeals(useItem.item.power);
		}
		else if (useItem.item.itemType == ItemType.CSTATS) {
			Boost boost = useItem.item.boost;
			player.stats.BoostBaseStats(boost);
		}
		else {
			Debug.LogWarning("WTF!?");
			return;
		}

		useItem.charge--;
		if (useItem.charge <= 0) {
			inventory[index].item = null;
			CleanupInventory(player.stats);
		}
		
		player.End();
	}

	/// <summary>
	/// Drops the item at the given index.
	/// </summary>
	/// <param name="index"></param>
	public void DropItem(int index, StatsContainer stats) {
		inventory[index].charge = 0;
		CleanupInventory(stats);
	}

	/// <summary>
	/// Adds the selected inventory tuple to the inventory if there's room.
	/// Returns true if there's room, false if there's not.
	/// </summary>
	/// <param name="pickup"></param>
	/// <returns></returns>
	public bool GainItem(InventoryTuple pickup) {
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].item == null) {
				inventory[i] = pickup;
				Debug.Log("Added the item to position " + i);
				return true;
			}
		}
		return false;
	}
}
