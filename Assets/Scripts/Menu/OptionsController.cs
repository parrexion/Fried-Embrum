using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {
	
	public GameObject optionMenu;
	public Text explanationField;
	
	public OptionEntry[] options;
	private EntryList<OptionEntry> optionList;


	private void Start() {
		optionList = new EntryList<OptionEntry>(options.Length);
		for (int i = 0; i < options.Length; i++) {
			optionList.CreateEntry(options[i].transform);
		}
		optionMenu.SetActive(false);
	}

	/// <summary>
	/// Updates the state of the how to play screen.
	/// </summary>
	/// <param name="active"></param>
    public void UpdateState(bool active) {
		optionList.ForcePosition(0);
        optionMenu.SetActive(active);
		UpdateOptions();
	}

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public void MoveVertical(int dir) {
		optionList.Move(dir);
    }

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public bool MoveHorizontal(int dir) {
		return optionList.GetEntry().MoveValue(dir);
    }

	/// <summary>
	/// Resets the help screen position back to the first one again.
	/// </summary>
	public void BackClicked() {
		optionMenu.SetActive(false);
	}

	/// <summary>
	/// Checks if it's possible to click on OK in this screen.
	/// </summary>
	/// <returns></returns>
	public bool OKClicked() {
		return optionList.GetEntry().OnClick();
	}


	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void UpdateOptions() {
		explanationField.text = optionList.GetEntry().explanation;
		for (int i = 0; i < optionList.Size; i++) {
			optionList.GetEntry(i).UpdateUI();
		}
	}

}
