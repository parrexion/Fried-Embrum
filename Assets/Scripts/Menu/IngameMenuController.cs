using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngameMenuController : InputReceiverDelegate {

	public Image overlay;

	[Header("Ingame Menu")]
	public GameObject ingameMenu;
	public MyButtonList ingameButtons;
	private int state;

	[Header("Objective")]
	public CharacterListVariable enemyList;
	public GameObject objectiveObject;
	public Text enemyCount;

	[Header("Other Menus")]
	public HowToPlayController howTo;
	public OptionsController options;

	[Header("Events")]
	public UnityEvent nextStateEvent;


	private void Start() {
		ingameMenu.SetActive(false);
		objectiveObject.SetActive(false);
		overlay.enabled = false;
		ingameButtons.ResetButtons();
		ingameButtons.AddButton("CONTROLS");
		ingameButtons.AddButton("OPTIONS");
		ingameButtons.AddButton("END TURN");
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.INGAME);
		ingameMenu.SetActive(active);
		objectiveObject.SetActive(active);
		overlay.enabled = active;
		if (active) {
			ingameButtons.ForcePosition(0);
			ShowObjective();
		}
    }

    public override void OnUpArrow() {
		if (state == 0) {
			ingameButtons.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (state == 1) {
			if (howTo.Move(-1))
				menuMoveEvent.Invoke();
		}
		else if (state == 2) {
			options.MoveVertical(-1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnDownArrow() {
		if (state == 0) {
			ingameButtons.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (state == 1) {
			if (howTo.Move(1))
				menuMoveEvent.Invoke();
		}
		else if (state == 2) {
			options.MoveVertical(1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnOkButton() {
		if (state == 0) {
			switch (ingameButtons.GetPosition())
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
			}
			menuAcceptEvent.Invoke();
		}
		else if (state == 1) {
			if (howTo.CheckOk()) {
				OnBackButton();
				menuBackEvent.Invoke();
			}
		}
		else if (state == 2) {
			bool res = options.OKClicked();
			if (res)
				menuAcceptEvent.Invoke();
		}
    }

    public override void OnBackButton() {
		if (state == 0) {
			MenuChangeDelay(MenuMode.MAP);
		}
		else if (state == 1) {
			state = 0;
			ingameMenu.SetActive(true);
			objectiveObject.SetActive(true);
			overlay.enabled = true;
			howTo.BackClicked();
		}
		else if (state == 2) {
			state = 0;
			ingameMenu.SetActive(true);
			objectiveObject.SetActive(true);
			options.BackClicked();
		}
		menuBackEvent.Invoke();
    }

	private void ShowObjective() {
		int enemies = 0;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (enemyList.values[i].IsAlive())
				enemies++;
		}
		enemyCount.text = enemies.ToString();
	}

	/// <summary>
	/// Ends the turn for the player.
	/// </summary>
	private void EndTurn() {
		nextStateEvent.Invoke();
	}

	private void Controls() {
		state = 1;
		ingameMenu.SetActive(false);
		objectiveObject.SetActive(false);
		overlay.enabled = false;
		howTo.UpdateState(true);
	}

	private void Options() {
		state = 2;
		ingameMenu.SetActive(false);
		objectiveObject.SetActive(false);
		options.UpdateState(true);
	}

    public override void OnLeftArrow() {
		if (state == 2) {
			if (options.MoveHorizontal(-1))
				menuMoveEvent.Invoke();
		}
	}
    public override void OnRightArrow() {
		if (state == 2) {
			if (options.MoveHorizontal(1))
				menuMoveEvent.Invoke();
		}
	}



    public override void OnLButton() {}
    public override void OnRButton() {}
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
