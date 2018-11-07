using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseMission : InputReceiver {

	public MyButton[] buttons;
	public GameObject[] mapPoints;
	public MapEntry[] maps;

	public IntVariable currentIndex;
	public UnityEvent missionChangedEvent;


	private void Start () {
		currentIndex.value = 0;
		SetupButtons();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_MISSION);
		currentIndex.value = 0;
		UpdateButtons();
	}

    public override void OnUpArrow() {
		if (!active)
			return;

		currentIndex.value--;
		if (currentIndex.value < 0) {
			currentIndex.value = maps.Length-1;
		}
		UpdateButtons();
		missionChangedEvent.Invoke();
	}

    public override void OnDownArrow() {
		if (!active)
			return;

		currentIndex.value++;
		if (currentIndex.value >= maps.Length) {
			currentIndex.value = 0;
		}
		UpdateButtons();
		missionChangedEvent.Invoke();
	}

    public override void OnOkButton() {

	}

    public override void OnBackButton() {

	}

	private void SetupButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			if (i < maps.Length) {
				buttons[i].buttonText.text = maps[i].entryName;
				buttons[i].SetSelected(i == currentIndex.value);
			}
			else {
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateButtons() {
		for (int i = 0; i < maps.Length; i++) {
			buttons[i].SetSelected(i == currentIndex.value);
			mapPoints[i].SetActive(i == currentIndex.value);
		}
	}

    public override void OnLButton() { }
    public override void OnLeftArrow() { }
    public override void OnRButton() { }
    public override void OnRightArrow() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

