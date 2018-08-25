using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour {

	public GameObject optionMenu;

	public Image[] options;
	public BoolVariable[] optionValues;
	public Image[] optionCheckmarks;
	// public Transform leftArrow;
	// public Transform rightArrow;

	private int optionIndex;


	private void Start() {
		optionMenu.SetActive(false);
	}

	/// <summary>
	/// Updates the state of the how to play screen.
	/// </summary>
	/// <param name="active"></param>
    public void UpdateState(bool active) {
        optionMenu.SetActive(active);
		SetupOptions();
	}

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public void MoveUp() {
        optionIndex = Mathf.Max(0, optionIndex -1);
		SetupOptions();
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public void MoveDown() {
        optionIndex = Mathf.Min(options.Length -1, optionIndex + 1);
		SetupOptions();
    }

	// /// <summary>
	// /// Moves one screen to the left if possible.
	// /// </summary>
    // public void MoveLeft() {
    //     screenPosition = Mathf.Max(0, screenPosition -1);
	// 	SetupOptions();
    // }

	// /// <summary>
	// /// Moves one screen to the right if possible.
	// /// </summary>
    // public void MoveRight() {
    //     screenPosition = Mathf.Min(screens.Length -1, screenPosition + 1);
	// 	SetupOptions();
    // }

	/// <summary>
	/// Resets the help screen position back to the first one again.
	/// </summary>
	public void BackClicked() {
		optionIndex = 0;
		optionMenu.SetActive(false);
	}

	/// <summary>
	/// Checks if it's possible to click on OK in this screen.
	/// </summary>
	/// <returns></returns>
	public void OKClicked() {
		optionValues[optionIndex].value = !optionValues[optionIndex].value;
		SetupOptions();
	}


	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void SetupOptions() {
		// leftArrow.position = new Vector3(leftArrow.position.x, options[optionIndex].transform.position.y, leftArrow.position.z);
		// rightArrow.position = new Vector3(rightArrow.position.x, options[optionIndex].transform.position.y, rightArrow.position.z);

		for (int i = 0; i < options.Length; i++) {
			options[i].color = (optionIndex == i) ? Color.cyan : Color.white;
			optionCheckmarks[i].enabled = optionValues[i].value;
		}
	}

}
