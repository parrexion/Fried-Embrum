using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseMission : InputReceiver {

	public SaveListVariable playerData;
	public MyButton[] buttons;
	public IntVariable missionIndex;
	public UnityEvent missionChangedEvent;

	private List<MissionContainer> availableMaps = new List<MissionContainer>();
	private int currentIndex;

	[Header("Mission Info")]
	public Text missionName;
	public Text missionLocation;
	public Text missionObjective;
	public Text missionCondition;
	public Text missionRewardMoney;
	public Text missionRewardScrap;
	public Text missionRewardItem;
	public Text missionRewardItem2;

	[Header("Mission Prompt")]
	public GameObject promptView;
	public MyButton promptYesButton;
	public MyButton promptNoButton;
	public UnityEvent startMissionEvent;
	private bool promptMode;
	private int promptPosition;

	[Header("Selected Mission")]
	public ScrObjEntryReference currentMap;


	private void Start () {
		currentIndex = 0;
		promptView.SetActive(false);
		SetupButtons();
	}

    public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.BASE_MISSION);
		currentIndex = 0;
		UpdateButtons();
		ShowMissionInfo();
	}

    public override void OnUpArrow() {
		if (!active || promptMode)
			return;

		currentIndex = OPMath.FullLoop(0, availableMaps.Count - 1, currentIndex - 1);
		UpdateButtons();
		ShowMissionInfo();
		missionChangedEvent.Invoke();
	}

    public override void OnDownArrow() {
		if (!active || promptMode)
			return;

		currentIndex = OPMath.FullLoop(0, availableMaps.Count - 1, currentIndex + 1);
		UpdateButtons();
		ShowMissionInfo();
		missionChangedEvent.Invoke();
	}

    public override void OnOkButton() {
		if (!active)
			return;
		if (!promptMode) {
			promptMode = true;
			promptPosition = 0;
			ChangePrompt(0);
			promptView.SetActive(true);
		}
		else if (promptPosition == 0) {
			StartMission();
		}
		else {
			OnBackButton();
		}
	}

    public override void OnBackButton() {
		if (!active)
			return;
		if (!promptMode)
			return;
		promptMode = false;
		promptView.SetActive(false);
	}

    public override void OnLeftArrow() {
		if (!active || !promptMode)
			return;
		ChangePrompt(-1);
	}

    public override void OnRightArrow() {
		if (!active || !promptMode)
			return;
		ChangePrompt(1);
	}

	private void SetupButtons() {
		availableMaps = playerData.missions.FindAll(m => !m.cleared);
		for (int i = 0; i < buttons.Length; i++) {
			if (i < availableMaps.Count) {
				buttons[i].buttonText.text = availableMaps[i].map.entryName;
				buttons[i].SetSelected(i == currentIndex);
			}
			else {
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateButtons() {
		for (int i = 0; i < availableMaps.Count; i++) {
			buttons[i].SetSelected(i == currentIndex);
		}
		missionIndex.value = currentIndex;

	}

	private void ShowMissionInfo() {
		if (availableMaps.Count <= currentIndex)
			return;

		MapEntry map = availableMaps[currentIndex].map;
		missionName.text = map.entryName;
		missionLocation.text = "Location:  " + map.mapLocation;
		missionObjective.text = "Objective:  " + map.winCondition;
		missionCondition.text = "Lose:    " + map.loseCondition;

		missionRewardMoney.text =  "Money:  " + map.reward.money;
		missionRewardMoney.gameObject.SetActive(map.reward.money > 0);
		missionRewardScrap.text = "Scrap:  " + map.reward.scrap;
		missionRewardScrap.gameObject.SetActive(map.reward.scrap > 0);
		missionRewardItem.text = (map.reward.items.Count > 0) ? "Item:  " + map.reward.items[0].entryName : "";
		missionRewardItem.gameObject.SetActive(map.reward.items.Count > 0);
		missionRewardItem2.text = (map.reward.items.Count > 1) ? "Item2:  " + map.reward.items[1].entryName : "";
		missionRewardItem2.gameObject.SetActive(map.reward.items.Count > 1);
	}

    public void ChangePrompt(int dir) {
        if (!promptMode)
            return;

		promptPosition = (promptPosition + dir) % 2;
		promptYesButton.SetSelected(promptPosition == 0);
		promptNoButton.SetSelected(promptPosition == 1);
    }

	private void StartMission() {
		currentMap.value = availableMaps[currentIndex].map;
		Debug.Log("Start mission:  " + currentMap.value.entryName);
		startMissionEvent.Invoke();
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

