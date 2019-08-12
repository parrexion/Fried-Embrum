using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugButtonController : MonoBehaviour {

	public GameObject debugView;

	private bool visible;


	void Start() {
		debugView.SetActive(false);
	}

	public void ToggleMenu() {
		visible = !visible;
		debugView.SetActive(visible);
		if (visible) {
			StartCoroutine(AutoClose());
		}
		else {
			StopAllCoroutines();
		}
	}

	private IEnumerator AutoClose() {
		yield return new WaitForSeconds(4f);
		ToggleMenu();
	}
}
