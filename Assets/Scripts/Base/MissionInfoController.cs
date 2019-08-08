using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionInfoController : MonoBehaviour {
	
	public PlayerData playerData;
	public StringVariable currentChapterId;
	public IntVariable locationIndex;
	public IntVariable currentDay;
	public MyButtonList buttons;

	private List<MissionContainer> availableMaps = new List<MissionContainer>();

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
		availableMaps = playerData.missions.FindAll(m => !m.cleared && m.mission.unlockDay <= currentDay.value);
		for (int i = 0; i < availableMaps.Count; i++) {
			buttons.AddButton(availableMaps[i].mission.entryName);
		}

		buttons.ForcePosition(0);
		locationIndex.value = (int)availableMaps[0].mission.mapLocation;
		missionChangedEvent.Invoke();
		ShowMissionInfo();
	}

	public bool Move(int dir) {
		int prevPos = buttons.GetPosition();
		int newPos = buttons.Move(dir);
		ShowMissionInfo();
		locationIndex.value = (int)availableMaps[newPos].mission.mapLocation;
		missionChangedEvent.Invoke();
		return (prevPos != newPos);
	}

	public bool Select() {
		currentChapterId.value = availableMaps[buttons.GetPosition()].mission.uuid;
		return true;
	}
	
	private void ShowMissionInfo() {
		int currentIndex = buttons.GetPosition();
		if (currentIndex == -1)
			return;

		MissionEntry mission = availableMaps[currentIndex].mission;
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
