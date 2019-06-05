using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingController : MonoBehaviour {

	public PlayerData playerData;
	public ScrObjLibraryVariable charLibrary;
	public ScrObjLibraryVariable classLibrary;

	[Header("Housing")]
	public IntVariable houseCount;
	public IntVariable roomCount;
	public int rowSize;
	public Transform housePrefab;
	public Transform houseParent;

	private List<House> houses = new List<House>();
	private int position;
	private int selectPosition;


	public void CreateHousing() {
		position = 0;
		selectPosition = -1;

		for (int i = 0; i < houses.Count; i++) {
			Destroy(houses[i].gameObject);
		}
		houses.Clear();

		housePrefab.gameObject.SetActive(true);
		for (int i = 0; i < houseCount.value; i++) {
			Transform t = Instantiate(housePrefab, houseParent);
			houses.Add(t.GetComponent<House>());
			houses[i].ResetRooms(i+1);
		}
		
		List<int> noRoomers = new List<int>();
		for (int i = 0; i < playerData.stats.Count; i++) {
			int roomNo = playerData.baseInfo[i].roomNo;
			if (roomNo != -1) {
				Room room = GetRoom(roomNo);
				room.residentIndex = i;
			}
			else {
				noRoomers.Add(i);
			}
		}
		int index = 0;
		for (int i = 0; i < houseCount.value; i++) {
			for (int j = 0; j < roomCount.value; j++) {
				if (index >= noRoomers.Count)
					break;
				if (houses[i].rooms[j].residentIndex == -1) {
					playerData.baseInfo[noRoomers[index]].roomNo = i * roomCount.value + j;
					houses[i].rooms[j].residentIndex = noRoomers[index];
					index++;
				}
			}
		}
		housePrefab.gameObject.SetActive(false);
		UpdateSelection();
	}

	public void MoveHorizontal(int dir) {
		position = OPMath.FullLoop(0, houseCount.value * roomCount.value, position + dir);
		UpdateSelection();
	}

	public void MoveVertical(int dir) {
		position = OPMath.FullLoop(0, houseCount.value * roomCount.value, position + dir * roomCount.value * rowSize);
		UpdateSelection();
	}

	public void SelectClick() {
		if (selectPosition == -1) {
			selectPosition = position;
		}
		else {
			//Time to swap the rooms!
			Room firstRoom = GetRoom(selectPosition);
			Room secondRoom = GetRoom(position);
			
			if (firstRoom.residentIndex != -1)
				playerData.baseInfo[firstRoom.residentIndex].roomNo = position;
			if (secondRoom.residentIndex != -1)
				playerData.baseInfo[secondRoom.residentIndex].roomNo = selectPosition;

			Room.SwapRoom(firstRoom, secondRoom);
			selectPosition = -1;
		}
		UpdateSelection();
	}

	public bool BackClicked() {
		if (selectPosition != -1) {
			selectPosition = -1;
			UpdateSelection();
			return false;
		}
		else {
			return true;
		}
	}

	private void UpdateSelection() {
		for (int i = 0; i < houseCount.value; i++) {
			for (int j = 0; j < roomCount.value; j++) {
				houses[i].rooms[j].UpdateAvailablity();
				houses[i].rooms[j].SetSelect(false);
				houses[i].rooms[j].SetHover(false);
			}
		}
		if (selectPosition != -1) {
			GetRoom(selectPosition).SetSelect(true);
		}
		GetRoom(position).SetHover(true);
	}

	public string GetRoomName() {
		int house = position / roomCount.value;
		int room = position % roomCount.value;
		return string.Format("Room  {0} - {1}", house, room);
	}

	public House GetHouse(int position) {
		int house = position / roomCount.value;
		return houses[house];
	}

	public Room GetCurrentRoom() {
		return GetRoom(position);
	}

	public Room GetRoom(int position) {
		int house = position / roomCount.value;
		int room = position % roomCount.value;
		return houses[house].rooms[room];
	}
}
