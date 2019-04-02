using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(UiStyler))]
public class UiStylerEditor : Editor {

	public override void OnInspectorGUI() {
		
		if (GUILayout.Button("Apply style")) {
			Debug.Log("Applying style - Starting...");
			UiStyler styler = (UiStyler)target;

			List<GameObject> roots = new List<GameObject>(EditorSceneManager.GetActiveScene().rootCount+1);
			EditorSceneManager.GetActiveScene().GetRootGameObjects(roots);

			for (int i = 0; i < roots.Count; i++) {
				MyButton[] buttons = roots[i].GetComponentsInChildren<MyButton>(true);
				for (int b = 0; b < buttons.Length; b++) {
					switch (buttons[b].style)
					{
					case MyButton.ButtonType.MAIN:
						buttons[b].SetStyle(styler.mainStyle);
						break;
					case MyButton.ButtonType.BASE:
						buttons[b].SetStyle(styler.baseStyle);
						break;
					}
				}

				MyText[] texts = roots[i].GetComponentsInChildren<MyText>(true);
				for (int t = 0; t < texts.Length; t++) {
					switch (texts[t].style)
					{
					case MyText.TextType.HUGE:
						texts[t].SetStyle(styler.hugeText);
						break;
					case MyText.TextType.TITLE:
						texts[t].SetStyle(styler.titleText);
						break;
					case MyText.TextType.SUBTITLE:
						texts[t].SetStyle(styler.subTitleText);
						break;
					case MyText.TextType.BREAD:
						texts[t].SetStyle(styler.breadText);
						break;
					}
				}
			}
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			EditorSceneManager.SaveOpenScenes();
			Debug.Log("Applying style - DONE");
		}
		GUILayout.Space(10);

		DrawDefaultInspector();
	}
}
