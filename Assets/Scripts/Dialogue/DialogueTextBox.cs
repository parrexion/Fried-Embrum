using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTextBox : MonoBehaviour {

	public StringVariable talkingName;
	public Text nameBox;
	public StringVariable dialogueText;
	public Text textBox;
	public IntVariable talkingIndex;
	public Transform bubble;
	public GameObject bubbleNoTalk;


	private void Start() {
		textBox.text = "";
		bubble.gameObject.SetActive(false);
		bubbleNoTalk.SetActive(false);
	}

	public void UpdateText() {
		nameBox.text = talkingName.value;
		textBox.text = dialogueText.value;
	}

	public void UpdateBubble() {
		if (talkingIndex == null) {
			bubble.gameObject.SetActive(true);
		}
		else {
			switch (talkingIndex.value)
			{
				case 0:
					bubble.localScale = new Vector3(-1,1,1);
					bubble.gameObject.SetActive(true);
					bubbleNoTalk.SetActive(false);
					transform.localPosition = new Vector3(-200, transform.localPosition.y, 0);
					break;
				case 1:
					bubble.localScale = new Vector3(-1,1,1);
					bubble.gameObject.SetActive(true);
					bubbleNoTalk.SetActive(false);
					transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
					break;
				case 2:
					bubble.localScale = new Vector3(1,1,1);
					bubble.gameObject.SetActive(true);
					bubbleNoTalk.SetActive(false);
					transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
					break;
				case 3:
					bubble.localScale = new Vector3(1,1,1);
					bubble.gameObject.SetActive(true);
					bubbleNoTalk.SetActive(false);
					transform.localPosition = new Vector3(200, transform.localPosition.y, 0);
					break;
				case -1:
				case 4:
					bubble.gameObject.SetActive(false);
					bubbleNoTalk.SetActive(true);
					transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
					break;
			}
		}
	}
}
