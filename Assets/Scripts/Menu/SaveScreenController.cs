using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveScreenController : InputReceiver {

	public MapInfoVariable currentMap;
	public SaveFileController saveFileController;

	[Header("No Save Popup")]
	public GameObject noSavePopup;
	public Image[] noSaveButtons;
	private int noSavePosition;

	[Header("Save Popup")]
	public GameObject savingPopup;
	public Text saveText;

	private int state;

	private void Start() {
		state = 0;
		savingPopup.SetActive(false);
	}

	public void NextLevel() {
		if (currentMap == null) {
			SceneManager.LoadScene("MainMenu");
		}
		else {
			SceneManager.LoadScene("Dialogue");
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
    }


    public override void OnOkButton() {
		if (state == 0) {
			bool res = saveFileController.OkClicked();
			if (res)
				state = 1;
		}
		else if (state == 1) {
			bool res = saveFileController.OkClicked();
			if (res) {
				state = 3;
				StartCoroutine(Transition());
			}
		}
		else if (state == 2) {
			if (noSavePosition == 0) {
				state = 3;
				StartCoroutine(Transition());
			}
			else if (noSavePosition == 1) {
				state = 0;
				noSavePopup.SetActive(false);
			}
		}
    }

    public override void OnBackButton() {
		if (state == 0) {
			state = 2;
			noSavePopup.SetActive(true);
			UpdateNoSavePopup();
		}
		else if (state == 1) {
			state = 0;
			saveFileController.BackClicked();
		}
		else if (state == 2) {
			state = 0;
			noSavePopup.SetActive(false);
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

		if (currentMap == null) {
			SceneManager.LoadScene("MainMenu");
		}
		else {
			SceneManager.LoadScene("Dialogue");
		}
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
