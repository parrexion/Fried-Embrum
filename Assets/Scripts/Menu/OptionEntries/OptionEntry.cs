using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class OptionEntry : MonoBehaviour {

	public string explanation;
	public UnityEvent updateEvent;


	public abstract void UpdateUI();
	public abstract bool OnLeft();
	public abstract bool OnRight();
	public abstract bool OnClick();
}
