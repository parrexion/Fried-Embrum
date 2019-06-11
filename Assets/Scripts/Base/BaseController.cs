using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : InputReceiverDelegate {

	public GameObject mainArea;
	public Transform[] areas;
	//public BoolVariable lockControls;
	//public float speed = 1;
	//public int widthOffset;
	public MenuMode[] screens;

	//private int width;
	//private int height;

	public MyButtonList menuButtons;


	private void Start() {
		//width = Screen.width;
		//height = Screen.height;
		//for (int i = 0; i < areas.Count; i++) {
		//	areas[i].gameObject.SetActive(true);
		//}
		//SetAreaPositions(0);
		//UpdateState((MenuMode)currentMenuMode.value);
		//InputDelegateController.instance.TriggerMenuChange(screens[1]);
		//lockControls.value = false;

		SetupButtons();
		MenuChangeDelay(MenuMode.BASE_MAIN);
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.BASE_MAIN);
		if (active) {
			mainArea.SetActive(true);
			for (int i = 0; i < areas.Length; i++) {
				areas[i].gameObject.SetActive(false);
			}
		}
	}

	private void SetupButtons() {
		menuButtons.ResetButtons();
		menuButtons.AddButton("MISSIONS");
		menuButtons.AddButton("TRAINING AREA");
		menuButtons.AddButton("ARMORY");
		menuButtons.AddButton("EQUIPMENT");
		menuButtons.AddButton("RESEARCH LAB");
		//menuButtons.AddButton("HOUSING");
	}

	public override void OnUpArrow() {
		menuButtons.Move(-1);
	}

	public override void OnDownArrow() {
		menuButtons.Move(1);
	}

	public override void OnOkButton() {
		mainArea.SetActive(false);
		int menu = menuButtons.GetPosition();
		for (int i = 0; i < areas.Length; i++) {
			areas[i].gameObject.SetActive(i == menu);
		}

		MenuChangeDelay(screens[menu]);
	}

	public void StartMission() {
		//lockControls.value = false;
		UpdateState(0);
		SceneChangeDelay(MenuMode.NONE, "LoadingScreen");
	}

	public override void OnLButton() {
		//Transform a = areas[areas.Count-1];
		//areas.RemoveAt(areas.Count-1);
		//areas.Insert(0, a);
		//MenuMode mode = screens[screens.Count-1];
		//screens.RemoveAt(screens.Count-1);
		//screens.Insert(0, mode);
		//StartCoroutine(ChangeArea(1));
	}

	public override void OnRButton() {
		//Transform a = areas[0];
		//areas.RemoveAt(0);
		//areas.Add(a);
		//MenuMode mode = screens[0];
		//screens.RemoveAt(0);
		//screens.Add(mode);
		//StartCoroutine(ChangeArea(-1));
	}

	private IEnumerator ChangeArea(int direction) {
		//lockControls.value = true;
		//float f = 0;
		//while (f < 1) {
		//	f += Time.deltaTime * speed;
		//	float offset = Mathf.Lerp(width + widthOffset, 0, f) * direction;
		//	SetAreaPositions(offset);
		//	yield return null;
		//}
		//InputDelegateController.instance.TriggerMenuChange(screens[1]);
		//lockControls.value = false;
		yield break;
	}

	private void SetAreaPositions(float offset) {
		//for (int i = 0; i < areas.Count; i++) {
		//	areas[i].transform.position = new Vector3(width * 0.5f + (width + widthOffset) * (i-1) - offset, height * 0.5f, 0);
		//}
	}



	public override void OnLeftArrow() { }
	public override void OnRightArrow() { }
	public override void OnBackButton() { }
	public override void OnStartButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }

}
