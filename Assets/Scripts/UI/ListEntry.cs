using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ListEntry : MonoBehaviour {

	public Image highlight;
	public bool dark;
	public bool show = true;

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
		dark = state;
		icon.color = (state) ? Color.grey : Color.white;
	}
}
