using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which encapsulates a list of MyButton as a visual representation.
/// Contains general functions to manipulating the list.
/// </summary>
[System.Serializable]
public class MyButtonList : MonoBehaviour {

	public MyButton[] buttons;

	private List<string> buttonNames = new List<string>();
	private List<Sprite> buttonIcons = new List<Sprite>();
	private List<int> buttonValues = new List<int>();
	private int size;
	private int position;
	private int top;
	private int bot;


	/// <summary>
	/// Resets and initializes the button list.
	/// Must be called before it can be used!
	/// </summary>
	public void ResetButtons() {
		size = buttons.Length;
		buttonNames.Clear();
		buttonIcons.Clear();
		buttonValues.Clear();
		bot = 0;
		top = 0;
		position = 0;
		UpdateButtons();
	}

	/// <summary>
	/// Adds a new button to the end of the list and updates the list.
	/// </summary>
	/// <param name="buttonText"></param>
	public void AddButton(Sprite buttonIcon, int buttonValue = -1) {
		if (size == 0)
			Debug.LogError("MyButtonList is not initialized");

		buttonNames.Add("");
		buttonIcons.Add(buttonIcon);
		buttonValues.Add(buttonValue);
		top = Mathf.Min(bot + size, buttonNames.Count);
		UpdateButtons();
	}

	/// <summary>
	/// Adds a new button to the end of the list and updates the list.
	/// </summary>
	/// <param name="buttonText"></param>
	public void AddButton(string buttonText, int buttonValue = -1) {
		if (size == 0)
			Debug.LogError("MyButtonList is not initialized");

		buttonNames.Add(buttonText);
		buttonIcons.Add(null);
		buttonValues.Add(buttonValue);
		top = Mathf.Min(bot + size, buttonNames.Count);
		UpdateButtons();
	}

	/// <summary>
	/// Forces the position of the button to the given index.
	/// </summary>
	/// <param name="pos"></param>
	public void ForcePosition(int pos) {
		position = (buttonNames.Count != 0) ? pos : -1;
		UpdateButtons();
	}

	/// <summary>
	/// Moves the highlight to the next button.
	/// dir defins which direction the selection should move in.
	/// </summary>
	/// <param name="dir"></param>
	public int Move(int dir) {
		if (buttonNames.Count == 0)
			return -1;

		position = OPMath.FullLoop(0, buttonNames.Count, position + dir);
		if(position <= bot)
			bot = Mathf.Max(0, position - 1);
		else if(top -1 <= position)
			bot = Mathf.Max(0, Mathf.Min(buttonNames.Count - size, position - size +2));
		top = Mathf.Min(bot + size, buttonNames.Count);

		UpdateButtons();
		return position;
	}

	/// <summary>
	/// Updates the highlighs and focus of the buttons.
	/// </summary>
	private void UpdateButtons() {
		for(int i = 0; i < size; i++) {
			buttons[i].SetSelected(bot + i == (position));
			buttons[i].gameObject.SetActive(bot + i < top);
			if (bot + i < top) {
				buttons[i].buttonText.text = buttonNames[bot + i];
				if (buttons[i].buttonIcon != null)
					buttons[i].buttonIcon.sprite = buttonIcons[bot + i];
			}
		}
	}

	/// <summary>
	/// Get the index for the currently highlighted button.
	/// </summary>
	/// <returns></returns>
	public int GetPosition() {
		return (buttonNames.Count != 0) ? position : -1;
	}

	/// <summary>
	/// Get the index for the currently highlighted button.
	/// </summary>
	/// <returns></returns>
	public int GetValue() {
		return (position != -1) ? buttonValues[position] : 0;
	}
	
	/// <summary>
	/// Returns true if there are more entries above the ones that are visible.
	/// </summary>
	/// <returns></returns>
	public bool CanScrollUp() {
		return bot > 0;
	}
	
	/// <summary>
	/// Returns true if there are more entries below the ones that are visible.
	/// </summary>
	/// <returns></returns>
	public bool CanScrollDown() {
		return top < buttons.Length;
	}
}
