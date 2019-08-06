using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadSelectionController : MonoBehaviour {

	public PlayerData playerData;
	public ScrObjEntryReference currentMap;

	public PrepListVariable[] squadLists;
	public GameObject buttonHighlight;

	[Header("Lists")]
	public Transform characterPrefab;
	public Image[] squadAreas;
	public Text[] squadTitles;
	private EntryList<SquadMemberEntry>[] characters = new EntryList<SquadMemberEntry>[3];

	private int selectionX, selectionY;
	private int targetX = -1, targetY = -1;
	private bool selectMode;
	private int squadCount = 1;
	private int[] squadLimits = new int[] { 0, 3, 1 };


	private void Start() {
		characters[0] = new EntryList<SquadMemberEntry>(8);
		characters[1] = new EntryList<SquadMemberEntry>(8);
		characters[2] = new EntryList<SquadMemberEntry>(8);
	}

	public void ResetLists() {
		selectionX = selectionY = 0;
		selectMode = false;

		MapEntry map = (MapEntry)currentMap.value;
		squadLists[0].ResetData();
		for (int i = 0; i < playerData.stats.Count; i++) {
			PrepCharacter pc = new PrepCharacter {
				index = i,
				forced = map.IsForced(playerData.stats[i].charData),
				locked = map.IsLocked(playerData.stats[i].charData)
			};
			squadLists[0].values.Add(pc);
		}
		for (int i = 1; i < squadLists.Length; i++) {
			squadLists[i].ResetData();
		}
	}

	public void GenerateLists() {
		targetX = targetY = -1;
		for (int i = 0; i < squadLists.Length; i++) {
			squadAreas[i].gameObject.SetActive(i <= squadCount);
			characters[i].ResetList();
			for (int j = 0; j < squadLists[i].values.Count; j++) {
				int index = squadLists[i].values[j].index;

				Transform t = Instantiate(characterPrefab, squadAreas[i].transform);
				SquadMemberEntry te = characters[i].CreateEntry(t);
				te.FillData(playerData.stats[index], index);
			}
		}
		characterPrefab.gameObject.SetActive(false);

		for (int i = 0; i < squadLists.Length; i++) {
			if (squadLists[i].values.Count > 0) {
				continue;
			}
			Transform t = Instantiate(characterPrefab, squadAreas[i].transform);
			SquadMemberEntry member = characters[i].CreateEntry(t);
			member.entryName.text = "EMPTY";
			member.icon.enabled = false;
			member.level.text = "";
		}

		UpdateSelection();
	}

	public bool Select() {
		if (selectionY == -1)
			return false;

		if (selectMode) {
			return MoveCharacter();
		}

		SquadMemberEntry member = characters[selectionX].GetEntry(selectionY);
		if (member.index == -1) {
			return false;
		}
		selectMode = true;
		targetX = selectionX;
		targetY = selectionY;
		UpdateSelection();
		return true;
	}

	public bool Back() {
		if (selectMode) {
			selectMode = false;
			targetX = targetY = -1;
			UpdateSelection();
			return false;
		}

		return true;
	}

	public bool MoveHorizontal(int dir) {
		if (selectionY == -1)
			return false;

		selectionX = OPMath.FullLoop(0, squadCount + 1, selectionX + dir);
		int listSize = Mathf.Max(squadLists[selectionX].values.Count - 1, 0);
		selectionY = Mathf.Min(selectionY, listSize);
		UpdateSelection();
		return true;
	}

	public bool MoveVertical(int dir) {
		if (!selectMode && selectionY + dir <= -1) {
			bool change = (selectionY + dir == -1);
			selectionY = -1;
			UpdateSelection();
			return change;
		}
		else if (selectionY == -1) {
			dir = 0;
		}
		characters[selectionX].Move(dir);
		selectionY = characters[selectionX].GetPosition();
		UpdateSelection();
		return true;
	}

	public bool MoveCharacter() {
		if (squadLists[selectionX].Count >= squadLimits[selectionX] && selectionX != 0)
			return false;

		PrepCharacter prep = squadLists[targetX].values[targetY];
		squadLists[targetX].values.RemoveAt(targetY);
		squadLists[selectionX].values.Add(new PrepCharacter() { index = prep.index });

		selectMode = false;
		targetX = -1;
		targetY = -1;
		GenerateLists();
		selectionY = squadLists[selectionX].Count - 1;
		characters[selectionX].ForcePosition(selectionY);
		return true;
	}

	public bool LaunchMission() {
		if (selectionY != -1)
			return false;
		bool hasMembers = true;
		for (int i = 1; i < squadCount + 1; i++) {
			if (squadLists[i].Count == 0) {
				hasMembers = false;
			}
		}
		return hasMembers;
	}

	private void UpdateSelection() {
		//Button
		bool buttonSelected = (selectionY == -1);
		buttonHighlight.SetActive(buttonSelected);

		//Squad titles
		for (int i = 1; i < squadTitles.Length; i++) {
			squadTitles[i].text = string.Format("Squad {0}   {1}/{2}", i, squadLists[i].Count, squadLimits[i]);
		}

		//Squad areas
		for (int x = 0; x < characters.Length; x++) {
			characters[x].SetSelection(!buttonSelected && x == selectionX && !selectMode);
			for (int y = 0; y < characters[x].Size; y++) {
				SquadMemberEntry member = characters[x].GetEntry(y);
				member.background.color = (x == targetX && y == targetY) ? Color.cyan : Color.yellow;
				member.highlight.color = (x == targetX && y == targetY) ? Color.cyan : Color.yellow;
			}
		}
		for (int i = 0; i < squadAreas.Length; i++) {
			squadAreas[i].color = (selectMode && i == selectionX) ? Color.cyan : Color.yellow;
		}
	}
}
