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

	private int currentMenu;
	public UnityEvent upgradeChangedEvent;


	private void Start() {
		menuTitle.text = "LAB";
		currentMenu = 0;
		basicView.SetActive(true);
		developView.SetActive(false);
		buttons.ResetButtons();
		buttons.AddButton("UPGRADE ITEM");
		buttons.AddButton("INVENT ITEM");
	}

	public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_LAB);
		buttons.ForcePosition(0);
	}

	public override void OnUpArrow() {
		if (currentMenu == 0) {
			buttons.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == 1 || currentMenu == 2) {
			scienceController.MoveSelection(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (currentMenu == 0) {
			buttons.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == 1 || currentMenu == 2) {
			scienceController.MoveSelection(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (currentMenu == 1 || currentMenu == 2) {
			scienceController.MovePromt(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (currentMenu == 1 || currentMenu == 2) {
			scienceController.MovePromt(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (currentMenu == 0) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				currentMenu = 1;
				menuTitle.text = "UPGRADE";
				scienceController.GenerateLists(true);
				developView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
			else if (currentIndex == 1) {
				currentMenu = 2;
				menuTitle.text = "INVENTIONS";
				scienceController.GenerateLists(false);
				developView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
		}
		else if (currentMenu == 1 || currentMenu == 2) {
			scienceController.SelectItem(false);
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (currentMenu == 0) {
			MenuChangeDelay(MenuMode.BASE_MAIN);
			menuBackEvent.Invoke();
		}
		else if (currentMenu == 1 || currentMenu == 2) {
			if (scienceController.DeselectItem()) {
				currentMenu = 0;
				menuTitle.text = "LAB";
				basicView.SetActive(true);
				developView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
		}
	}


	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnStartButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}

