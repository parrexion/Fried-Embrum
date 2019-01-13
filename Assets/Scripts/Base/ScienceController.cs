using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScienceController : MonoBehaviour {

	[Header("Entry List")]
	public SaveListVariable playerData;
	public IntVariable totalScrap;
	public IntVariable totalMoney;
    public Transform listParent;
	public Transform entryPrefab;
	private bool upgradeMode;
	private int currentListIndex;
	private int listSize;
	private List<UpgradeListEntry> entryList = new List<UpgradeListEntry>();

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
Visa uppgraderingar och sedan spara dem.

    private void GenerateList() {
		TotalMoneyText.text = "Money:  " + totalMoney.value;
        for (int i = listParent.childCount - 1; i > 2; i--) {
            GameObject.Destroy(listParent.GetChild(i).gameObject);
        }

        entryList = new List<UpgradeListEntry>();
        listSize = playerData.upgrades.Count;
        int tempListSize = 0;
		UpgradeType currentType = (upgradeMode) ? UpgradeType.UPGRADE : UpgradeType.INVENTION;
        for (int i = 0; i < listSize; i++) {
			if (playerData.upgrades[i].type == currentType) {
                CreateListEntry(i, playerData.upgrades[i]);
                tempListSize++;
            }
        }
        listSize = tempListSize;
        entryPrefab.gameObject.SetActive(false);
    }

	private void CreateListEntry(int index, UpgradeEntry item) {
		Transform t = Instantiate(entryPrefab, listParent);

		UpgradeListEntry entry = t.GetComponent<UpgradeListEntry>();
		entry.FillData(index, item, totalScrap.value, totalMoney.value);
		entry.SetHighlight(false);
		entryList.Add(entry);

		t.gameObject.SetActive(true);
	}

	public void MoveSelection(int dir) {
		if (!promptMode) {
			if (entryList.Count > 0) {
				currentListIndex = OPMath.FullLoop(0, listSize-1, currentListIndex + dir);
				for (int i = 0; i < listSize; i++) {
					entryList[i].SetHighlight(currentListIndex == i);
				}
			}
            SetupItemInfo();
        }
	}

	public void SelectItem(bool isUpgrade) {
		if (entryList.Count == 0 || !entryList[currentListIndex].affordable) {
			return;
		}
		else if (!promptMode) {
			promptMode = true;
			SetupTradePrompt(isUpgrade);
			promptView.SetActive(true);
		}
		else {
			if (promptPosition == 0) {
				Debug.Log((isUpgrade) ? "Upgrade" : "Invent");
				
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
		promptText.text = (isUpgrade) ? "Buy upgrade?" : "Develop item?";
		promptPosition = 0;
	}

	private void SetupItemInfo() {
		if (entryList.Count == 0) {
			itemName.text = "";
			itemIcon.sprite = null;

			pwrText.text = "Pwr:  ";
			rangeText.text = "Range:  ";
			hitText.text = "Hit:  ";
			critText.text = "Crit:  ";
			reqText.text = "Req:  ";
			weightText.text = "Weight:  ";
			return;
		}

		UpgradeEntry item = entryList[currentListIndex].upgrade;
		itemName.text = item.entryName;
		itemIcon.sprite = entryList[currentListIndex].icon.sprite;

		pwrText.text  = "Pwr:  " + item.power.ToString();
		//rangeText.text = "Range:  " + item.range.ToString();
		hitText.text = "Hit:  " + item.hit.ToString();
		//critText.text = "Crit:  " + item.critRate.ToString();
		//reqText.text = "Req:  " + item.skillReq.ToString();
		weightText.text = "Weight:  " + item.weight.ToString();
	}

}
