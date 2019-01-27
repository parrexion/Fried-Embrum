using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseController : InputReceiver {

	public List<Transform> areas;
	public BoolVariable lockControls;
	public float speed = 1;
	public int widthOffset;
	public List<MenuMode> screens = new List<MenuMode>();

	private int currentArea;
	private int width;
	private int height;


    private void Start () {
		width = Screen.width;
		height = Screen.height;
		for (int i = 0; i < areas.Count; i++) {
			areas[i].gameObject.SetActive(true);
		}
		SetAreaPositions(0);
		currentMenuMode.value = (int)screens[1];
		menuModeChangedEvent.Invoke();
		lockControls.value = false;
	}

    public override void OnLButton() {
		Transform a = areas[areas.Count-1];
		areas.RemoveAt(areas.Count-1);
		areas.Insert(0, a);
		MenuMode mode = screens[screens.Count-1];
		screens.RemoveAt(screens.Count-1);
		screens.Insert(0, mode);
		StartCoroutine(ChangeArea(1));
    }

    public override void OnRButton() {
		Transform a = areas[0];
		areas.RemoveAt(0);
		areas.Add(a);
		MenuMode mode = screens[0];
		screens.RemoveAt(0);
		screens.Add(mode);
		StartCoroutine(ChangeArea(-1));
    }

	private IEnumerator ChangeArea(int direction) {
		lockControls.value = true;
		float f = 0;
		while (f < 1) {
			f += Time.deltaTime * speed;
			float offset = Mathf.Lerp(width + widthOffset, 0, f) * direction;
			SetAreaPositions(offset);
			yield return null;
		}
		currentMenuMode.value = (int)screens[1];
		StartCoroutine(MenuChangeDelay());
		lockControls.value = false;
	}

	private void SetAreaPositions(float offset) {
		for (int i = 0; i < areas.Count; i++) {
			areas[i].transform.position = new Vector3(width * 0.5f + (width + widthOffset) * (i-1) - offset, height * 0.5f, 0);
		}
	}

	public void StartMission() {
		lockControls.value = false;
		SceneManager.LoadScene("LoadingScreen");
	}


    public override void OnBackButton() { }
    public override void OnDownArrow() { }
    public override void OnLeftArrow() { }
    public override void OnMenuModeChanged() { }
    public override void OnOkButton() { }
    public override void OnRightArrow() { }
    public override void OnStartButton() { }
    public override void OnUpArrow() { }
    public override void OnXButton() { }
    public override void OnYButton() { }

}
