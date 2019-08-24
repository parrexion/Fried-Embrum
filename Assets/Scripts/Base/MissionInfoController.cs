using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionInfoController : MonoBehaviour {
	
	public ScrObjLibraryVariable missionLibrary;
	public PlayerData playerData;
	public ScrObjEntryReference currentMission;
	public IntVariable locationIndex;
	public IntVariable currentDay;
	public MyButtonList buttons;

	private List<MissionEntry> availableMaps = new List<MissionEntry>();

	public Text currentDayText;

	[Header("Planets")]
	public Image planetImage;
	public Text planetName;
	public PlanetInfo[] planets;
	public UnityEvent missionChangedEvent;
	
	[Header("Mission Info")]
	public Text missionName;
	public Text missionDesc;
	public Text missionRewardMoney;
	public Text missionRewardScrap;
	public Text missionRewardItem;
	public Text missionRewardItem2;
	

	public void SetupList() {
		buttons.ResetButtons();
		availableMaps.Clear();
		for (int i = 0; i < missionLibrary.Size(); i++) {
			MissionEntry mission = (MissionEntry)missionLibrary.GetEntryByIndex(i);
			MissionProgress progress = playerData.GetMissionProgress(mission.uuid);
			if (progress.cleared)
				continue;

			switch (mission.unlockReq) {
				case MissionEntry.Unlocking.TIME:
					if (mission.unlockDay <= currentDay.value) {
						availableMaps.Add(mission);
					}
					break;
				case MissionEntry.Unlocking.SQUADSIZE:
					if (playerData.stats.Count <= mission.squadSize) {
						availableMaps.Add(mission);
					}
					break;
				case MissionEntry.Unlocking.RECRUITED:
					if (playerData.HasCharacter(mission.characterReq.uuid)) {
						availableMaps.Add(mission);
					}
					break;
				case MissionEntry.Unlocking.MISSION:
					if (playerData.GetMissionProgress(mission.clearedMission.uuid).cleared) {
						availableMaps.Add(mission);
					}
					break;
				case MissionEntry.Unlocking.DEATH:
					if (playerData.IsDead(mission.characterReq.uuid)) {
						availableMaps.Add(mission);
					}
					break;
			}
		}
		for (int i = 0; i < availableMaps.Count; i++) {
			buttons.AddButton(availableMaps[i].entryName);
		}

		buttons.ForcePosition(0);
		locationIndex.value = (int)availableMaps[0].mapLocation;
		missionChangedEvent.Invoke();
		ShowMissionInfo();
	}

	public bool Move(int dir) {
		int prevPos = buttons.GetPosition();
		int newPos = buttons.Move(dir);
		ShowMissionInfo();
		locationIndex.value = (int)availableMaps[newPos].mapLocation;
		missionChangedEvent.Invoke();
		return (prevPos != newPos);
	}

	public bool Select() {
		currentMission.value = availableMaps[buttons.GetPosition()];
		return true;
	}
	
	private void ShowMissionInfo() {
		int currentIndex = buttons.GetPosition();
		if (currentIndex == -1)
			return;

		currentDayText.text = "Day:  " + currentDay.value;

		MissionEntry mission = availableMaps[currentIndex];
		missionName.text = mission.entryName;
		missionDesc.text = mission.mapDescription;

		//Reward
		missionRewardMoney.text = "Money:  " + mission.reward.money;
		missionRewardMoney.gameObject.SetActive(mission.reward.money > 0);
		missionRewardScrap.text = "Scrap:  " + mission.reward.scrap;
		missionRewardScrap.gameObject.SetActive(mission.reward.scrap > 0);
		missionRewardItem.text = (mission.reward.items.Count > 0) ? "Item:  " + mission.reward.items[0].entryName : "";
		missionRewardItem.gameObject.SetActive(mission.reward.items.Count > 0);
		missionRewardItem2.text = (mission.reward.items.Count > 1) ? "Item2:  " + mission.reward.items[1].entryName : "";
		missionRewardItem2.gameObject.SetActive(mission.reward.items.Count > 1);

		//Planet info
		PlanetInfo info = planets[(int)mission.mapLocation];
		planetImage.color = info.planetColor;
		planetImage.transform.localScale = new Vector3(info.size, info.size, info.size);
		planetName.text = mission.mapLocation.ToString();
	}
}
