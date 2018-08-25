using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialogueAdvanceFrame : MonoBehaviour {

	public ScrObjLibraryVariable dialogueLibrary;
	public MapInfoVariable currentMap;

    public IntVariable currentFrame;
    public BoolVariable skippableDialogue;
	public UnityEvent dialogueClickEvent;


    void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && skippableDialogue.value){
            currentFrame.value = currentMap.value.preDialogue.actions.Count-1;
            dialogueClickEvent.Invoke();
            dialogueClickEvent.Invoke();
        }
    }

}
