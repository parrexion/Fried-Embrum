using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI class which handles a group of tabs.
/// Has functions for changing tab and updating current tab.
/// </summary>
public class MyTabber : MonoBehaviour {

	public GameObject[] tabs;

	private int position;


    private void Start() {
        if (tabs.Length == 0) {
			Debug.LogWarning("MyTabber is empty.   " + name);
			Destroy(this);
		}
    }

	/// <summary>
	/// Changes the current tab in the direction of dir.
	/// </summary>
	/// <param name="dir"></param>
	public void Move(int dir) {
		position = OPMath.FullLoop(0, tabs.Length, position + dir);
		UpdateTabs();
	}

	/// <summary>
	/// Currently selected tab.
	/// </summary>
	/// <returns></returns>
	public int GetPosition() {
		return position;
	}

	private void UpdateTabs() {
		for (int i = 0; i < tabs.Length; i++) {
			tabs[i].SetActive(i == position);
		}
	}
}
