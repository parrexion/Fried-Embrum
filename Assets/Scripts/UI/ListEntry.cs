using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class ListEntry : MonoBehaviour {

	public Image highlight;
	public bool dark;

	public Image icon;
	public Text entryName;


	/// <summary>
	/// Updates the cursor highlight for the entry.
	/// </summary>
	/// <param name="state"></param>
	public void SetHighlight(bool state) {
		highlight.enabled = state;
	}

	/// <summary>
	/// Updates the icon of the entry and greys it if it should be darkened.
	/// </summary>
	/// <param name="state"></param>
	public void SetDark(bool state) {
		dark = state;
		icon.color = (state) ? Color.grey : Color.white;
	}
}
