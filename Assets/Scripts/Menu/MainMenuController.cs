using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : InputReceiver {

	public GameObject startMenuObject;
	public HowToPlayController howTo;
	public BoolVariable dialoguePrePost;

	public Image[] menuButtons;

	private int state;
	private int menuPosition;
	private int buttonPosition;


	private void Start() {
		state = 0;
		startMenuObject.SetActive(true);
		SetupMenuButtons();
	}

	public void StartClicked() {
		dialoguePrePost.value = false;
		SceneManager.LoadScene("Dialogue");
	}

	public void ControlsClicked() {
		state = 1;
		startMenuObject.SetActive(false);
		howTo.UpdateState(true);
	}

	public void StartBattle() {
		SceneManager.LoadScene("BattleScene");
	}

    public override void OnUpArrow() {
		if (state != 0)
			return;

		buttonPosition--;
		if (buttonPosition < 0)
			buttonPosition += menuButtons.Length;
		SetupMenuButtons();	
    }

    public override void OnDownArrow() {
		if (state != 0)
			return;

		buttonPosition++;
		if (buttonPosition >= menuButtons.Length)
			buttonPosition = 0;
		SetupMenuButtons();
    }

    public override void OnLeftArrow() {
		if (state != 1)
			return;

        howTo.MoveLeft();
    }

    public override void OnRightArrow() {
		if (state != 1)
			return;

        howTo.MoveRight();
    }

    public override void OnOkButton() {
		if (state == 0) {
			switch (buttonPosition)
			{
				case 0:
					ControlsClicked();
					break;
				case 1:
					StartClicked();
					break;
			}
		}
		else if (state == 1 && howTo.CheckOk()) {
			StartClicked();
		}
    }

    public override void OnBackButton() {
		if (state == 1) {
			state = 0;
			howTo.BackClicked();
			startMenuObject.SetActive(true);
		}
	}

	/// <summary>
	/// Shows which button is currently selected.
	/// </summary>
	private void SetupMenuButtons() {
		for (int i = 0; i < menuButtons.Length; i++) {
			menuButtons[i].enabled = (i == buttonPosition);
		}
	}


    public override void OnMenuModeChanged() { }
    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}
