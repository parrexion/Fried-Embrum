using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour {

	public Text orbText;
	public IntVariable currentOrbs;
	

	// Use this for initialization
	private void Start () {
		orbText.text = currentOrbs.value.ToString();
	}
}
