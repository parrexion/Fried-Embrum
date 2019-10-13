using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerController : InputReceiverDelegate {

	public GameObject view;
	public BoolVariable controlsSet;
	public IntVariable selectedScheme;
	public GameObject[] schemes;

	public UnityEvent saveGameEvent;

	private int currentSchema;

	private void Awake() {
		StartCoroutine(WaitForInit());
	}

	private IEnumerator WaitForInit() {
		while (InputDelegateController.instance == null) {
			yield return null;
		}
		if (controlsSet.value) {
			MenuChangeDelay(MenuMode.MAIN_MENU);
		}
		else {
			MenuChangeDelay(MenuMode.PRE_CONTROLLER);
		}
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.PRE_CONTROLLER);
		view.SetActive(active);
		if (active) {
			currentSchema = selectedScheme.value;
			UpdateScheme();
		}
	}

	private void UpdateScheme() {
		for (int i = 0; i < schemes.Length; i++) {
			schemes[i].SetActive(i == currentSchema);
		}
	}

	private void ChangeSchema(int dir) {
		currentSchema = OPMath.FullLoop(0, schemes.Length, currentSchema += dir);
		UpdateScheme();
	}

	public override void OnLeftArrow() {
		ChangeSchema(-1);
		menuMoveEvent.Invoke();
	}

	public override void OnRightArrow() {
		ChangeSchema(1);
		menuMoveEvent.Invoke();
	}

	public override void OnOkButton() {
		controlsSet.value = true;
		selectedScheme.value = currentSchema;
		saveGameEvent.Invoke();
		menuAcceptEvent.Invoke();
		MenuChangeDelay(MenuMode.MAIN_MENU);
	}

	public override void OnBackButton() { }
	public override void OnUpArrow() { }
	public override void OnDownArrow() { }
	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
	public override void OnStartButton() { }
}
