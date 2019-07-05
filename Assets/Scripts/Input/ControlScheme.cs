using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ControlScheme : ScriptableObject {

	public string schemeName = "";

	[Header("Movement")]
	public KeyCode moveUp;
	public KeyCode moveDown;
	public KeyCode moveLeft;
	public KeyCode moveRight;

	[Header("Buttons")]
	public KeyCode accept;
	public KeyCode cancel;
	public KeyCode optionLeft;
	public KeyCode optionRight;
	public KeyCode triggerLeft;
	public KeyCode triggerRight;
	public KeyCode start;
	public KeyCode select;

}
