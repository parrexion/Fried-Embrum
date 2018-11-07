using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseHousing : InputReceiver {

	[Header("Button menu")]
	public MyButton[] buttons;
    public SaveListVariable saveList;
	public IntVariable currentIndex;
    public UnityEvent housingChangedEvent;

    [Header("Housing")]
    public GameObject roomGrid;
    private House[] houses;
	private House selectedHouse;


	private void Start () {
		currentIndex.value = 0;
		SetupButtons();
        houses = roomGrid.GetComponentsInChildren<House>();
        for (int i = 0; i < houses.Length; i++) {
            houses[i].SetupRooms(null,null,null);
        }
		houses[0].SetupRooms(saveList.stats[0].charData, null, null);
		houses[1].SetupRooms(null, saveList.stats[1].charData, null);
		houses[2].SetupRooms(null, null, saveList.stats[2].charData);

		selectedHouse = houses[0].SelectRoom(0);
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_HOUSE);
		UpdateButtons();
	}

    public override void OnUpArrow() {
		if (!active)
			return;

		// currentIndex.value--;
		// if (currentIndex.value < 0) {
		// 	currentIndex.value = saveList.stats.Count-1;
		// }
		// UpdateButtons();
		// housingChangedEvent.Invoke();

		selectedHouse = selectedHouse.MoveUp();
	}

    public override void OnDownArrow() {
		if (!active)
			return;

		// currentIndex.value++;
		// if (currentIndex.value >= saveList.stats.Count) {
		// 	currentIndex.value = 0;
		// }
		// UpdateButtons();
		// housingChangedEvent.Invoke();

		selectedHouse = selectedHouse.MoveDown();
	}
    
	public void SetupButtons() {
		// for (int i = 0; i < buttons.Length; i++) {
		// 	if (i < saveList.stats.Count) {
		// 		buttons[i].buttonText.text = saveList.stats[i].charData.entryName;
		// 		buttons[i].SetSelected(i == currentIndex.value);
		// 	}
		// 	else {
		// 		buttons[i].gameObject.SetActive(false);
		// 	}
		// }
	}

	private void UpdateButtons() {
		for (int i = 0; i < saveList.stats.Count; i++) {
			buttons[i].SetSelected(i == currentIndex.value);
		}
	}

    public override void OnLeftArrow() {
		selectedHouse = selectedHouse.MoveLeft();
	}
    
    public override void OnRightArrow() {
		
		selectedHouse = selectedHouse.MoveRight();
	}


    public override void OnBackButton() { }
    public override void OnLButton() { }
    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}
