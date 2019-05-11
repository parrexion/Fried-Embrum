using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour {
	
	public House house;
	public Image occupied;
	public Image cursor;
	public StatsContainer resident;

	private bool hovering;
	private bool selected;


	public void UpdateAvailablity() {
		occupied.color = (resident != null) ? new Color(0.8f,0.6f,0) : Color.white;
	}

	public void SetHover(bool hover) {
		hovering = hover;
		UpdateCursor();
	}

	public void SetSelect(bool select) {
		selected = select;
		UpdateCursor();
	}

	private void UpdateCursor() {
		cursor.enabled = hovering || selected;
		cursor.color = (hovering) ? Color.yellow : Color.red;
	}

	public static void SwapRoom(Room r1, Room r2) {
		StatsContainer temp = r1.resident;
		r1.resident = r2.resident;
		r2.resident = temp;
	}
}
