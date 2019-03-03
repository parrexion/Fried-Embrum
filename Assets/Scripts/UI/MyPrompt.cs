using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI class for handling a simple prompt view with a yes and a no button.
/// </summary>
public class MyPrompt : MonoBehaviour {

	public enum Result { OK1, OK2, CANCEL }
	const string YES_NAME = "YES";
	const string NO_NAME = "NO";
	const string OK_NAME = "OK";

	public GameObject promptView;
	public TMPro.TextMeshProUGUI textArea;
	public MyButton yesButton;
	public MyButton yes2Button;
	public MyButton noButton;
	public MyButton okButton;
	private int position;
	private int optionSize;


	private void Start() {
		promptView.SetActive(false);
	}

	/// <summary>
	/// Displays the prompt window at the given start position and message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="startOk"></param>
	public void ShowWindow(string message, bool startOk) {
		optionSize = 2;
		position = startOk ? 1 : 0;
		textArea.text = message;
		yesButton.buttonText.text = YES_NAME;
		noButton.buttonText.text = NO_NAME;
		yesButton.gameObject.SetActive(true);
		yes2Button.gameObject.SetActive(false);
		noButton.gameObject.SetActive(true);
		okButton.gameObject.SetActive(false);
		UpdateButtons();
		promptView.SetActive(true);
	}

	/// <summary>
	/// Shows a simple popup with a message and an OK button to click.
	/// </summary>
	/// <param name="message"></param>
	public void ShowPopup(string message) {
		optionSize = 1;
		position = -1;
		textArea.text = message;
		okButton.buttonText.text = OK_NAME;
		yesButton.gameObject.SetActive(false);
		yes2Button.gameObject.SetActive(false);
		noButton.gameObject.SetActive(false);
		okButton.gameObject.SetActive(true);
		UpdateButtons();
		promptView.SetActive(true);
	}

	/// <summary>
	/// Displays the prompt window with two ok options and a cancel button.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="startOk"></param>
	public void Show2Options(string message, string option1, string option2, string cancel, bool startOption) {
		optionSize = 3;
		position = startOption ? 2 : 0;
		textArea.text = message;
		yesButton.buttonText.text = option1;
		yes2Button.buttonText.text = option2;
		noButton.buttonText.text = cancel;
		yesButton.gameObject.SetActive(true);
		yes2Button.gameObject.SetActive(true);
		noButton.gameObject.SetActive(true);
		okButton.gameObject.SetActive(false);
		UpdateButtons();
		promptView.SetActive(true);
	}

	/// <summary>
	/// Moves the cursor on the prompt in dir direction.
	/// </summary>
	public void Move(int dir) {
		if (position == -1)
			return;
		if (optionSize > 2)
			dir *= -1;

		position = OPMath.FullLoop(0,optionSize, position - dir);
		UpdateButtons();
	}

	/// <summary>
	/// Clicks the prompt window. isOk should be true if accept is clicked.
	/// Returns the results depending on the position.
	/// </summary>
	/// <param name="isOk"></param>
	/// <returns></returns>
	public Result Click(bool isOk) {
		if (!isOk)
			return Result.CANCEL;

		promptView.SetActive(false);
		switch (position) {
		case 1:
			return Result.OK1;
		case 2:
			return Result.OK2;
		default:
			return Result.CANCEL;
		}
	}

	/// <summary>
	/// Updates the visuals on the buttons.
	/// </summary>
	private void UpdateButtons() {
		okButton.SetSelected(position == -1);
		noButton.SetSelected(position == 0);
		yesButton.SetSelected(position == 1);
		yes2Button.SetSelected(position == 2);
	}
}
