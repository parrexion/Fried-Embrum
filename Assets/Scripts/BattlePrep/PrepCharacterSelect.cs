﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepCharacterSelect : MonoBehaviour {

	public PlayerData playerData;
	public PrepListVariable prepList;
	public ScrObjEntryReference currentMap;

	[Header("EntryList")]
	public Transform entryPrefab;
	public Transform listParent;
	public int visibleSize;
	private EntryList<PrepCharacterEntry> entryList;
	private int playerCap;

	[Header("SelectCharacterInfo")]
	public TMPro.TextMeshProUGUI playerCapText;


	public void GenerateList() {
		if (entryList == null)
			entryList = new EntryList<PrepCharacterEntry>(visibleSize);
		entryList.ResetList();

		MapEntry map = (MapEntry)currentMap.value;
		playerCap = map.spawnPoints.Count;

		for (int i = 0; i < prepList.values.Count; i++) {
			Transform entry = Instantiate(entryPrefab, listParent.transform);
			PrepCharacterEntry pce = entryList.CreateEntry(entry);
			pce.FillData(playerData.stats[prepList.values[i].index], null, prepList.values[i]);
		}
		MoveSelection(0);
		ShowInfo();
		entryPrefab.gameObject.SetActive(false);
	}

	public void ShowInfo() {
		int selected = CountSelected();
		playerCapText.text = string.Format("{0} / {1}", selected, playerCap);
	}

	public void MoveSelection(int dir) {
		entryList.Move(dir);
	}

	public void SelectCharacter() {
		PrepCharacter pc = prepList.values[entryList.GetPosition()];
		int sum = CountSelected();
		if (pc.selected) {
			if (sum > 1 && !pc.forced)
				pc.selected = false;
		}
		else if (sum < playerCap && !pc.locked) {
			pc.selected = true;
		}
		entryList.GetEntry().SetDark(!pc.selected || pc.locked);
		ShowInfo();
	}

	public void LeaveMenu() {
		prepList.SortListPicked();
	}

	private int CountSelected() {
		int selected = 0;
		for (int i = 0; i < entryList.Size; i++) {
			if (prepList.values[i].selected) {
				selected++;
			}
		}
		return selected;
	}
}
