using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

	public int number;
	public Room[] rooms;


	public void ResetRooms(int index) {
		number = index;
		rooms[0].SetResident(null);
		rooms[1].SetResident(null);
		rooms[2].SetResident(null);
	}

	public List<Room> GetNeighbours(Room main) {
		List<Room> neighbours = new List<Room>();
		bool found = false;
		for (int i = 0; i < rooms.Length; i++) {
			if (rooms[i] == main) {
				found = true;
			}
			else if (rooms[i].resident != null) {
				neighbours.Add(rooms[i]);
			}
		}
		return (found) ? neighbours : new List<Room>();
	}
	
}
