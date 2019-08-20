using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum DialogueMode { NONE, PRELUDE, INTRO, ENDING, EVENT, VISIT, TALK, QUOTE }

[System.Serializable]
public class DialogueScene : MonoBehaviour {

	[Header("Statics")]
	public BackgroundEntry emptyBackground;
	public ScrObjEntryReference[] characters;

	[Header("Values")]
	public ScrObjEntryReference background;
	public ScrObjEntryReference villageVisitor1;
	public IntVariable[] poses;
	public StringVariable talkingName;
	public IntVariable talkingIndex;
	public StringVariable dialogueText;
	public StringVariable inputText;
	public BoolVariable musicFocusSource;
	public AudioVariable bkgMusic;
	public AudioQueueVariable sfxClip;
	public ScrObjEntryReference flashBackground;
	public FloatVariable effectStartDuration;
	public FloatVariable effectEndDuration;
	
	[Header("Animations")]
	public Character[] characterTransforms;

	[Header("Events")]
	public UnityEvent backgroundChanged;
	public UnityEvent characterChanged;
	public UnityEvent bkgMusicChanged;
	public UnityEvent playSfx;
	public UnityEvent dialogueTextChanged;
	public UnityEvent dialogueEndEvent;
	public UnityEvent screenFlashEvent;
	public UnityEvent screenShakeEvent;


	public void Reset() {
		background.value = emptyBackground;
		for (int i = 0; i < characters.Length; i++) {
			characters[i].value = null;
		}
		for (int i = 0; i < poses.Length; i++) {
			poses[i].value = -1;
		}
		talkingName.value = "";
		talkingIndex.value = -1;
		dialogueText.value = "";
		bkgMusic.value = null;
		
		backgroundChanged.Invoke();
		characterChanged.Invoke();
		//bkgMusicChanged.Invoke();
		dialogueTextChanged.Invoke();
	}

}
