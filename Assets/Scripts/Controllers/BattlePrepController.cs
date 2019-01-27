using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrepController : InputReceiver {

	public ScrObjEntryReference currentMapEntry;
	public SaveListVariable playerData;
	
	public UnityEvent startTurnEvent;

	[Header("Main menu")]
	public MyButton[] mainButtons;
	private int mainIndex;
	private int menuMode;

	[Header("Handlers")]
	public PrepCharacterSelect characterSelect;

	[Header("Views")]
	public GameObject mainMenu;
	public GameObject characterSelectView;


	private void Start() {
		mainMenu.SetActive(false);
		currentMenuMode.value = (int)MenuMode.PREP;
		menuModeChangedEvent.Invoke();
		ShowBattlePrep();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.PREP);
		UpdateButtons();
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

		mainMenu.SetActive(true);
		characterSelectView.SetActive(false);
	}

	/// <summary>
	/// Ends battle prep and starts the battle.
	/// </summary>
	public void StartMission() {
		mainMenu.SetActive(false);
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
			menuMode = 1;
			characterSelect.GenerateList();
			characterSelectView.SetActive(true);
		}
	}

	public override void OnBackButton() {
		if (!active)
			return;
		if (menuMode == 1) {
			menuMode = 0;
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
