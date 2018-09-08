using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour {

	public GameObject optionMenu;
	public Text explanationField;

	public Image[] options;
	public ScriptableObject[] optionValues;
	public string[] explanations;
	public Image[] optionCheckmarks;
	public Text[] optionTexts;

	public UnityEvent volumeChanged;

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
			optionIndex = options.Length -1;
		SetupOptions();
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public void MoveDown() {
        optionIndex++;
		if (optionIndex >= options.Length)
			optionIndex = 0;
		SetupOptions();
    }

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public bool MoveLeft() {
		if (optionIndex >= 2)
			return false;

		IntVariable option = (IntVariable)optionValues[optionIndex];
		int before = option.value;
		option.value = Mathf.Max(0, option.value - 10);

		SetupOptions();
		volumeChanged.Invoke();
		return (option.value != before);
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public bool MoveRight() {
		if (optionIndex >= 2)
			return false;

		IntVariable option = (IntVariable)optionValues[optionIndex];
		int before = option.value;
		option.value = Mathf.Min(100, option.value + 10);

		SetupOptions();
		volumeChanged.Invoke();
		return (option.value != before);
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
		if (optionIndex < 2)
			return false;
		BoolVariable option = (BoolVariable)optionValues[optionIndex];
		option.value = !option.value;
		SetupOptions();
		return true;
	}


	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void SetupOptions() {
		explanationField.text = explanations[optionIndex];
		for (int i = 0; i < options.Length; i++) {
			options[i].color = (optionIndex == i) ? Color.cyan : Color.white;
			if (i < 2) {
				IntVariable option = (IntVariable)optionValues[i];
				optionTexts[i].text = option.value.ToString();
			}
			else {
				BoolVariable option = (BoolVariable)optionValues[i];
				optionCheckmarks[i].enabled = option.value;
			}
		}
	}

}
