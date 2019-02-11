using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListEntry : MonoBehaviour {

	public Image highlight;
	public Image icon;
	public Text entryName;

	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

	public void SetDark(bool state) {
		icon.color = (!state) ? Color.white : Color.grey;
	}
}
