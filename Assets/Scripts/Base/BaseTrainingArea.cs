using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseTrainingArea : InputReceiver {

	[Header("Button menu")]
	public MyButton[] buttons;
	public Text menuTitle;
    public SaveListVariable saveList;
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

	private int menuMode;
	private int currentIndex;
	private int characterIndex;


	private void Start () {
		menuTitle.text = "TRAINING";
		menuMode = 0;
		currentIndex = 0;
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_TRAIN);
		UpdateButtons();
	}

    public override void OnUpArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length-1, currentIndex-1);
			UpdateButtons();
		}
		else if (menuMode == 1) {
			characterIndex = OPMath.FullLoop(0, saveList.stats.Count-1, characterIndex-1);
			characterChangedEvent.Invoke();
		}
		else if (menuMode == 2) {
			characterIndex = OPMath.FullLoop(0, saveList.stats.Count-1, characterIndex-1);
			characterChangedEvent.Invoke();
		}
	}

    public override void OnDownArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length-1, currentIndex+1);
			UpdateButtons();
		}
		else if (menuMode == 1) {
			characterIndex = OPMath.FullLoop(0, buttons.Length-1, characterIndex+1);
			characterChangedEvent.Invoke();
		}
		else if (menuMode == 2) {
			characterIndex = OPMath.FullLoop(0, buttons.Length-1, characterIndex+1);
			characterChangedEvent.Invoke();
		}
	}

    public override void OnOkButton() { }
    public override void OnBackButton() { }
    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }

	public void SetupButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			if (i < saveList.stats.Count) {
				buttons[i].buttonText.text = saveList.stats[i].charData.entryName;
				buttons[i].SetSelected(i == currentIndex);
			}
			else {
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateButtons() {
		if (menuMode == 0) {
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].SetSelected(i == currentIndex);
			}
		}
		else {
			for (int i = 0; i < saveList.stats.Count; i++) {
				buttons[i].SetSelected(i == characterIndex);
			}
			ShowCharacter();
			ShowBonusExp();
		}
	}

	private void ShowCharacter() {
		StatsContainer stats = saveList.stats[characterIndex];

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
		StatsContainer stats = saveList.stats[characterIndex];
		portrait.sprite = stats.charData.portrait;
		bonusExp.text = "Available EXP:  " + totalBonusExp.value;
		spendExp.text = "34";
		currentLevel.text = "Current level:  " + stats.level;
		currentExp.text = "Current EXP:   " + stats.currentExp;
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

