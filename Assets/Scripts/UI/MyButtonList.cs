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
		bot = 0;
		top = 0;
		position = 0;
		UpdateButtons();
	}

	/// <summary>
	/// Adds a new button to the end of the list and updates the list.
	/// </summary>
	/// <param name="button"></param>
	public void AddButton(string button) {
		buttonNames.Add(button);
		top = Mathf.Min(bot + size, buttonNames.Count);
		UpdateButtons();
	}

	/// <summary>
	/// Forces the position of the button to the given index.
	/// </summary>
	/// <param name="pos"></param>
	public void ForcePosition(int pos) {
		position = pos;
		UpdateButtons();
	}

	/// <summary>
	/// Moves the highlight to the next button.
	/// dir defins which direction the selection should move in.
	/// </summary>
	/// <param name="dir"></param>
	public void Move(int dir) {
		position = OPMath.FullLoop(0, buttonNames.Count, position + dir);
		if(position <= bot)
			bot = Mathf.Max(0, position - 1);
		else if(top -1 <= position)
			bot = Mathf.Max(0, Mathf.Min(buttonNames.Count - size, position - size +2));
		top = Mathf.Min(bot + size, buttonNames.Count);

		UpdateButtons();
	}

	/// <summary>
	/// Updates the highlighs and focus of the buttons.
	/// </summary>
	private void UpdateButtons() {
		for(int i = 0; i < size; i++) {
			buttons[i].SetSelected(bot + i == (position));
			buttons[i].gameObject.SetActive(bot + i < top);
			if (bot + i < top)
				buttons[i].buttonText.text = buttonNames[bot + i];
		}
	}

	/// <summary>
	/// Get the index for the currently highlighted button.
	/// </summary>
	/// <returns></returns>
	public int GetPosition() {
		return position;
	}
}
