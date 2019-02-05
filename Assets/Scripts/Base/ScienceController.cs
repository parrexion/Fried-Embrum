using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScienceController : MonoBehaviour {

	[Header("Data")]
	public SaveListVariable playerData;
	public IntVariable totalScrap;
	public IntVariable totalMoney;

	[Header("Entry List")]
    public MyButton[] listButtons;
	private bool upgradeMode;
	private int currentListIndex;
	private int listSize;
	private List<UpgradeListEntry> entryList = new List<UpgradeListEntry>();

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
	public Text weightText;

	[Header("Invention info")]
	public GameObject inventionInfoObject;
	public Text invPwrText;
	public Text invRangeText;
	public Text invHitText;
	public Text invCritText;
	public Text invWeightText;
	public Text invReqText;

	[Header("Buy upgrade promt")]
	public GameObject promptView;
	public Text promptText;
	public MyButton promptYesButton;
	public MyButton promptNoButton;
	private bool promptMode;
	private int promptPosition;


	public void GenerateLists(bool upgrading) {
		promptView.SetActive(false);
		upgradeMode = upgrading;
		GenerateList();
		currentListIndex = 0;
		MoveSelection(0);
	}

    private void GenerateList() {
		TotalScrapText.text = "Scraps:  " + totalScrap.value;
		TotalMoneyText.text = "Money:  " + totalMoney.value;

        entryList = new List<UpgradeListEntry>();
		listSize = 0;
        int tempListSize = playerData.upgrader.listSize;
		UpgradeType currentType = (upgradeMode) ? UpgradeType.UPGRADE : UpgradeType.INVENTION;
        for (int i = 0; i < tempListSize; i++) {
			if (playerData.upgrader.upgrades[i].upgrade.type == currentType) {
                CreateListEntry(i, playerData.upgrader.upgrades[i].upgrade, playerData.upgrader.upgrades[i].researched);
                listSize++;
            }
        }
		upgradeInfoObject.SetActive(upgradeMode);
		inventionInfoObject.SetActive(!upgradeMode);
	}

	private void CreateListEntry(int index, UpgradeEntry upgrade, bool done) {
		UpgradeListEntry entry = new UpgradeListEntry();
		entry.FillData(index, upgrade, done, totalScrap.value, totalMoney.value);
		if (done)
			entryList.Add(entry);
		else
			entryList.Insert(0, entry);
	}

	public void MoveSelection(int dir) {
		if (promptMode || entryList.Count == 0)
			return;

		currentListIndex = OPMath.FullLoop(0, listSize, currentListIndex + dir);
		for (int i = 0; i < listButtons.Length; i++) {
			if (entryList.Count <= i) {
				listButtons[i].buttonText.text = "DONE";
				break;
			}
			listButtons[i].buttonText.text = entryList[i].upgrade.entryName;
			listButtons[i].buttonText.fontStyle = (entryList[i].done) ? FontStyle.Italic : FontStyle.Normal;
			listButtons[i].highlight.color = (entryList[i].done) ? new Color(0.2f, 0.5f, 0.75f) : 
											 (entryList[i].affordable) ? new Color(0.3f, 0.75f, 0.55f) : Color.grey;
			listButtons[i].SetSelected(i == currentListIndex);
		}
		if (upgradeMode)
			SetupUpgradeInfo();
		else
			SetupDevelopInfo();
	}

	public void MovePromt(int dir) {
		if (!promptMode)
			return;

		promptPosition = OPMath.FullLoop(0, 2, promptPosition + dir);
		promptYesButton.SetSelected(promptPosition == 0);
		promptNoButton.SetSelected(promptPosition == 1);
	}

	public void SelectItem(bool isUpgrade) {
		if (entryList.Count == 0 || !entryList[currentListIndex].affordable) {
			return;
		}
		else if (!promptMode) {
			if (entryList[currentListIndex].affordable) {
				SetupTradePrompt(isUpgrade);
			}
		}
		else {
			if (promptPosition == 0) {
				Debug.Log((isUpgrade) ? "Upgrade" : "Invent");
				totalMoney.value -= entryList[currentListIndex].upgrade.cost;
				totalScrap.value -= entryList[currentListIndex].upgrade.scrap;
				playerData.upgrader.upgrades[entryList[currentListIndex].index].researched = true;
				playerData.upgrader.CalculateResearch();
			}
			GenerateList();
			DeselectItem();
			MoveSelection(currentListIndex >= entryList.Count ? entryList.Count - currentListIndex -1 : 0);
		}
	}

	public bool DeselectItem() {
		if (promptMode) {
			promptMode = false;
			promptView.SetActive(false);
			return false;
		}

		return true;
	}

	private void SetupTradePrompt(bool isUpgrade) {
		promptMode = true;
		promptText.text = (isUpgrade) ? "Buy upgrade?" : "Develop item?";
		promptPosition = 0;
		MovePromt(0);
		promptView.SetActive(true);
	}

	private void SetupUpgradeInfo() {
		if (entryList.Count == 0) {
			upgradeName.text = "";
			itemIcon.sprite = null;

			costMoney.text = "Cost:";
			costScrap.text = "Scrap:";
			level.text = "Level:";
			relatedItem.text = "Item:";

			pwrText.gameObject.SetActive(false);
			rangeText.gameObject.SetActive(false);
			hitText.gameObject.SetActive(false);
			weightText.gameObject.SetActive(false);
			return;
		}

		UpgradeEntry upgrade = entryList[currentListIndex].upgrade;
		upgradeName.text = upgrade.entryName;
		relatedItem.text = "Item:  " + upgrade.item.entryName;
		itemIcon.sprite = upgrade.item.icon;
		itemIcon.color = upgrade.repColor;

		if (entryList[currentListIndex].done) {
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
		weightText.text = "Weight:  " + upgrade.item.weight + "  ->  " + (upgrade.item.weight - upgrade.weight);
		weightText.gameObject.SetActive(upgrade.weight != 0);
	}

	private void SetupDevelopInfo() {
		if (entryList.Count == 0) {
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
			weightText.text = "";
			invReqText.text = "";
			return;
		}

		UpgradeEntry upgrade = entryList[currentListIndex].upgrade;
		upgradeName.text = upgrade.entryName;
		relatedItem.text = "Item:  " + upgrade.item.entryName;
		itemIcon.sprite = upgrade.item.icon;
		itemIcon.color = upgrade.repColor;

		if (entryList[currentListIndex].done) {
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
		weightText.text = "Weight:  " + upgrade.item.weight;
		invReqText.text = "Req:  " + ItemEntry.GetRankLetter(upgrade.item.skillReq);
		return;
	}
}
