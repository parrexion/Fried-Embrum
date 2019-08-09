using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VictoryScreen : InputReceiverDelegate {

	public ScrObjEntryReference currentMission;
	public ScrObjEntryReference currentMap;
	public IntVariable totalTurns;
	public IntVariable totalKills;
	public IntVariable totalDeaths;

	[Header("Texts")]
	public GameObject victoryView;
	public Text turnText;
	public Text killText;
	public Text deathText;
	public Text moneyText;
	public Text scrapText;
	public Text itemText;

	[Header("Ending dialogue")]
	public IntVariable currentDialogueMode;
	public ScrObjEntryReference currentDialogue;
	public IntVariable nextLoadState;
	public UnityEvent startDialogueEvent;


	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.GAMEOVER);
		victoryView.SetActive(active);
		if (active) {
			SetInformation();
		}
	}

	public void SetInformation() {
		Reward reward = ((MissionEntry)currentMission.value).reward;
		turnText.text = "Turns:  " + totalTurns.value;
		killText.text = "Kills:  " + totalKills.value;
		deathText.text = "Deaths:  " + totalDeaths.value;

		moneyText.text = "Money:  " + reward.money;
		scrapText.text = "Scrap:     " + reward.scrap;
		itemText.text = (reward.items.Count > 0) ? reward.items[0].entryName : "";
	}
	
	public override void OnOkButton() {
		//Move to the ending dialogue
		currentDialogueMode.value = (int)DialogueMode.ENDING;
		currentDialogue.value = ((MapEntry)currentMap.value).endDialogue;
		nextLoadState.value = (int)SaveScreenController.NextState.BASE;
		startDialogueEvent.Invoke();
	}

	public void Win() {
		MenuChangeDelay(MenuMode.GAMEOVER);
	}

	public override void OnBackButton() { }
	public override void OnDownArrow() { }
	public override void OnLButton() { }
	public override void OnLeftArrow() { }
	public override void OnRButton() { }
	public override void OnRightArrow() { }
	public override void OnStartButton() { }
	public override void OnUpArrow() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}
