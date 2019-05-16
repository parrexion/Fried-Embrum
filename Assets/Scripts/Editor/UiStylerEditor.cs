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
				case MyButton.StyleType.MAIN:
					buttons[b].SetStyle(styler.mainStyle, styler.font);
					break;
				case MyButton.StyleType.BASE:
					buttons[b].SetStyle(styler.baseStyle, styler.font);
					break;
				case MyButton.StyleType.ACTION:
					buttons[b].SetStyle(styler.actionStyle, styler.font);
					break;
				case MyButton.StyleType.NOSELECT:
					buttons[b].SetStyle(styler.noSelectStyle, styler.font);
					break;
				case MyButton.StyleType.ICON:
					buttons[b].SetStyle(styler.iconStyle, styler.font);
					break;
				case MyButton.StyleType.OPTIONS:
					buttons[b].SetStyle(styler.optionsStyle, styler.font);
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
				case MyText.StyleType.HUGE:
					texts[t].SetStyle(styler.hugeText, styler.font);
					break;
				case MyText.StyleType.TITLE:
					texts[t].SetStyle(styler.titleText, styler.font);
					break;
				case MyText.StyleType.SUBTITLE:
					texts[t].SetStyle(styler.subTitleText, styler.font);
					break;
				case MyText.StyleType.BREAD:
					texts[t].SetStyle(styler.breadText, styler.font);
					break;
				case MyText.StyleType.OBJECTIVE:
					texts[t].SetStyle(styler.objectiveText, styler.font);
					break;
				case MyText.StyleType.MENU_TITLE:
					texts[t].SetStyle(styler.menuTitleText, styler.font);
					break;
				case MyText.StyleType.STATS_BIG:
					texts[t].SetStyle(styler.statsBigText, styler.font);
					break;
				case MyText.StyleType.STATS_MID:
					texts[t].SetStyle(styler.statsMediumText, styler.font);
					break;
				case MyText.StyleType.STATS_SMALL:
					texts[t].SetStyle(styler.statsSmallText, styler.font);
					break;
				case MyText.StyleType.STATS_PENALTY:
					texts[t].SetStyle(styler.statsPenaltyText, styler.font);
					break;
				case MyText.StyleType.BASE_TITLE:
					texts[t].SetStyle(styler.baseTitleText, styler.font);
					break;
				case MyText.StyleType.BASE_HUGE:
					texts[t].SetStyle(styler.baseBigText, styler.font);
					break;
				case MyText.StyleType.BASE_MID:
					texts[t].SetStyle(styler.baseMediumText, styler.font);
					break;
				case MyText.StyleType.BASE_SMALL:
					texts[t].SetStyle(styler.baseSmallText, styler.font);
					break;
				case MyText.StyleType.LEVEL_BONUS:
					texts[t].SetStyle(styler.levelBonusText, styler.font);
					break;
				case MyText.StyleType.DAMAGE:
					texts[t].SetStyle(styler.damageText, styler.font);
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
					case MyBar.StyleType.BIG_EXP:
						bars[p].SetStyle(styler.bigExpBar, styler.font);
						break;
				}
				if(bars[p].gameObject.scene.name != null) {
					EditorUtility.SetDirty(bars[p].gameObject);
				}
			}

			ListEntry[] lists = Resources.FindObjectsOfTypeAll<ListEntry>();
			for(int l = 0; l < lists.Length; l++) {
				switch(lists[l].style) {
					case ListEntry.StyleType.OPTIONS:
						lists[l].SetStyle(styler.optionsList, styler.font);
						break;
					case ListEntry.StyleType.THIN:
						lists[l].SetStyle(styler.thinList, styler.font);
						break;
					case ListEntry.StyleType.SAVE:
						lists[l].SetStyle(styler.saveList, styler.font);
						break;
					case ListEntry.StyleType.TRADE:
						lists[l].SetStyle(styler.tradeList, styler.font);
						break;
					case ListEntry.StyleType.PREP:
						lists[l].SetStyle(styler.prepList, styler.font);
						break;
				}
				if(lists[l].gameObject.scene.name != null) {
					EditorUtility.SetDirty(lists[l]);
				}
			}

			MyPrompt[] prompts = Resources.FindObjectsOfTypeAll<MyPrompt>();
			for(int p = 0; p < prompts.Length; p++) {
				switch(prompts[p].style) {
					case MyPrompt.StyleType.BIG:
						prompts[p].SetStyle(styler.selectPopup, styler.font);
						break;
					case MyPrompt.StyleType.SMALL:
						prompts[p].SetStyle(styler.smallPopup, styler.font);
						break;
				}
				if(prompts[p].gameObject.scene.name != null) {
					EditorUtility.SetDirty(prompts[p]);
				}
			}

			MySpinner[] spinners = Resources.FindObjectsOfTypeAll<MySpinner>();
			for(int s = 0; s < spinners.Length; s++) {
				switch(spinners[s].style) {
					case MySpinner.StyleType.BIG:
						spinners[s].SetStyle(styler.bigSpinner, styler.font);
						break;
					case MySpinner.StyleType.SMALL:
						spinners[s].SetStyle(styler.smallSpinner, styler.font);
						break;
				}
				if(spinners[s].gameObject.scene.name != null) {
					EditorUtility.SetDirty(spinners[s]);
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
