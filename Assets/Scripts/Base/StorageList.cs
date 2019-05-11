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
		//categories.AddButton(ItemType.SWORD.ToString());
		//categories.AddButton(ItemType.LANCE.ToString());
		//categories.AddButton(ItemType.AXE.ToString());
		//categories.AddButton(ItemType.MAGIC.ToString());
		//categories.AddButton(ItemType.THROW.ToString());
		//categories.AddButton(ItemType.BOW.ToString());
		//categories.AddButton(ItemType.HEAL.ToString());
		//categories.AddButton(ItemType.CHEAL.ToString());
		for (int i = 1; i < weaponIcons.icons.Length; i++) {
			categories.AddButton(weaponIcons.icons[i]);
		}
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

			int charges = (buyMode) ? shopList.items[i].maxCharge : playerData.items[i].charges;
            Transform t = Instantiate(entryPrefab, entryListParent);
			ItemListEntry entry = entryList.CreateEntry(t);
			entry.FillData(i, item, charges.ToString(), totalMoney.value, buyMode, sellRatio.value);
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
			entry.FillDataSimple(i, item, charges.ToString(), "");
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
		ItemType currentCategory = (ItemType)(categories.GetPosition()+1);
		if (currentCategory == ItemType.CHEAL) {
			entryList.FilterShow(x => { return x.item.itemType == currentCategory || x.item.itemType == ItemType.CSTATS; });
		}
		else if (currentCategory == ItemType.HEAL) {
			entryList.FilterShow(x => { return x.item.itemType == currentCategory || x.item.itemType == ItemType.BUFF; });
		}
		else {
			entryList.FilterShow(x => { return x.item.itemType == currentCategory; });
		}
		entryList.ForcePosition(0);
	}

	public void UpdateCost() {
		for (int i = 0; i < entryList.Size; i++) {
			ItemListEntry item = entryList.GetEntry(i);
			item.SetAffordable(item.item.cost <= totalMoney.value);
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
	}
}
