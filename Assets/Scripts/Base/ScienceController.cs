using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceController : MonoBehaviour {

	[Header("Data")]
	public PlayerData playerData;
	public IntVariable totalScrap;
	public IntVariable totalMoney;

	[Header("Entry List")]
	public Transform listParent;
	public Transform entryPrefab;
	public EntryList<UpgradeListEntry> entryList;
	public int visibleSize;
	private bool upgradeMode;

	[Header("Upgrade Box")]
	public Text TotalScrapText;
	public Text TotalMoneyText;
	public Image itemIcon;

	[Header("Information box")]
	public Text upgradeName;
	public Text relatedItem;
	public Text costMoney;
	public Text costScrap;
	public Text level;

	[Header("Upgrade info")]
	public GameObject upgradeInfoObject;
	public Text pwrText;
	public Text rangeText;
	public Text hitText;

	[Header("Invention info")]
	public GameObject inventionInfoObject;
	public Text invPwrText;
	public Text invRangeText;
	public Text invHitText;
	public Text invCritText;
	public Text invReqText;

	[Header("Buy upgrade promt")]
	public MyPrompt buyPrompt;
	private bool promptMode;
	

	public void GenerateLists(bool upgrading) {
		upgradeMode = upgrading;
		GenerateList();
		MoveSelection(0);
	}

    private void GenerateList() {
		TotalScrapText.text = "Scraps:  " + totalScrap.value;
		TotalMoneyText.text = "Money:  " + totalMoney.value;

		if (entryList == null)
			entryList = new EntryList<UpgradeListEntry>(visibleSize);
        entryList.ResetList();
        int tempListSize = playerData.upgrader.listSize;
		UpgradeType currentType = (upgradeMode) ? UpgradeType.UPGRADE : UpgradeType.INVENTION;
        for (int i = 0; i < tempListSize; i++) {
			if (playerData.upgrader.upgrades[i].upgrade.type == currentType) {
				Transform t = Instantiate(entryPrefab, listParent);
				UpgradeListEntry ue = entryList.CreateEntry(t);
				ue.FillData(i, playerData.upgrader.upgrades[i].upgrade, playerData.upgrader.upgrades[i].researched, totalScrap.value, totalMoney.value);
            }
        }
		entryPrefab.gameObject.SetActive(false);
		upgradeInfoObject.SetActive(upgradeMode);
		inventionInfoObject.SetActive(!upgradeMode);
		UpdateListDarkness();
	}

	public void MoveSelection(int dir) {
		if (promptMode)
			return;

		entryList.Move(dir);

		if (upgradeMode)
			SetupUpgradeInfo();
		else
			SetupDevelopInfo();
	}

	public void MovePromt(int dir) {
		if (!promptMode)
			return;

		buyPrompt.Move(dir);
	}

	public void SelectItem(bool isUpgrade) {
		UpgradeListEntry upgrade = entryList.GetEntry();
		if (!upgrade || !upgrade.affordable) {
			return;
		}
		else if (!promptMode) {
			if (upgrade.affordable) {
				promptMode = true;
				buyPrompt.ShowYesNoPopup((isUpgrade) ? "Buy upgrade?" : "Develop item?", true);
			}
		}
		else {
			if (buyPrompt.Click(true) == MyPrompt.Result.OK1) {
				Debug.Log((isUpgrade) ? "Upgrade" : "Invent");
				totalMoney.value -= upgrade.upgrade.cost;
				totalScrap.value -= upgrade.upgrade.scrap;
				playerData.upgrader.upgrades[upgrade.index].researched = true;
				playerData.upgrader.CalculateResearch();
				UpdateListDarkness();
			}
			GenerateList();
			DeselectItem();
			MoveSelection(0);
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

	private void UpdateListDarkness() {
		entryList.FilterDark((e) => { return (e.done || e.upgrade.scrap > totalScrap.value || e.upgrade.cost > totalMoney.value); });
	}

	private void SetupUpgradeInfo() {
		if (entryList.GetEntry(0) == null) {
			upgradeName.text = "";
			itemIcon.sprite = null;

			costMoney.text = "Cost:";
			costScrap.text = "Scrap:";
			level.text = "Level:";
			relatedItem.text = "Item:";

			pwrText.gameObject.SetActive(false);
			rangeText.gameObject.SetActive(false);
			hitText.gameObject.SetActive(false);
			return;
		}

		UpgradeEntry upgrade = entryList.GetEntry().upgrade;
		upgradeName.text = upgrade.entryName;
		relatedItem.text = "Item:  " + upgrade.item.entryName;
		itemIcon.sprite = upgrade.item.icon;
		itemIcon.color = upgrade.repColor;

		if (entryList.GetEntry().done) {
			costMoney.text = "";
			costScrap.text = "";
			level.text = "Researched!";
		}
		else {
			costMoney.text = "Cost:  " + upgrade.cost;
			costScrap.text = "Scrap:  " + upgrade.scrap;
			level.text = "Level:  " + upgrade.level;
		}

		rangeText.text = upgrade.minRange + " - " + upgrade.maxRange + " Range";
		rangeText.gameObject.SetActive(upgrade.minRange != 0 && upgrade.maxRange != 0);
		pwrText.text = "Power:  " + upgrade.item.power + "  ->  " + (upgrade.item.power + upgrade.power);
		pwrText.gameObject.SetActive(upgrade.power != 0);
		hitText.text = "Hit Rate:  " + upgrade.item.hitRate + "  ->  " + (upgrade.item.hitRate + upgrade.hit);
		hitText.gameObject.SetActive(upgrade.hit != 0);
	}

	private void SetupDevelopInfo() {
		if (entryList.GetEntry() == null) {
			upgradeName.text = "";
			itemIcon.sprite = null;

			costMoney.text = "Cost:";
			costScrap.text = "Scrap:";
			level.text = "Level:";
			relatedItem.text = "Item:";

			invPwrText.text = "";
			invRangeText.text = "";
			invHitText.text = "";
			invCritText.text = "";
			invReqText.text = "";
			return;
		}

		UpgradeEntry upgrade = entryList.GetEntry().upgrade;
		upgradeName.text = upgrade.entryName;
		relatedItem.text = "Item:  " + upgrade.item.entryName;
		itemIcon.sprite = upgrade.item.icon;
		itemIcon.color = upgrade.repColor;

		if (entryList.GetEntry().done) {
			costMoney.text = "";
			costScrap.text = "";
			level.text = "Researched!";
		}
		else {
			costMoney.text = "Cost:  " + upgrade.cost;
			costScrap.text = "Scrap:  " + upgrade.scrap;
			level.text = "Level:  " + upgrade.level;
		}

		invPwrText.text = "Power:  " + upgrade.item.power;
		invRangeText.text = "Range:  " + upgrade.item.range.ToString();
		invHitText.text = "Hit Rate:  " + upgrade.item.hitRate;
		invCritText.text = "Crit Rate:  " + upgrade.item.critRate;
		invReqText.text = "Req:  " + upgrade.item.skillReq.ToString();
		return;
	}
}
