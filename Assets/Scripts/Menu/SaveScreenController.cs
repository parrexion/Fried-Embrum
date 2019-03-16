using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SaveScreenController : InputReceiverDelegate {

	public ScrObjEntryReference currentMap;
	public ScrObjEntryReference currentDialogue;
	public SaveFileController saveFileController;
	public MapInfoListVariable chapterList;
	public IntVariable chapterIndex;
	public BoolVariable lockControls;

	[Header("Save Popup")]
	public MyPrompt savePrompt;
	private bool isPrompt;

	public UnityEvent stopMusicEvent;


	private void Start() {
		MenuChangeDelay(MenuMode.SAVE);
		stopMusicEvent.Invoke();
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.SAVE);
	}

	public void NextLevel() {
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
		if (!isPrompt) {
			saveFileController.Move(-1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnDownArrow() {
		if (!isPrompt) {
			saveFileController.Move(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (isPrompt) {
			savePrompt.Move(-1);
		}
		else {
			saveFileController.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (isPrompt) {
			savePrompt.Move(1);
		}
		else {
			saveFileController.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (isPrompt) {
			if (savePrompt.Click(true) == MyPrompt.Result.OK1)
				NextLevel();
		}
		else if (saveFileController.OkClicked()) {
			StartCoroutine(Transition());
		}
		menuAcceptEvent.Invoke();
    }

    public override void OnBackButton() {
		if (isPrompt){
			isPrompt = false;
			savePrompt.Click(false);
			menuBackEvent.Invoke();
		}
		else if (saveFileController.BackClicked()) {
			isPrompt = true;
			savePrompt.ShowWindow("Continue without saving?", false);
			menuAcceptEvent.Invoke();
		}
	}

	private IEnumerator Transition() {
		lockControls.value = true;
		//Show popup
		savePrompt.ShowSpinner("Saving...");
		yield return new WaitForSeconds(1f);
		savePrompt.ShowSpinner("Saved  :)");
		yield return new WaitForSeconds(1f);
		savePrompt.Click(true);
		
		NextLevel();
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
