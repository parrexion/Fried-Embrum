using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseHousing : InputReceiverDelegate {

	private enum State { MAIN, HOUSE, SUPPORT }

	[Header("Button menu")]
	public MyButtonList buttons;
	public PlayerData playerData;
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

	private State currentMenu;


	private void Start() {
		currentMenu = State.MAIN;
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
		if (currentMenu == State.MAIN) {
			int currentIndex = buttons.GetPosition();
			if (currentIndex == 0) {
				currentMenu = State.HOUSE;
				basicCanvas.SetActive(false);
				houseCanvas.SetActive(true);
				housingController.CreateHousing();
				UpdateSelectedHouse();
				menuAcceptEvent.Invoke();
			}
			else if (currentIndex == 1) {
				currentMenu = State.SUPPORT;
				basicCanvas.SetActive(false);
				supportCanvas.SetActive(true);
				supportList.CreateList();
				menuAcceptEvent.Invoke();
			}
		}
		else if (currentMenu == State.HOUSE) {
			housingController.SelectClick();
			UpdateSelectedHouse();
			menuAcceptEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			supportList.SelectCharacter();
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (currentMenu == State.MAIN) {
			MenuChangeDelay(MenuMode.BASE_MAIN);
			menuBackEvent.Invoke();
		}
		else if (currentMenu == State.HOUSE) {
			bool res = housingController.BackClicked();
			if (res) {
				currentMenu = State.MAIN;
				basicCanvas.SetActive(true);
				houseCanvas.SetActive(false);
			}
			menuBackEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			bool res = supportList.DeselectCharacter();
			if (res) {
				currentMenu = State.MAIN;
				basicCanvas.SetActive(true);
				supportCanvas.SetActive(false);
			}
			menuBackEvent.Invoke();
		}
	}

	public override void OnUpArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(-1);
			housingChangedEvent.Invoke();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.HOUSE) {
			housingController.MoveVertical(-1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			supportList.MoveVertical(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(1);
			housingChangedEvent.Invoke();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.HOUSE) {
			housingController.MoveVertical(1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			supportList.MoveVertical(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (currentMenu == State.HOUSE) {
			housingController.MoveHorizontal(-1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			supportList.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (currentMenu == State.HOUSE) {
			housingController.MoveHorizontal(1);
			UpdateSelectedHouse();
			menuMoveEvent.Invoke();
		}
		else if (currentMenu == State.SUPPORT) {
			supportList.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
	}

	private void UpdateSelectedHouse() {
		Room currentRoom = housingController.GetCurrentRoom();
		StatsContainer stats = playerData.stats[currentRoom.residentIndex];
		roomNumber.text = housingController.GetRoomName();
		characterName.text = (stats != null) ? stats.charData.entryName : "";
		characterClass.text = (stats != null) ? stats.currentClass.entryName : "";
		characterLevel.text = (stats != null) ? "Level  " + stats.level : "";
		portrait.sprite = (stats != null) ? stats.charData.bigPortrait : null;

		List<Room> neighbours = currentRoom.house.GetNeighbours(currentRoom);
		string nr1 = "", nr2 = "";
		if (stats != null) {
			if (neighbours.Count > 0) {
				StatsContainer neigh1 = playerData.stats[neighbours[0].residentIndex];
				SupportContainer supportCon1 = playerData.baseInfo[neighbours[0].residentIndex];
				nr1 += neigh1.charData.entryName;
				SupportTuple support = neigh1.charData.GetSupport(stats.charData);
				if (support != null) {
					int supportValue = supportCon1.GetSupportValue(neigh1.charData).value;
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
				StatsContainer neigh2 = playerData.stats[neighbours[1].residentIndex];
				SupportContainer supportCon2 = playerData.baseInfo[neighbours[1].residentIndex];
				nr2 += neigh2.charData.entryName;
				SupportTuple support = neigh2.charData.GetSupport(stats.charData);
				if (support != null) {
					int supportValue = supportCon2.GetSupportValue(neigh2.charData).value;
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
