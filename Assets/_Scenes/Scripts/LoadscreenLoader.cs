using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadscreenLoader : MonoBehaviour {

	const string DIALOGUE_SCENE = "DialogueScene";
	const string BATTLE_SCENE = "BattleScene";

	public GameObject loadCanvas;
	public bool fakeLoading;
	
	public UnityEvent loadMapEvent;
	

	private void Start () {
		StartCoroutine(LoadScenes());
	}

	private IEnumerator LoadScenes() {
		AsyncOperation dia = SceneManager.LoadSceneAsync(DIALOGUE_SCENE, LoadSceneMode.Additive);
		AsyncOperation bat = SceneManager.LoadSceneAsync(BATTLE_SCENE, LoadSceneMode.Additive);

		while(!dia.isDone || !bat.isDone) {
			yield return null;
		}

		if (fakeLoading)
			yield return new WaitForSeconds(3f);

		loadCanvas.SetActive(false);
		loadMapEvent.Invoke();
		yield break;
	}

}
