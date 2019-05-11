using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MySpinnerData {
	public Sprite icon;
	public string text;
	public SfxEntry sfx;
}

public class MySpinner : MonoBehaviour {

	public enum StyleType { NONE, BIG, SMALL }
    
	public StyleType style;
	public BoolVariable lockControls;

	[Header("Objects")]
	public GameObject spinnerView;
	public Image backgroundImage;
	public Image spinnerIcon;
	public Text messageText;

	[Header("Sound")]
	public AudioQueueVariable sfxQueue;
	public UnityEvent playSfxEvent;

	private bool showing;


	private void Start() {
		spinnerView.SetActive(false);
	}
	
	/// <summary>
	/// Displays a spinner notification with the given information.
	/// Locks the controls for the duration if lockGame is true.
	/// </summary>
	/// <param name="duration"></param>
	/// <param name="icon"></param>
	/// <param name="text"></param>
	/// <param name="lockGame"></param>
	public IEnumerator ShowSpinner(MySpinnerData data, float duration = 2f, bool lockGame = true) {
		yield return StartCoroutine(ShowSpinner(data.icon, data.text, data.sfx, duration, lockGame));
	}

	/// <summary>
	/// Displays a spinner notification with the given information.
	/// Locks the controls for the duration if lockGame is true.
	/// </summary>
	/// <param name="duration"></param>
	/// <param name="icon"></param>
	/// <param name="text"></param>
	/// <param name="lockGame"></param>
	public IEnumerator ShowSpinner(Sprite icon, string text, SfxEntry sfx, float duration = 2f, bool lockGame = true) {
		spinnerIcon.sprite = icon;
		messageText.text = text;

		if (sfx != null) {
			sfxQueue.Enqueue(sfx);
			playSfxEvent.Invoke();
		}

		yield return StartCoroutine(DisplaySpinner(duration, lockGame));
	}

	private IEnumerator DisplaySpinner(float duration, bool lockGame) {
		if (showing)
			yield break;

		showing = true;
		if (lockGame)
			lockControls.value = true;

		spinnerView.SetActive(true);
		yield return new WaitForSeconds(duration);
		spinnerView.SetActive(false);

		if (lockGame)
			lockControls.value = false;
		showing = false;
	}

	/// <summary>
	/// Updates the style to the one in the UIStyler.
	/// </summary>
	/// <param name="style"></param>
	/// <param name="font"></param>
	public void SetStyle(SpinnerStyle style, Font font) {
		backgroundImage.sprite = style.backgroundImage;
		backgroundImage.color = style.backgroundColor;
		
		messageText.color = style.fontColor;
		messageText.resizeTextMaxSize = style.fontMaxSize;
		messageText.font = font;
	}
}
