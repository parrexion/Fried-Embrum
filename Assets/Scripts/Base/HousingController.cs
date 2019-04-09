using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingController : MonoBehaviour {

	public SaveListVariable saveList;

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
		
		List<StatsContainer> noRoomers = new List<StatsContainer>();
		for (int i = 0; i < saveList.stats.Count; i++) {
			int roomNo = saveList.stats[i].roomNo;
			if (roomNo != -1) {
				Room room = GetRoom(roomNo);
				room.SetResident(saveList.stats[i]);
			}
			else {
				noRoomers.Add(saveList.stats[i]);
			}
		}
		int index = 0;
		for (int i = 0; i < houseCount.value; i++) {
			for (int j = 0; j < roomCount.value; j++) {
				if (index >= noRoomers.Count)
					break;
				if (houses[i].rooms[j].resident == null) {
					noRoomers[i].roomNo = i * roomCount.value + j;
					houses[i].rooms[j].SetResident(noRoomers[index]);
					index++;
					Debug.Log("Houses in " + i + " , " + j);
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
			GetRoom(position).SetSelect(true);
		}
		else {
			Room firstRoom = GetRoom(selectPosition);
			Room secondRoom = GetRoom(position);
			Room.SwapRoom(firstRoom, secondRoom);
			selectPosition = -1;
			secondRoom.SetHover(true);
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
				houses[i].rooms[j].SetHover(false);
				houses[i].rooms[j].SetSelect(false);
			}
		}
		GetRoom(position).SetHover(true);
		if (selectPosition != -1)
			GetRoom(selectPosition).SetSelect(true);
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
