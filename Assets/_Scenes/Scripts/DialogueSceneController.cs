using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueSceneController : MonoBehaviour {

	public GameObject[] dialogueObjects;
	public Canvas[] dialogueCanvases;
	public DialogueLines lines;
	public IntVariable currentDialogueMode;
	public BoolVariable musicFocusSource;

	[Header("Events")]
	public UnityEvent resumeBattleEvent;
	public UnityEvent resumeTurnEvent;
	public UnityEvent nextTurnEvent;
	public UnityEvent playSubMusicEvent;


	private void Start () {
		currentDialogueMode.value = 0;
		ActivateStuff(false);
	}

	/// <summary>
	/// Sets the state of all the objects in the dialogue scene to active.
	/// </summary>
	/// <param name="active"></param>
	public void ActivateStuff(bool active) {
		for (int i = 0; i < dialogueCanvases.Length; i++) {
			dialogueCanvases[i].enabled = active;
		}
		for (int i = 0; i < dialogueObjects.Length; i++) {
			dialogueObjects[i].SetActive(active);
		}
	}

	/// <summary>
	/// Triggered when the dialogue ends. 
	/// Deactivates the dialogue stuff and returns back to game again.
	/// </summary>
	public void DialogueEnd() {

		switch (currentDialogueMode.value)
		{
			case (int)DialogueMode.PRELUDE:
				InputDelegateController.instance.TriggerMenuChange(MenuMode.NONE);
				nextTurnEvent.Invoke();
				break;
			case (int)DialogueMode.INTRO:
				InputDelegateController.instance.TriggerMenuChange(MenuMode.NONE);
				nextTurnEvent.Invoke();
				break;
			case (int)DialogueMode.ENDING:
				StartCoroutine(SaveMenuTransition());
				break;
			case (int)DialogueMode.EVENT:
				InputDelegateController.instance.TriggerMenuChange(MenuMode.NONE);
				resumeTurnEvent.Invoke();
				musicFocusSource.value = true;
				playSubMusicEvent.Invoke();
				break;
			case (int)DialogueMode.VISIT:
				InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
				musicFocusSource.value = true;
				playSubMusicEvent.Invoke();
				break;
			case (int)DialogueMode.QUOTE:
				InputDelegateController.instance.TriggerMenuChange(MenuMode.BATTLE);
				resumeBattleEvent.Invoke();
				break;
		}
		//currentDialogueMode.value = (int)DialogueMode.NONE;
	}


	private IEnumerator SaveMenuTransition() {
		InputDelegateController.instance.TriggerMenuChange(MenuMode.SAVE);
		yield return null;
		yield return null;
		SceneManager.LoadScene("SaveScene");
	}
}
