using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SaveScreenController : InputReceiverDelegate {

	public enum NextState { MAIN, BASE, LOADSCREEN }

	public SaveFileController saveFileController;
	public ScrObjLibraryVariable chapterLibrary;
	public IntVariable nextState;
	public ScrObjEntryReference currentMap;
	public StringVariable currentChapterId;
	public IntVariable currentPlayDays;
	public BoolVariable lockControls;

	[Header("Save Popup")]
	public MyPrompt savePrompt;
	private bool isPrompt;

	public UnityEvent stopMusicEvent;


	private void Start() {
		MenuChangeDelay(MenuMode.SAVE);
		stopMusicEvent.Invoke();
		if (currentMap.value == null) {
			currentChapterId.value = "";
			return;
		}

		currentPlayDays.value += ((MapEntry)currentMap.value).mapDuration;
		MapEntry nextEntry = ((MapEntry)currentMap.value).autoNextChapter;
		if (nextEntry == null) {
			currentChapterId.value = "";
		}
		else {
			currentChapterId.value = nextEntry.uuid;
		}
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.SAVE);
	}

	public void NextLevel() {
		currentMap.value = ((MapEntry)currentMap.value)?.autoNextChapter;

		if (currentMap.value == null) {
			NextState next = ((NextState)nextState.value);
			if (next == NextState.MAIN) {
				InputDelegateController.instance.TriggerSceneChange(MenuMode.MAIN_MENU, "MainMenu");
				return;
			}
			else if (next == NextState.BASE) {
				InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "BaseScene");
				return;
			}
		}

		currentChapterId.value = currentMap.value.uuid;

		if (currentChapterId.value == SaveFileController.CLEAR_GAME_ID) {
			InputDelegateController.instance.TriggerSceneChange(MenuMode.MAIN_MENU, "MainMenu");
		}
		else {
			InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "LoadingScreen");
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
			menuMoveEvent.Invoke();
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
			savePrompt.ShowYesNoPopup("Continue without saving?", false);
			menuBackEvent.Invoke();
		}
	}

	private IEnumerator Transition() {
		lockControls.value = true;
		//Show popup
		savePrompt.ShowSpinner("Saving...");
		yield return new WaitForSeconds(1f);
		savePrompt.ShowSpinner("Saved  :)");
		saveFileController.UpdateFiles();
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
