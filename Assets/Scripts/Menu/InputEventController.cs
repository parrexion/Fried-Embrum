using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MenuMode { NONE, MAP, UNIT, INV, ATTACK, HEAL, STATS, INGAME, HELP, TRADE, DIALOGUE, TOOL, 
						BASE_LAB, BASE_MISSION, BASE_HOUSE, BASE_TRAIN, BASE_SHOP, PREP, FORMATION }

public class InputEventController : MonoBehaviour {

#region Singleton
	private static InputEventController instance = null;
	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			Setup();
		}
	}
#endregion

	public IntVariable menuMode;
	public MenuMode startMode;
	public ActionModeVariable currentAction;
	public UnityEvent menuModeChanged;
	public int holdDelay = 25;
	public int scrollSpeed = 5;

	[Header("Control locks")]
	public BoolVariable lockAllControls;

	[Header("Play Time Clock")]
	public IntVariable currentPlayTime;

	[Header("Move events")]
	public UnityEvent upArrowEvent;
	public UnityEvent downArrowEvent;
	public UnityEvent leftArrowEvent;
	public UnityEvent rightArrowEvent;
	
	[Header("Button Events")]
	public UnityEvent okButtonEvent;
	public UnityEvent backButtonEvent;
	public UnityEvent lButtonEvent;
	public UnityEvent rButtonEvent;
	public UnityEvent xButtonEvent;
	public UnityEvent yButtonEvent;
	public UnityEvent startButtonEvent;

	private int holdUp;
	private int holdDown;
	private int holdLeft;
	private int holdRight;

	private bool axisUp;
	private bool axisDown;
	private bool axisLeft;
	private bool axisRight;
	

	private void Setup() {
		currentAction.value = ActionMode.NONE;
		menuMode.value = (int)startMode;
		StartCoroutine(TransitionDelay());
		StartCoroutine(CountPlayTime());
	}

	private IEnumerator TransitionDelay() {
		yield return null;
		menuModeChanged.Invoke();
	}

	private void Update() {
		if (lockAllControls.value)
			return;

		//Button holds
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("DpadVertical") == 1 || Input.GetAxis("LstickVertical") == 1) {
			holdUp++;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("DpadVertical") == -1 || Input.GetAxis("LstickVertical") == -1) {
			holdDown++;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("DpadHorizontal") == -1 || Input.GetAxis("LstickHorizontal") == -1) {
			holdLeft++;
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("DpadHorizontal") == 1 || Input.GetAxis("LstickHorizontal") == 1) {
			holdRight++;
		}

		//Button releases
		if (Input.GetKeyUp(KeyCode.UpArrow) && Input.GetAxis("DpadVertical") == 0 && Input.GetAxis("LstickVertical") == 0) {
			holdUp = 0;
			axisUp = false;
		}
		if (Input.GetKeyUp(KeyCode.DownArrow) && Input.GetAxis("DpadVertical") == 0 && Input.GetAxis("LstickVertical") == 0) {
			holdDown = 0;
			axisDown = false;
		}
		if (Input.GetKeyUp(KeyCode.LeftArrow) && Input.GetAxis("DpadHorizontal") == 0 && Input.GetAxis("LstickHorizontal") == 0) {
			holdLeft = 0;
			axisLeft = false;
		}
		if (Input.GetKeyUp(KeyCode.RightArrow) && Input.GetAxis("DpadHorizontal") == 0 && Input.GetAxis("LstickHorizontal") == 0) {
			holdRight = 0;
			axisRight = false;
		}

		// Arrow presses
		if (Input.GetKeyDown(KeyCode.UpArrow) || holdUp > holdDelay || (!axisUp && (Input.GetAxis("DpadVertical") == 1 || Input.GetAxis("LstickVertical") == 1))) {
			upArrowEvent.Invoke();
			holdUp -= scrollSpeed;
			axisUp = true;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || holdDown > holdDelay || (!axisDown && (Input.GetAxis("DpadVertical") == -1 || Input.GetAxis("LstickVertical") == -1))) {
			downArrowEvent.Invoke();
			holdDown -= scrollSpeed;
			axisDown = true;
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || holdLeft > holdDelay || (!axisLeft && (Input.GetAxis("DpadHorizontal") == -1 || Input.GetAxis("LstickHorizontal") == -1))) {
			leftArrowEvent.Invoke();
			holdLeft -= scrollSpeed;
			axisLeft = true;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) || holdRight > holdDelay || (!axisRight && (Input.GetAxis("DpadHorizontal") == 1 || Input.GetAxis("LstickHorizontal") == 1))) {
			rightArrowEvent.Invoke();
			holdRight -= scrollSpeed;
			axisRight = true;
		}

		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
			okButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
			backButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton4)) {
			lButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
			rButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.JoystickButton3)) {
			xButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
			yButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6)) {
			startButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
			startButtonEvent.Invoke();
		}
	}

	/// <summary>
	/// Continuosly counts up the current play time.
	/// </summary>
	/// <returns></returns>
	IEnumerator CountPlayTime() {
        while (true) {
            yield return new WaitForSeconds(1);
            currentPlayTime.value++;
        }
	}
}
