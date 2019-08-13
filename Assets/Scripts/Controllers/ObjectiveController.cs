using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour {

	public GameObject objectiveObject;
	public ScrObjEntryReference currentMission;
	public IntVariable mapIndex;
	public CharacterListVariable enemyList;

	[Header("Objective")]
	public Text explanation;
	public Text enemyCount;

	[Header("Reward")]
	public Text rewardMoney;
	public Text rewardScrap;
	public Text rewardItem;


	private void Start() {
		objectiveObject.SetActive(false);
		UpdateObjective();
	}

	public void UpdateState(bool state) {
		objectiveObject.SetActive(state);
		if (state)
			UpdateObjective();
	}

	private void UpdateObjective() {
		MissionEntry mission = (MissionEntry)currentMission.value;
		MapEntry map = mission.maps[mapIndex.value];

		int enemies = 0;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (enemyList.values[i].IsAlive())
				enemies++;
		}
		enemyCount.text = enemies.ToString();

		switch (map.winCondition) {
			case WinCondition.ROUT:
				explanation.text = "Rout the enemy.";
				break;

			case WinCondition.CAPTURE:
				explanation.text = "Capture command room.";
				break;

			case WinCondition.BOSS:
				explanation.text = "Defeat boss.";
				break;
		}

		rewardMoney.text = (mission.reward.money > 0) ? mission.reward.money + " Money" : "";
		rewardScrap.text = (mission.reward.scrap > 0) ? mission.reward.scrap + " Scrap" : "";
		rewardItem.text = (mission.reward.items.Count > 0) ? mission.reward.items[0].entryName : "";
	}

}
