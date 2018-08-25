using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngameMenuController : InputReceiver {

	[Header("Ingame Menu")]
	public GameObject ingameMenu;
	public Image overlay;
	public IntVariable ingameMenuPosition;
	private Image[] ingameButtons = new Image[0];
	private int state;

	[Header("Objective")]
	public CharacterListVariable enemyList;
	public GameObject objectiveObject;
	public Text enemyCount;

	[Header("Other Menus")]
	public HowToPlayController howTo;
	public OptionsController options;

	[Header("Events")]
	public UnityEvent changeTurnEvent;


	private void Start() {
		ingameMenu.SetActive(false);
		objectiveObject.SetActive(false);
		overlay.enabled = false;
		ingameButtons = ingameMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.INGAME);
		ingameMenu.SetActive(active);
		objectiveObject.SetActive(active);
		overlay.enabled = active;
		if (active)
			ingameMenuPosition.value = 0;
		ShowObjective();
		ButtonHighlighting();
    }

    public override void OnUpArrow() {
		if (!active)
			return;

		if (state == 0) {
			ingameMenuPosition.value--;
			if (ingameMenuPosition.value < 0)
				ingameMenuPosition.value = ingameButtons.Length-1;

			ButtonHighlighting();
		}
		else if (state == 2) {
			options.MoveUp();
		}
    }

    public override void OnDownArrow() {
		if (!active)
			return;

		if (state == 0) {
			ingameMenuPosition.value++;
			if (ingameMenuPosition.value >= ingameButtons.Length)
				ingameMenuPosition.value = 0;
				
			ButtonHighlighting();
		}
		else if (state == 2) {
			options.MoveDown();
		}
    }

    public override void OnOkButton() {
		if (!active)
			return;

		if (state == 0) {
			switch (ingameMenuPosition.value)
			{
				case 0:
					Controls();
					break;
				case 1:
					Options();
					break;
				case 2:
					EndTurn();
					break;
				// case 3:
				// 	SaveGame();
				// 	break;
			}
		}
		else if (state == 1) {
			if (howTo.CheckOk())
				OnBackButton();
		}
		else if (state == 2) {
			options.OKClicked();
		}
    }

    public override void OnBackButton() {
		if (!active)
			return;

		if (state == 0) {
			currentMenuMode.value = (int)MenuMode.MAP;
			StartCoroutine(MenuChangeDelay());
		}
		else if (state == 1) {
			state = 0;
			howTo.BackClicked();
		}
		else if (state == 2) {
			state = 0;
			ingameMenu.SetActive(true);
			objectiveObject.SetActive(true);
			options.BackClicked();
		}
    }

	private void ShowObjective() {
		if (!active)
			return;

		int enemies = 0;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (enemyList.values[i].IsAlive())
				enemies++;
		}
		enemyCount.text = enemies.ToString();
	}

	/// <summary>
	/// Colors the selected button to show the current selection.
	/// </summary>
	private void ButtonHighlighting() {
		if (!active)
			return;

		for (int i = 0; i < ingameButtons.Length; i++) {
			ingameButtons[i].color = (ingameMenuPosition.value == i) ? Color.cyan : Color.white;
		}
	}

	/// <summary>
	/// Ends the turn for the player.
	/// </summary>
	private void EndTurn() {
		changeTurnEvent.Invoke();
	}

	private void Controls() {
		state = 1;
		howTo.UpdateState(true);
	}

	private void Options() {
		state = 2;
		ingameMenu.SetActive(false);
		objectiveObject.SetActive(false);
		options.UpdateState(true);
	}

	private void SaveGame() {

	}

    public override void OnLeftArrow() {
		if (!active)
			return;
		
		if (state == 1)
			howTo.MoveLeft();
	}
    public override void OnRightArrow() {
		if (!active)
			return;
		
		if (state == 1)
			howTo.MoveRight();
	}



    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}
