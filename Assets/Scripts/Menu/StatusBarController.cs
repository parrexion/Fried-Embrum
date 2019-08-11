using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarController : MonoBehaviour {
	
	[Header("Texts")]
	public Text turnCountText;
	public Text totalMoneyText;
	public Text totalScrapText;

	[Header("Variables")]
	public IntVariable currentTurn;
	public IntVariable totalMoney;
	public IntVariable totalScrap;


	void Start() {
		UpdateTurn();
		UpdateCash();
	}

	public void UpdateTurn() {
		turnCountText.text = "Turn:  " + currentTurn.value;
	}

	public void UpdateCash() {
		totalMoneyText.text = "Money:  " + totalMoney.value;
		totalScrapText.text = "Scrap:  " + totalScrap.value;
	}
}
