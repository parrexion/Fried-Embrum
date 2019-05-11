using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI class for handling a simple prompt view with a yes and a no button.
/// </summary>
public class MyPrompt : MonoBehaviour {

	public enum StyleType { NONE, BIG, SMALL }
	public enum Result { OK1, OK2, CANCEL, LOCKED }
	const string YES_NAME = "YES";
	const string NO_NAME = "NO";
	const string OK_NAME = "OK";
	
	public StyleType style;

	[Header("Objects")]
	public Image backgroundImage;
	public GameObject promptView;
	public Text textArea;
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
	/// Show a prompt without any buttons to click the prompt away.
	/// </summary>
	/// <param name="message"></param>
	public void ShowSpinner(string message) {
		optionSize = 0;
		textArea.text = message;
		yesButton.gameObject.SetActive(false);
		yes2Button.gameObject.SetActive(false);
		noButton.gameObject.SetActive(false);
		okButton.gameObject.SetActive(false);
		promptView.SetActive(true);
	}

	/// <summary>
	/// Displays the prompt window at the given start position and message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="startOk"></param>
	public void ShowWindow(string message, bool startOk, string yesName = "", string noName = "") {
		optionSize = 2;
		position = startOk ? 1 : 0;
		textArea.text = message;
		yesButton.buttonText.text = (string.IsNullOrEmpty(yesName)) ? YES_NAME : yesName;
		noButton.buttonText.text = (string.IsNullOrEmpty(noName)) ? NO_NAME : noName;
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
	public void Show3Options(string message, string option1, string option2, string cancel, bool startOption) {
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
		if (position == -1 || optionSize == 0)
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
		if(optionSize == 0)
			return Result.LOCKED;

		promptView.SetActive(false);

		if (!isOk)
			return Result.CANCEL;

		switch (position) {
		case 1:
			return Result.OK1;
		case 2:
			return Result.OK2;
		default:
			return Result.CANCEL;
		}
	}

	public void StopSpinner() {
		if (optionSize != 0)
			return;
		promptView.SetActive(false);
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

	/// <summary>
	/// Updates the style to the one in the UIStyler.
	/// </summary>
	/// <param name="style"></param>
	/// <param name="font"></param>
	public void SetStyle(PromptStyle style, Font font) {
		backgroundImage.sprite = style.backgroundImage;
		backgroundImage.color = style.backgroundColor;
		
		yesButton.SetStyle(style.buttonStyle, font);
		yes2Button.SetStyle(style.buttonStyle, font);
		noButton.SetStyle(style.buttonStyle, font);
		okButton.SetStyle(style.buttonStyle, font);
	}
}
