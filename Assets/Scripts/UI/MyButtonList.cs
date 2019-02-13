using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtonList : MonoBehaviour {

	public MyButton[] buttons;

	private List<string> buttonNames = new List<string>();
	private int size;
	private int position;
	private int top;
	private int bot;


	public void ResetButtons() {
		size = buttons.Length;
		buttonNames.Clear();
		bot = 0;
		top = 0;
		position = 0;
		UpdateButtons();
	}

	public void AddButton(string button) {
		buttonNames.Add(button);
		top = Mathf.Min(bot + size, buttonNames.Count);
		UpdateButtons();
	}

	public void ForcePosition(int pos) {
		position = pos;
		UpdateButtons();
	}

	public void Move(int dir) {
		position = OPMath.FullLoop(0, buttonNames.Count, position + dir);
		if(position <= bot)
			bot = Mathf.Max(0, position - 1);
		else if(top -1 <= position)
			bot = Mathf.Max(0, Mathf.Min(buttonNames.Count - size, position - size +2));
		top = Mathf.Min(bot + size, buttonNames.Count);

		UpdateButtons();
	}

	private void UpdateButtons() {
			//Debug.Log("#   " + size);
		for(int i = 0; i < size; i++) {
			buttons[i].SetSelected(bot + i == (position));
			buttons[i].gameObject.SetActive(bot + i < top);
			if (bot + i < top)
				buttons[i].buttonText.text = buttonNames[bot + i];
		}
	}

	public int GetPosition() {
		return position;
	}
}
