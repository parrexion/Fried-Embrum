using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GatchaController : MonoBehaviour {

	private enum MenuState { RETURN, SELECT, OPENING, FINISH }
	
	public Gatcha[] gatchas;
	public float[] probabilities;
	public StarList[] starLists;
	public SaveListVariable availableCharacters;
	
	[Header("Orbs")]
	public IntVariable currentOrbs;
	public int[] orbCosts;

	[Header("Return Menu")]
	public GameObject returnCanvas;
	public Text orbCostText;
	public Button payButton;
	public Button returnButton;
	public GameObject finishCanvas;
	
	[Header("Opening Button Menu")]
	public GameObject openButtonCanvas;
	public Button openButton;

	[Header("Opening Animations Menu")] 
	public int selectedIndex;
	public GameObject animationObject;
	public Image animationImage;
	public Button animOkButton;
	public Text animationName;
	public Image[] animationStars;

	[Space(10)]
	public UnityEvent saveGameEvent;
	
	private MenuState _currentState;
	private int _currentCost;
	
	
	// Use this for initialization
	private void Start () {
		for (int i = 0; i < gatchas.Length; i++) {
			int stars = 5 - GetRandomStarList();
			CharacterStats character = starLists[stars-1].GetRandom();
			gatchas[i].index = i;
			gatchas[i].icon.color = (character.weapons.Length == 0) ? Color.white : character.weapons[0].GetTypeColor();
			gatchas[i].stars = stars;
			gatchas[i].character = character;
			gatchas[i].isSelected.enabled = false;
		}

		_currentCost = 1;
		selectedIndex = -1;
		_currentState = MenuState.SELECT;
		UpdateMenu();
	}

	public void UpdateMenu() {
		returnCanvas.SetActive(_currentState == MenuState.RETURN);
		openButtonCanvas.SetActive(_currentState == MenuState.SELECT);
		animationObject.SetActive(_currentState == MenuState.OPENING);
		finishCanvas.SetActive(_currentState == MenuState.FINISH);

		openButton.interactable = false;
		animOkButton.interactable = false;
		if (_currentState == MenuState.RETURN) {
			orbCostText.text = "Orbs:  " + currentOrbs.value + "  ->  " + (currentOrbs.value - orbCosts[_currentCost]);
			payButton.interactable = currentOrbs.value >= orbCosts[_currentCost];
		}
	}

	private int GetRandomStarList() {
		float number = Random.Range(0f, 1f);
		for (int i = 0; i < probabilities.Length; i++) {
			number -= probabilities[i];
			if (number <= 0)
				return i;
		}

		return starLists.Length - 1;
	}

	public void GatchaClicked(int index) {
		if (_currentState != MenuState.SELECT)
			index = -1;
		
		selectedIndex = index;
		for (int i = 0; i < gatchas.Length; i++) {
			gatchas[i].isSelected.enabled = (selectedIndex == gatchas[i].index);
		}
		openButton.interactable = (selectedIndex != -1 && !gatchas[selectedIndex].hasBeenOpened);
	}

	private IEnumerator RunOpeningAnimation() {
		animationObject.SetActive(true);
		animationName.text = "";
		animationImage.sprite = null;
		animationImage.color = gatchas[selectedIndex].icon.color;
		for (int i = 0; i < animationStars.Length; i++) {
			animationStars[i].enabled = false;
		}
		
		yield return new WaitForSeconds(1f);
		
		animationName.text = gatchas[selectedIndex].character.charName;
		animationImage.color = Color.white;
		animationImage.sprite = gatchas[selectedIndex].character.portrait;

		for (int i = 0; i < gatchas[selectedIndex].stars; i++) {
			yield return new WaitForSeconds(0.5f);
			animationStars[i].enabled = true;
		}
		yield return new WaitForSeconds(0.5f);
		
		animOkButton.interactable = true;
	}

	public void PayButton() {
		currentOrbs.value -= orbCosts[_currentCost];
		_currentState = MenuState.SELECT;
		_currentCost++;
		UpdateMenu();
	}

	public void OpenButton() {
		_currentState = MenuState.OPENING;
		UpdateMenu();
		AddCharacter(gatchas[selectedIndex]);
		StartCoroutine(RunOpeningAnimation());
	}

	public void AnimationOkButton() {
		gatchas[selectedIndex].hasBeenOpened = true;
		gatchas[selectedIndex].icon.color = Color.white;
		gatchas[selectedIndex].icon.sprite = gatchas[selectedIndex].character.portrait;
		GatchaClicked(-1);
		_currentState = (_currentCost >= 5) ? MenuState.FINISH : MenuState.RETURN;
		UpdateMenu();
	}

	public void AddCharacter(Gatcha g) {
		bool res = availableCharacters.AddNew(g.stars, g.character);
		Debug.Log(res ? "Successfully added a new character" : "Failed to add the new character :(");
		saveGameEvent.Invoke();
	}
	
}
