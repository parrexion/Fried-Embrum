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
			
			MyButton[] buttons = Resources.FindObjectsOfTypeAll<MyButton>();
			for (int b = 0; b < buttons.Length; b++) {
				switch (buttons[b].style)
				{
				case MyButton.ButtonType.MAIN:
					buttons[b].SetStyle(styler.mainStyle, styler.font);
					break;
				case MyButton.ButtonType.BASE:
					buttons[b].SetStyle(styler.baseStyle, styler.font);
					break;
				case MyButton.ButtonType.ACTION:
					buttons[b].SetStyle(styler.actionStyle, styler.font);
					break;
				case MyButton.ButtonType.NOSELECT:
					buttons[b].SetStyle(styler.noSelectStyle, styler.font);
					break;
				}
				if (buttons[b].gameObject.scene.name != null) {
					EditorUtility.SetDirty(buttons[b]);
				}
			}

			MyText[] texts = Resources.FindObjectsOfTypeAll<MyText>();
			for (int t = 0; t < texts.Length; t++) {
				switch (texts[t].style)
				{
				case MyText.TextType.HUGE:
					texts[t].SetStyle(styler.hugeText, styler.font);
					break;
				case MyText.TextType.TITLE:
					texts[t].SetStyle(styler.titleText, styler.font);
					break;
				case MyText.TextType.SUBTITLE:
					texts[t].SetStyle(styler.subTitleText, styler.font);
					break;
				case MyText.TextType.BREAD:
					texts[t].SetStyle(styler.breadText, styler.font);
					break;
				case MyText.TextType.LIST_TITLE:
					texts[t].SetStyle(styler.listTitleText, styler.font);
					break;
				case MyText.TextType.MENU_TITLE:
					texts[t].SetStyle(styler.menuTitleText, styler.font);
					break;
				case MyText.TextType.STATS_BIG:
					texts[t].SetStyle(styler.statsBigText, styler.font);
					break;
				case MyText.TextType.STATS_MID:
					texts[t].SetStyle(styler.statsMediumText, styler.font);
					break;
				case MyText.TextType.STATS_SMALL:
					texts[t].SetStyle(styler.statsSmallText, styler.font);
					break;
				case MyText.TextType.STATS_PENALTY:
					texts[t].SetStyle(styler.statsPenaltyText, styler.font);
					break;
				}
				if(texts[t].gameObject.scene.name != null) {
					EditorUtility.SetDirty(texts[t]);
				}
			}

			MyBar[] bars = Resources.FindObjectsOfTypeAll<MyBar>();
			for(int p = 0; p < bars.Length; p++) {
				switch(bars[p].style) {
					case MyBar.StyleType.HEALTH:
						bars[p].SetStyle(styler.healthBar, styler.font);
						break;
					case MyBar.StyleType.EXP:
						bars[p].SetStyle(styler.expBar, styler.font);
						break;
					case MyBar.StyleType.FULFILL:
						bars[p].SetStyle(styler.fulfillBar, styler.font);
						break;
				}
				if(bars[p].gameObject.scene.name != null) {
					EditorUtility.SetDirty(bars[p].gameObject);
				}
			}

			ListEntry[] lists = Resources.FindObjectsOfTypeAll<ListEntry>();
			for(int l = 0; l < lists.Length; l++) {
				switch(lists[l].style) {
					case ListEntry.ListType.NORMAL:
						lists[l].SetStyle(styler.normalList, styler.font);
						break;
					case ListEntry.ListType.THIN:
						lists[l].SetStyle(styler.thinList, styler.font);
						break;
				}
				if(lists[l].gameObject.scene.name == null) {
					EditorUtility.SetDirty(lists[l]);
				}
			}

			MyPrompt[] prompts = Resources.FindObjectsOfTypeAll<MyPrompt>();
			for(int p = 0; p < prompts.Length; p++) {
				switch(prompts[p].style) {
					case MyPrompt.PromptType.BIG:
						prompts[p].SetStyle(styler.selectPopup, styler.font);
						break;
					case MyPrompt.PromptType.SMALL:
						prompts[p].SetStyle(styler.smallPopup, styler.font);
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
