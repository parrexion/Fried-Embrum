using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryContainer {

	public const int INVENTORY_SIZE = 5;
	
	public InventoryTuple[] inventory;

	public InventoryContainer(ItemLibrary iLib, CharacterSaveData saveData) {
		SetupValues(iLib, saveData);
	}

	private void SetupValues(ItemLibrary iLib, CharacterSaveData saveData) {
		
		inventory = new InventoryTuple[INVENTORY_SIZE];
		for (int i = 0; i < inventory.Length; i++) {
			if (i < saveData.inventory.Count) {
				inventory[i] = new InventoryTuple {
					index = i,
					item = (WeaponItem) iLib.GetEntry(saveData.inventory[i]),
					charge = saveData.invCharges[i]
				};
			}
			else {
				inventory[i] = new InventoryTuple {
					index = i,
					charge = 0
				};
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
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null || inventory[i].item.itemCategory != category)
				continue;
			close = Mathf.Min(close, inventory[i].item.range.min);
			far = Mathf.Max(far, inventory[i].item.range.max);
		}

		return new WeaponRange(close,far);
	}

	/// <summary>
	/// Returns the first item in the inventory the player can use matching the item category.
	/// </summary>
	/// <param name="category"></param>
	/// <param name="player"></param>
	/// <returns></returns>
	public WeaponItem GetFirstUsableItem(ItemCategory category, StatsContainer player) {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill))
				return inventory[i].item;
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
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill) && inventory[i].item.InRange(range)) {
				EquipItem(i);
				return;
			}
		}
	}

	public InventoryTuple GetUsableItemTuple(ItemCategory category, StatsContainer player) {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill))
				return inventory[i];
		}
		return null;
	}

	public List<InventoryTuple> GetAllUsableItemTuple(ItemCategory category, StatsContainer player) {
		List<InventoryTuple> list = new List<InventoryTuple>();
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null)
				continue;
			int skill = player.GetWpnSkill(inventory[i].item);
			if (inventory[i].item.itemCategory == category && inventory[i].item.CanUse(skill))
				list.Add(inventory[i]);
		}
		return list;
	}

	public void ReduceItemCharge(ItemCategory category) {
		for (int i = 0; i < inventory.Length; i++) {
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
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i].item == null)
				continue;
			if (inventory[i].charge <= 0) {
				inventory[i].item = null;
				Debug.Log("Weapon broke!");
			}
			else {
				InventoryTuple temp = inventory[i];
				inventory[i] = inventory[pos];
				inventory[pos] = temp;
				pos++;
			}
		}
		Debug.Log("Cleaned up inventory");
	}

	public InventoryTuple GetItem(int index) {
		return inventory[index];
	}

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
			inventory[index] = null;
			CleanupInventory();
		}
		
		player.End();
	}

	public void DropItem(int index) {
		inventory[index].charge = 0;
		CleanupInventory();
	}

	public bool GainItem(InventoryTuple pickup) {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i] == null) {
				inventory[i] = pickup;
				Debug.Log("Added the item to position " + i);
				return true;
			}
		}
		return false;
	}
}
