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

	[Header("Events")]
	public UnityEvent beginBattleEvent;
	public UnityEvent resumeBattleEvent;


	private void Start () {
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

	public void DialogueEnd() {
		switch (currentDialogueMode.value)
		{
			case (int)DialogueMode.PRE:
				beginBattleEvent.Invoke();
				ActivateStuff(false);
				break;
			case (int)DialogueMode.POST:
				SceneManager.LoadScene("SaveScene");
				break;
			case (int)DialogueMode.VISIT:
			case (int)DialogueMode.EVENT:
			case (int)DialogueMode.QUOTE:
				Debug.Log("Resume!");
				resumeBattleEvent.Invoke();
				ActivateStuff(false);
				break;
		}
	}
}
