using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuController : InputReceiverDelegate {
	private enum State { MAIN, CONTROLS, LOAD, OPTIONS }

	public GameObject startMenuView;
	public HowToPlayController howTo;
	public SaveFileController saveFileController;
	public OptionsController optionsController;
	public MapInfoListVariable chapterList;
	public BoolVariable lockControls;

	[Header("Views")]
	public GameObject saveView;

	[Header("Current Data")]
	public StringVariable currentChapterIndex;
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
	public MapEntry firstMission;
	public PlayerData playerData;
	public ClassWheel classWheel;
	public PlayerPosition[] startingCharacters;
	public ItemEntry[] startItems;
	public UpgradeEntry[] startUpgrade;
	public MapEntry[] startMissions;

	public UnityEvent saveGameEvent;

	private State currentState;


	private void Awake() {
		StartCoroutine(WaitForInit());
	}

	private IEnumerator WaitForInit() {
		while (InputDelegateController.instance == null) {
			yield return null;
		}

		currentState = State.MAIN;
		currentChapterIndex.value = "";
		currentTotalDays.value = 0;
		lockControls.value = false;
		startMenuView.SetActive(true);
		saveView.SetActive(false);

		mainButtons.ResetButtons();
		mainButtons.AddButton("NEW GAME");
		mainButtons.AddButton("LOAD GAME");
		mainButtons.AddButton("OPTIONS");
		mainButtons.AddButton("QUIT");

		musicFocus.value = true;
		mainMusic.value = mainTheme.clip;
		subMusic.value = null;
		playBkgMusicEvent.Invoke();

		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAIN_MENU);
	}

	public override void OnMenuModeChanged() {
		UpdateState(MenuMode.MAIN_MENU);
	}

	private void ControlsClicked() {
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
		currentChapterIndex.value = firstMission.uuid;
		currentPlayTime.value = 0;
		saveController.ResetCurrentData();
		for (int i = 0; i < startingCharacters.Length; i++) {
			playerData.stats.Add(new StatsContainer(startingCharacters[i]));
			playerData.inventory.Add(new InventoryContainer(classWheel.GetWpnSkillFromLevel(startingCharacters[i].charData.startClassLevels), startingCharacters[i].inventory));
			playerData.skills.Add(new SkillsContainer(classWheel.GetSkillsFromLevel(startingCharacters[i].charData.startClassLevels, startingCharacters[i].charData.startClass, startingCharacters[i].level)));
			playerData.baseInfo.Add(new SupportContainer(null));
		}
		for (int i = 0; i < startItems.Length; i++) {
			playerData.items.Add(new InventoryItem(startItems[i]));
		}
		for (int i = 0; i < startUpgrade.Length; i++) {
			playerData.upgrader.AddEntry(new UpgradeItem(startUpgrade[i]));
		}
		for (int i = 0; i < startMissions.Length; i++) {
			playerData.missions.Add(new MissionContainer(startMissions[i]));
		}
		InputDelegateController.instance.TriggerSceneChange(MenuMode.NONE, "LoadingScreen");
	}

	/// <summary>
	/// Called when starting a new game. Sets the starting values.
	/// </summary>
	public void LoadGameFinished() {
		if (currentChapterIndex.value == "") {
			Debug.Log("Set DialogueC to:  BASE");
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
					ControlsClicked();
					break;
				case 1:
					LoadClicked();
					break;
				case 2:
					OptionsClicked();
					break;
				case 3:
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
	}

	public override void OnStartButton() {
		if (currentState == State.MAIN) {
			ControlsClicked();
			menuAcceptEvent.Invoke();
		}
		else if (currentState == State.CONTROLS) {
			NewGameClicked();
			menuAcceptEvent.Invoke();
		}
	}


	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
}
