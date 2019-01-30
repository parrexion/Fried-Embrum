using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepCharacterSelect : MonoBehaviour {

	public SaveListVariable playerData;
	public PrepListVariable prepList;
	public ScrObjEntryReference currentMap;

	[Header("EntryList")]
	public Transform entryPrefab;
	public Transform listParent;
	private List<PrepCharacterEntry> entryList = new List<PrepCharacterEntry>();
	private int listSize;
	private int playerCap;
	private int currentIndex;

	[Header("SelectCharacterInfo")]
	public TMPro.TextMeshProUGUI playerCapText;


	public void GenerateList() {
		listSize = prepList.preps.Count;
		entryList = new List<PrepCharacterEntry>();
		currentIndex = 0;
		MapEntry map = (MapEntry)currentMap.value;
		playerCap = map.spawnPoints.Count;

		for (int i = listParent.transform.childCount-1; i > 0; i--) {
			Destroy(listParent.transform.GetChild(i).gameObject);
		}
		
		for (int i = 0; i < listSize; i++) {
			CreateListEntry(playerData.stats[prepList.preps[i].index], prepList.preps[i]);
		}
		MoveSelection(0);
		ShowInfo();
		entryPrefab.gameObject.SetActive(false);
	}

	private void CreateListEntry(StatsContainer stats, PrepCharacter character) {
		Transform entry = Instantiate(entryPrefab, listParent.transform);
		PrepCharacterEntry pce = entry.GetComponent<PrepCharacterEntry>();
		pce.FillData(stats, character);
		entryList.Add(pce);
		entry.gameObject.SetActive(true);
	}

	public void ShowInfo() {
		int selected = CountSelected();
		playerCapText.text = string.Format("{0} / {1}", selected, playerCap);
	}

	public void MoveSelection(int dir) {
		currentIndex = OPMath.FullLoop(0, listSize-1, currentIndex + dir);
		for (int i = 0; i < listSize; i++) {
			entryList[i].SetHighlight(currentIndex == i);
		}
	}

	public void SelectCharacter() {
		PrepCharacter pc = prepList.preps[currentIndex];
		int sum = CountSelected();
		if (pc.selected) {
			if (sum > 1 && !pc.forced)
				pc.selected = false;
		}
		else if (sum < playerCap && !pc.locked) {
			pc.selected = true;
		}
		entryList[currentIndex].SetDark(!pc.selected || pc.locked);
		ShowInfo();
	}

	public void LeaveMenu() {
		prepList.SortListPicked();
	}

	private int CountSelected() {
		int selected = 0;
		for (int i = 0; i < entryList.Count; i++) {
			if (prepList.preps[i].selected) {
				selected++;
			}
		}
		return selected;
	}
}
