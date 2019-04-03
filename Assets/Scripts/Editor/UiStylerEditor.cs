﻿using System.Collections;
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
			
			MyButton[] buttons = Resources.FindObjectsOfTypeAll<MyButton>();
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
				if (buttons[b].gameObject.scene.name == null) {
					EditorUtility.SetDirty(buttons[b]);
				}
			}

			MyText[] texts = Resources.FindObjectsOfTypeAll<MyText>();
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
				if(texts[t].gameObject.scene.name == null) {
					EditorUtility.SetDirty(texts[t]);
				}
			}

			MyPrompt[] prompts = Resources.FindObjectsOfTypeAll<MyPrompt>();
			for(int p = 0; p < prompts.Length; p++) {
				switch(prompts[p].style) {
					case MyPrompt.PromptType.BIG:
						prompts[p].SetStyle(styler.selectPopup);
						break;
					case MyPrompt.PromptType.SMALL:
						prompts[p].SetStyle(styler.smallPopup);
						break;
				}
				if(prompts[p].gameObject.scene.name == null) {
					EditorUtility.SetDirty(prompts[p]);
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
