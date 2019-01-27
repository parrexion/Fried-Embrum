using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepCharacterSelect : MonoBehaviour {

	public SaveListVariable playerData;
	public ScrObjEntryReference currentMap;

	[Header("EntryList")]
	public Transform entryPrefab;
	private List<PrepCharacterEntry> entryList = new List<PrepCharacterEntry>();
	private int listSize;
	private int playerCap;
	private int currentIndex;

	[Header("SelectCharacterInfo")]
	public TMPro.TextMeshProUGUI playerCapText;


	private void Start() {
		MapEntry map = (MapEntry)currentMap.value;
		playerCap = map.spawnPoints.Count;
		ShowInfo();
	}

	public void GenerateList() {
		listSize = playerData.stats.Count;
		entryList = new List<PrepCharacterEntry>();
		currentIndex = 0;

		for (int i = transform.childCount-1; i > 0; i--) {
			Destroy(transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < listSize; i++) {
			CreateListEntry(playerData.stats[i], i < playerCap);
		}
		MoveSelection(0);
		entryPrefab.gameObject.SetActive(false);
	}

	private void CreateListEntry(StatsContainer stats, bool selected) {
		Transform entry = Instantiate(entryPrefab, transform);
		PrepCharacterEntry pce = entry.GetComponent<PrepCharacterEntry>();
		pce.FillData(stats, selected, true);
		entryList.Add(pce);
	}

	public void ShowInfo() {
		playerCapText.text = string.Format("{0} / {1}", 1, playerCap);
	}

	public void MoveSelection(int dir) {
		currentIndex = OPMath.FullLoop(0, listSize-1, currentIndex + dir);
		for (int i = 0; i < listSize; i++) {
			entryList[i].SetHighlight(currentIndex == i);
		}
	}
}
