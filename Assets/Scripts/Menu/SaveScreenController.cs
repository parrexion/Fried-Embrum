using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveScreenController : InputReceiverDelegate {

	private enum State { MAIN, POPUP, SAVE, TRANSITION }

	public ScrObjEntryReference currentMap;
	public ScrObjEntryReference currentDialogue;
	public SaveFileController saveFileController;
	public MapInfoListVariable chapterList;
	public IntVariable chapterIndex;

	[Header("No Save Popup")]
	public GameObject noSavePopup;
	public Image[] noSaveButtons;
	private int noSavePosition;

	[Header("Save Popup")]
	public GameObject savingPopup;
	public Text saveText;

	public UnityEvent stopMusicEvent;

	private State state;
	/*
	state 0 = Main menu
	state 1 = Save popup
	state 2 = No save popup
	state 3 = Transitioning
	 */


	private void Start() {
		state = State.MAIN;
		savingPopup.SetActive(false);
		stopMusicEvent.Invoke();
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.NONE);
	}

	public void NextLevel() {
		Debug.Log("Current index is:  " + chapterIndex.value);
		if (chapterIndex.value >= chapterList.values.Count) {
			SceneManager.LoadScene("MainMenu");
		}
		else {
			currentMap.value = chapterList.values[chapterIndex.value];
			chapterIndex.value++;
			currentDialogue.value = ((MapEntry)currentMap.value).preDialogue;
			SceneManager.LoadScene("LoadingScreen");
		}
	}

    public override void OnUpArrow() {
		if (state == State.MAIN || state == State.POPUP) {
			saveFileController.Move(-1);
		}
		else if (state == State.SAVE) {
			noSavePosition--;
			if (noSavePosition < 0)
				noSavePosition = noSaveButtons.Length -1;
			UpdateNoSavePopup();
		}
		menuMoveEvent.Invoke();
    }

    public override void OnDownArrow() {
		if (state == State.MAIN || state == State.POPUP) {
			saveFileController.Move(1);
		}
		else if (state == State.SAVE) {
			noSavePosition++;
			if (noSavePosition >= noSaveButtons.Length)
				noSavePosition = 0;
			UpdateNoSavePopup();
		}
		menuMoveEvent.Invoke();
    }


    public override void OnOkButton() {
		if (state == State.MAIN) {
			bool res = saveFileController.OkClicked();
			if (res) {
				state = State.POPUP;
				menuAcceptEvent.Invoke();
			}
		}
		else if (state == State.POPUP) {
			bool res = saveFileController.OkClicked();
			if (res) {
				state = State.TRANSITION;
				StartCoroutine(Transition());
				menuAcceptEvent.Invoke();
			}
			else {
				state = State.MAIN;
				menuBackEvent.Invoke();
			}
		}
		else if (state == State.SAVE) {
			if (noSavePosition == 0) {
				state = State.TRANSITION;
				NextLevel();
				menuAcceptEvent.Invoke();
			}
			else if (noSavePosition == 1) {
				state = State.MAIN;
				noSavePopup.SetActive(false);
				menuBackEvent.Invoke();
			}
		}
    }

    public override void OnBackButton() {
		if (state == State.MAIN) {
			state = State.SAVE;
			noSavePopup.SetActive(true);
			UpdateNoSavePopup();
			menuBackEvent.Invoke();
		}
		else if (state == State.POPUP) {
			state = State.MAIN;
			saveFileController.BackClicked();
			menuBackEvent.Invoke();
		}
		else if (state == State.SAVE) {
			state = State.MAIN;
			noSavePopup.SetActive(false);
			menuBackEvent.Invoke();
		}
	}

	private IEnumerator Transition() {
		//Show popup
		saveText.text = "Saving...";
		savingPopup.SetActive(true);
		yield return new WaitForSeconds(1f);
		saveText.text = "Saved  :)";
		yield return new WaitForSeconds(1f);
		savingPopup.SetActive(false);

		NextLevel();
	}

	private void UpdateNoSavePopup() {
		for (int i = 0; i < noSaveButtons.Length; i++) {
			noSaveButtons[i].enabled = (i == noSavePosition);
		}
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
