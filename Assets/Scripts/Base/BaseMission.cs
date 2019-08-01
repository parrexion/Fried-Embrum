using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseMission : InputReceiverDelegate {

	public PlayerData playerData;
	public IntVariable currentDay;
	public MyButtonList buttons;

	private List<MissionContainer> availableMaps = new List<MissionContainer>();

	[Header("Mission Info")]
	public Text missionName;
	public Text missionDesc;
	public Text missionRewardMoney;
	public Text missionRewardScrap;
	public Text missionRewardItem;
	public Text missionRewardItem2;

	[Header("Mission Prompt")]
	public MyPrompt startPrompt;
	public UnityEvent startMissionEvent;
	private bool promptMode;

	[Header("Selected Mission")]
	public StringVariable currentChapterId;

	[Header("Planets")]
	public Image planetImage;
	public Text planetName;
	public PlanetInfo[] planets;
	public UnityEvent missionChangedEvent;


	private void Start() {
		promptMode = false;
		SetupButtons();
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.BASE_MISSION);
		if (active) {
			SetupButtons();
			buttons.ForcePosition(0);
			ShowMissionInfo();
		}
	}

	public override void OnUpArrow() {
		if (promptMode)
			return;

		buttons.Move(-1);
		ShowMissionInfo();
		missionChangedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnDownArrow() {
		if (promptMode)
			return;

		buttons.Move(1);
		ShowMissionInfo();
		missionChangedEvent.Invoke();
		menuMoveEvent.Invoke();
	}

	public override void OnOkButton() {
		if (!promptMode) {
			promptMode = true;
			ChangePrompt(0);
			startPrompt.ShowYesNoPopup("Start mission?", false);
			menuAcceptEvent.Invoke();
		}
		else if (startPrompt.Click(true) == MyPrompt.Result.OK1) {
			StartMission();
			menuAcceptEvent.Invoke();
		}
		else {
			OnBackButton();
		}
	}

	public override void OnBackButton() {
		if (promptMode) {
			promptMode = false;
			startPrompt.Click(false);
		}
		else {
			MenuChangeDelay(MenuMode.BASE_MAIN);
			menuBackEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (!promptMode)
			return;
		ChangePrompt(-1);
		menuMoveEvent.Invoke();
	}

	public override void OnRightArrow() {
		if (!promptMode)
			return;
		ChangePrompt(1);
		menuMoveEvent.Invoke();
	}

	private void SetupButtons() {
		buttons.ResetButtons();
		availableMaps = playerData.missions.FindAll(m => !m.cleared && m.map.unlockDay <= currentDay.value);
		for (int i = 0; i < availableMaps.Count; i++) {
			buttons.AddButton(availableMaps[i].map.entryName);
		}
	}

	private void ShowMissionInfo() {
		int currentIndex = buttons.GetPosition();
		if (currentIndex == -1)
			return;

		MapEntry map = availableMaps[currentIndex].map;
		missionName.text = map.entryName;
		missionDesc.text = map.mapDescription;

		//Reward
		missionRewardMoney.text = "Money:  " + map.reward.money;
		missionRewardMoney.gameObject.SetActive(map.reward.money > 0);
		missionRewardScrap.text = "Scrap:  " + map.reward.scrap;
		missionRewardScrap.gameObject.SetActive(map.reward.scrap > 0);
		missionRewardItem.text = (map.reward.items.Count > 0) ? "Item:  " + map.reward.items[0].entryName : "";
		missionRewardItem.gameObject.SetActive(map.reward.items.Count > 0);
		missionRewardItem2.text = (map.reward.items.Count > 1) ? "Item2:  " + map.reward.items[1].entryName : "";
		missionRewardItem2.gameObject.SetActive(map.reward.items.Count > 1);

		//Planet info
		PlanetInfo info = planets[(int)map.mapLocation];
		planetImage.color = info.planetColor;
		planetImage.transform.localScale = new Vector3(info.size, info.size, info.size);
		planetName.text = map.mapLocation.ToString();
	}

	public void ChangePrompt(int dir) {
		if (!promptMode)
			return;

		startPrompt.Move(dir);
	}

	private void StartMission() {
		currentChapterId.value = availableMaps[buttons.GetPosition()].map.uuid;
		Debug.Log("Start mission:  " + currentChapterId.value);
		startMissionEvent.Invoke();
	}


	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnStartButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}

