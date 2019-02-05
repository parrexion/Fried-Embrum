using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseScienceLab : InputReceiver {

	[Header("Button menu")]
	public MyButton[] buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject developView;

	[Header("Handlers")]
	public ScienceController scienceController;

	[Header("Visuals")]
	public GameObject upgradeInfo;
    public Image previousItem;
    public Image upgradedItem;

	private int currentIndex;
	private int menuMode;
    public UnityEvent upgradeChangedEvent;


	private void Start () {
		menuTitle.text = "LAB";
		currentIndex = 0;
		menuMode = 0;
		basicView.SetActive(true);
		developView.SetActive(false);
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_LAB);
		UpdateButtons();
	}

	public override void OnUpArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length, currentIndex - 1);
			UpdateButtons();
		}
		else if (menuMode == 1 || menuMode == 2) {
			scienceController.MoveSelection(-1);
		}
	}

	public override void OnDownArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length, currentIndex + 1);
			UpdateButtons();
		}
		else if (menuMode == 1 || menuMode == 2) {
			scienceController.MoveSelection(1);
		}
	}

	public override void OnLeftArrow() {
		if (!active)
			return;

		if (menuMode == 1 || menuMode == 2) {
			scienceController.MovePromt(-1);
		}
	}

	public override void OnRightArrow() {
		if (!active)
			return;

		if (menuMode == 1 || menuMode == 2) {
			scienceController.MovePromt(1);
		}
	}

	private void UpdateButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].SetSelected(i == currentIndex);
		}
	}

	public override void OnOkButton() {
		if (!active)
			return;
		if (menuMode == 0) {
			if (currentIndex == 0) {
				menuMode = 1;
				menuTitle.text = "UPGRADE";
				scienceController.GenerateLists(true);
				developView.SetActive(true);
				basicView.SetActive(false);
			}
			else if (currentIndex == 1) {
				menuMode = 2;
				menuTitle.text = "INVENTIONS";
				scienceController.GenerateLists(false);
				developView.SetActive(true);
				basicView.SetActive(false);
			}
		}
		else if (menuMode == 1 || menuMode == 2) {
			scienceController.SelectItem(false);
		}
	}

	public override void OnBackButton() {
		if (!active)
			return;
		if (menuMode == 0) {

		}
		else if (menuMode == 1 || menuMode == 2) {
			if (scienceController.DeselectItem()) {
				menuMode = 0;
				menuTitle.text = "LAB";
				basicView.SetActive(true);
				developView.SetActive(false);
			}
		}
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

