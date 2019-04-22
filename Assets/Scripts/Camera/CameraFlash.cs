using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour {

	[Header("References")]
	public Image flashImage;
	public Color flashColor = Color.white;
	public ScrObjEntryReference flashBackground;

	[Header("Settings")]
	public FloatVariable flashBeforeTime;
	public FloatVariable flashAfterTime;
	
	
	private void OnEnable () {
		flashColor = Color.white;
		flashColor.a = 0;
		flashImage.sprite = (flashBackground.value != null) ? ((BackgroundEntry)flashBackground.value).sprite : null;
		flashImage.color = flashColor;
	}


	public void StartScreenFlash() {
		flashImage.sprite = (flashBackground.value != null) ? ((BackgroundEntry)flashBackground.value).sprite : null;
		if (flashBeforeTime.value > 0 && flashAfterTime.value > 0)
			StartCoroutine(ScreenFlash());
		else if (flashBeforeTime.value > 0)
			StartCoroutine(ScreenFadeOut());
		else if (flashAfterTime.value > 0)
			StartCoroutine(ScreenFadeIn());
	}

	private IEnumerator ScreenFlash() {
		float currentTime = 0;
		while (currentTime < flashBeforeTime.value) {
			flashColor.a = Mathf.Lerp(0,1,currentTime/flashBeforeTime.value);
			flashImage.color = flashColor;
			currentTime += Time.deltaTime;
			yield return null;
		}

		currentTime = 0;
		while (currentTime < flashAfterTime.value) {
			flashColor.a = Mathf.Lerp(1,0,currentTime/flashAfterTime.value);
			flashImage.color = flashColor;
			currentTime += Time.deltaTime;
			yield return null;
		}

		flashColor.a = 0;
		flashImage.color = flashColor;
		yield break;
	}

	private IEnumerator ScreenFadeOut() {
		float currentTime = 0;
		while (currentTime < flashBeforeTime.value) {
			flashColor.a = Mathf.Lerp(0,1,currentTime/flashBeforeTime.value);
			flashImage.color = flashColor;
			currentTime += Time.deltaTime;
			yield return null;
		}

		flashColor.a = 1;
		flashImage.color = flashColor;
		yield break;
	}

	private IEnumerator ScreenFadeIn() {
		float currentTime = 0;
		while (currentTime < flashAfterTime.value) {
			flashColor.a = Mathf.Lerp(1,0,currentTime/flashAfterTime.value);
			flashImage.color = flashColor;
			currentTime += Time.deltaTime;
			yield return null;
		}

		flashColor.a = 0;
		flashImage.color = flashColor;
		yield break;
	}
}
