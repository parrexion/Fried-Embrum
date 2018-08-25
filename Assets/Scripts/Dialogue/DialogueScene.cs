﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueScene : MonoBehaviour {

	public ScrObjEntryReference background;
	public ScrObjEntryReference[] characters;
	public IntVariable[] poses;
	public StringVariable talkingName;
	public IntVariable talkingIndex;
	public StringVariable dialogueText;
	public StringVariable inputText;
	public AudioVariable bkgMusic;
	public AudioVariable sfxClip;
	public ScrObjEntryReference flashBackground;
	public FloatVariable effectStartDuration;
	public FloatVariable effectEndDuration;

	
	[Header("Animations")]
	public Character[] characterTransforms;

	[Header("Events")]
	public UnityEvent dialogueEndEvent;
	public UnityEvent backgroundChanged;
	public UnityEvent bkgMusicChanged;
	public UnityEvent playSfx;
	public UnityEvent characterChanged;
	public UnityEvent dialogueTextChanged;
	public UnityEvent screenFlashEvent;
	public UnityEvent screenShakeEvent;


	public void Reset() {
		Debug.Log("RESET!");
		background.value = null;
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
	}

}