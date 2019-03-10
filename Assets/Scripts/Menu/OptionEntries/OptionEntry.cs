using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class OptionEntry : ListEntry {

	public string explanation;
	public UnityEvent updateEvent;


	public abstract void UpdateUI();
	public abstract bool MoveValue(int dir);
	public abstract bool OnClick();
}
