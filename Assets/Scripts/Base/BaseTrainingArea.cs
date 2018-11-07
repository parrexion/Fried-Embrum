using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseTrainingArea : InputReceiver {

	[Header("Button menu")]
	public MyButton[] buttons;
    public SaveListVariable saveList;
	public IntVariable currentIndex;
    public UnityEvent characterChangedEvent;

	[Header("Information box")]
	public Text characterName;
	public Text level;
	public Text className;

	public Text hpText;
	public Text atkText;
	public Text sklText;
	public Text spdText;
	public Text lckText;
	public Text defText;
	public Text resText;
	public Text conText;
	public Text movText;

	[Header("Bonus EXP")]
	public Image portrait;
	public Text bonusExp;
	public Text spendExp;
	public Text currentLevel;
	public Text currentExp;
	public IntVariable totalBonusExp;


	private void Start () {
		currentIndex.value = 0;
		SetupButtons();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_TRAIN);
		UpdateButtons();
	}

    public override void OnUpArrow() {
		if (!active)
			return;

		currentIndex.value--;
		if (currentIndex.value < 0) {
			currentIndex.value = saveList.stats.Count-1;
		}
		UpdateButtons();
		characterChangedEvent.Invoke();
	}

    public override void OnDownArrow() {
		if (!active)
			return;

		currentIndex.value++;
		if (currentIndex.value >= saveList.stats.Count) {
			currentIndex.value = 0;
		}
		UpdateButtons();
		characterChangedEvent.Invoke();
	}

	public void SetupButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			if (i < saveList.stats.Count) {
				buttons[i].buttonText.text = saveList.stats[i].charData.entryName;
				buttons[i].SetSelected(i == currentIndex.value);
			}
			else {
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateButtons() {
		for (int i = 0; i < saveList.stats.Count; i++) {
			buttons[i].SetSelected(i == currentIndex.value);
		}
        // previousItem.sprite = upgradedItem.sprite = upgrades[currentIndex.value].item.icon;
		ShowCharacter();
		ShowBonusExp();
	}

	private void ShowCharacter() {
		StatsContainer stats = saveList.stats[currentIndex.value];

		characterName.text = stats.charData.entryName;
		level.text = "Level:  " + stats.level.ToString();
		className.text = stats.classData.entryName;

		hpText.text  = "HP:  " + stats.hp.ToString();
		atkText.text = "Atk:  " + stats.atk.ToString();
		sklText.text = "Skl:  " + stats.skl.ToString();
		spdText.text = "Spd:  " + stats.spd.ToString();
		lckText.text = "Lck:  " + stats.lck.ToString();
		defText.text = "Def:  " + stats.def.ToString();
		resText.text = "Res:  " + stats.res.ToString();
		conText.text = "Con:  " + stats.GetConstitution().ToString();
		movText.text = "Mov:  " + stats.GetMovespeed().ToString();
	}

	private void ShowBonusExp() {
		StatsContainer stats = saveList.stats[currentIndex.value];
		portrait.sprite = stats.charData.portrait;
		bonusExp.text = "Available EXP:  " + totalBonusExp.value;
		spendExp.text = "34";
		currentLevel.text = "Current level:  " + stats.level;
		currentExp.text = "Current EXP:   " + stats.currentExp;
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

