using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

	[Header("Popup Objects")]
	public GameObject popupObject;
	public Image popupIcon;
	public Text popupText;

	[Header("Sound")]
	public AudioQueueVariable sfxQueue;
	public UnityEvent playSfxEvent;

	[Header("Default Values")]
	public float defaultDuration = 2f;
	public float defaultCooldown = 0.5f;


	private void Start () {
		popupObject.SetActive(false);
	}

	/// <summary>
	/// Shows the popup with the given text and icon.
	/// Uses the given custom duration and cooldown.
	/// </summary>
	/// <param name="icon"></param>
	/// <param name="text"></param>
	/// <param name="sfx"></param>
	/// <returns></returns>
	public IEnumerator ShowPopup(Sprite icon, string text, SfxEntry sfx, float showDuration, float cooldown) {
		popupIcon.sprite = icon;
		popupText.text = text;
		popupObject.SetActive(true);
		if (sfx != null) {
			sfxQueue.Enqueue(sfx);
			playSfxEvent.Invoke();
		}
		yield return new WaitForSeconds(showDuration);
		popupObject.SetActive(false);
		yield return new WaitForSeconds(cooldown);
	}

	/// <summary>
	/// Shows the popup with the given text and icon.
	/// Uses the default duration and cooldown.
	/// </summary>
	/// <param name="icon"></param>
	/// <param name="text"></param>
	/// <param name="sfx"></param>
	/// <returns></returns>
	public IEnumerator ShowPopup(Sprite icon, string text, SfxEntry sfx) {
		popupIcon.sprite = icon;
		popupText.text = text;
		popupObject.SetActive(true);
		if (sfx != null) {
			sfxQueue.Enqueue(sfx);
			playSfxEvent.Invoke();
		}
		yield return new WaitForSeconds(defaultDuration);
		popupObject.SetActive(false);
		yield return new WaitForSeconds(defaultCooldown);
	}
}
