using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuMode { NONE, MAP, UNIT, INV, ATTACK, HEAL, STATS, INGAME, NNN, TRADE, DIALOGUE, TOOLTIP, 
						BASE_LAB, BASE_MISSION, BASE_HOUSE, BASE_TRAIN, BASE_SHOP, PREP, FORMATION, SAVE }

public class InputDelegateController : MonoBehaviour {

#region Singleton
	public static InputDelegateController instance = null;
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
	public int holdDelay = 25;
	public int scrollSpeed = 5;

	[Header("Control locks")]
	public BoolVariable lockAllControls;

	[Header("Play Time Clock")]
	public IntVariable currentPlayTime;

	[Header("Move values")]
	private int holdUp;
	private int holdDown;
	private int holdLeft;
	private int holdRight;

	private bool axisUp;
	private bool axisDown;
	private bool axisLeft;
	private bool axisRight;

	//Delegates
	public delegate void ButtonDelegate();
	public ButtonDelegate menuModeChanged;

	public ButtonDelegate upArrowDelegate;
	public ButtonDelegate downArrowDelegate;
	public ButtonDelegate leftArrowDelegate;
	public ButtonDelegate rightArrowDelegate;

	public ButtonDelegate okButtonDelegate;
	public ButtonDelegate backButtonDelegate;
	public ButtonDelegate lButtonDelegate;
	public ButtonDelegate rButtonDelegate;
	public ButtonDelegate xButtonDelegate;
	public ButtonDelegate yButtonDelegate;
	public ButtonDelegate startButtonDelegate;


	private void Setup() {
		//currentAction.value = ActionMode.NONE;
		//StartCoroutine(TransitionDelay(startMode));
		//StartCoroutine(CountPlayTime());
	}

	private IEnumerator TransitionDelay(MenuMode newMode) {
		yield return null;
		menuMode.value = (int)newMode;
		if(menuModeChanged != null)
			menuModeChanged.Invoke();
		lockAllControls.value = false;
	}

	public void TriggerMenuChange(MenuMode newMode) {
		lockAllControls.value = true;
		Debug.Log("set MENU to:  " + newMode);
		StartCoroutine(TransitionDelay(newMode));
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
			if(upArrowDelegate != null)
				upArrowDelegate.Invoke();
			holdUp -= scrollSpeed;
			axisUp = true;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || holdDown > holdDelay || (!axisDown && (Input.GetAxis("DpadVertical") == -1 || Input.GetAxis("LstickVertical") == -1))) {
			if(downArrowDelegate != null)
				downArrowDelegate.Invoke();
			holdDown -= scrollSpeed;
			axisDown = true;
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || holdLeft > holdDelay || (!axisLeft && (Input.GetAxis("DpadHorizontal") == -1 || Input.GetAxis("LstickHorizontal") == -1))) {
			if(leftArrowDelegate != null)
				leftArrowDelegate.Invoke();
			holdLeft -= scrollSpeed;
			axisLeft = true;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) || holdRight > holdDelay || (!axisRight && (Input.GetAxis("DpadHorizontal") == 1 || Input.GetAxis("LstickHorizontal") == 1))) {
			if(rightArrowDelegate != null)
				rightArrowDelegate.Invoke();
			holdRight -= scrollSpeed;
			axisRight = true;
		}

		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
			if (okButtonDelegate != null)
				okButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
			if(backButtonDelegate != null)
				backButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton4)) {
			if(lButtonDelegate != null)
				lButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
			if(rButtonDelegate != null)
				rButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.JoystickButton3)) {
			if(xButtonDelegate != null)
				xButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
			if(yButtonDelegate != null)
				yButtonDelegate();
		}
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6) ||
			Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
			if(startButtonDelegate != null)
				startButtonDelegate();
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
