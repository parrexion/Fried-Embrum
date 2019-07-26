using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class which contains functions for all the buttons used in the game.
/// </summary>
public abstract class InputReceiverDelegate : MonoBehaviour {

	public IntVariable currentMenuMode;
	public UnityEvent menuAcceptEvent;
	public UnityEvent menuBackEvent;
	public UnityEvent menuMoveEvent;
	public UnityEvent menuFailEvent;
	private bool active = false;

	/// <summary>
	/// What should happen when the menu mode is changed.
	/// Usually used to enable/disable the menu.
	/// </summary>
	public abstract void OnMenuModeChanged();

	public abstract void OnUpArrow();
	public abstract void OnDownArrow();
	public abstract void OnLeftArrow();
	public abstract void OnRightArrow();

	public abstract void OnOkButton();
	public abstract void OnBackButton();
	public abstract void OnLButton();
	public abstract void OnRButton();
	public abstract void OnXButton();
	public abstract void OnYButton();
	public abstract void OnStartButton();
	

	protected void MenuChangeDelay(MenuMode newMode) {
		InputDelegateController.instance.TriggerMenuChange(newMode);
	}

	protected void SceneChangeDelay(MenuMode newMode, string scene) {
		InputDelegateController.instance.TriggerSceneChange(newMode, scene);
	}

	protected void OnEnable() {
		Debug.Log("Enabled " + gameObject.name);
		InputDelegateController.instance.menuModeChanged += OnMenuModeChanged;
	}

	protected void OnDisable() {
		InputDelegateController.instance.menuModeChanged -= OnMenuModeChanged;
	}

	protected bool UpdateState(params MenuMode[] mode) {
		bool prevActive = active;
		active = false;
		for (int i = 0; i < mode.Length; i++) {
			if (currentMenuMode.value == (int)mode[i]) {
				active = true;
				break;
			}
		}
		if (prevActive != active)
			ActivateDelegates();
		return active;
	}

	protected void ActivateDelegates() {
		if (active) {
			InputDelegateController.instance.upArrowDelegate += OnUpArrow;
			InputDelegateController.instance.downArrowDelegate += OnDownArrow;
			InputDelegateController.instance.leftArrowDelegate += OnLeftArrow;
			InputDelegateController.instance.rightArrowDelegate += OnRightArrow;
			
			InputDelegateController.instance.acceptButtonDelegate += OnOkButton;
			InputDelegateController.instance.cancelButtonDelegate += OnBackButton;
			InputDelegateController.instance.trigLButtonDelegate += OnLButton;
			InputDelegateController.instance.trigRButtonDelegate += OnRButton;
			InputDelegateController.instance.opLButtonDelegate += OnXButton;
			InputDelegateController.instance.opRButtonDelegate += OnYButton;
			InputDelegateController.instance.startButtonDelegate += OnStartButton;
}
		else {
			InputDelegateController.instance.upArrowDelegate -= OnUpArrow;
			InputDelegateController.instance.downArrowDelegate -= OnDownArrow;
			InputDelegateController.instance.leftArrowDelegate -= OnLeftArrow;
			InputDelegateController.instance.rightArrowDelegate -= OnRightArrow;

			InputDelegateController.instance.acceptButtonDelegate -= OnOkButton;
			InputDelegateController.instance.cancelButtonDelegate -= OnBackButton;
			InputDelegateController.instance.trigLButtonDelegate -= OnLButton;
			InputDelegateController.instance.trigRButtonDelegate -= OnRButton;
			InputDelegateController.instance.opLButtonDelegate -= OnXButton;
			InputDelegateController.instance.opRButtonDelegate -= OnYButton;
			InputDelegateController.instance.startButtonDelegate -= OnStartButton;
		}
	}
}
