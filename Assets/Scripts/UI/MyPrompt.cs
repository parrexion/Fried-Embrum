using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI class for handling a simple prompt view with a yes and a no button.
/// </summary>
public class MyPrompt : MonoBehaviour {

	public GameObject promptView;
	public TMPro.TextMeshProUGUI textArea;
	public MyButton yesButton;
	public MyButton noButton;
	private int position;


	private void Start() {
		promptView.SetActive(false);
	}

	/// <summary>
	/// Displays the prompt window at the given start position and message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="startOk"></param>
	public void ShowWindow(string message, bool startOk) {
		position = startOk ? 0 : 1;
		textArea.text = message;
		UpdateButtons();
		promptView.SetActive(true);
	}

	/// <summary>
	/// Moves the cursor on the prompt in dir direction.
	/// </summary>
	public void Move(int dir) {
		position = OPMath.FullLoop(0,2, position + dir);
		UpdateButtons();
	}

	/// <summary>
	/// Clicks the prompt window. Returns true if the click is an ok, false otherwise.
	/// </summary>
	/// <param name="isOk"></param>
	/// <returns></returns>
	public bool Click(bool isOk) {
		promptView.SetActive(false);
		return (isOk && position == 0);
	}

	/// <summary>
	/// Updates the visuals on the buttons.
	/// </summary>
	private void UpdateButtons() {
		yesButton.SetSelected(position == 0);
		noButton.SetSelected(position == 1);
	}
}
