using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadscreenLoader : MonoBehaviour {

	const string DIALOGUE_SCENE = "DialogueScene";
	const string BATTLE_SCENE = "BattleScene";

	[Header("Chapter")]
	public StringVariable loadMapID;
	public ScrObjEntryReference currentMap;
	public ScrObjLibraryVariable mapLibrary;

	[Header("Loading")]
	public GameObject loadCanvas;
	public bool fakeLoading;
	public UnityEvent loadMapEvent;
	

	private void Start () {
		currentMap.value = mapLibrary.GetEntry(loadMapID.value);
		StartCoroutine(LoadBattleScenes());
	}

	private IEnumerator LoadBattleScenes() {
		AsyncOperation bat = SceneManager.LoadSceneAsync(BATTLE_SCENE, LoadSceneMode.Additive);
		AsyncOperation dia = SceneManager.LoadSceneAsync(DIALOGUE_SCENE, LoadSceneMode.Additive);

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
