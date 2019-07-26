using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuMode { NONE, MAP, MAIN_MENU, INV, BATTLE, WEAPON, WWW, INGAME, NNN, TRADE, DIALOGUE, TOOLTIP, 
						BASE_LAB, BASE_MISSION, BASE_HOUSE, BASE_TRAIN, BASE_SHOP, PREP, FORMATION, SAVE,
						BASE_MAIN, BASE_EQUIP, PRE_CONTROLLER }

public class InputDelegateController : MonoBehaviour {

#region Singleton
	public static InputDelegateController instance = null;
	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}
		else {
		Debug.Log("Awake " + gameObject.name);
			instance = this;
			StartCoroutine(CountPlayTime());
			DontDestroyOnLoad(gameObject);
		}
	}
#endregion

	[Header("Input mode")]
	public IntVariable controlSchemeIndex;
	public ControlScheme[] controlSchemes;

	[Header("Menu references")]
	public IntVariable menuMode;
	public MenuMode startMode;
	public ActionModeVariable currentAction;

	[Header("Control locks")]
	public BoolVariable lockAllControls;
	public int holdDelay = 12;
	public int scrollSpeed = 5;

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

	public ButtonDelegate acceptButtonDelegate;
	public ButtonDelegate cancelButtonDelegate;
	public ButtonDelegate trigLButtonDelegate;
	public ButtonDelegate trigRButtonDelegate;
	public ButtonDelegate opLButtonDelegate;
	public ButtonDelegate opRButtonDelegate;
	public ButtonDelegate startButtonDelegate;


	public void TriggerMenuChange(MenuMode newMode) {
		lockAllControls.value = true;
		StartCoroutine(TransitionDelay(newMode));
	}

	private IEnumerator TransitionDelay(MenuMode newMode) {
		yield return null;
		menuMode.value = (int)newMode;
		if(menuModeChanged != null)
			menuModeChanged.Invoke();
		lockAllControls.value = false;
	}

	public void TriggerSceneChange(MenuMode newMode, string scene) {
		lockAllControls.value = true;
		StartCoroutine(LoadNextScene(newMode, scene));
	}

	private IEnumerator LoadNextScene(MenuMode newMode, string scene) {
		yield return null;
		menuMode.value = (int)newMode;
		if(menuModeChanged != null)
			menuModeChanged.Invoke();
		yield return null;
		SceneManager.LoadScene(scene);
	}

	private void Update() {
		if (lockAllControls.value)
			return;

		ControlScheme cs = controlSchemes[controlSchemeIndex.value];

		//Button holds
		if (Input.GetKey(cs.moveUp) || Input.GetAxis("DpadVertical") > 0.8f || Input.GetAxis("LstickVertical") > 0.8f) {
			holdUp++;
		}
		if (Input.GetKey(cs.moveDown) || Input.GetAxis("DpadVertical") < -0.8f || Input.GetAxis("LstickVertical") < -0.8f) {
			holdDown++;
		}
		if (Input.GetKey(cs.moveLeft) || Input.GetAxis("DpadHorizontal") < -0.8f || Input.GetAxis("LstickHorizontal") < -0.8f) {
			holdLeft++;
		}
		if (Input.GetKey(cs.moveRight) || Input.GetAxis("DpadHorizontal") > 0.8f || Input.GetAxis("LstickHorizontal") > 0.8f) {
			holdRight++;
		}

		//Button releases
		if (!Input.GetKey(cs.moveUp) && Input.GetAxis("DpadVertical") < 0.3f && Input.GetAxis("LstickVertical") < 0.3f) {
			holdUp = 0;
			axisUp = false;
		}
		if (!Input.GetKey(cs.moveDown) && Input.GetAxis("DpadVertical") > -0.3f && Input.GetAxis("LstickVertical") > -0.3f) {
			holdDown = 0;
			axisDown = false;
		}
		if (!Input.GetKey(cs.moveLeft) && Input.GetAxis("DpadHorizontal") > -0.3f && Input.GetAxis("LstickHorizontal") > -0.3f) {
			holdLeft = 0;
			axisLeft = false;
		}
		if (!Input.GetKey(cs.moveRight) && Input.GetAxis("DpadHorizontal") < 0.3f && Input.GetAxis("LstickHorizontal") < 0.3f) {
			holdRight = 0;
			axisRight = false;
		}

		// Arrow presses
		if (Input.GetKeyDown(cs.moveUp) || holdUp > holdDelay || (!axisUp && (Input.GetAxis("DpadVertical") > 0.8f || Input.GetAxis("LstickVertical") > 0.8f))) {
			if(upArrowDelegate != null)
				upArrowDelegate.Invoke();
			holdUp -= scrollSpeed;
			axisUp = true;
		}
		if (Input.GetKeyDown(cs.moveDown) || holdDown > holdDelay || (!axisDown && (Input.GetAxis("DpadVertical") < -0.8f || Input.GetAxis("LstickVertical") < -0.8f))) {
			if(downArrowDelegate != null)
				downArrowDelegate.Invoke();
			holdDown -= scrollSpeed;
			axisDown = true;
		}
		if (Input.GetKeyDown(cs.moveLeft) || holdLeft > holdDelay || (!axisLeft && (Input.GetAxis("DpadHorizontal") < -0.8f || Input.GetAxis("LstickHorizontal") < -0.8f))) {
			if(leftArrowDelegate != null)
				leftArrowDelegate.Invoke();
			holdLeft -= scrollSpeed;
			axisLeft = true;
		}
		if (Input.GetKeyDown(cs.moveRight) || holdRight > holdDelay || (!axisRight && (Input.GetAxis("DpadHorizontal") > 0.8f || Input.GetAxis("LstickHorizontal") > 0.8f))) {
			if(rightArrowDelegate != null)
				rightArrowDelegate.Invoke();
			holdRight -= scrollSpeed;
			axisRight = true;
		}

		if (Input.GetKeyDown(cs.accept) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
			acceptButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.cancel) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
			cancelButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.optionLeft) || Input.GetKeyDown(KeyCode.JoystickButton3)) {
			opLButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.optionRight) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
			opRButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.triggerLeft) || Input.GetKeyDown(KeyCode.JoystickButton4)) {
			trigLButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.triggerRight) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
			trigRButtonDelegate?.Invoke();
		}
		if (Input.GetKeyDown(cs.start) || Input.GetKeyDown(KeyCode.JoystickButton6) ||
			Input.GetKeyDown(cs.select) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
			startButtonDelegate?.Invoke();
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
