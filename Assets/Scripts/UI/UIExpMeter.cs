using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExpMeter : MonoBehaviour {

	[Header("Exp Meter")]
	public Image expMeter;
	public Text expText;
	public int currentExp;

	
	// Update is called once per frame
	private void Update () {
		float fill = currentExp / 100.0f;
		expMeter.fillAmount = fill;
		expText.text = currentExp.ToString();
	}
}
