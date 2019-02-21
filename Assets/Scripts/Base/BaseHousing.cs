using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseHousing : InputReceiverDelegate {

	[Header("Button menu")]
	public MyButtonList buttons;
    public SaveListVariable saveList;
    public UnityEvent housingChangedEvent;

	[Header("Canvases")]
	public GameObject basicCanvas;
	public GameObject houseCanvas;
	public GameObject supportCanvas;

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

	[Header("Supports")]
	public SupportList supportList;
	
	private int menuMode;
	private Room targetRoom;


	private void Start() {
		menuMode = 0;
        houses = roomGrid.GetComponentsInChildren<House>();
        for (int i = 0; i < houses.Length; i++) {
            houses[i].SetupRooms(i+1,null,null,null);
        }
		basicCanvas.SetActive(true);
		houseCanvas.SetActive(false);
		supportCanvas.SetActive(false);

		buttons.ResetButtons();
		buttons.AddButton("EDIT ROOMS");
		buttons.AddButton("CHECK SUPPORTS");
		buttons.AddButton("SUPPORT LIST");

		//DEBUG
		houses[0].SetupRooms(1, saveList.stats[0], null, null);
		houses[1].SetupRooms(2, null, saveList.stats[1], null);
		houses[2].SetupRooms(3, null, null, saveList.stats[2]);
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_HOUSE);
		buttons.ForcePosition(0);
	}

	public override void OnOkButton() {
		if (menuMode == 0) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				basicCanvas.SetActive(false);
				houseCanvas.SetActive(true);
				selectedHouse = houses[0].HoverRoom(0);
				UpdateSelectedHouse();
				menuMode = 1;
				menuAcceptEvent.Invoke();
			}
			else if (currentIndex == 1) {
				basicCanvas.SetActive(false);
				supportCanvas.SetActive(true);
				menuMode = 2;
				supportList.CreateList();
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
		else if (menuMode == 2) {
			supportList.SelectCharacter();
		}
	}

	public override void OnBackButton() {
		if (menuMode == 0) {

		}
		else if (menuMode == 1) {
			basicCanvas.SetActive(true);
			houseCanvas.SetActive(false);
			selectedHouse.HideRoom();
			menuMode = 0;
			menuBackEvent.Invoke();
		}
		else if (menuMode == 2) {
			bool res = supportList.DeselectCharacter();
			if (res) {
				basicCanvas.SetActive(true);
				supportCanvas.SetActive(false);
				menuMode = 0;
				menuBackEvent.Invoke();
			}
		}
	}

    public override void OnUpArrow() {
		if (menuMode == 0) {
			buttons.Move(-1);
			housingChangedEvent.Invoke();
		}
		else if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveUp();
			UpdateSelectedHouse();
		}
		else if (menuMode == 2) {
			supportList.MoveVertical(-1);
		}
	}

    public override void OnDownArrow() {
		if (menuMode == 0) {
			buttons.Move(1);
			housingChangedEvent.Invoke();
		}
		else if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveDown();
			UpdateSelectedHouse();
		}
		else if (menuMode == 2) {
			supportList.MoveVertical(1);
		}
	}

    public override void OnLeftArrow() {
		if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveLeft();
			UpdateSelectedHouse();
		}
		else if (menuMode == 2) {
			supportList.MoveHorizontal(-1);
		}
	}
    
    public override void OnRightArrow() {
		if (menuMode == 1) {
			selectedHouse = selectedHouse.MoveRight();
			UpdateSelectedHouse();
		}
		else if (menuMode == 2) {
			supportList.MoveHorizontal(1);
		}
	}

	private void UpdateSelectedHouse() {
		Room currentRoom = selectedHouse.GetSelectedRoom();
		StatsContainer data = currentRoom.resident;
		roomNumber.text = string.Format("Room  {0} - {1}", selectedHouse.number, currentRoom.number);
		characterName.text = (data != null) ? data.charData.entryName : "";
		characterClass.text = (data != null) ? data.classData.entryName : "";
		characterLevel.text = (data != null) ? "Level  " + data.currentLevel : "";
		portrait.sprite = (data != null) ? data.charData.bigPortrait : null;

		List<Room> neighbours = selectedHouse.GetNeighbours();
		string nr1 = "", nr2 = "";
		if (data != null) {
			if (neighbours.Count > 0) {
				nr1 += neighbours[0].resident.charData.entryName;
				SupportTuple support = neighbours[0].resident.charData.GetSupport(data.charData);
				if (support != null) {
					int supportValue = data.GetSupportValue(neighbours[0].resident.charData).value;
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
					int supportValue = data.GetSupportValue(neighbours[1].resident.charData).value;
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
