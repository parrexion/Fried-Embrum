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
	public GameObject alreadyResearched;
	public Image[] levelStars;

	[Header("Upgrade info")]
	public GameObject upgradeInfoObject;
	public Text pwrText;
	public Text hitText;
	public Text critText;
	public Text chargesText;
	public Text costValueText;

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
		entryList.Sort(SortUpgrades);
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
				playerData.UpdateUpgrades();
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
			relatedItem.text = "Item:";
			for (int i = 0; i < levelStars.Length; i++) {
				levelStars[i].enabled = false;
			}

			pwrText.gameObject.SetActive(false);
			hitText.gameObject.SetActive(false);
			critText.gameObject.SetActive(false);
			chargesText.gameObject.SetActive(false);
			costValueText.gameObject.SetActive(false);
			return;
		}

		UpgradeEntry upgrade = entryList.GetEntry().upgrade;
		upgradeName.text = upgrade.entryName;
		InventoryTuple tuple = new InventoryTuple(upgrade.item);
		tuple.UpdateUpgrades(playerData.upgrader);
		relatedItem.text = "Item:  " + tuple.entryName;
		itemIcon.sprite = tuple.icon;
		itemIcon.color = upgrade.repColor;

		if (entryList.GetEntry().done) {
			costMoney.text = "";
			costScrap.text = "";
			alreadyResearched.SetActive(true);
			pwrText.text = "Power:  " + tuple.power;
			hitText.text = "Hit Rate:  " + tuple.hitRate;
			critText.text = "Crit Rate:  " + tuple.critRate;
			chargesText.text = "Max Charges:  " + tuple.maxCharge;
			costValueText.text = "Item Value:  " + tuple.cost;
		}
		else {
			costMoney.text = "Cost:  " + upgrade.cost;
			costScrap.text = "Scrap:  " + upgrade.scrap;
			alreadyResearched.SetActive(false);
			pwrText.text = "Power:  " + tuple.power + "  ->  " + (tuple.power + upgrade.power);
			hitText.text = "Hit Rate:  " + tuple.hitRate + "  ->  " + (tuple.hitRate + upgrade.hit);
			critText.text = "Crit Rate:  " + tuple.critRate + "  ->  " + (tuple.critRate + upgrade.crit);
			chargesText.text = "Max Charges:  " + tuple.maxCharge + "  ->  " + (tuple.maxCharge + upgrade.charges);
			costValueText.text = "Item Value:  " + tuple.cost + "  ->  " + (tuple.cost + upgrade.costValue);
		}

		for (int i = 0; i < levelStars.Length; i++) {
			levelStars[i].enabled = i < upgrade.rank;
		}

		pwrText.gameObject.SetActive(upgrade.power != 0);
		hitText.gameObject.SetActive(upgrade.hit != 0);
		critText.gameObject.SetActive(upgrade.crit != 0);
		chargesText.gameObject.SetActive(upgrade.charges != 0);
		costValueText.gameObject.SetActive(upgrade.costValue != 0);
	}

	private void SetupDevelopInfo() {
		if (entryList.GetEntry() == null) {
			upgradeName.text = "";
			itemIcon.sprite = null;

			costMoney.text = "Cost:";
			costScrap.text = "Scrap:";
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
			alreadyResearched.SetActive(true);
		}
		else {
			costMoney.text = "Cost:  " + upgrade.cost;
			costScrap.text = "Scrap:  " + upgrade.scrap;
			alreadyResearched.SetActive(false);
		}

		for (int i = 0; i < levelStars.Length; i++) {
			levelStars[i].enabled = i < upgrade.rank;
		}

		invPwrText.text = "Power:  " + upgrade.item.power;
		invRangeText.text = "Range:  " + upgrade.item.range.ToString();
		invHitText.text = "Hit Rate:  " + upgrade.item.hitRate;
		invCritText.text = "Crit Rate:  " + upgrade.item.critRate;
		invReqText.text = "Req:  " + upgrade.item.skillReq.ToString();
		return;
	}

	private int SortUpgrades(UpgradeListEntry x, UpgradeListEntry y) {
		if (x.done != y.done) {
			return (x.done) ? 1 : -1;
		}
		else {
			return y.index - x.index;
		}
	}
}
