using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleSceneController : MonoBehaviour {

	public GameObject[] battleObjects;
	public IntVariable currentMenuMode;
	public IntVariable currentDialogueMode;
	public UnityEvent menuModeChanged;


	private void Start () {

	}

	public void ActivateStuff(bool active) {
		for (int i = 0; i < battleObjects.Length; i++) {
			battleObjects[i].SetActive(active);
		}
		if (!active) {
			currentMenuMode.value = (int)MenuMode.DIALOGUE;
			menuModeChanged.Invoke();
		}
	}
}
