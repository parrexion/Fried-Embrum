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

	public EntryList<SupportListEntry> supportList;
	public int maxEntries;

	private int oldIndex;
	private bool detailedMode;
	private string selectedCharacter;


	private void Start() {
		supportList = new EntryList<SupportListEntry>(maxEntries);
	}

	public void CreateList() {
		supportList.ResetList();

		for (int i = 0; i < availableUnits.stats.Count; i++) {
			Transform t = Instantiate(entryPrefab, transform);
			SupportListEntry support = supportList.CreateEntry(t);
			support.FillData(availableUnits.stats[i]);
		}
		entryPrefab.gameObject.SetActive(false);
	}

	public void MoveSelection(int dir) {
		if (!detailedMode) {
			supportList.Move(dir);
		}
		else {
			do {
				supportList.Move(dir);
			} while (!supportList.GetEntry().gameObject.activeSelf);
		}
	}

	public void SelectCharacter() {
		if(detailedMode)
			return;

		detailedMode = true;
		selectedCharacter = supportList.GetEntry().uuid;
		selectName.text = supportList.GetEntry().entryName.text;
		selectIcon.sprite = supportList.GetEntry().icon.sprite;
		CreateSupportEntry();
		supportList.ForcePosition(0);
		if(!supportList.GetEntry().gameObject.activeSelf)
			supportList.Move(1);
	}

	public bool DeselectCharacter() {
		if (detailedMode) {
			detailedMode = false;
			for (int i = 0; i < availableUnits.stats.Count; i++) {
				supportList.GetEntry(i).show = true;
				supportList.GetEntry(i).SetDark(false);
				supportList.GetEntry(i).SetSupportValue(null, 0);
				supportList.GetEntry(i).gameObject.SetActive(true);
			}
			selectName.text = "";
			selectIcon.sprite = null;
			supportList.ForcePosition(oldIndex);
			return false;
		}

		return true;
	}

	private void CreateSupportEntry() {
		oldIndex = supportList.GetPosition();
		for (int i = 0; i < availableUnits.stats.Count; i++) {
			if (supportList.GetEntry(i).uuid == selectedCharacter) {
				supportList.GetEntry(i).show = false;
			}
			else {
				SupportTuple tuple = availableUnits.stats[i].charData.GetSupport(selectedCharacter);
				int value = availableUnits.stats[i].GetSupportValue(selectedCharacter);

				supportList.GetEntry(i).SetDark(tuple == null);
				supportList.GetEntry(i).SetSupportValue(tuple, value);
			}
		}
	}

}
