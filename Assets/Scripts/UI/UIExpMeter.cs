using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExpMeter : MonoBehaviour {

	[Header("Exp Meter")]
	public MyBar expBar;
	public int currentExp;

	
	private void Update () {
		expBar.SetAmount(currentExp, 100);
	}
}
