using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveScreenController : InputReceiver {

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

	private int state;
	/*
	state 0 = Main menu
	state 1 = Save popup
	state 2 = No save popup
	state 3 = Transitioning
	 */


	private void Start() {
		state = 0;
		savingPopup.SetActive(false);
		stopMusicEvent.Invoke();
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
		if (state == 0 || state == 1) {
			saveFileController.UpClicked();
		}
		else if (state == 2) {
			noSavePosition--;
			if (noSavePosition < 0)
				noSavePosition = noSaveButtons.Length -1;
			UpdateNoSavePopup();
		}
		menuMoveEvent.Invoke();
    }

    public override void OnDownArrow() {
		if (state == 0 || state == 1) {
			saveFileController.DownClicked();
		}
		else if (state == 2) {
			noSavePosition++;
			if (noSavePosition >= noSaveButtons.Length)
				noSavePosition = 0;
			UpdateNoSavePopup();
		}
		menuMoveEvent.Invoke();
    }


    public override void OnOkButton() {
		if (state == 0) {
			bool res = saveFileController.OkClicked();
			if (res) {
				state = 1;
				menuAcceptEvent.Invoke();
			}
		}
		else if (state == 1) {
			bool res = saveFileController.OkClicked();
			if (res) {
				state = 3;
				StartCoroutine(Transition());
				menuAcceptEvent.Invoke();
			}
			else {
				state = 0;
				menuBackEvent.Invoke();
			}
		}
		else if (state == 2) {
			if (noSavePosition == 0) {
				state = 3;
				NextLevel();
				menuAcceptEvent.Invoke();
			}
			else if (noSavePosition == 1) {
				state = 0;
				noSavePopup.SetActive(false);
				menuBackEvent.Invoke();
			}
		}
    }

    public override void OnBackButton() {
		if (state == 0) {
			state = 2;
			noSavePopup.SetActive(true);
			UpdateNoSavePopup();
			menuBackEvent.Invoke();
		}
		else if (state == 1) {
			state = 0;
			saveFileController.BackClicked();
			menuBackEvent.Invoke();
		}
		else if (state == 2) {
			state = 0;
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


    public override void OnMenuModeChanged() { }
    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}
