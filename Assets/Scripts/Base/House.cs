using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

	public int number;
	public Room[] rooms;

	[Header("Neighbours")]
	public House houseUp;
	public House houseDown;
	public House houseLeft;
	public House houseRight;

	private int _roomIndex;


	public void SetupRooms(int index, StatsContainer res1, StatsContainer res2, StatsContainer res3) {
		number = index;
		rooms[0].SetResident(res1, 1);
		rooms[1].SetResident(res2, 2);
		rooms[2].SetResident(res3, 3);
		UpdateRooms();
	}

	public void UpdateRooms() {
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i].UpdateAvailablity();
		}
	}

	public House HoverRoom(int index) {
		_roomIndex = index;
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i].SetHover(i == index);
		}
		return this;
	}

	public void SelectRoom(bool selected) {
		rooms[_roomIndex].SetSelect(selected);
	}

	public void HideRoom() {
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i].SetHover(false);
		}
	}

	public Room GetSelectedRoom() {
		return rooms[_roomIndex];
	}

	public List<Room> GetNeighbours() {
		List<Room> neighbours = new List<Room>();
		for (int i = 0; i < rooms.Length; i++) {
			if (i != _roomIndex && rooms[i].resident != null)
				neighbours.Add(rooms[i]);
		}
		return neighbours;
	}

	public House MoveLeft() {
		_roomIndex--;
		if (_roomIndex < 0) {
			if (houseLeft) {
				rooms[0].SetHover(false);
				houseLeft.HoverRoom(houseLeft.rooms.Length-1);
				return houseLeft;
			}
			else
				_roomIndex = 0;
		}
		HoverRoom(_roomIndex);
		return this;
	}

	public House MoveRight() {
		_roomIndex++;
		if (_roomIndex >= rooms.Length) {
			if (houseRight) {
				rooms[_roomIndex-1].SetHover(false);
				houseRight.HoverRoom(0);
				return houseRight;
			}
			else
				_roomIndex = rooms.Length-1;
		}
		HoverRoom(_roomIndex);
		return this;
	}

	public House MoveDown() {
		if (!houseDown)
			return this;
		
		rooms[_roomIndex].SetHover(false);
		houseDown.HoverRoom(_roomIndex);
		return houseDown;
	}

	public House MoveUp() {
		if (!houseUp)
			return this;
		
		rooms[_roomIndex].SetHover(false);
		houseUp.HoverRoom(_roomIndex);
		return houseUp;
	}
}
