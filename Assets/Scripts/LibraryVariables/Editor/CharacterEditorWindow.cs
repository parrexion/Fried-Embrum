using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterEditorWindow {

	public ScrObjLibraryVariable characterLibrary;
	public CharData charValues;
	private GUIContent[] currentEntryList;

	// Selection screen
	Rect selectRect = new Rect();
	Texture2D selectTex;
	Vector2 scrollPos;
	int selEntry = -1;
	string filterStr = "";

	// Display screen
	Rect dispRect = new Rect();
	Texture2D dispTex;
	Vector2 dispScrollPos;
	Vector2 floatRangeRep;
	private bool showStats;

	//Creation
	string charUuid;
	Color repColor = new Color(0,0,0,1f);


	public CharacterEditorWindow(ScrObjLibraryVariable entries, CharData container){
		characterLibrary = entries;
		charValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading character libraries...");

		characterLibrary.GenerateDictionary();

		Debug.Log("Finished loading character libraries");

		InitializeWindow();
	}

	public void InitializeWindow() {
		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.1f, 0.4f, 0.6f));
		dispTex.Apply();

		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		charValues.ResetValues();
		currentEntryList = characterLibrary.GetRepresentations("","");
		filterStr = "";
	}


	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Character Editor", EditorStyles.boldLabel);
		if (selEntry != -1) {
			if (GUILayout.Button("Save Character")){
				SaveSelectedEntry();
			}
		}
		GUILayout.EndHorizontal();

		GenerateAreas(screenWidth, screenHeight);
		DrawBackgrounds();
		DrawEntryList();
		if (selEntry != -1)
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
			currentEntryList = characterLibrary.GetRepresentations("",filterStr);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(selectRect.width), 
						GUILayout.Height(selectRect.height-130));

		int oldSelected = selEntry;
		selEntry = GUILayout.SelectionGrid(selEntry, currentEntryList,1);
		EditorGUILayout.EndScrollView();
		
		if (oldSelected != selEntry)
			SelectEntry();

		EditorGUIUtility.labelWidth = 90;
		GUILayout.Label("Create new character", EditorStyles.boldLabel);
		charUuid = EditorGUILayout.TextField("Entry uuid", charUuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		if (GUILayout.Button("Create new")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete Character")) {
			DeleteEntry();
		}
		EditorGUIUtility.labelWidth = 0;

		GUILayout.EndArea();
	}

	void DrawDisplayWindow() {
		EditorGUIUtility.labelWidth = 110;
		GUILayout.BeginArea(dispRect);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect.width), 
					GUILayout.Height(dispRect.height-25));

		EditorGUILayout.SelectableLabel("Selected Character UUID: " + charValues.uuid, EditorStyles.boldLabel);
		charValues.repColor = EditorGUILayout.ColorField("List color", charValues.repColor);
		charValues.entryName = EditorGUILayout.TextField("Name", charValues.entryName);

		GUILayout.Space(10);

		GUILayout.Label("Visuals", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Trade portrait", GUILayout.Width(130));
		GUILayout.Label("Info portrait", GUILayout.Width(130));
		GUILayout.Label("Battle sprite", GUILayout.Width(130));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		charValues.bigPortrait = (Sprite)EditorGUILayout.ObjectField("",charValues.bigPortrait, typeof(Sprite),false, GUILayout.Width(130));
		charValues.portrait = (Sprite)EditorGUILayout.ObjectField("",charValues.portrait, typeof(Sprite),false, GUILayout.Width(130));
		charValues.battleSprite = (Sprite)EditorGUILayout.ObjectField("",charValues.battleSprite, typeof(Sprite),false, GUILayout.Width(130));
		GUILayout.EndHorizontal();
		charValues.portraitSet = (PortraitEntry)EditorGUILayout.ObjectField("Portrait set", charValues.portraitSet, typeof(PortraitEntry), false);
		
		GUILayout.Space(10);

		GUILayout.Label("Other values", EditorStyles.boldLabel);
		charValues.deathQuote = (DialogueEntry)EditorGUILayout.ObjectField("Death Quote", charValues.deathQuote, typeof(DialogueEntry),false);
		charValues.mustSurvive = EditorGUILayout.Toggle("Must survive", charValues.mustSurvive);

		GUILayout.Space(10);

		GUILayout.Label("Class", EditorStyles.boldLabel);
		charValues.charClass = (CharClass)EditorGUILayout.ObjectField("Class", charValues.charClass, typeof(CharClass),false);
		charValues.personalSkill = (CharacterSkill)EditorGUILayout.ObjectField("Personal skill", charValues.personalSkill, typeof(CharacterSkill),false);

		GUILayout.Space(30);

		if (charValues.charClass != null) {
			showStats = EditorGUILayout.Toggle("Show stats", showStats);
			if (showStats) {
				ShowBaseStats();
				ShowGrowths();
			}
			ShowSupports();
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void ShowBaseStats() {
		GUILayout.Label("Base stats", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("HP  " + charValues.charClass.hp);
		charValues.hp = EditorGUILayout.IntSlider(charValues.hp,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.hp + charValues.hp));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DMG  " + charValues.charClass.dmg);
		charValues.dmg = EditorGUILayout.IntSlider(charValues.dmg,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.dmg + charValues.dmg));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("MND  " + charValues.charClass.mnd);
		charValues.mnd = EditorGUILayout.IntSlider(charValues.mnd,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.mnd + charValues.mnd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SKL  " + charValues.charClass.skl);
		charValues.skl = EditorGUILayout.IntSlider(charValues.skl,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.skl + charValues.skl));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SPD  " + charValues.charClass.spd);
		charValues.spd = EditorGUILayout.IntSlider(charValues.spd,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.spd + charValues.spd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DEF  " + charValues.charClass.def);
		charValues.def = EditorGUILayout.IntSlider(charValues.def,-10,10);
		GUILayout.Label("Tot: " + (charValues.charClass.def + charValues.def));
		GUILayout.EndHorizontal();
	}
	
	private void ShowGrowths() {
		GUILayout.Label("Stat growths", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("HP  " + charValues.charClass.gHp);
		charValues.gHp = 5 * (EditorGUILayout.IntSlider(charValues.gHp,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gHp + charValues.gHp));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DMG  " + charValues.charClass.gDmg);
		charValues.gDmg = 5 * (EditorGUILayout.IntSlider(charValues.gDmg,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gDmg + charValues.gDmg));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("RES  " + charValues.charClass.gMnd);
		charValues.gMnd = 5 * (EditorGUILayout.IntSlider(charValues.gMnd,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gMnd + charValues.gMnd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SKL  " + charValues.charClass.gSkl);
		charValues.gSkl = 5 * (EditorGUILayout.IntSlider(charValues.gSkl,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gSkl + charValues.gSkl));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("SPD  " + charValues.charClass.gSpd);
		charValues.gSpd = 5 * (EditorGUILayout.IntSlider(charValues.gSpd,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gSpd + charValues.gSpd));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("DEF  " + charValues.charClass.gDef);
		charValues.gDef = 5 * (EditorGUILayout.IntSlider(charValues.gDef,-50,50) / 5);
		GUILayout.Label("Tot: " + (charValues.charClass.gDef + charValues.gDef));
		GUILayout.EndHorizontal();
	}
	
	private void ShowSupports() {
		GUILayout.Label("Supports", EditorStyles.boldLabel);
		GUILayout.Space(5);
		for (int i = 0; i < charValues.supports.Count; i++) {
			GUILayout.Label("Support " + (i+1));
			GUILayout.BeginHorizontal();
			charValues.supports[i].partner = (CharData)EditorGUILayout.ObjectField("Partner",charValues.supports[i].partner, typeof(CharData),false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				charValues.supports.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
			
			if (charValues.supports[i].partner != null) {
				GUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 70;
				charValues.supports[i].maxlevel = (SupportLetter)EditorGUILayout.EnumPopup("Max level", charValues.supports[i].maxlevel);
				charValues.supports[i].speed = (SupportSpeed)EditorGUILayout.EnumPopup("Level", charValues.supports[i].speed);
				EditorGUIUtility.labelWidth = 120;
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			charValues.supports.Add(new SupportTuple());
		}
	}

	void SelectEntry() {
		GUI.FocusControl(null);
		// Nothing selected
		if (selEntry == -1) {
			charValues.ResetValues();
		}
		else {
			// Something selected
			CharData cd = (CharData)characterLibrary.GetEntryByIndex(selEntry);
			charValues.CopyValues(cd);
		}
	}

	void SaveSelectedEntry() {
		CharData cd = (CharData)characterLibrary.GetEntryByIndex(selEntry);
		cd.CopyValues(charValues);
		Undo.RecordObject(cd, "Updated entry");
		EditorUtility.SetDirty(cd);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (characterLibrary.ContainsID(charUuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		CharData cd = Editor.CreateInstance<CharData>();
		cd.name = charUuid;
		cd.uuid = charUuid;
		cd.entryName = charUuid;
		cd.repColor = repColor;
		string path = "Assets/LibraryData/Characters/" + charUuid + ".asset";

		characterLibrary.InsertEntry(cd,0);
		Undo.RecordObject(characterLibrary, "Added entry");
		EditorUtility.SetDirty(characterLibrary);
		AssetDatabase.CreateAsset(cd, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = characterLibrary.GetRepresentations("",filterStr);
		charUuid = "";
		selEntry = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		CharData cd = (CharData)characterLibrary.GetEntryByIndex(selEntry);
		string path = "Assets/LibraryData/Characters/" + cd.uuid + ".asset";

		characterLibrary.RemoveEntryByIndex(selEntry);
		Undo.RecordObject(characterLibrary, "Deleted entry");
		EditorUtility.SetDirty(characterLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = characterLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed entry: " + cd.uuid);
			selEntry = -1;
		}
	}
}
