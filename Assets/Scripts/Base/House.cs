using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

	public Room[] rooms;

	[Header("Neighbours")]
	public House houseUp;
	public House houseDown;
	public House houseLeft;
	public House houseRight;

	private int _roomIndex;


	public void SetupRooms(CharData res1, CharData res2, CharData res3) {
		rooms[0].SetResident(res1);
		rooms[1].SetResident(res2);
		rooms[2].SetResident(res3);
		UpdateRooms();
	}

	public void UpdateRooms() {
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i].UpdateAvailablity();
		}
	}

	public House SelectRoom(int index) {
		_roomIndex = index;
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i].SetSelect(i == index);
		}
		return this;
	}

	public House MoveLeft() {
		_roomIndex--;
		if (_roomIndex < 0) {
			if (houseLeft) {
				rooms[0].SetSelect(false);
				houseLeft.SelectRoom(houseLeft.rooms.Length-1);
				return houseLeft;
			}
			else
				_roomIndex = 0;
		}
		SelectRoom(_roomIndex);
		return this;
	}

	public House MoveRight() {
		_roomIndex++;
		if (_roomIndex >= rooms.Length) {
			if (houseRight) {
				rooms[_roomIndex-1].SetSelect(false);
				houseRight.SelectRoom(0);
				return houseRight;
			}
			else
				_roomIndex = rooms.Length-1;
		}
		SelectRoom(_roomIndex);
		return this;
	}

	public House MoveDown() {
		if (!houseDown)
			return this;
		
		rooms[_roomIndex].SetSelect(false);
		houseDown.SelectRoom(_roomIndex);
		return houseDown;
	}

	public House MoveUp() {
		if (!houseUp)
			return this;
		
		rooms[_roomIndex].SetSelect(false);
		houseUp.SelectRoom(_roomIndex);
		return houseUp;
	}
}
