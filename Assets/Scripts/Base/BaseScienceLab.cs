using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseScienceLab : InputReceiver {

	public MyButton[] buttons;
    public UpgradeEntry[] upgrades;

    public IntVariable currentMoney;
    public Text currentMoneyText;
    public Image previousItem;
    public Image upgradedItem;

    public GameObject upgradeInfo;
    public GameObject developInfo;

	public IntVariable currentIndex;
    public UnityEvent upgradeChangedEvent;


	private void Start () {
		currentIndex.value = 0;
		SetupButtons();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_LAB);
		currentIndex.value = 0;
		UpdateButtons();
	}

    public override void OnUpArrow() {
		currentIndex.value--;
		if (currentIndex.value < 0) {
			currentIndex.value = upgrades.Length-1;
		}
		UpdateButtons();
		upgradeChangedEvent.Invoke();
	}

    public override void OnDownArrow() {
		currentIndex.value++;
		if (currentIndex.value >= upgrades.Length) {
			currentIndex.value = 0;
		}
		UpdateButtons();
		upgradeChangedEvent.Invoke();
	}

	private void SetupButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			if (i < upgrades.Length) {
				buttons[i].buttonText.text = upgrades[i].entryName;
				buttons[i].SetSelected(i == currentIndex.value);
			}
			else {
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateButtons() {
        currentMoneyText.text = "Currency:  " + currentMoney.value;
        UpgradeEntry upgrade = upgrades[currentIndex.value];
        upgradeInfo.SetActive(upgrade.type == UpgradeType.UPGRADE);
        developInfo.SetActive(upgrade.type == UpgradeType.INVENTION);
        
		for (int i = 0; i < upgrades.Length; i++) {
			buttons[i].SetSelected(i == currentIndex.value);
		}
        previousItem.sprite = upgradedItem.sprite = upgrades[currentIndex.value].item.icon;
	}

    public override void OnBackButton() { }
    public override void OnLButton() { }
    public override void OnLeftArrow() { }
    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnRightArrow() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

