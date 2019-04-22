using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupScript : MonoBehaviour {

	private bool starting = false;


    public void LoadDone() {
		if (starting)
			return;
		starting = true;
        StartCoroutine(InitStuff());
    }

	private IEnumerator InitStuff() {
		while(InputDelegateController.instance == null) {
			yield return null;
		}
		yield return null;
		SceneManager.LoadScene(1);
		yield break;
	}
}
