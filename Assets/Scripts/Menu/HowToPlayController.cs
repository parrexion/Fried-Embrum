using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlayController : MonoBehaviour {

	public GameObject controlsObject;

	public GameObject[] screens;
	public GameObject leftArrow;
	public GameObject rightArrow;

	private int screenPosition;


	private void Start() {
		controlsObject.SetActive(false);
	}

	/// <summary>
	/// Updates the state of the how to play screen.
	/// </summary>
	/// <param name="active"></param>
    public void UpdateState(bool active) {
        controlsObject.SetActive(active);
		SetupScreens();
	}

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public bool MoveLeft() {
		int pos = screenPosition;
        screenPosition = Mathf.Max(0, screenPosition -1);
		SetupScreens();
		return (pos != screenPosition);
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public bool MoveRight() {
		int pos = screenPosition;
        screenPosition = Mathf.Min(screens.Length -1, screenPosition + 1);
		SetupScreens();
		return (pos != screenPosition);
    }

	/// <summary>
	/// Resets the help screen position back to the first one again.
	/// </summary>
	public void BackClicked() {
		screenPosition = 0;
		controlsObject.SetActive(false);
	}

	/// <summary>
	/// Checks if it's possible to click on OK in this screen.
	/// </summary>
	/// <returns></returns>
	public bool CheckOk() {
		return (screenPosition == screens.Length -1);
	}


	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void SetupScreens() {
		leftArrow.SetActive(screenPosition != 0);
		rightArrow.SetActive(screenPosition < screens.Length-1);

		for (int i = 0; i < screens.Length; i++) {
			screens[i].SetActive(i == screenPosition);
		}
	}

}
