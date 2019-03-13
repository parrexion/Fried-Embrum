using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattlePrepController : InputReceiverDelegate {

	private enum State { MAIN, CHAR, FORMATION, INVENTORY, OBJECTIVE, PROMPT }

	public ScrObjEntryReference currentMapEntry;
	public SaveListVariable playerData;
	public PrepListVariable prepList;
	
	public UnityEvent nextStateEvent;

	[Header("Main menu")]
	public MyButtonList mainButtons;
	private State currentState;

	[Header("Handlers")]
	public PrepCharacterSelect characterSelect;
	public PrepInventorySelect inventorySelect;

	[Header("Views")]
	public GameObject menuCollectionView;
	public GameObject mainMenuView;
	public GameObject characterSelectView;
	public GameObject inventoryView;
	public GameObject objectiveView;

	[Header("Popup")]
	public MyPrompt startPrompt;


	private void Start() {
		mainMenuView.SetActive(false);
		characterSelectView.SetActive(false);
		inventoryView.SetActive(false);
		objectiveView.SetActive(false);

		mainButtons.ResetButtons();
		mainButtons.AddButton("Select characters");
		mainButtons.AddButton("Change formation");
		mainButtons.AddButton("Change equipment");
		mainButtons.AddButton("Map objective");
		mainButtons.AddButton("Start game");

		GeneratePrepList();
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.PREP);
		mainMenuView.SetActive(active);
		menuCollectionView.SetActive(!active);
		if (active) {
			currentState = State.MAIN;
			mainButtons.ForcePosition(0);
			//SkipBattlePrep();
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
        }
        else if (currentState == State.CHAR) {
			characterSelect.MoveSelection(-1);
		}
        else if (currentState == State.INVENTORY) {
			inventorySelect.MoveSelection(-1);
		}
	}

	public override void OnDownArrow() {
		if (currentState == State.MAIN) {
			mainButtons.Move(1);
        }
        else if (currentState == State.CHAR) {
            characterSelect.MoveSelection(1);
        }
        else if (currentState == State.INVENTORY) {
			inventorySelect.MoveSelection(1);
		}
	}

	public override void OnLeftArrow() {
		if (currentState == State.INVENTORY) {
			inventorySelect.MoveHorizontal(-1);
		}
		else if (currentState == State.PROMPT) {
			startPrompt.Move(-1);
		}
	}

	public override void OnRightArrow() {
		if (currentState == State.INVENTORY) {
			inventorySelect.MoveHorizontal(1);
		}
		else if(currentState == State.PROMPT) {
			startPrompt.Move(1);
		}
	}

	public override void OnOkButton() {
		if (currentState == State.MAIN) {
			int mainIndex = mainButtons.GetPosition();
			if (mainIndex == 0) {
				currentState = State.CHAR;
				characterSelectView.SetActive(true);
				characterSelect.GenerateList();
			}
			else if (mainIndex == 1) {
				currentState = State.FORMATION;
				InputDelegateController.instance.TriggerMenuChange(MenuMode.FORMATION);
				mainMenuView.SetActive(false);
				menuCollectionView.SetActive(true);
			}
			else if (mainIndex == 2) {
				currentState = State.INVENTORY;
				inventoryView.SetActive(true);
				inventorySelect.GenerateList();
			}
			else if (mainIndex == 3) {
				currentState = State.OBJECTIVE;
				objectiveView.SetActive(true);
			}
			else if (mainIndex == 4) {
				currentState = State.PROMPT;
				startPrompt.ShowWindow("Start mission?", false);
			}
		}
		else if (currentState == State.CHAR) {
			characterSelect.SelectCharacter();
		}
		//else if (currentState == State.FORMATION) {

		//}
		else if (currentState == State.INVENTORY) {
			inventorySelect.SelectItem();
		}
		else if (currentState == State.OBJECTIVE) {
			objectiveView.SetActive(false);
			currentState = State.MAIN;
		}
		else if (currentState == State.PROMPT) {
			if (startPrompt.Click(true) == MyPrompt.Result.OK1) {
				StartMission();
			}
			currentState = State.MAIN;
		}
	}

	public override void OnBackButton() {
		if (currentState == State.CHAR) {
			currentState = State.MAIN;
			characterSelect.LeaveMenu();
			characterSelectView.SetActive(false);
		}
		else if (currentState == State.INVENTORY) {
			if (inventorySelect.DeselectItem()) {
				inventoryView.SetActive(false);
				currentState = State.MAIN;
			}
		}
		else if (currentState == State.OBJECTIVE) {
			objectiveView.SetActive(false);
			currentState = State.MAIN;
		}
		else if (currentState == State.PROMPT) {
			startPrompt.Click(false);
			currentState = State.MAIN;
		}
	}

	public override void OnStartButton() {
		StartMission();
	}

	public void ReturnToMain() {
		currentState = State.MAIN;

	}
	

	public override void OnLButton() {}
	public override void OnRButton() {}
	public override void OnXButton() {}
	public override void OnYButton() {}
}
