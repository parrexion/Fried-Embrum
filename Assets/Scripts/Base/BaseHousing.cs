using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseHousing : InputReceiverDelegate {

	private enum State { MAIN, HOUSE, SUPPORT }

	[Header("Button menu")]
	public MyButtonList buttons;
    public PlayerData saveList;
    public UnityEvent housingChangedEvent;

	[Header("Canvases")]
	public GameObject basicCanvas;
	public GameObject houseCanvas;
	public GameObject supportCanvas;

    [Header("Housing")]
	public HousingController housingController;

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
	
	private State menuMode;


	private void Start() {
		menuMode = State.MAIN;
		basicCanvas.SetActive(true);
		houseCanvas.SetActive(false);
		supportCanvas.SetActive(false);

		buttons.ResetButtons();
		buttons.AddButton("EDIT ROOMS");
		buttons.AddButton("CHECK SUPPORTS");
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_HOUSE);
		buttons.ForcePosition(0);
	}

	public override void OnOkButton() {
		if (menuMode == State.MAIN) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				menuMode = State.HOUSE;
				basicCanvas.SetActive(false);
				houseCanvas.SetActive(true);
				housingController.CreateHousing();
				UpdateSelectedHouse();
				menuAcceptEvent.Invoke();
			}
			else if (currentIndex == 1) {
				menuMode = State.SUPPORT;
				basicCanvas.SetActive(false);
				supportCanvas.SetActive(true);
				supportList.CreateList();
				menuAcceptEvent.Invoke();
			}
		}
		else if (menuMode == State.HOUSE) {
			housingController.SelectClick();
			UpdateSelectedHouse();
			menuAcceptEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			supportList.SelectCharacter();
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (menuMode == State.HOUSE) {
			bool res = housingController.BackClicked();
			if (res) {
				menuMode = State.MAIN;
				basicCanvas.SetActive(true);
				houseCanvas.SetActive(false);
			}
			menuBackEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			bool res = supportList.DeselectCharacter();
			if (res) {
				menuMode = State.MAIN;
				basicCanvas.SetActive(true);
				supportCanvas.SetActive(false);
			}
			menuBackEvent.Invoke();
		}
	}

    public override void OnUpArrow() {
		if (menuMode == State.MAIN) {
			buttons.Move(-1);
			housingChangedEvent.Invoke();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.HOUSE) {
			housingController.MoveVertical(-1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			supportList.MoveVertical(-1);
			menuMoveEvent.Invoke();
		}
	}

    public override void OnDownArrow() {
		if (menuMode == State.MAIN) {
			buttons.Move(1);
			housingChangedEvent.Invoke();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.HOUSE) {
			housingController.MoveVertical(1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			supportList.MoveVertical(1);
			menuMoveEvent.Invoke();
		}
	}

    public override void OnLeftArrow() {
		if (menuMode == State.HOUSE) {
			housingController.MoveHorizontal(-1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			supportList.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
	}
    
    public override void OnRightArrow() {
		if (menuMode == State.HOUSE) {
			housingController.MoveHorizontal(1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (menuMode == State.SUPPORT) {
			supportList.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
	}

	private void UpdateSelectedHouse() {
		Room currentRoom = housingController.GetCurrentRoom();
		StatsContainer data = currentRoom.resident;
		roomNumber.text = housingController.GetRoomName();
		characterName.text = (data != null) ? data.charData.entryName : "";
		characterClass.text = (data != null) ? data.classData.entryName : "";
		characterLevel.text = (data != null) ? "Level  " + data.level : "";
		portrait.sprite = (data != null) ? data.charData.bigPortrait : null;

		List<Room> neighbours = currentRoom.house.GetNeighbours(currentRoom);
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
