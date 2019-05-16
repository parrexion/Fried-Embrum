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

	[Header("Other Menus")]
	public HowToPlayController howTo;
	public OptionsController options;
	public ObjectiveController objective;

	[Header("Events")]
	public UnityEvent nextStateEvent;


	private void Start() {
		ingameMenu.SetActive(false);
		overlay.enabled = false;
		ingameButtons.ResetButtons();
		ingameButtons.AddButton("CONTROLS");
		ingameButtons.AddButton("OPTIONS");
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
			objective.UpdateState(false);
		}
		else if (state == 1) {
			state = 0;
			ingameMenu.SetActive(true);
			objective.UpdateState(true);
			overlay.enabled = true;
			howTo.BackClicked();
		}
		else if (state == 2) {
			state = 0;
			ingameMenu.SetActive(true);
			objective.UpdateState(true);
			options.BackClicked();
		}
		menuBackEvent.Invoke();
    }

	private void Controls() {
		state = 1;
		ingameMenu.SetActive(false);
		objective.UpdateState(false);
		overlay.enabled = false;
		howTo.UpdateState(true);
	}

	private void Options() {
		state = 2;
		ingameMenu.SetActive(false);
		objective.UpdateState(false);
		options.UpdateState(true);
	}

	/// <summary>
	/// Ends the turn for the player.
	/// </summary>
	private void EndTurn() {
		objective.UpdateState(false);
		nextStateEvent.Invoke();
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
