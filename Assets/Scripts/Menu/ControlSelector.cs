using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSelector : MonoBehaviour {

	public IntVariable selectedScheme;
	public GameObject[] schemes;


	private void OnEnable() {
		UpdateScheme();
	}

	public void UpdateScheme() {
		for (int i = 0; i < schemes.Length; i++) {
			schemes[i].SetActive(i == selectedScheme.value);
		}
	}

}
