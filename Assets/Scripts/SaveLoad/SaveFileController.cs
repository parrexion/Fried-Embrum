using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveFileController : MonoBehaviour {

	public MapInfoListVariable chapterList;
	public GameObject fileMenu;
	public Image[] saveFiles;

	[Header("Popup")]
	public GameObject filePopup;
	public Image[] popupButtons;
	public Text popupText;
	public bool usedForLoad;
	public GameObject loadButton;
	public GameObject saveButton;

	[Header("Save File Texts")]
	public Text[] emptyFileText;
	public Text[] chapterIndexTexts;
	public Text[] levelNameTexts;
	public Text[] playTimeTexts;

	[Header("Save Data")]
	public IntVariable saveIndex;
	public IntVariable[] chapterIndex;
	public IntVariable[] playTimes;

	public UnityEvent saveGameEvent;
	public UnityEvent loadGameEvent;

	private bool isPopup;
	private int popupPosition;


	private void Start() {
		SetupSaveFiles();
	}

	public void HideMenu() {
		fileMenu.SetActive(false);
		filePopup.SetActive(false);
	}

	public void ActivateMenu() {
		saveIndex.value = 0;
		popupPosition = -1;
		isPopup = false;
		SetupSaveFiles();
		fileMenu.SetActive(true);
	}

	/// <summary>
	/// Update index when up is clicked.
	/// </summary>
	public void UpClicked() {
		if (!isPopup) {
			saveIndex.value--;
			if (saveIndex.value < 0)
				saveIndex.value = saveFiles.Length -1;
			SetupSaveFileSelection();
		}
		else {
			popupPosition--;
			if (popupPosition < 0)
				popupPosition = popupButtons.Length -1;
			if (popupPosition == 0 && !usedForLoad)
				popupPosition = popupButtons.Length -1;
			if (popupPosition == 1 && usedForLoad)
				popupPosition = 0;
			SetupPopupButtons();
		}
	}

	/// <summary>
	/// Update index when down is clicked.
	/// </summary>
	public void DownClicked() {
		if (!isPopup) {
			saveIndex.value++;
			if (saveIndex.value >= saveFiles.Length)
				saveIndex.value = 0;
			SetupSaveFileSelection();
		}
		else {
			popupPosition++;
			if (popupPosition >= popupButtons.Length)
				popupPosition = 0;
			if (popupPosition == 0 && !usedForLoad)
				popupPosition = 1;
			if (popupPosition == 1 && usedForLoad)
				popupPosition = 2;
			SetupPopupButtons();
		}
	}

	/// <summary>
	/// Returns true or false to indicate whether it is possible to advance 
	/// further into the menus at the current position.
	/// </summary>
	/// <returns></returns>
	public bool OkClicked() {
		if (!isPopup) {
			if (usedForLoad && (playTimes[saveIndex.value].value == 0 || chapterIndex[saveIndex.value].value >= chapterList.values.Count)) {
				return false;
			}
			else {
				isPopup = true;
				popupPosition = -1;
				SetupPopupButtons();
				loadButton.SetActive(usedForLoad);
				saveButton.SetActive(!usedForLoad);
				filePopup.SetActive(true);
				DownClicked();
				return true;
			}
		}
		else {
			if (popupPosition == 0) {
				filePopup.SetActive(false);
				loadGameEvent.Invoke();
				return true;
			}
			else if (popupPosition == 1) {
				filePopup.SetActive(false);
				saveGameEvent.Invoke();
				return true;
			}

			BackClicked();
			return false;
		}
	}

	/// <summary>
	/// Move back when the back button is clicked.
	/// </summary>
	public void BackClicked() {
		if (isPopup) {
			isPopup = false;
			filePopup.SetActive(false);
		}
		else {
			fileMenu.SetActive(false);
		}
	}

	/// <summary>
	/// Sets up the save file information for the save files.
	/// </summary>
	private void SetupSaveFiles() {
		for (int i = 0; i < saveFiles.Length; i++) {
			if (playTimes[i].value == 0) {
				emptyFileText[i].gameObject.SetActive(true);
				chapterIndexTexts[i].text = "";
				levelNameTexts[i].text = "";
				playTimeTexts[i].text = "";
			}
			else if (chapterIndex[i].value >= chapterList.values.Count) {
				emptyFileText[i].gameObject.SetActive(false);
				chapterIndexTexts[i].text = "Ch " + chapterIndex[i].value;
				levelNameTexts[i].text = "All maps cleared!";
				playTimeTexts[i].text = Constants.PlayTimeFromInt(playTimes[i].value,false);
			}
			else {
				emptyFileText[i].gameObject.SetActive(false);
				chapterIndexTexts[i].text = "Ch " + chapterIndex[i].value;
				levelNameTexts[i].text = chapterList.values[chapterIndex[i].value].entryName;
				playTimeTexts[i].text = Constants.PlayTimeFromInt(playTimes[i].value,false);
			}
		}
		SetupSaveFileSelection();
	}

	/// <summary>
	/// Shows the current selection for the save files.
	/// </summary>
	private void SetupSaveFileSelection() {
		for (int i = 0; i < saveFiles.Length; i++) {
			saveFiles[i].enabled = (i == saveIndex.value);
		}
	}

	/// <summary>
	/// Shows the current selection for the popup.
	/// </summary>
	private void SetupPopupButtons() {
		for (int i = 0; i < popupButtons.Length; i++) {
			popupButtons[i].enabled = (i == popupPosition);
		}
	}
}
