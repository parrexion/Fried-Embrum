using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour {

	public GameObject objectiveObject;
	public ScrObjEntryReference currentMap;
	public IntVariable mapIndex;
	public CharacterListVariable enemyList;

	[Header("Objective")]
	public Text winExplanation;
	public Text loseExplanation;
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
				winExplanation.text = "Rout the enemy.";
				break;

			case WinCondition.CAPTURE:
				winExplanation.text = "Capture command point.";
				break;

			case WinCondition.BOSS:
				winExplanation.text = "Defeat boss.";
				break;

			case WinCondition.ESCAPE:
				winExplanation.text = "Escape with everyone.";
				break;

			default:
				Debug.LogError("Unsupported explanation type  " + map.winCondition);
				break;
		}

		switch (map.loseCondition) {
			case LoseCondition.NONE:
				loseExplanation.text = "";
				break;
			case LoseCondition.TIME:
				loseExplanation.text = "Win by turn " + map.turnLimit;
				break;
		}
	}

}
