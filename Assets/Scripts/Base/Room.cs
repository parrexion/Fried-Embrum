using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour {
	
	public House house;
	public Image occupied;
	public Image cursor;
	public int residentIndex = -1;

	private bool hovering;
	private bool selected;


	public void UpdateAvailablity() {
		occupied.color = (residentIndex != -1) ? new Color(0.8f,0.6f,0) : Color.white;
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
		int temp = r1.residentIndex;
		r1.residentIndex = r2.residentIndex;
		r2.residentIndex = temp;
	}
}
