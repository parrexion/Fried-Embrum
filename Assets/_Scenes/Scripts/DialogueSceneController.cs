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
	public UnityEvent startBattleprepEvent;
	public UnityEvent playSubMusicEvent;


	private void Start () {
		currentDialogueMode.value = 0;
		ActivateStuff(false);
	}

	public void ActivateStuff(bool active) {
		for (int i = 0; i < dialogueObjects.Length; i++) {
			dialogueObjects[i].SetActive(active);
		}
		for (int i = 0; i < dialogueCanvases.Length; i++) {
			dialogueCanvases[i].enabled = active;
		}
		if (active) {
			lines.StartDialogue();
		}
	}

	/// <summary>
	/// Triggered when the dialogue ends. 
	/// Deactivates the dialogue stuff and returns back to game again.
	/// </summary>
	public void DialogueEnd() {
		switch (currentDialogueMode.value)
		{
			case (int)DialogueMode.PRE:
				startBattleprepEvent.Invoke();
				ActivateStuff(false);
				break;
			case (int)DialogueMode.POST:
				SceneManager.LoadScene("SaveScene");
				break;
			case (int)DialogueMode.VISIT:
				resumeBattleEvent.Invoke();
				ActivateStuff(false);
				musicFocusSource.value = true;
				playSubMusicEvent.Invoke();
				break;
			case (int)DialogueMode.QUOTE:
				resumeBattleEvent.Invoke();
				ActivateStuff(false);
				break;
			case (int)DialogueMode.EVENT:
				resumeTurnEvent.Invoke();
				ActivateStuff(false);
				musicFocusSource.value = true;
				playSubMusicEvent.Invoke();
				break;
		}
	}
}
