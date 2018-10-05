using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour {

	public GameObject optionMenu;
	public Text explanationField;

	public string[] explanations;
	public Image[] optionBackgrounds;
	public OptionEntry[] options;

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
        optionIndex--;
		if (optionIndex < 0)
			optionIndex = optionBackgrounds.Length -1;
		SetupOptions();
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public void MoveDown() {
        optionIndex++;
		if (optionIndex >= optionBackgrounds.Length)
			optionIndex = 0;
		SetupOptions();
    }

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public bool MoveLeft() {
		return options[optionIndex].OnLeft();
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public bool MoveRight() {
		return options[optionIndex].OnRight();
    }

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
	public bool OKClicked() {
		return options[optionIndex].OnClick();
	}


	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void SetupOptions() {
		explanationField.text = options[optionIndex].explanation;
		for (int i = 0; i < optionBackgrounds.Length; i++) {
			optionBackgrounds[i].color = (optionIndex == i) ? Color.cyan : Color.white;
		}
		for (int i = 0; i < options.Length; i++) {
			options[i].UpdateUI();
		}
	}

}
