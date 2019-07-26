using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageList : MonoBehaviour {

	[Header("Data")]
	public PlayerData playerData;
	public IntVariable totalMoney;
	public FloatVariable sellRatio;
	private ItemListVariable shopList;
	public IconLibrary weaponIcons;

	[Header("Header")]
	public Text costHeader;

	[Header("Stock List")]
	public Transform entryListParent;
	public Transform entryPrefab;
	public int visibleSize;
	public MyButtonList categories;
	private EntryList<ItemListEntry> entryList;
	public bool buyMode;


	public void Setup() {
		entryList = new EntryList<ItemListEntry>(visibleSize);

		categories.ResetButtons();

		categories.AddButton(weaponIcons.icons[(int)WeaponType.RIFLE], (int)WeaponType.RIFLE);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.SHOTGUN], (int)WeaponType.SHOTGUN);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.ROCKET], (int)WeaponType.ROCKET);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.PISTOL], (int)WeaponType.PISTOL);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.MEDKIT], (int)WeaponType.MEDKIT);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.PSI_BLADE], (int)WeaponType.PSI_BLADE);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.PSI_BLAST], (int)WeaponType.PSI_BLAST);
		categories.AddButton(weaponIcons.icons[(int)WeaponType.C_HEAL], (int)WeaponType.C_HEAL);
		categories.ForcePosition(0);
	}

	public void SetupBuy(ItemListVariable currentShopList) {
		shopList = currentShopList;
		buyMode = true;
		costHeader.text = "Cost";
		GenerateShopList();
	}

	public void SetupSell() {
		buyMode = false;
		costHeader.text = "Value";
		GenerateShopList();
	}

	public void SetupStorage() {
		buyMode = false;
		costHeader.text = "";
		GenerateStorageList();
	}

	private void GenerateShopList() {
		entryList.ResetList();
		int listSize = (buyMode) ? shopList.items.Count : playerData.items.Count;
		for (int i = 0; i < listSize; i++) {
			ItemEntry item = (buyMode) ? shopList.items[i] : playerData.items[i].item;

			if (buyMode && item.researchNeeded && !playerData.upgrader.IsResearched(item.uuid))
				continue;

			Transform t = Instantiate(entryPrefab, entryListParent);
			ItemListEntry entry = entryList.CreateEntry(t);
			InventoryTuple tup = new InventoryTuple(item);
			tup.UpdateUpgrades(playerData.upgrader);
			int charges = tup.maxCharge;
			entry.FillData(i, tup, charges.ToString(), totalMoney.value, buyMode, sellRatio.value);
		}
		entryPrefab.gameObject.SetActive(false);
		ForceCategory(0);
	}

	private void GenerateStorageList() {
		entryList.ResetList();
		int listSize = playerData.items.Count;
		for (int i = 0; i < listSize; i++) {
			ItemEntry item = playerData.items[i].item;
			int charges = playerData.items[i].charges;
			Transform t = Instantiate(entryPrefab, entryListParent);
			ItemListEntry entry = entryList.CreateEntry(t);
			InventoryTuple tup = new InventoryTuple(item);
			tup.UpdateUpgrades(playerData.upgrader);
			entry.FillDataSimple(i, tup, charges.ToString(), "");
		}
		entryPrefab.gameObject.SetActive(false);
		ForceCategory(0);
	}

	public void Move(int dir) {
		entryList.Move(dir);
	}

	public void ForceCategory(int index) {
		categories.ForcePosition(index);
		UpdateFilter();
	}

	public void ChangeCategory(int dir) {
		categories.Move(dir);
		UpdateFilter();
	}

	private void UpdateFilter() {
		WeaponType currentCategory = (WeaponType)categories.GetValue();
		if (currentCategory == WeaponType.C_HEAL) {
			entryList.FilterShow(x => { return x.tuple.itemCategory == ItemCategory.CONSUME; });
		}
		else {
			entryList.FilterShow(x => { return x.tuple.weaponType == currentCategory; });
		}
		UpdateCost();
		entryList.ForcePosition(0);
	}

	public void UpdateCost() {
		if (!buyMode)
			return;

		for (int i = 0; i < entryList.Size; i++) {
			ItemListEntry item = entryList.GetEntry(i);
			item.SetAffordable(item.tuple.cost <= totalMoney.value);
		}
	}

	public ItemListEntry GetEntry() {
		return entryList.GetEntry();
	}

	public ItemListEntry GetEntry(int index) {
		return entryList.GetEntry(index);
	}

	public void RemoveEntry() {
		entryList.RemoveEntry();
		for (int i = 0; i < entryList.Size; i++) {
			entryList.GetEntry(i).index = i;
		}
	}
}
