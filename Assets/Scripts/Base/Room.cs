using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour {

	public int number;
	public Image occupied;
	public Image cursor;
	public StatsContainer resident;

	public bool selected;


	private void Start() {
		cursor.enabled = false;
	}

	public void SetResident(StatsContainer character, int index) {
		number = index;
		resident = character;
		UpdateAvailablity();
	}

	public void UpdateAvailablity() {
		occupied.color = (resident != null) ? new Color(0.8f,0.6f,0) : Color.white;
	}

	public void SetHover(bool hover) {
		cursor.enabled = hover || selected;
		cursor.color = (hover) ? Color.white : Color.grey;
	}

	public void SetSelect(bool select) {
		selected = select;
	}


	public static void SwapRoom(Room r1, Room r2) {
		StatsContainer temp = r1.resident;
		r1.resident = r2.resident;
		r2.resident = temp;
		r1.SetSelect(false);
		r2.SetSelect(false);
		r1.SetHover(false);
		r2.SetHover(false);
		r1.UpdateAvailablity();
		r2.UpdateAvailablity();
	}
}
