using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseTrainingArea : InputReceiverDelegate {

	private enum State { MAIN, EXP, CLASS }

	[Header("Button menu")]
	public MyButtonList buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject bexpView;
	public GameObject classView;
	private State currentMenu;

	[Header("Handlers")]
	public BexpController bexpController;
	public ClassChangeController changeController;


	private void Start() {
		menuTitle.text = "TRAINING";
		basicView.SetActive(true);
		bexpView.SetActive(false);
		classView.SetActive(false);
		buttons.ResetButtons();
		buttons.AddButton("BATTLE TRAINING");
		buttons.AddButton("CLASS UPGRADE");
		//buttons.AddButton("CLASS CHANGE");
		currentMenu = State.MAIN;
	}

	public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_TRAIN);
		buttons.ForcePosition(0);
		currentMenu = State.MAIN;
	}

	public override void OnUpArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.EXP) {
			bexpController.MoveSelection(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.CLASS) {
			changeController.MoveSelection(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.EXP) {
			bexpController.MoveSelection(1);
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.CLASS) {
			changeController.MoveSelection(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (currentMenu == State.MAIN) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				currentMenu = State.EXP;
				menuTitle.text = "BEXP";
				bexpController.GenerateList();
				bexpView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
			else if (currentIndex == 1) {
				currentMenu = State.CLASS;
				menuTitle.text = "CLASS";
				classView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
		}
		else if (currentMenu == State.EXP) {
			bexpController.SelectCharacter();
			menuAcceptEvent.Invoke();
		}
		else if (currentMenu == State.CLASS) {
			changeController.SelectCharacter();
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (currentMenu == State.MAIN) {
			MenuChangeDelay(MenuMode.BASE_MAIN);
			menuBackEvent.Invoke();
		}
		else if (currentMenu == State.EXP) {
			if (bexpController.DeselectCharacter()) {
				currentMenu = State.MAIN;
				menuTitle.text = "TRAINING";
				basicView.SetActive(true);
				bexpView.SetActive(false);
				menuBackEvent.Invoke();
			}
		}
		else if (currentMenu == State.CLASS) {
			if (changeController.DeselectCharacter()) {
				currentMenu = State.MAIN;
				menuTitle.text = "TRAINING";
				basicView.SetActive(true);
				classView.SetActive(false);
				menuBackEvent.Invoke();
			}
		}
	}

	public override void OnLeftArrow() {
		if (currentMenu == State.EXP) {
			bexpController.UpdateAwardExp(-1);
			menuMoveEvent.Invoke();
		}
	}
	public override void OnRightArrow() {
		if (currentMenu == State.EXP) {
			bexpController.UpdateAwardExp(1);
			menuMoveEvent.Invoke();
		}
	}


	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnStartButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}

