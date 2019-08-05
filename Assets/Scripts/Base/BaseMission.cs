using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseMission : InputReceiverDelegate {
	private enum State { INFO, SQUAD, PROMPT }

	public MissionInfoController missionInfo;
	public SquadSelectionController squadSelection;
	private State state = State.INFO;

	[Header("Views")]
	public GameObject infoView;
	public GameObject squadView;

	[Header("Mission Prompt")]
	public MyPrompt startPrompt;
	public UnityEvent startMissionEvent;



	private void Start() {
		state = State.INFO;
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.BASE_MISSION);
		if (active) {
			squadView.SetActive(false);
			infoView.SetActive(true);
			missionInfo.SetupList();
		}
	}

	public override void OnOkButton() {
		if (state == State.INFO) {
			state = State.SQUAD;
			missionInfo.Select();
			squadSelection.ResetLists();
			squadSelection.GenerateLists();
			infoView.SetActive(false);
			squadView.SetActive(true);
			menuAcceptEvent.Invoke();
		}
		else if (state == State.SQUAD) {
			if (squadSelection.LaunchMission()) {
				state = State.PROMPT;
				startPrompt.ShowYesNoPopup("Start mission?", false);
					menuAcceptEvent.Invoke();
			} else {
				if (squadSelection.Select()) {
					menuAcceptEvent.Invoke();
				}
				else {
					menuFailEvent.Invoke();
				}
			}
		}
		else if (state == State.PROMPT) {
			if (startPrompt.Click(true) == MyPrompt.Result.OK1) {
				menuAcceptEvent.Invoke();
				startMissionEvent.Invoke();
			}
			else {
				OnBackButton();
			}
		}
	}

	public override void OnBackButton() {
		if (state == State.INFO) {
			MenuChangeDelay(MenuMode.BASE_MAIN);
		}
		else if (state == State.SQUAD) {
			if (squadSelection.Back()) {
				state = State.INFO;
				squadView.SetActive(false);
				infoView.SetActive(true);
			}
		}
		else if (state == State.PROMPT) {
			startPrompt.Click(false);
			state = State.SQUAD;
		}
		menuBackEvent.Invoke();
	}

	public override void OnUpArrow() {
		bool res = false;
		if (state == State.INFO) {
			res = missionInfo.Move(-1);
		}
		else if (state == State.SQUAD) {
			res = squadSelection.MoveVertical(-1);
		}

		if (res)
			menuMoveEvent.Invoke();
	}

	public override void OnDownArrow() {
		bool res = false;
		if (state == State.INFO) {
			res = missionInfo.Move(1);
		}
		else if (state == State.SQUAD) {
			res = squadSelection.MoveVertical(1);
		}

		if (res)
			menuMoveEvent.Invoke();
	}

	public override void OnLeftArrow() {
		bool res = false;
		if (state == State.SQUAD) {
			res = squadSelection.MoveHorizontal(-1);
		}
		else if (state == State.PROMPT) {
			startPrompt.Move(-1);
			menuMoveEvent.Invoke();
		}

		if (res)
			menuMoveEvent.Invoke();
	}

	public override void OnRightArrow() {
		bool res = false;
		if (state == State.SQUAD) {
			res = squadSelection.MoveHorizontal(1);
		}
		else if (state == State.PROMPT) {
			startPrompt.Move(1);
			menuMoveEvent.Invoke();
		}

		if (res)
			menuMoveEvent.Invoke();
	}


	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnStartButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}

