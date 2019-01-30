using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrepController : InputReceiver {

	public ScrObjEntryReference currentMapEntry;
	public SaveListVariable playerData;
	public PrepListVariable prepList;
	
	public UnityEvent startTurnEvent;

	[Header("Main menu")]
	public MyButton[] mainButtons;
	private int mainIndex;
	private int menuMode;

	[Header("Handlers")]
	public PrepCharacterSelect characterSelect;

	[Header("Views")]
	public GameObject mainMenuView;
	public GameObject characterSelectView;


	private void Start() {
		mainMenuView.SetActive(false);
		currentMenuMode.value = (int)MenuMode.PREP;
		menuModeChangedEvent.Invoke();
		ShowBattlePrep();
		GeneratePrepList();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.PREP);
		UpdateButtons();
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
		if (!active)
			return;
		if (menuMode == 0) {
			mainIndex = OPMath.FullLoop(0, mainButtons.Length-1, mainIndex-1);
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
			mainIndex = OPMath.FullLoop(0, mainButtons.Length-1, mainIndex+1);
			UpdateButtons();
        }
        else if (menuMode == 1) {
            characterSelect.MoveSelection(1);
        }
	}

	public override void OnLeftArrow() {
		throw new System.NotImplementedException();
	}

	public override void OnRightArrow() {
		throw new System.NotImplementedException();
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
				currentMenuMode.value = (int)MenuMode.FORMATION;
				menuModeChangedEvent.Invoke();
				mainMenuView.SetActive(false);
			}
		}
		else if (menuMode == 1) {
			characterSelect.SelectCharacter();
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
