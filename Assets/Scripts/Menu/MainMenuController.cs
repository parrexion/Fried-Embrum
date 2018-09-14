using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : InputReceiver {

	public GameObject startMenuObject;
	public HowToPlayController howTo;
	public SaveFileController saveFileController;
	public OptionsController OptionsController;
	public MapInfoListVariable chapterList;
	
	[Header("Current Data")]
	public IntVariable currentChapterIndex;
	public IntVariable currentPlayTime;
	public ScrObjEntryReference currentMap;
	public BoolVariable dialoguePrePost;

	[Header("Menu")]
	public Image[] menuButtons;

	[Header("Music")]
	public MusicEntry mainTheme;
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public BoolVariable musicFocus;
	public UnityEvent playBkgMusicEvent;

	public UnityEvent saveGameEvent;

	private int state;
	private int menuPosition;
	private int buttonPosition;


	private void Start() {
		state = 0;
		startMenuObject.SetActive(true);
		saveFileController.HideMenu();
		SetupMenuButtons();

		musicFocus.value = true;
		mainMusic.value = mainTheme.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();
	}

	public void ControlsClicked() {
		state = 1;
		startMenuObject.SetActive(false);
		howTo.UpdateState(true);
	}

	public void LoadClicked() {
		state = 2;
		startMenuObject.SetActive(false);
		saveFileController.ActivateMenu();
	}

	public void OptionsClicked() {
		state = 4;
		startMenuObject.SetActive(false);
		OptionsController.UpdateState(true);
	}

	/// <summary>
	/// Called when starting a new game. Sets the starting values.
	/// </summary>
	public void NewGameClicked() {
		currentChapterIndex.value = 1;
		currentPlayTime.value = 0;
		currentMap.value = chapterList.values[0];
		dialoguePrePost.value = false;
		SceneManager.LoadScene("LoadingScreen");
	}

	/// <summary>
	/// Called when starting a new game. Sets the starting values.
	/// </summary>
	public void LoadGameFinished() {
		currentMap.value = chapterList.values[currentChapterIndex.value];
		dialoguePrePost.value = false;
		SceneManager.LoadScene("LoadingScreen");
	}

    public override void OnUpArrow() {
		if (state == 0) {
			buttonPosition--;
			if (buttonPosition < 0)
				buttonPosition += menuButtons.Length;
			SetupMenuButtons();	
			menuMoveEvent.Invoke();
		}
		else if (state == 2 || state == 3) {
			saveFileController.UpClicked();
			menuMoveEvent.Invoke();
		}
		else if (state == 4) {
			OptionsController.MoveUp();
			menuMoveEvent.Invoke();
		}
    }

    public override void OnDownArrow() {
		if (state == 0) {
			buttonPosition++;
			if (buttonPosition >= menuButtons.Length)
				buttonPosition = 0;
			SetupMenuButtons();
			menuMoveEvent.Invoke();
		}
		else if (state == 2 || state == 3) {
			saveFileController.DownClicked();
			menuMoveEvent.Invoke();
		}
		else if (state == 4) {
			OptionsController.MoveDown();
			menuMoveEvent.Invoke();
		}
    }

    public override void OnLeftArrow() {
		if (state == 1) {
			if (howTo.MoveLeft())
				menuMoveEvent.Invoke();
		}
		else if (state == 4) {
			if (OptionsController.MoveLeft())
				menuMoveEvent.Invoke();
		}
    }

    public override void OnRightArrow() {
		if (state == 1) {
			if (howTo.MoveRight())
				menuMoveEvent.Invoke();
		}
		else if (state == 4) {
			if (OptionsController.MoveRight())
				menuMoveEvent.Invoke();
		}
    }

    public override void OnOkButton() {
		if (state == 0) {
			switch (buttonPosition)
			{
				case 0:
					ControlsClicked();
					break;
				case 1:
					LoadClicked();
					break;
				case 2:
					OptionsClicked();
					break;
			}
			menuAcceptEvent.Invoke();
		}
		else if (state == 1 && howTo.CheckOk()) {
			NewGameClicked();
			menuAcceptEvent.Invoke();
		}
		else if (state == 2) {
			bool res = saveFileController.OkClicked();
			if (res) {
				menuAcceptEvent.Invoke();
				state = 3;
			}
		}
		else if (state == 3) {
			if (saveFileController.OkClicked()) {
				menuAcceptEvent.Invoke();
				state = -1;
			}
			else {
				menuBackEvent.Invoke();
				state = 2;
			}
		}
    }

    public override void OnBackButton() {
		if (state == 1) {
			state = 0;
			howTo.BackClicked();
			menuBackEvent.Invoke();
			startMenuObject.SetActive(true);
		}
		else if (state == 2) {
			state = 0;
			saveFileController.BackClicked();
			menuBackEvent.Invoke();
			startMenuObject.SetActive(true);
		}
		else if (state == 3) {
			state = 2;
			saveFileController.BackClicked();
			menuBackEvent.Invoke();
		}
		else if (state == 4) {
			state = 0;
			OptionsController.BackClicked();
			menuBackEvent.Invoke();
			startMenuObject.SetActive(true);
			saveGameEvent.Invoke();
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
