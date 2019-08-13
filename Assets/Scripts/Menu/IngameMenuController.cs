using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngameMenuController : InputReceiverDelegate {

	private enum State { MAIN, CONTROLS, OPTION, POPUP }
	public Image overlay;
	public BoolVariable autoEndTurn;

	[Header("Ingame Menu")]
	public GameObject ingameMenu;
	public MyButtonList ingameButtons;
	public MyPrompt prompt;
	private State state;

	[Header("Other Menus")]
	public HowToPlayController howTo;
	public OptionsController options;
	public ObjectiveController objective;

	[Header("Events")]
	public UnityEvent waitPlayerEvent;
	public UnityEvent nextStateEvent;


	private void Start() {
		ingameMenu.SetActive(false);
		overlay.enabled = false;
		ingameButtons.ResetButtons();
		ingameButtons.AddButton("CONTROLS");
		ingameButtons.AddButton("OPTIONS");
		ingameButtons.AddButton("RETURN TO MAIN");
		ingameButtons.AddButton("END TURN");
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.INGAME);
		ingameMenu.SetActive(active);
		overlay.enabled = active;
		if (active) {
			ingameButtons.ForcePosition(0);
			objective.UpdateState(true);
		}
	}

	public override void OnUpArrow() {
		if (state == State.MAIN) {
			ingameButtons.Move(-1);
			menuMoveEvent.Invoke();
		}
		else if (state == State.CONTROLS) {
			if (howTo.Move(-1))
				menuMoveEvent.Invoke();
		}
		else if (state == State.OPTION) {
			options.MoveVertical(-1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnDownArrow() {
		if (state == State.MAIN) {
			ingameButtons.Move(1);
			menuMoveEvent.Invoke();
		}
		else if (state == State.CONTROLS) {
			if (howTo.Move(1))
				menuMoveEvent.Invoke();
		}
		else if (state == State.OPTION) {
			options.MoveVertical(1);
			menuMoveEvent.Invoke();
		}
	}

	public override void OnOkButton() {
		if (state == State.MAIN) {
			switch (ingameButtons.GetPosition()) {
				case 0:
					Controls();
					break;
				case 1:
					Options();
					break;
				case 2:
					ReturnToMain();
					break;
				case 3:
					EndTurn();
					break;
			}
			menuAcceptEvent.Invoke();
		}
		else if (state == State.CONTROLS) {
			if (howTo.CheckOk()) {
				OnBackButton();
			}
		}
		else if (state == State.OPTION) {
			bool res = options.OKClicked();
			if (res)
				menuAcceptEvent.Invoke();
		}
		else if (state == State.POPUP) {
			MyPrompt.Result res = prompt.Click(true);
			if (res == MyPrompt.Result.OK1) {
				SceneChangeDelay(MenuMode.MAIN_MENU, "MainMenu");
				menuAcceptEvent.Invoke();
			}
			else {
				state = State.MAIN;
				menuBackEvent.Invoke();
			}
		}
	}

	public override void OnBackButton() {
		if (state == State.MAIN) {
			MenuChangeDelay(MenuMode.MAP);
			objective.UpdateState(false);
		}
		else if (state == State.CONTROLS) {
			state = 0;
			ingameMenu.SetActive(true);
			objective.UpdateState(true);
			overlay.enabled = true;
			howTo.BackClicked();
		}
		else if (state == State.OPTION) {
			state = 0;
			ingameMenu.SetActive(true);
			objective.UpdateState(true);
			options.BackClicked();
		}
		else if (state == State.POPUP) {
			state = State.MAIN;
			prompt.Click(false);
		}
		menuBackEvent.Invoke();
	}

	private void Controls() {
		state = State.CONTROLS;
		ingameMenu.SetActive(false);
		objective.UpdateState(false);
		overlay.enabled = false;
		howTo.UpdateState(true);
	}

	private void Options() {
		state = State.OPTION;
		ingameMenu.SetActive(false);
		objective.UpdateState(false);
		options.UpdateState(true);
	}

	private void ReturnToMain() {
		state = State.POPUP;
		prompt.ShowYesNoPopup("Quit to main menu? Your progress will not be saved.", false);
	}

	/// <summary>
	/// Ends the turn for the player.
	/// </summary>
	private void EndTurn() {
		objective.UpdateState(false);
		waitPlayerEvent.Invoke();
		if (!autoEndTurn.value) {
			nextStateEvent.Invoke();
		}
	}

	public override void OnLeftArrow() {
		if (state == State.OPTION) {
			if (options.MoveHorizontal(-1))
				menuMoveEvent.Invoke();
		}
		else if (state == State.POPUP) {
			prompt.Move(-1);
			menuMoveEvent.Invoke();
		}
	}
	public override void OnRightArrow() {
		if (state == State.OPTION) {
			if (options.MoveHorizontal(1))
				menuMoveEvent.Invoke();
		}
		else if (state == State.POPUP) {
			prompt.Move(1);
			menuMoveEvent.Invoke();
		}
	}



	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
	public override void OnStartButton() { }
}
