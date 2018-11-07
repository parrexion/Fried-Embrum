using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour {

	public Image occupied;
	public Image cursor;
	public CharData resident;


	private void Start() {
		cursor.enabled = false;
	}

	public void SetResident(CharData character) {
		resident = character;
		UpdateAvailablity();
	}

	public void UpdateAvailablity() {
		occupied.color = (resident != null) ? new Color(0.8f,0.6f,0) : Color.white;
	}

	public void SetSelect(bool selected) {
		cursor.enabled = selected;
	}
}
