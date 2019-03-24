using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour {

	public ScrObjEntryReference dialogueBackground;
	public Image image;
	public Image backdrop;

	
	private void Start () {
		backdrop.enabled = true;
		image.enabled = false;
	}
	
	public void UpdateBackground () {
		if (dialogueBackground.value == null) {
			backdrop.enabled = true;
			image.enabled = false;
		}
		else if (((BackgroundEntry)dialogueBackground.value).sprite == null) {
			image.enabled = false;
			backdrop.enabled = false;
		}
		else {
			backdrop.enabled = true;
			image.sprite = ((BackgroundEntry)dialogueBackground.value).sprite;
			image.enabled = true;
		}
	}
}
