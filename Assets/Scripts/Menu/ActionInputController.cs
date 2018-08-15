using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInputController : InputReceiver {

	public TacticsMoveVariable lastSelectedCharacter;
	public ActionModeVariable currentActionMode;
	public CharacterListVariable targetList;

	public GameObject buttonMenu;
	public IntVariable buttonMenuPosition;

	private Image[] buttons = new Image[0];


	private void Start() {
		buttons = GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.UNIT);
		buttonMenu.SetActive(active);
		ButtonSetup();
    }

    public override void OnDownArrow() {
		if (!active)
			return;
			
        buttonMenuPosition.value++;
		if (buttonMenuPosition.value >= buttons.Length)
			buttonMenuPosition.value = 0;
		ButtonColoring();
    }

    public override void OnUpArrow() {
		if (!active)
			return;

        buttonMenuPosition.value--;
		if (buttonMenuPosition.value < 0)
			buttonMenuPosition.value += buttons.Length;
		ButtonColoring();
    }

    public override void OnBackButton() {
		if (!active)
			return;
		
		currentMenuMode.value = (int)MenuMode.MAP;
		currentActionMode.value = ActionMode.MOVE;
		StartCoroutine(MenuChangeDelay());
		Debug.Log("Oh, not there");
    }

    public override void OnOkButton() {
		if (!active)
			return;

		switch (buttonMenuPosition.value)
		{
			case 0: // ATTACK
				targetList.values = lastSelectedCharacter.value.GetEnemiesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.ATTACK;
				Debug.Log("Attack!");
				StartCoroutine(MenuChangeDelay());
				break;
			case 1: // HEAL
				targetList.values = lastSelectedCharacter.value.FindSupportablesInRange();
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.HEAL;
				Debug.Log("Heal!");
				StartCoroutine(MenuChangeDelay());
				break;
			case 3: // WAIT
				currentMenuMode.value = (int)MenuMode.MAP;
				currentActionMode.value = ActionMode.NONE;
				StartCoroutine(MenuChangeDelay());
				lastSelectedCharacter.value.End();
				break;
		}
    }

	private void ButtonSetup() {
		// for (int i = 0; i < buttons.Length; i++) {
		// 	//Show button or not
		// }
		ButtonColoring();
	}

	private void ButtonColoring() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].color = (buttonMenuPosition.value == i) ? Color.cyan : Color.white;
		}
	}



    public override void OnLeftArrow() {}
    public override void OnRightArrow() {}
    public override void OnSp1Button() {}
    public override void OnSp2Button() {}
    public override void OnStartButton() {}

}
