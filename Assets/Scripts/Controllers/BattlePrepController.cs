using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrepController : InputReceiverDelegate {

	public ScrObjEntryReference currentMapEntry;
	public SaveListVariable playerData;
	public PrepListVariable prepList;
	
	public UnityEvent startTurnEvent;

	[Header("Main menu")]
	public MyButton[] mainButtons;
	private int mainIndex;
	public int menuMode;

	[Header("Handlers")]
	public PrepCharacterSelect characterSelect;

	[Header("Views")]
	public GameObject menuCollectionView;
	public GameObject mainMenuView;
	public GameObject characterSelectView;
	public GameObject inventoryView;
	public GameObject objectiveView;

	[Header("Popup")]
	public GameObject popupObject;
	//public TMPro.TextMeshProUGUI popupMessage;
	public MyButton[] popupButtons;
	private int popupPosition;


	private void Start() {
		mainMenuView.SetActive(false);
		characterSelectView.SetActive(false);
		inventoryView.SetActive(false);
		objectiveView.SetActive(false);
		popupObject.SetActive(false);
		InputDelegateController.instance.TriggerMenuChange(MenuMode.PREP);
		ShowBattlePrep();
		GeneratePrepList();
	}

    public override void OnMenuModeChanged() {
		bool prevActive = active;
		active = (currentMenuMode.value == (int)MenuMode.PREP);
		mainMenuView.SetActive(active);
		menuCollectionView.SetActive(!active);
		UpdateButtons();
		if (prevActive != active) {
			ActivateDelegates(active);
		}
	}

	private void GeneratePrepList() {
		MapEntry map = (MapEntry)currentMapEntry.value;
		int playerCap = map.spawnPoints.Count;
		prepList.preps = new List<PrepCharacter>();
		for (int i = 0; i < playerData.stats.Count; i++) {
			PrepCharacter pc = new PrepCharacter {
				index = i,
				forced = map.IsForced(playerData.stats[i].charData),
				locked = map.IsLocked(playerData.stats[i].charData)
			};
			if (!pc.locked && playerCap > 0) {
				pc.selected = (playerCap > 0);
				playerCap--;
			}
			prepList.preps.Add(pc);
		}
		prepList.SortListPicked();
	}

	/// <summary>
	/// Called when the StartBattlePrepEvent is called.
	/// </summary>
	public void ShowBattlePrep() {
		MapEntry map = (MapEntry)currentMapEntry.value;
		if (map.skipBattlePrep) {
			StartMission();
			return;
		}

		mainMenuView.SetActive(true);
		characterSelectView.SetActive(false);
		menuCollectionView.SetActive(false);
	}

	private void DisplayPopup() {
		popupObject.SetActive(true);
		popupPosition = 1;
		UpdatePopupButtons(0);
	}

	private void UpdatePopupButtons(int dir) {
		popupPosition = OPMath.FullLoop(0, popupButtons.Length, popupPosition+dir);
		for (int i = 0; i < popupButtons.Length; i++) {
			popupButtons[i].SetSelected(i == popupPosition);
		}
	}

	/// <summary>
	/// Ends battle prep and starts the battle.
	/// </summary>
	public void StartMission() {
		mainMenuView.SetActive(false);
		startTurnEvent.Invoke();
	}

	public void UpdateButtons() {
		for (int i = 0; i < mainButtons.Length; i++) {
			mainButtons[i].SetSelected(i == mainIndex);
		}
	}

	public override void OnUpArrow() {
		Debug.Log("UP   " + active);
		if (!active)
			return;
		if (menuMode == 0) {
			mainIndex = OPMath.FullLoop(0, mainButtons.Length, mainIndex-1);
			UpdateButtons();
        }
        else if (menuMode == 1) {
			characterSelect.MoveSelection(-1);
		}
	}

	public override void OnDownArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			mainIndex = OPMath.FullLoop(0, mainButtons.Length, mainIndex+1);
			UpdateButtons();
        }
        else if (menuMode == 1) {
            characterSelect.MoveSelection(1);
        }
	}

	public override void OnLeftArrow() {
		if (!active)
			return;
		if (menuMode == 4)
			UpdatePopupButtons(-1);
	}

	public override void OnRightArrow() {
		if (!active)
			return;
		if(menuMode == 4)
			UpdatePopupButtons(1);
	}

	public override void OnOkButton() {
		if (!active)
			return;
		if (menuMode == 0) {
			if (mainIndex == 0) {
				menuMode = 1;
				characterSelectView.SetActive(true);
				characterSelect.GenerateList();
			}
			else if (mainIndex == 1) {
				menuMode = 2;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.FORMATION);
				mainMenuView.SetActive(false);
				menuCollectionView.SetActive(true);
			}
		}
		else if (menuMode == 1) {
			characterSelect.SelectCharacter();
		}
		else if (menuMode == 2) {

		}
		else if (menuMode == 3) {

		}
		else if (menuMode == 4) {
			DisplayPopup();
		}
	}

	public override void OnBackButton() {
		if (!active)
			return;
		if (menuMode == 1) {
			menuMode = 0;
			characterSelect.LeaveMenu();
			characterSelectView.SetActive(false);
		}
	}

	public override void OnStartButton() {
		StartMission();
	}
	

	public override void OnLButton() {}
	public override void OnRButton() {}
	public override void OnXButton() {}
	public override void OnYButton() {}
}
