using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VictoryScreen : InputReceiverDelegate {

	[Header("Mission")]
	public ScrObjEntryReference currentMission;
	public ScrObjEntryReference currentMap;
	public IntVariable mapIndex;
	public IntVariable nextLoadState;

	[Header("Stats")]
	public PlayerData playerData;
	public IntVariable totalMoney;
	public IntVariable totalScrap;
	public IntVariable totalTurns;
	public IntVariable totalKills;
	public IntVariable totalDeaths;
	public IntVariable gatherScrap;

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
	public UnityEvent startDialogueEvent;
	public UnityEvent stopSfxEvent;


	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.GAMEOVER);
		victoryView.SetActive(active);
		if (active) {
			SavePlayerInfo();
			SetInformation();
		}
	}

	public void Win() {
		MenuChangeDelay(MenuMode.GAMEOVER);
	}

	private void SetInformation() {
		MissionEntry mission = (MissionEntry)currentMission.value;
		turnText.text = totalTurns.value.ToString();
		killText.text = totalKills.value.ToString();
		deathText.text = totalDeaths.value.ToString();

		if (mapIndex.value >= mission.maps.Count) {
			moneyText.text = mission.reward.money.ToString();
			scrapText.text = (gatherScrap.value + mission.reward.scrap).ToString();
			itemText.text = (mission.reward.items.Count > 0) ? mission.reward.items[0].entryName : "";
		}
		else {
			moneyText.text = "0";
			scrapText.text = gatherScrap.value.ToString();
			itemText.text = "";
		}
	}

	private void SavePlayerInfo() {
		mapIndex.value++;
		nextLoadState.value = (int)SaveScreenController.NextState.LOADSCREEN;
		// Save all rewards
		MissionEntry mission = (MissionEntry)currentMission.value;
		if (mapIndex.value >= mission.maps.Count) {
			if (mission.reward.money > 0) {
				totalMoney.value += mission.reward.money;
			}
			if (mission.reward.scrap > 0) {
				totalScrap.value += gatherScrap.value + mission.reward.scrap;
			}
			for (int i = 0; i < mission.reward.items.Count; i++) {
				playerData.items.Add(new InventoryItem(mission.reward.items[i]));
			}
		}
	}

	public override void OnOkButton() {
		//Move to the ending dialogue
		stopSfxEvent.Invoke();
		menuAcceptEvent.Invoke();
		currentDialogueMode.value = (int)DialogueMode.ENDING;
		currentDialogue.value = ((MapEntry)currentMap.value).endDialogue;
		startDialogueEvent.Invoke();
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
