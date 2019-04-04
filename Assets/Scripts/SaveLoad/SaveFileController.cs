using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveFileController : MonoBehaviour {

	public MapInfoListVariable chapterList;
	public ScrObjEntryReference currenMap;

	[Header("Popup")]
	public bool usedForLoad;
	public MyPrompt filePrompt;
	private bool isPopup;

	[Header("Save Files")]
	public int visibleSize = 3;
	public Transform listParent;
	public Transform entryPrefab;
	private EntryList<SaveFileEntry> saveFiles;

	[Header("Save Data")]
	public IntVariable[] chapterIndex;
	public IntVariable[] totalDays;
	public IntVariable[] playTimes;

	public UnityEvent saveGameEvent;
	public UnityEvent loadGameEvent;
	

	private void Start() {
		SetupSaveFiles();
	}

	/// <summary>
	/// Update index when up or down is clicked.
	/// </summary>
	public void Move(int dir) {
		if (isPopup)
			return;

		saveFiles.Move(dir);
	}

	/// <summary>
	/// Update index when left or right is clicked.
	/// </summary>
	public void MoveHorizontal(int dir) {
		if (!isPopup)
			return;

		filePrompt.Move(dir);
	}

	/// <summary>
	/// Returns true or false to indicate whether it is possible to advance 
	/// further into the menus at the current position.
	/// </summary>
	/// <returns></returns>
	public bool OkClicked() {
		if (!isPopup) {
			if (usedForLoad && (playTimes[saveFiles.GetPosition()].value == 0 || chapterIndex[saveFiles.GetPosition()].value >= chapterList.values.Count))
				return false;

			isPopup = true;
			if (usedForLoad) {
				filePrompt.ShowWindow("Load selected file?", false);
			}
			else {
				filePrompt.ShowWindow("Save to selected file?", false);
			}
			return false;
		}
		else {
			if (filePrompt.Click(true) == MyPrompt.Result.OK1) {
				if (usedForLoad) {
					loadGameEvent.Invoke();
				}
				else {
					saveGameEvent.Invoke();
				}
				return true;
			}

			isPopup = false;
			return false;
		}
	}

	/// <summary>
	/// Move back when the back button is clicked.
	/// </summary>
	public bool BackClicked() {
		if (isPopup) {
			filePrompt.Click(false);
			isPopup = false;
			return false;
		}

		return true;
	}

	/// <summary>
	/// Sets up the save file information for the save files.
	/// </summary>
	private void SetupSaveFiles() {
		saveFiles = new EntryList<SaveFileEntry>(visibleSize);

		for (int i = 0; i < SaveController.SAVE_FILES_COUNT; i++) {
			Transform t = Instantiate(entryPrefab, listParent);
			SaveFileEntry entry = saveFiles.CreateEntry(t);
			if (chapterIndex[i].value >= chapterList.values.Count) {
				entry.FillData("All maps cleared!", totalDays[i].value, playTimes[i].value);
			}
			else if (chapterIndex[i].value == -1) {
				entry.FillData("BASE", totalDays[i].value, playTimes[i].value);
			}
			else {
				entry.FillData(chapterList.values[chapterIndex[i].value].entryName, totalDays[i].value, playTimes[i].value);
			}
		}
		entryPrefab.gameObject.SetActive(false);
	}

}
