using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ClassEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Class";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<CharClass>();
	protected override Color BackgroundColor => new Color(0.3f, 0.6f, 0.4f);


	public ClassEditorWindow(ScrObjLibraryVariable entries, CharClass container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		CharClass classValues = (CharClass)entryValues;

		GUILayout.Label("Movement", EditorStyles.boldLabel);
		classValues.classType = (MovementType)EditorGUILayout.EnumPopup("Class type", classValues.classType);
		classValues.movespeed = EditorGUILayout.IntField("Move speed", classValues.movespeed);

		GUILayout.Space(10);

		GUILayout.Label("Base stats", EditorStyles.boldLabel);
		classValues.hp = EditorGUILayout.IntField("HP", classValues.hp);
		classValues.dmg = EditorGUILayout.IntField("ATK", classValues.dmg);
		classValues.mnd = EditorGUILayout.IntField("MND", classValues.mnd);
		classValues.skl = EditorGUILayout.IntField("SKL", classValues.skl);
		classValues.spd = EditorGUILayout.IntField("SPD", classValues.spd);
		classValues.def = EditorGUILayout.IntField("DEF", classValues.def);
		GUILayout.Label("Total bases:  " + (classValues.hp + classValues.dmg + classValues.mnd + classValues.skl + classValues.spd + classValues.def));

		GUILayout.Label("Growth rates", EditorStyles.boldLabel);
		classValues.gHp = 5 * (EditorGUILayout.IntSlider("HP", classValues.gHp, 0, 100) / 5);
		classValues.gDmg = 5 * (EditorGUILayout.IntSlider("DMG", classValues.gDmg, 0, 100) / 5);
		classValues.gMnd = 5 * (EditorGUILayout.IntSlider("MND", classValues.gMnd, 0, 100) / 5);
		classValues.gSkl = 5 * (EditorGUILayout.IntSlider("SKL", classValues.gSkl, 0, 100) / 5);
		classValues.gSpd = 5 * (EditorGUILayout.IntSlider("SPD", classValues.gSpd, 0, 100) / 5);
		classValues.gDef = 5 * (EditorGUILayout.IntSlider("DEF", classValues.gDef, 0, 100) / 5);
		GUILayout.Label("Total growths:  " + (classValues.gHp + classValues.gDmg + classValues.gMnd + classValues.gSkl + classValues.gSpd + classValues.gDef));

		GUILayout.Space(10);

		GUILayout.Label("Usable weapons", EditorStyles.boldLabel);
		for(int i = 0; i < classValues.weaponSkills.Count; i++) {
			GUILayout.BeginHorizontal();
			classValues.weaponSkills[i] = (WeaponType)EditorGUILayout.EnumPopup("Weapon " + i, classValues.weaponSkills[i]);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				classValues.weaponSkills.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
		}
		if(GUILayout.Button("+")) {
			classValues.weaponSkills.Add(WeaponType.NONE);
		}
		GUILayout.Space(10);

		GUILayout.Label("Skill Gains", EditorStyles.boldLabel);
		classValues.lockTouch = EditorGUILayout.Toggle("Locktouch", classValues.lockTouch);
		for(int i = 0; i < classValues.skills.Count; i++) {
			GUILayout.BeginHorizontal();
			classValues.skills[i] = (CharacterSkill)EditorGUILayout.ObjectField("Skill", classValues.skills[i], typeof(CharacterSkill), false);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				classValues.skills.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			classValues.skills.Add(null);
		}

		GUILayout.Label("Promotion gains", EditorStyles.boldLabel);
		classValues.bonusHp = EditorGUILayout.IntField("HP", classValues.bonusHp);
		classValues.bonusDmg = EditorGUILayout.IntField("ATK", classValues.bonusDmg);
		classValues.bonusMnd = EditorGUILayout.IntField("MND", classValues.bonusMnd);
		classValues.bonusSkl = EditorGUILayout.IntField("SKL", classValues.bonusSkl);
		classValues.bonusSpd = EditorGUILayout.IntField("SPD", classValues.bonusSpd);
		classValues.bonusDef = EditorGUILayout.IntField("DEF", classValues.bonusDef);
	}

}
