using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngameMenuController : InputReceiver {

	[Header("Inventory Menu")]
	public GameObject ingameMenu;
	public Image overlay;
	public IntVariable ingameMenuPosition;
	private Image[] ingameButtons = new Image[0];
	private int state;

	[Header("How To Play")]
	public HowToPlayController howTo;

	[Header("Events")]
	public UnityEvent changeTurnEvent;


	private void Start() {
		ingameMenu.SetActive(false);
		overlay.enabled = false;
		ingameButtons = ingameMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.INGAME);
		ingameMenu.SetActive(active);
		overlay.enabled = active;
		if (active)
			ingameMenuPosition.value = 0;
		ButtonHighlighting();
    }

    public override void OnUpArrow() {
		if (!active || state == 1)
			return;

		ingameMenuPosition.value--;
		if (ingameMenuPosition.value < 0)
			ingameMenuPosition.value = ingameButtons.Length-1;

		ButtonHighlighting();
    }

    public override void OnDownArrow() {
		if (!active || state == 1)
			return;

		ingameMenuPosition.value++;
		if (ingameMenuPosition.value >= ingameButtons.Length)
			ingameMenuPosition.value = 0;
			
		ButtonHighlighting();
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
					EndTurn();
					break;
				case 2:
					SaveGame();
					break;
			}
		}
		else if (state == 1) {
			if (howTo.CheckOk())
				OnBackButton();
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

	private void SaveGame() {

	}

    public override void OnLeftArrow() {
		if (!active)
			return;
		howTo.MoveLeft();
	}
    public override void OnRightArrow() {
		if (!active)
			return;
		howTo.MoveRight();
	}



    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}
