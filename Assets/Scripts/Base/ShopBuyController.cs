using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyController : MonoBehaviour {
    
	public SaveListVariable playerData;
	public IntVariable totalMoney;
	public FloatVariable sellRatio;

	[Header("Entry List")]
    public Transform listParent;
	public Transform entryPrefab;
    public MyButtonList categories;
	public int visibleSize;
	private bool buyMode;
	private ItemListVariable shopList;
	private EntryList<ItemListEntry> entryList;

	[Header("Information box")]
	public Text TotalMoneyText;
	public Text itemName;
	public Image itemIcon;

	public Text pwrText;
	public Text rangeText;
	public Text hitText;
	public Text critText;
	public Text reqText;
	public Text weightText;

	[Header("Buy/Sell items")]
	public MyPrompt buyPrompt;
	private bool promptMode;


	private void Start() {
		categories.ResetButtons();
		categories.AddButton(ItemType.SWORD.ToString());
		categories.AddButton(ItemType.LANCE.ToString());
		categories.AddButton(ItemType.AXE.ToString());
		categories.AddButton(ItemType.MAGIC.ToString());
		categories.AddButton(ItemType.THROW.ToString());
		categories.AddButton(ItemType.BOW.ToString());
		categories.AddButton(ItemType.HEAL.ToString());
		categories.AddButton(ItemType.CHEAL.ToString());
		entryList = new EntryList<ItemListEntry>(visibleSize);
	}

	public void GenerateLists(ItemListVariable currentShopList, bool buying) {
		buyMode = buying;
		shopList = currentShopList;
		GenerateList();
        categories.ForcePosition(0);
		ChangeCategory(0);
		entryList.ForcePosition(0);
	}

    private void GenerateList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;

        entryList.ResetList();
        int listSize = (buyMode) ? shopList.items.Count : playerData.items.Count;
        for (int i = 0; i < listSize; i++) {
			ItemEntry item = (buyMode) ? shopList.items[i] : playerData.items[i].item;

			if (buyMode && item.researchNeeded && !playerData.upgrader.IsResearched(item.uuid))
				continue;

			int charges = (buyMode) ? shopList.items[i].maxCharge : playerData.items[i].charges;
            CreateListEntry(i, item, charges);
        }
        entryPrefab.gameObject.SetActive(false);
    }

	private void CreateListEntry(int index, ItemEntry item, int charges) {
		Transform t = Instantiate(entryPrefab, listParent);
		ItemListEntry entry = entryList.CreateEntry(t);
		entry.FillData(index, item, charges, totalMoney.value, buyMode, sellRatio.value);
	}

	public void MoveSelection(int dir) {
		if (promptMode)
			return;

		entryList.Move(dir);
        SetupItemInfo();
	}

    public void ChangeCategory(int dir) {
        if (!promptMode) {
            categories.Move(dir);
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
			MoveSelection(0);
        }
		else {
			buyPrompt.Move(dir);
		}
    }

	public void SelectItem(bool isBuy) {
		ItemListEntry itemEntry = entryList.GetEntry();
		if (!itemEntry || !itemEntry.affordable)
			return;

		if (!promptMode) {
			promptMode = true;
			buyPrompt.ShowWindow((isBuy) ? "Buy item?" : "Sell item?", true);
		}
		else {
			if (buyPrompt.Click(true)) {
				Debug.Log((isBuy) ? "Buy item" : "Sell item");
				if (isBuy) { // Buy item
					ItemEntry item = ScriptableObject.CreateInstance<ItemEntry>();
					item.CopyValues(itemEntry.item);
					totalMoney.value -= item.cost;
					playerData.items.Add(new InventoryItem { item = item, charges = item.maxCharge });
				}
				else { // Sell item
					ItemEntry item = itemEntry.item;
					totalMoney.value += (int)(item.cost * sellRatio.value);
					playerData.items.RemoveAt(itemEntry.index);
					entryList.RemoveEntry();
				}
			}
			GenerateList();
			DeselectItem();
		}
	}

	public bool DeselectItem() {
		if (promptMode) {
			promptMode = false;
			buyPrompt.Click(false);
			return false;
		}

		return true;
	}

	private void SetupItemInfo() {
		ItemListEntry itemEntry = entryList.GetEntry();
		if (!itemEntry) {
			itemName.text = "";
			itemIcon.sprite = null;
			itemIcon.color = new Color(0,0,0,0);

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			weightText.text = "Weight:  ";
			return;
		}

		ItemEntry item = itemEntry.item;
		itemName.text = item.entryName;
		itemIcon.sprite = item.icon;
		itemIcon.color = item.repColor;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hitRate.ToString();
		critText.text = "Crit:  " + item.critRate.ToString();
		reqText.text = "Req:  " + item.skillReq.ToString();
		weightText.text = "Weight:  " + item.weight.ToString();
	}

}
