using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableKeeper : MonoBehaviour {

#region Singleton

	private static VariableKeeper instance = null;
	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

#endregion


	public ScrObjEntryReference currentMap;
	public ScrObjEntryReference currentDialogue;

}
