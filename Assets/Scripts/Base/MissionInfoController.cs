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
		availableMaps = playerData.missions.FindAll(m => !m.cleared && m.map.unlockDay <= currentDay.value);
		for (int i = 0; i < availableMaps.Count; i++) {
			buttons.AddButton(availableMaps[i].map.entryName);
		}

		buttons.ForcePosition(0);
		locationIndex.value = (int)availableMaps[0].map.mapLocation;
		missionChangedEvent.Invoke();
		ShowMissionInfo();
	}

	public bool Move(int dir) {
		int prevPos = buttons.GetPosition();
		int newPos = buttons.Move(dir);
		ShowMissionInfo();
		locationIndex.value = (int)availableMaps[newPos].map.mapLocation;
		missionChangedEvent.Invoke();
		return (prevPos != newPos);
	}

	public bool Select() {
		currentChapterId.value = availableMaps[buttons.GetPosition()].map.uuid;
		return true;
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
}
