using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveScreenController : InputReceiverDelegate {

	private enum State { MAIN, SAVE, TRANSITION }

	public ScrObjEntryReference currentMap;
	public ScrObjEntryReference currentDialogue;
	public SaveFileController saveFileController;
	public MapInfoListVariable chapterList;
	public IntVariable chapterIndex;

	[Header("Save Popup")]
	public MyPrompt savePrompt;
	private bool isPrompt;

	public UnityEvent stopMusicEvent;

	private State state;


	private void Start() {
		StartCoroutine(MenuChangeDelay(MenuMode.SAVE));
		state = State.MAIN;
		stopMusicEvent.Invoke();
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.SAVE);
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
		if (state == State.MAIN) {
			saveFileController.Move(-1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnDownArrow() {
		if (state == State.MAIN) {
			saveFileController.Move(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if(state == State.SAVE) {
			saveFileController.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if(state == State.SAVE) {
			saveFileController.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (state == State.MAIN) {
			if (saveFileController.OkClicked()) {
				state = State.SAVE;
				menuAcceptEvent.Invoke();
			}
		}
		else if (state == State.SAVE) {
			if (!saveFileController.OkClicked()) {
				state = State.TRANSITION;
				NextLevel();
				menuAcceptEvent.Invoke();
			}
			else {
				state = State.MAIN;
				menuBackEvent.Invoke();
			}
		}
    }

    public override void OnBackButton() {
		if (state == State.MAIN) {
			state = State.SAVE;
			menuBackEvent.Invoke();
		}
		else if (state == State.SAVE) {
			state = State.MAIN;
			menuBackEvent.Invoke();
		}
	}

	private IEnumerator Transition() {
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
