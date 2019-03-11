using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadscreenLoader : MonoBehaviour {

	public GameObject loadCanvas;

	[Header("Dialogue")]
	public IntVariable currentDialogueMode;
	public UnityEvent startDialogueEvent;
	

	private void Start () {
		StartCoroutine(LoadScenes());
	}

	private IEnumerator LoadScenes() {
		AsyncOperation dia = SceneManager.LoadSceneAsync("DialogueScene", LoadSceneMode.Additive);
		AsyncOperation bat = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

		while(!dia.isDone || !bat.isDone) {
			yield return null;
		}

		loadCanvas.SetActive(false);
		//yield return null;
		//currentDialogueMode.value = (int)DialogueMode.PRE;
		//startDialogueEvent.Invoke();
		yield break;
	}

}
