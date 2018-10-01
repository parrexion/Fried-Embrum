using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MenuMode { NONE, MAP, UNIT, INV, ATTACK, HEAL, STATS, INGAME, HELP, TRADE, DIALOGUE }

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
	// public BoolVariable lockMoveControls;

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
	public UnityEvent sp1ButtonEvent;
	public UnityEvent sp2ButtonEvent;
	public UnityEvent startButtonEvent;

	private int holdUp;
	private int holdDown;
	private int holdLeft;
	private int holdRight;


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

		// if (!lockMoveControls.value) {

			//Button holds
			if (Input.GetKey(KeyCode.UpArrow)) {
				holdUp++;
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
				holdDown++;
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
				holdLeft++;
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				holdRight++;
			}

			//Button releases
			if (Input.GetKeyUp(KeyCode.UpArrow)) {
				holdUp = 0;
			}
			if (Input.GetKeyUp(KeyCode.DownArrow)) {
				holdDown = 0;
			}
			if (Input.GetKeyUp(KeyCode.LeftArrow)) {
				holdLeft = 0;
			}
			if (Input.GetKeyUp(KeyCode.RightArrow)) {
				holdRight = 0;
			}

			// Arrow presses
			if (Input.GetKeyDown(KeyCode.UpArrow) || holdUp > holdDelay) {
				upArrowEvent.Invoke();
				holdUp -= scrollSpeed;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow) || holdDown > holdDelay) {
				downArrowEvent.Invoke();
				holdDown -= scrollSpeed;
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow) || holdLeft > holdDelay) {
				leftArrowEvent.Invoke();
				holdLeft -= scrollSpeed;
			}
			if (Input.GetKeyDown(KeyCode.RightArrow) || holdRight > holdDelay) {
				rightArrowEvent.Invoke();
				holdRight -= scrollSpeed;
			}
		// }

		if (Input.GetKeyDown(KeyCode.Z)) {
			okButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.X)) {
			backButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			sp1ButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			sp2ButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			startButtonEvent.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
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
