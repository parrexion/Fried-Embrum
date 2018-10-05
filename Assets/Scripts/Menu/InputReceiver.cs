using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class which contains functions for all the buttons used in the game.
/// </summary>
public abstract class InputReceiver : MonoBehaviour {

	public IntVariable currentMenuMode;
	public UnityEvent menuModeChangedEvent;
	public UnityEvent menuAcceptEvent;
	public UnityEvent menuBackEvent;
	public UnityEvent menuMoveEvent;
	protected bool active;

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


	protected IEnumerator MenuChangeDelay() {
		active = false;
		yield return null;
		menuModeChangedEvent.Invoke();
	}
}
