using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ClassEditorWindow {

	public ScrObjLibraryVariable classLibrary;
	public CharClass classValues;
	private GUIContent[] currentEntryList;

	// Display screen
	Rect dispRect = new Rect();
	Rect dispRect2 = new Rect();
	Texture2D dispTex;
	Vector2 dispScrollPos;

	// Selection screen
	Rect selectRect = new Rect();
	Texture2D selectTex;
	Vector2 selScrollPos;
	int selIndex = -1;
	string filterStr = "";

	//Creation
	string uuid;
	Color repColor = new Color(0, 0, 0, 1f);


	public ClassEditorWindow(ScrObjLibraryVariable entries, CharClass container){
		classLibrary = entries;
		classValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading class library...");

		classLibrary.GenerateDictionary();

		Debug.Log("Finished loading class library");

		InitializeWindow();
	}

	public void InitializeWindow() {
		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.3f, 0.6f, 0.4f));
		dispTex.Apply();

		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		classValues.ResetValues();
		currentEntryList = classLibrary.GetRepresentations("","");
		filterStr = "";
	}

	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Class Editor", EditorStyles.boldLabel);
		if (selIndex != -1) {
			if (GUILayout.Button("Save Class")){
				SaveSelectedEntry();
			}
		}
		GUILayout.EndHorizontal();

		GenerateAreas(screenWidth, screenHeight);
		DrawBackgrounds();
		DrawEntryList();
		if (selIndex != -1)
			DrawDisplayWindow();
	}

	void GenerateAreas(int screenWidth, int screenHeight) {
		selectRect.x = 0;
		selectRect.y = 50;
		selectRect.width = 200;
		selectRect.height = screenHeight - 50;

		dispRect.x = 200;
		dispRect.y = 50;
		dispRect.width = screenWidth - 200;
		dispRect.height = screenHeight - 50;

		dispRect2.x = 200;
		dispRect2.y = 50;
		dispRect2.width = screenWidth - 205;
		dispRect2.height = screenHeight - 50;
	}

	void DrawBackgrounds() {
		GUI.DrawTexture(selectRect, selectTex);
		GUI.DrawTexture(dispRect, dispTex);
	}

	void DrawEntryList() {
		GUILayout.BeginArea(selectRect);
		GUILayout.Space(5);
		EditorGUIUtility.labelWidth = 80;

		string oldFilter = filterStr;
		filterStr = EditorGUILayout.TextField("Filter", filterStr);
		if (filterStr != oldFilter)
			currentEntryList = classLibrary.GetRepresentations("",filterStr);

		selScrollPos = EditorGUILayout.BeginScrollView(selScrollPos, GUILayout.Width(selectRect.width), 
							GUILayout.Height(selectRect.height-130));

		int oldSelected = selIndex;
		selIndex = GUILayout.SelectionGrid(selIndex, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selIndex) {
			GUI.FocusControl(null);
			SelectEntry();
		}

		GUILayout.Label("Create new class", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Class uuid", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		if (GUILayout.Button("Create New")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete Entry")) {
			DeleteEntry();
		}
		EditorGUIUtility.labelWidth = 0;

		GUILayout.EndArea();
	}

	void DrawDisplayWindow() {
		EditorGUIUtility.labelWidth = 120;
		GUILayout.BeginArea(dispRect2);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect2.width), 
							GUILayout.Height(dispRect.height-45));

		EditorGUILayout.SelectableLabel("Selected Class:   " + classValues.uuid, EditorStyles.boldLabel);
		classValues.entryName = EditorGUILayout.TextField("Class Name", classValues.entryName);
		classValues.className = (PlayerClassName)EditorGUILayout.EnumPopup("Class Type Name", classValues.className);

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
		classValues.gHp = 5 * (EditorGUILayout.IntSlider("HP", classValues.gHp, 0, 100)/5);
		classValues.gDmg = 5 * (EditorGUILayout.IntSlider("DMG", classValues.gDmg, 0, 100)/5);
		classValues.gMnd = 5 * (EditorGUILayout.IntSlider("MND", classValues.gMnd, 0, 100)/5);
		classValues.gSkl = 5 * (EditorGUILayout.IntSlider("SKL", classValues.gSkl, 0, 100)/5);
		classValues.gSpd = 5 * (EditorGUILayout.IntSlider("SPD", classValues.gSpd, 0, 100)/5);
		classValues.gDef = 5 * (EditorGUILayout.IntSlider("DEF", classValues.gDef, 0, 100)/5);
		GUILayout.Label("Total growths:  " + (classValues.gHp + classValues.gDmg + classValues.gMnd + classValues.gSkl + classValues.gSpd + classValues.gDef));

		GUILayout.Space(10);

		GUILayout.Label("Usable weapons", EditorStyles.boldLabel);
		for (int i = 0; i < classValues.weaponSkills.Count; i++) {
			GUILayout.BeginHorizontal();
			classValues.weaponSkills[i] = (WeaponType)EditorGUILayout.EnumPopup("Weapon " + i, classValues.weaponSkills[i]);
			if (GUILayout.Button("X", GUILayout.Width(50))){
				classValues.weaponSkills.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
		}
		if (GUILayout.Button("+")) {
			classValues.weaponSkills.Add(WeaponType.NONE);
		}
		GUILayout.Space(10);

		GUILayout.Label("Skill Gains", EditorStyles.boldLabel);
		for (int i = 0; i < classValues.skills.Count; i++) {
			GUILayout.BeginHorizontal();
			classValues.skills[i] = (CharacterSkill)EditorGUILayout.ObjectField("Skill",classValues.skills[i], typeof(CharacterSkill),false);
			if (GUILayout.Button("X", GUILayout.Width(50))){
				classValues.skills.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			classValues.skills.Add(null);
		}

		GUILayout.Label("Promotion gains", EditorStyles.boldLabel);
		classValues.bonusHp = EditorGUILayout.IntField("HP", classValues.bonusHp);
		classValues.bonusDmg = EditorGUILayout.IntField("ATK", classValues.bonusDmg);
		classValues.bonusMnd = EditorGUILayout.IntField("MND", classValues.bonusMnd);
		classValues.bonusSkl = EditorGUILayout.IntField("SKL", classValues.bonusSkl);
		classValues.bonusSpd = EditorGUILayout.IntField("SPD", classValues.bonusSpd);
		classValues.bonusDef = EditorGUILayout.IntField("DEF", classValues.bonusDef);

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void SelectEntry() {
		// Nothing selected
		if (selIndex == -1) {
			classValues.ResetValues();
		}
		else {
			// Something selected
			CharClass cc = (CharClass)classLibrary.GetEntryByIndex(selIndex);
			classValues.CopyValues(cc);
		}
	}

	void SaveSelectedEntry() {
		CharClass cc = (CharClass)classLibrary.GetEntryByIndex(selIndex);
		cc.CopyValues(classValues);
		Undo.RecordObject(cc, "Updated entry");
		EditorUtility.SetDirty(cc);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (classLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		CharClass cc = Editor.CreateInstance<CharClass>();
		cc.name = uuid;
		cc.uuid = uuid;
		cc.repColor = repColor;
		cc.entryName = uuid;
		string path = "Assets/LibraryData/Classes/" + uuid + ".asset";

		classLibrary.InsertEntry(cc,0);
		Undo.RecordObject(classLibrary, "Added entry");
		EditorUtility.SetDirty(classLibrary);
		AssetDatabase.CreateAsset(cc, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = classLibrary.GetRepresentations("",filterStr);
		uuid = "";
		selIndex = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		CharClass cc = (CharClass)classLibrary.GetEntryByIndex(selIndex);
		string path = "Assets/LibraryData/Classes/" + cc.uuid + ".asset";

		classLibrary.RemoveEntryByIndex(selIndex);
		Undo.RecordObject(classLibrary, "Deleted entry");
		EditorUtility.SetDirty(classLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = classLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed entry: " + cc.uuid);
			selIndex = -1;
		}
	}

}
