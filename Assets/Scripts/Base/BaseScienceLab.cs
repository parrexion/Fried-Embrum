using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseScienceLab : InputReceiverDelegate {

	[Header("Button menu")]
	public MyButtonList buttons;
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
	
	private int menuMode;
    public UnityEvent upgradeChangedEvent;


	private void Start () {
		menuTitle.text = "LAB";
		menuMode = 0;
		basicView.SetActive(true);
		developView.SetActive(false);
		buttons.ResetButtons();
		buttons.AddButton("UPGRADE ITEM");
		buttons.AddButton("INVENT ITEM");
		buttons.AddButton("ITEM LIST");
	}

    public override void OnMenuModeChanged() {
		bool prevActive = active;
		active = (currentMenuMode.value == (int)MenuMode.BASE_LAB);
		buttons.ForcePosition(0);
		if(prevActive != active)
			ActivateDelegates(active);
	}

	public override void OnUpArrow() {
		if (menuMode == 0) {
			buttons.Move(-1);
		}
		else if (menuMode == 1 || menuMode == 2) {
			scienceController.MoveSelection(-1);
		}
	}

	public override void OnDownArrow() {
		if (menuMode == 0) {
			buttons.Move(1);
		}
		else if (menuMode == 1 || menuMode == 2) {
			scienceController.MoveSelection(1);
		}
	}

	public override void OnLeftArrow() {
		if (menuMode == 1 || menuMode == 2) {
			scienceController.MovePromt(-1);
		}
	}

	public override void OnRightArrow() {
		if (menuMode == 1 || menuMode == 2) {
			scienceController.MovePromt(1);
		}
	}

	public override void OnOkButton() {
		if (menuMode == 0) {
			int currentIndex = buttons.GetPosition();
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

