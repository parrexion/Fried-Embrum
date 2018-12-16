using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseTrainingArea : InputReceiver {

	[Header("Button menu")]
	public MyButton[] buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject bexpView;
	public GameObject classView;

	[Header("Handlers")]
	public BexpController bexpController;
	public ClassChangeController changeController;

	private int menuMode;
	private int currentIndex;
	private int characterIndex;


	private void Start () {
		menuTitle.text = "TRAINING";
		menuMode = 0;
		currentIndex = 0;
		basicView.SetActive(true);
		bexpView.SetActive(false);
		classView.SetActive(false);
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
			bexpController.MoveSelection(-1);
		}
		else if (menuMode == 2) {
			changeController.MoveSelection(-1);
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
			bexpController.MoveSelection(1);
		}
		else if (menuMode == 2) {
			changeController.MoveSelection(1);
		}
	}

    public override void OnOkButton() {
		if (menuMode == 0) {
			if (currentIndex == 0) {
				menuMode = 1;
				menuTitle.text = "BEXP";
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


	private void UpdateButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].SetSelected(i == currentIndex);
		}
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

