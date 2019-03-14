using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInputController : InputReceiverDelegate {

	public DialogueTextHandler textHandler;
	public DialogueLines textLines;


	public void StartDialogue() {
		InputDelegateController.instance.TriggerMenuChange(MenuMode.DIALOGUE);
	}

	public override void OnMenuModeChanged() {
        UpdateState(MenuMode.DIALOGUE);
	}

	public override void OnOkButton() {
		textHandler.DialogueBoxClicked();
		menuAcceptEvent.Invoke();
	}

	public override void OnBackButton() {
		textHandler.DialogueBoxClicked();
		menuAcceptEvent.Invoke();
	}

	public override void OnStartButton() {
		textLines.SkipDialogue();
		menuAcceptEvent.Invoke();
	}



	public override void OnDownArrow() {}
	public override void OnLButton() {}
	public override void OnLeftArrow() {}
	public override void OnRButton() {}
	public override void OnRightArrow() {}
	public override void OnUpArrow() {}
	public override void OnXButton() {}
	public override void OnYButton() {}

}
