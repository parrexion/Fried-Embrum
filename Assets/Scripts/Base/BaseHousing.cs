using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

	[Header("Information")]
	public Text roomNumber;
	public Text characterName;
	public Text characterClass;
	public Text characterLevel;
	public Image portrait;
	public Text neighbourStats1;
	public Text neighbourStats2;

	private int menuMode;
	private Room targetRoom;


	private void Start() {
		currentIndex.value = 0;
		menuMode = 0;
		SetupButtons();
        houses = roomGrid.GetComponentsInChildren<House>();
        for (int i = 0; i < houses.Length; i++) {
            houses[i].SetupRooms(i+1,null,null,null);
        }
		houses[0].SetupRooms(1, saveList.stats[0], null, null);
		houses[1].SetupRooms(2, null, saveList.stats[1], null);
		houses[2].SetupRooms(3, null, null, saveList.stats[2]);
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_HOUSE);
		UpdateButtons();
	}

	public override void OnOkButton() {
		if (menuMode == 0) {
			if (currentIndex.value == 0) {
				selectedHouse = houses[0].HoverRoom(0);
				UpdateSelectedHouse();
				menuMode = 1;
				menuAcceptEvent.Invoke();
			}
		}
		else if (menuMode == 1) {
			if (!targetRoom) {
				selectedHouse.SelectRoom(true);
				targetRoom = selectedHouse.GetSelectedRoom();
			}
			else {
				Room secondRoom = selectedHouse.GetSelectedRoom();
				Room.SwapRoom(targetRoom, secondRoom);
				targetRoom = null;
				secondRoom.SetHover(true);
				UpdateSelectedHouse();
			}
		}
	}

	public override void OnBackButton() {
		if (menuMode == 0) {

		}
		else if (menuMode == 1) {
			selectedHouse.HideRoom();
			menuMode = 0;
			menuBackEvent.Invoke();
		}
	}

    public override void OnUpArrow() {
		if (!active)
			return;

		if (menuMode == 0) {
			currentIndex.value--;
			if (currentIndex.value < 0) {
				currentIndex.value = saveList.stats.Count-1;
			}
			UpdateButtons();
			housingChangedEvent.Invoke();
		}
		else if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveUp();
			UpdateSelectedHouse();
		}
	}

    public override void OnDownArrow() {
		if (!active)
			return;

		if (menuMode == 0) {
			currentIndex.value++;
			if (currentIndex.value >= saveList.stats.Count) {
				currentIndex.value = 0;
			}
			UpdateButtons();
			housingChangedEvent.Invoke();
		}
		else if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveDown();
			UpdateSelectedHouse();
		}
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
		if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveLeft();
			UpdateSelectedHouse();
		}
	}
    
    public override void OnRightArrow() {
		if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveRight();
			UpdateSelectedHouse();
		}
	}

	private void UpdateSelectedHouse() {
		Room currentRoom = selectedHouse.GetSelectedRoom();
		StatsContainer data = currentRoom.resident;
		roomNumber.text = string.Format("Room  {0} - {1}", selectedHouse.number, currentRoom.number);
		characterName.text = (data != null) ? data.charData.entryName : "";
		characterClass.text = (data != null) ? data.classData.entryName : "";
		characterLevel.text = (data != null) ? "Level  " + data.level : "";
		portrait.sprite = (data != null) ? data.charData.bigPortrait : null;
Fixa support level lista
		List<Room> neighbours = selectedHouse.GetNeighbours();
		string nr1 = "", nr2 = "";
		if (data != null) {
			if (neighbours.Count > 0) {
				nr1 += neighbours[0].resident.charData.entryName;
				SupportTuple support = neighbours[0].resident.charData.GetSupport(data.charData);
				if (support != null) {
					int supportValue = data.GetSupportValue(neighbours[0].resident.charData.uuid);
					nr1 += "\nRank " + support.CalculateLevel(supportValue) + "  " + support.GetSpeedString();
				}
				else {
					nr1 += "\nRank -  (x)";
				}
			}
			else {
				nr1 = "-Empty-";
			}

			if (neighbours.Count > 1) {
				nr2 += neighbours[1].resident.charData.entryName;
				SupportTuple support = neighbours[1].resident.charData.GetSupport(data.charData);
				if (support != null) {
					int supportValue = data.GetSupportValue(neighbours[1].resident.charData.uuid);
					nr2 += "\nRank " + support.CalculateLevel(supportValue) + "  " + support.GetSpeedString();
				}
				else {
					nr2 += "\nRank -  (x)";
				}
			}
			else {
				nr2 = "-Empty-";
			}
		}

		neighbourStats1.text = nr1;
		neighbourStats2.text = nr2;
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}
