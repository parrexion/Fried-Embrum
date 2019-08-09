using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CharacterEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Character";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<CharData>();
	protected override Color BackgroundColor => new Color(0.1f, 0.4f, 0.6f);

	private bool showStats;


	public CharacterEditorWindow(ScrObjLibraryVariable entries, CharData container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		CharData charValues = (CharData)entryValues;
		GUILayout.BeginHorizontal();
		charValues.repColor = EditorGUILayout.ColorField("List color", charValues.repColor);
		charValues.faction = (Faction)EditorGUILayout.EnumPopup("Class faction", charValues.faction);
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("Visuals", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Trade portrait", GUILayout.Width(130));
		GUILayout.Label("Info portrait", GUILayout.Width(130));
		GUILayout.Label("Battle sprite", GUILayout.Width(130));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		charValues.bigPortrait = (Sprite)EditorGUILayout.ObjectField("", charValues.bigPortrait, typeof(Sprite), false, GUILayout.Width(130));
		charValues.portrait = (Sprite)EditorGUILayout.ObjectField("", charValues.portrait, typeof(Sprite), false, GUILayout.Width(130));
		charValues.battleSprite = (Sprite)EditorGUILayout.ObjectField("", charValues.battleSprite, typeof(Sprite), false, GUILayout.Width(130));
		GUILayout.EndHorizontal();
		charValues.portraitSet = (PortraitEntry)EditorGUILayout.ObjectField("Portrait set", charValues.portraitSet, typeof(PortraitEntry), false);

		GUILayout.Space(10);

		GUILayout.Label("Other values", EditorStyles.boldLabel);
		charValues.deathQuote = (DialogueEntry)EditorGUILayout.ObjectField("Death Quote", charValues.deathQuote, typeof(DialogueEntry), false);
		charValues.mustSurvive = EditorGUILayout.Toggle("Must survive", charValues.mustSurvive);

		GUILayout.Space(10);

		GUILayout.Label("Class", EditorStyles.boldLabel);
		charValues.startClass = (CharClass)EditorGUILayout.ObjectField("Class", charValues.startClass, typeof(CharClass), false);
		GUILayout.Label("Class levels");
		if(charValues.startClassLevels.Length != ClassWheel.CLASS_COUNT)
			charValues.startClassLevels = new int[ClassWheel.CLASS_COUNT];
		if(charValues.faction == Faction.PLAYER) {
			for(int i = 0; i < ClassWheel.CLASS_COUNT; i++) {
				charValues.startClassLevels[i] = EditorGUILayout.IntField(((PlayerClassName)i).ToString(), charValues.startClassLevels[i]);
			}
		}
		if(charValues.faction == Faction.ENEMY) {
			for(int i = 0; i < ClassWheel.ENEMY_CLASS_COUNT; i++) {
				charValues.startClassLevels[i] = EditorGUILayout.IntField(((EnemyClassName)i).ToString(), charValues.startClassLevels[i]);
			}
		}
		if(charValues.faction == Faction.ALLY) {
			GUILayout.Label("Not implemented!");
		}

		GUILayout.Space(30);

		if(charValues.startClass != null && charValues.faction == Faction.PLAYER) {
			showStats = EditorGUILayout.Toggle("Show stats", showStats);
			if(showStats) {
				ShowBaseStats();
				ShowGrowths();
			}
			ShowSupports();
		}
	}

	private void ShowBaseStats() {
		CharData charValues = (CharData)entryValues;
		GUILayout.Label("Base stats", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("HP  " + charValues.startClass.hp);
		charValues.hp = EditorGUILayout.IntSlider(charValues.hp, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.hp + charValues.hp));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DMG  " + charValues.startClass.dmg);
		charValues.dmg = EditorGUILayout.IntSlider(charValues.dmg, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.dmg + charValues.dmg));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("MND  " + charValues.startClass.mnd);
		charValues.mnd = EditorGUILayout.IntSlider(charValues.mnd, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.mnd + charValues.mnd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SKL  " + charValues.startClass.skl);
		charValues.skl = EditorGUILayout.IntSlider(charValues.skl, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.skl + charValues.skl));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SPD  " + charValues.startClass.spd);
		charValues.spd = EditorGUILayout.IntSlider(charValues.spd, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.spd + charValues.spd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DEF  " + charValues.startClass.def);
		charValues.def = EditorGUILayout.IntSlider(charValues.def, -10, 10);
		GUILayout.Label("Tot: " + (charValues.startClass.def + charValues.def));
		GUILayout.EndHorizontal();
		GUILayout.Label("Total base diff:  " + (charValues.hp + charValues.dmg + charValues.mnd + charValues.skl + charValues.spd + charValues.def));
	}

	private void ShowGrowths() {
		CharData charValues = (CharData)entryValues;
		GUILayout.Label("Stat growths", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("HP  " + charValues.startClass.gHp);
		charValues.gHp = 5 * (EditorGUILayout.IntSlider(charValues.gHp, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gHp + charValues.gHp));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DMG  " + charValues.startClass.gDmg);
		charValues.gDmg = 5 * (EditorGUILayout.IntSlider(charValues.gDmg, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gDmg + charValues.gDmg));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("MND  " + charValues.startClass.gMnd);
		charValues.gMnd = 5 * (EditorGUILayout.IntSlider(charValues.gMnd, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gMnd + charValues.gMnd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SKL  " + charValues.startClass.gSkl);
		charValues.gSkl = 5 * (EditorGUILayout.IntSlider(charValues.gSkl, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gSkl + charValues.gSkl));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SPD  " + charValues.startClass.gSpd);
		charValues.gSpd = 5 * (EditorGUILayout.IntSlider(charValues.gSpd, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gSpd + charValues.gSpd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DEF  " + charValues.startClass.gDef);
		charValues.gDef = 5 * (EditorGUILayout.IntSlider(charValues.gDef, -30, 30) / 5);
		GUILayout.Label("Tot: " + (charValues.startClass.gDef + charValues.gDef));
		GUILayout.EndHorizontal();
		GUILayout.Label("Total growth diff:  " + (charValues.gHp + charValues.gDmg + charValues.gMnd + charValues.gSkl + charValues.gSpd + charValues.gDef));
	}

	private void ShowSupports() {
		CharData charValues = (CharData)entryValues;
		GUILayout.Label("Supports", EditorStyles.boldLabel);
		GUILayout.Space(5);
		for(int i = 0; i < charValues.supports.Count; i++) {
			GUILayout.Label("Support " + (i + 1));
			GUILayout.BeginHorizontal();
			charValues.supports[i].partner = (CharData)EditorGUILayout.ObjectField("Partner", charValues.supports[i].partner, typeof(CharData), false);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				charValues.supports.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();

			if(charValues.supports[i].partner != null) {
				GUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 70;
				charValues.supports[i].maxlevel = (SupportLetter)EditorGUILayout.EnumPopup("Max level", charValues.supports[i].maxlevel);
				charValues.supports[i].speed = (SupportSpeed)EditorGUILayout.EnumPopup("Level", charValues.supports[i].speed);
				EditorGUIUtility.labelWidth = 120;
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			charValues.supports.Add(new SupportTuple());
		}
	}

}
