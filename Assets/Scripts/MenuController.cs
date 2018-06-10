using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public IntVariable currentOrbs;
	public Button payButton;
	
	
	private void Start() {
		if (payButton != null)
			payButton.interactable = (currentOrbs.value >= 5);
	}

	public void BattleClicked() {
		SceneManager.LoadScene("LoadoutScene");
	}

	public void StartClicked() {
		SceneManager.LoadScene("BattleScene");
	}

	public void ReturnClicked() {
		SceneManager.LoadScene("MainMenu");
	}

	public void EditClicked() {
		SceneManager.LoadScene("EditScene");
	}

	public void CharacterClicked() {
		currentOrbs.value -= 5;
		SceneManager.LoadScene("GatchaScene");
	}
}
