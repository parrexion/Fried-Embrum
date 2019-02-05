using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportList : MonoBehaviour {

    public SaveListVariable availableUnits;
	public Transform entryPrefab;

	[Header("Selected Character")]
	public Text selectName;
	public Image selectIcon;

	private int currentListIndex;
	private int listSize;
	private List<SupportListEntry> supportList = new List<SupportListEntry>();

	private bool detailedMode;
	private string selectedCharacter;
	private int detailedCharacterIndex;


	private void OnEnable() {
		listSize = availableUnits.stats.Count;
		supportList = new List<SupportListEntry>();
		currentListIndex = 0;

		for (int i = transform.childCount-1; i > 0; i--) {
			Destroy(transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < listSize; i++) {
			CreateListEntry(availableUnits.stats[i]);
		}
		MoveSelection(0);
		entryPrefab.gameObject.SetActive(false);
	}

	private void CreateListEntry(StatsContainer stats) {
		Transform t = Instantiate(entryPrefab, transform);

		SupportListEntry support = t.GetComponent<SupportListEntry>();
		support.FillData(stats);
		support.SetHighlight(false);
		supportList.Add(support);

		t.gameObject.SetActive(true);
	}

	public void MoveSelection(int dir) {
		if (!detailedMode) {
			currentListIndex = OPMath.FullLoop(0, listSize, currentListIndex + dir);
			for (int i = 0; i < listSize; i++) {
				supportList[i].SetHighlight(currentListIndex == i);
			}
		}
		else {
			do {
				detailedCharacterIndex = OPMath.FullLoop(0, listSize, detailedCharacterIndex + dir);
			} while (!supportList[detailedCharacterIndex].gameObject.activeSelf);
			for (int i = 0; i < listSize; i++) {
				supportList[i].SetHighlight(detailedCharacterIndex == i);
			}
		}
	}

	public void SelectCharacter() {
		if (!detailedMode) {
			detailedMode = true;
			selectedCharacter = supportList[currentListIndex].uuid;
			selectName.text = supportList[currentListIndex].entryName.text;
			selectIcon.sprite = supportList[currentListIndex].icon.sprite;
			CreateSupportEntry();
			detailedCharacterIndex = -1;
			MoveSelection(1);
		}
	}

	public bool DeselectCharacter() {
		if (detailedMode) {
			detailedMode = false;
			for (int i = 0; i < listSize; i++) {
				supportList[i].SetDark(false);
				supportList[i].SetSupportValue(null, 0);
				supportList[i].gameObject.SetActive(true);
			}
			selectName.text = "";
			selectIcon.sprite = null;
			MoveSelection(0);
			return false;
		}

		return true;
	}

	private void CreateSupportEntry() {
		for (int i = 0; i < listSize; i++) {
			if (supportList[i].uuid == selectedCharacter) {
				supportList[i].gameObject.SetActive(false);
			}
			else {
				SupportTuple tuple = availableUnits.stats[i].charData.GetSupport(selectedCharacter);
				int value = availableUnits.stats[i].GetSupportValue(selectedCharacter);

				supportList[i].SetDark(tuple == null);
				supportList[i].SetSupportValue(tuple, value);
			}
		}
	}

}
