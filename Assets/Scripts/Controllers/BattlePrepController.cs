﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattlePrepController : InputReceiverDelegate {

	private enum State { MAIN, CHAR, FORMATION, INVENTORY, OBJECTIVE, PROMPT }

	public ScrObjEntryReference currentMapEntry;
	public PlayerData playerData;
	public PrepListVariable prepList;
	
	public UnityEvent nextStateEvent;
	public UnityEvent respawnCharactersEvent;

	[Header("Main menu")]
	public MyButtonList mainButtons;
	private State currentState;

	[Header("Handlers")]
	public PrepCharacterSelect characterSelect;
	public PrepInventorySelect inventorySelect;
	public ObjectiveController objective; 

	[Header("Views")]
	public GameObject menuCollectionView;
	public GameObject mainMenuView;
	public GameObject buttonMenuView;
	public GameObject characterSelectView;
	public GameObject inventoryView;
	public GameObject objectiveView;

	[Header("Popup")]
	public MyPrompt startPrompt;


	private void Start() {
		mainMenuView.SetActive(false);
		buttonMenuView.SetActive(false);
		characterSelectView.SetActive(false);
		inventoryView.SetActive(false);

		mainButtons.ResetButtons();
		mainButtons.AddButton("Select characters");
		mainButtons.AddButton("Change formation");
		mainButtons.AddButton("Change equipment");
		mainButtons.AddButton("Map objective");
		mainButtons.AddButton("Start game");

		GeneratePrepList();
		mainButtons.ForcePosition(0);
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.PREP);
		mainMenuView.SetActive(active);
		if (active) {
			menuCollectionView.SetActive(!active);
			buttonMenuView.SetActive(true);
			currentState = State.MAIN;
		}
	}

	private void GeneratePrepList() {
		MapEntry map = (MapEntry)currentMapEntry.value;
		int playerCap = map.spawnPoints.Count;
		prepList.values = new List<PrepCharacter>();
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
			prepList.values.Add(pc);
		}
		prepList.SortListPicked();
	}

	/// <summary>
	/// Called when the StartBattlePrepEvent is called.
	/// </summary>
	public void SkipBattlePrep() {
		MapEntry map = (MapEntry)currentMapEntry.value;
		if (map.skipBattlePrep) {
			StartMission();
			return;
		}
	}

	/// <summary>
	/// Ends battle prep and starts the mission.
	/// </summary>
	public void StartMission() {
		mainMenuView.SetActive(false);
		menuCollectionView.SetActive(true);
		nextStateEvent.Invoke();
	}

	public override void OnUpArrow() {
		if (currentState == State.MAIN) {
			mainButtons.Move(-1);
			menuMoveEvent.Invoke();
        }
        else if (currentState == State.CHAR) {
			characterSelect.MoveSelection(-1);
			menuMoveEvent.Invoke();
		}
        else if (currentState == State.INVENTORY) {
			inventorySelect.MoveSelection(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (currentState == State.MAIN) {
			mainButtons.Move(1);
			menuMoveEvent.Invoke();
        }
        else if (currentState == State.CHAR) {
            characterSelect.MoveSelection(1);
			menuMoveEvent.Invoke();
        }
        else if (currentState == State.INVENTORY) {
			inventorySelect.MoveSelection(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (currentState == State.INVENTORY) {
			inventorySelect.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.PROMPT) {
			startPrompt.Move(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (currentState == State.INVENTORY) {
			inventorySelect.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
		else if(currentState == State.PROMPT) {
			startPrompt.Move(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (currentState == State.MAIN) {
			int mainIndex = mainButtons.GetPosition();
			if (mainIndex == 0) {
				currentState = State.CHAR;
				buttonMenuView.SetActive(false);
				characterSelectView.SetActive(true);
				characterSelect.GenerateList();
				menuAcceptEvent.Invoke();
			}
			else if (mainIndex == 1) {
				currentState = State.FORMATION;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.FORMATION);
				mainMenuView.SetActive(false);
				menuCollectionView.SetActive(true);
				menuAcceptEvent.Invoke();
			}
			else if (mainIndex == 2) {
				currentState = State.INVENTORY;
				buttonMenuView.SetActive(false);
				inventoryView.SetActive(true);
				inventorySelect.GenerateList();
				menuAcceptEvent.Invoke();
			}
			else if (mainIndex == 3) {
				currentState = State.OBJECTIVE;
				buttonMenuView.SetActive(false);
				objectiveView.SetActive(true);
				objective.UpdateState(true);
				menuAcceptEvent.Invoke();
			}
			else if (mainIndex == 4) {
				currentState = State.PROMPT;
				startPrompt.ShowWindow("Start mission?", false);
				menuAcceptEvent.Invoke();
			}
		}
		else if (currentState == State.CHAR) {
			characterSelect.SelectCharacter();
			menuAcceptEvent.Invoke();
		}
		//else if (currentState == State.FORMATION) { }
		else if (currentState == State.INVENTORY) {
			inventorySelect.SelectItem();
			menuAcceptEvent.Invoke();
		}
		else if (currentState == State.OBJECTIVE) {
			objective.UpdateState(false);
			objectiveView.SetActive(false);
			buttonMenuView.SetActive(true);
			currentState = State.MAIN;
			menuAcceptEvent.Invoke();
		}
		else if (currentState == State.PROMPT) {
			if (startPrompt.Click(true) == MyPrompt.Result.OK1) {
				StartMission();
				menuAcceptEvent.Invoke();
			}
			currentState = State.MAIN;
		}
	}

	public override void OnBackButton() {
		if (currentState == State.CHAR) {
			currentState = State.MAIN;
			bool res = characterSelect.LeaveMenu();
			buttonMenuView.SetActive(true);
			characterSelectView.SetActive(false);
			if (res) {
				respawnCharactersEvent.Invoke();
				menuBackEvent.Invoke();
			}
		}
		else if (currentState == State.INVENTORY) {
			if (inventorySelect.DeselectItem()) {
				buttonMenuView.SetActive(true);
				inventoryView.SetActive(false);
				currentState = State.MAIN;
				menuBackEvent.Invoke();
			}
		}
		else if (currentState == State.OBJECTIVE) {
			objective.UpdateState(false);
			objectiveView.SetActive(false);
			buttonMenuView.SetActive(true);
			currentState = State.MAIN;
			menuBackEvent.Invoke();
		}
		else if (currentState == State.PROMPT) {
			startPrompt.Click(false);
			buttonMenuView.SetActive(true);
			currentState = State.MAIN;
			menuBackEvent.Invoke();
		}
	}

	public override void OnStartButton() {
		if (currentState != State.MAIN)
			return;
		currentState = State.PROMPT;
		startPrompt.ShowWindow("Start mission?", false);
		menuAcceptEvent.Invoke();
	}

	public void ReturnToMain() {
		currentState = State.MAIN;
	}
	

	public override void OnLButton() {}
	public override void OnRButton() {}
	public override void OnXButton() {}
	public override void OnYButton() {}
}
