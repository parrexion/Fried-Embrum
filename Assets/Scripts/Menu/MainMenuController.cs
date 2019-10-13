using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuController : InputReceiverDelegate {
	private enum State { MAIN, CONTROLS, LOAD, OPTIONS, CHANGELOG }

	public HowToPlayController howTo;
	public SaveFileController saveFileController;
	public OptionsController optionsController;
	public BoolVariable lockControls;
	public BoolVariable controlsSet;

	[Header("Views")]
	public GameObject startMenuView;
	public GameObject saveView;
	public GameObject changelogView;

	[Header("Current Data")]
	public StringVariable loadMapID;
	public IntVariable currentTotalDays;
	public IntVariable currentPlayTime;

	[Header("Menu")]
	public MyButtonList mainButtons;

	[Header("Music")]
	public MusicEntry mainTheme;
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public BoolVariable musicFocus;
	public UnityEvent playBkgMusicEvent;

	[Header("New Game Data")]
	public SaveController saveController;
	public ScrObjEntryReference currentMission;
	public IntVariable mapIndex;
	public PrepListVariable squad1;
	public PrepListVariable squad2;
	public PlayerData playerData;
	public ClassWheel classWheel;
	public MissionEntry startMission;
	public PlayerPosition[] startingCharacters;
	public ItemEntry[] startItems;
	public UpgradeEntry[] startUpgrade;
	public MissionEntry[] otherMissions;

	public UnityEvent saveGameEvent;

	private State currentState;


	private void Start() {
		musicFocus.value = true;
		mainMusic.value = mainTheme.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.MAIN_MENU);
		if (!active)
			return;

		currentState = State.MAIN;

		lockControls.value = false;
		currentTotalDays.value = 0;
		startMenuView.SetActive(true);
		saveView.SetActive(false);
		changelogView.SetActive(false);

		mainButtons.ResetButtons();
		mainButtons.AddButton("NEW GAME");
		mainButtons.AddButton("LOAD GAME");
		mainButtons.AddButton("OPTIONS");
		mainButtons.AddButton("CONTROLS");
		mainButtons.AddButton("CHANGELOG");
		mainButtons.AddButton("QUIT");
	}

	private void NewgameClicked() {
		currentState = State.CONTROLS;
		startMenuView.SetActive(false);
		howTo.UpdateState(true);
	}

	private void LoadClicked() {
		currentState = State.LOAD;
		startMenuView.SetActive(false);
		saveView.SetActive(true);
	}

	private void OptionsClicked() {
		currentState = State.OPTIONS;
		startMenuView.SetActive(false);
		optionsController.UpdateState(true);
	}

	private void ControlsClicked() {
		controlsSet.value = false;
		MenuChangeDelay(MenuMode.PRE_CONTROLLER);
	}

	private void ChangelogClicked() {
		currentState = State.CHANGELOG;
		changelogView.SetActive(true);
	}

	private void QuitClicked() {
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	/// <summary>
	/// Called when starting a new game. Sets the starting values.
	/// </summary>
	private void NewGameClicked() {
		currentMission.value = startMission;
		mapIndex.value = 0;
		loadMapID.value = startMission.maps[0].uuid;
		currentPlayTime.value = 0;
		saveController.ResetCurrentData();
		squad1.values.Clear();
		squad2.values.Clear();
		for (int i = 0; i < startingCharacters.Length; i++) {
			playerData.stats.Add(new StatsContainer(startingCharacters[i]));
			playerData.inventory.Add(new InventoryContainer(classWheel.GetWpnSkillFromLevel(startingCharacters[i].charData.startClassLevels), startingCharacters[i].inventory));
			playerData.skills.Add(new SkillsContainer(classWheel.GetSkillsFromLevel(startingCharacters[i].charData.startClassLevels, startingCharacters[i].charData.startClass, startingCharacters[i].level)));
			playerData.baseInfo.Add(new SupportContainer(null));
			squad1.values.Add(new PrepCharacter(i));
		}
		for (int i = 0; i < startItems.Length; i++) {
			playerData.items.Add(new InventoryItem(startItems[i]));
		}
		for (int i = 0; i < startUpgrade.Length; i++) {
			playerData.upgrader.AddEntry(new UpgradeItem(startUpgrade[i]));
		}
		for (int i = 0; i < otherMissions.Length; i++) {
			playerData.missions.Add(new MissionProgress(otherMissions[i].uuid));
		}

		InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "LoadingScreen");
	}

	/// <summary>
	/// Called when starting a new game. Sets the starting values.
	/// </summary>
	public void LoadGameFinished() {
		if (loadMapID.value == "") {
			InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "BaseScene");
		}
		else {
			InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "LoadingScreen");
		}
	}

	public override void OnUpArrow() {
		if (currentState == State.MAIN) {
			mainButtons.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.CONTROLS) {
			if (howTo.Move(-1))
				menuMoveEvent.Invoke();
		}
		else if (currentState == State.LOAD) {
			saveFileController.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.OPTIONS) {
			optionsController.MoveVertical(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (currentState == State.MAIN) {
			mainButtons.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.CONTROLS) {
			if (howTo.Move(1))
				menuMoveEvent.Invoke();
		}
		else if (currentState == State.LOAD) {
			saveFileController.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.OPTIONS) {
			optionsController.MoveVertical(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (currentState == State.LOAD) {
			saveFileController.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.OPTIONS) {
			if (optionsController.MoveHorizontal(-1))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnRightArrow() {
		if (currentState == State.LOAD) {
			saveFileController.MoveHorizontal(1);
			menuMoveEvent.Invoke();
		}
		else if (currentState == State.OPTIONS) {
			if (optionsController.MoveHorizontal(1))
				menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (currentState == State.MAIN) {
			switch (mainButtons.GetPosition()) {
				case 0:
					NewgameClicked();
					break;
				case 1:
					LoadClicked();
					break;
				case 2:
					OptionsClicked();
					break;
				case 3:
					ControlsClicked();
					break;
				case 4:
					ChangelogClicked();
					break;
				case 5:
					QuitClicked();
					break;
			}
			menuAcceptEvent.Invoke();
		}
		else if (currentState == State.LOAD) {
			if (saveFileController.OkClicked()) {
				menuAcceptEvent.Invoke();
			}
		}
		else if (currentState == State.OPTIONS) {
			if (optionsController.OKClicked()) {
				menuAcceptEvent.Invoke();
			}
		}
	}

	public override void OnBackButton() {
		if (currentState == State.CONTROLS) {
			currentState = State.MAIN;
			howTo.BackClicked();
			startMenuView.SetActive(true);
			menuBackEvent.Invoke();
		}
		else if (currentState == State.LOAD) {
			if (saveFileController.BackClicked()) {
				currentState = State.MAIN;
				startMenuView.SetActive(true);
				saveView.SetActive(false);
				menuBackEvent.Invoke();
			}
		}
		else if (currentState == State.OPTIONS) {
			currentState = State.MAIN;
			optionsController.BackClicked();
			startMenuView.SetActive(true);
			saveGameEvent.Invoke();
			menuBackEvent.Invoke();
		}
		else if (currentState == State.CHANGELOG) {
			currentState = State.MAIN;
			changelogView.SetActive(false);
			menuBackEvent.Invoke();
		}
	}

	public override void OnXButton() {
		if (currentState == State.CONTROLS) {
			NewGameClicked();
			menuAcceptEvent.Invoke();
		}
	}


	public override void OnStartButton() { }
	public override void OnYButton() { }
	public override void OnLButton() { }
	public override void OnRButton() { }
}
