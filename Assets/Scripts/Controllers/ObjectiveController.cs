using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour {

	public GameObject objectiveObject;
	public ScrObjEntryReference currentMap;
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
		MapEntry map = (MapEntry)currentMap.value;
		
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

		case WinCondition.SEIZE:
			explanation.text = "Seize throne.";
			break;

		case WinCondition.BOSS:
			explanation.text = "Defeat boss.";
			break;
		}

		rewardMoney.text = (map.reward.money > 0) ? map.reward.money + " Money" : "";
		rewardScrap.text = (map.reward.scrap > 0) ? map.reward.scrap + " Scrap" : "";
		rewardItem.text = (map.reward.items.Count > 0) ? map.reward.items[0].entryName : "";
	}

}
