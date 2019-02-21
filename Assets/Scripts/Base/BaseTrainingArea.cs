using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseTrainingArea : InputReceiverDelegate {

	[Header("Button menu")]
	public MyButtonList buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject bexpView;
	public GameObject classView;

	[Header("Handlers")]
	public BexpController bexpController;
	public ClassChangeController changeController;

	private int menuMode;


	private void Start () {
		menuTitle.text = "TRAINING";
		menuMode = 0;
		basicView.SetActive(true);
		bexpView.SetActive(false);
		classView.SetActive(false);
		buttons.ResetButtons();
		buttons.AddButton("BATTLE EXP");
		buttons.AddButton("CLASS CHANGE");
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_TRAIN);
		buttons.ForcePosition(0);
	}

    public override void OnUpArrow() {
		if (menuMode == 0) {
			buttons.Move(-1);
		}
		else if (menuMode == 1) {
			bexpController.MoveSelection(-1);
		}
		else if (menuMode == 2) {
			changeController.MoveSelection(-1);
		}
	}

    public override void OnDownArrow() {
		if (menuMode == 0) {
			buttons.Move(1);
		}
		else if (menuMode == 1) {
			bexpController.MoveSelection(1);
		}
		else if (menuMode == 2) {
			changeController.MoveSelection(1);
		}
	}

    public override void OnOkButton() {
		if (menuMode == 0) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				menuMode = 1;
				menuTitle.text = "BEXP";
				bexpController.GenerateList();
				bexpView.SetActive(true);
				basicView.SetActive(false);
				// bexpList.
			}
			else if (currentIndex == 1) {
				menuMode = 2;
				menuTitle.text = "CLASS";
				classView.SetActive(true);
				basicView.SetActive(false);
			}
		}
		else if (menuMode == 1) {
			bexpController.SelectCharacter();
		}
		else if (menuMode == 2) {
			changeController.SelectCharacter();
		}
	}

    public override void OnBackButton() {
		if (menuMode == 0) {

		}
		else if (menuMode == 1) {
			if (bexpController.DeselectCharacter()) {
				menuMode = 0;
				menuTitle.text = "TRAINING";
				basicView.SetActive(true);
				bexpView.SetActive(false);
			}
		}
		else if (menuMode == 2) {
			if (changeController.DeselectCharacter()) {
				menuMode = 0;
				menuTitle.text = "TRAINING";
				basicView.SetActive(true);
				classView.SetActive(false);
			}
		}
	}

    public override void OnLeftArrow() {
		if (menuMode == 1) {
			bexpController.UpdateAwardExp(-1);
		}
	}
    public override void OnRightArrow() {
		if (menuMode == 1) {
			bexpController.UpdateAwardExp(1);
		}
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

