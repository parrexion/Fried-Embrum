using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UpgradeEditorWindow {

	public ScrObjLibraryVariable entryLibrary;
	public UpgradeEntry entryValues;
	private GUIContent[] currentEntryList;

	// Selection screen
	Rect selectRect = new Rect();
	Texture2D selectTex;
	Vector2 scrollPos;
	int selEntry = -1;
	string filterStr = "";

	// Display screen
	Rect dispRect = new Rect();
	RectOffset dispOffset = new RectOffset();
	Texture2D dispTex;
	Vector2 dispScrollPos;

	//Creation
	string uuid = "";
	Color repColor = new Color(0,0,0,1f);


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public UpgradeEditorWindow(ScrObjLibraryVariable entries, UpgradeEntry container){
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading upgrade libraries...");

		entryLibrary.GenerateDictionary();

		Debug.Log("Finished loading upgrade libraries");

		InitializeWindow();
	}

	public void InitializeWindow() {
		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.3f, 0.8f, 0.4f));
		dispTex.Apply();

		dispOffset.right = 10;

		entryValues.ResetValues();
		currentEntryList = entryLibrary.GetRepresentations("","");
		filterStr = "";
	}


	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Upgrade Editor", EditorStyles.boldLabel);
		if (selEntry != -1) {
			if (GUILayout.Button("Save Upgrade")){
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
			currentEntryList = entryLibrary.GetRepresentations("",filterStr);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(selectRect.width), 
						GUILayout.Height(selectRect.height-130));

		int oldSelected = selEntry;
		selEntry = GUILayout.SelectionGrid(selEntry, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selEntry)
			SelectEntry();

		EditorGUIUtility.labelWidth = 110;
		GUILayout.Label("Create new entry", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Entry Name", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		if (GUILayout.Button("Create New")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete Entry")) {
			DeleteEntry();
		}

		GUILayout.EndArea();
	}

	void DrawDisplayWindow() {
		GUILayout.BeginArea(dispRect);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect.width), 
							GUILayout.Height(dispRect.height-25));

		GUI.skin.textField.margin.right = 20;

		GUILayout.Label("Selected Entry", EditorStyles.boldLabel);
		EditorGUILayout.SelectableLabel("UUID: " + entryValues.uuid);

		GUILayout.Space(10);

		entryValues.entryName = EditorGUILayout.TextField("Name", entryValues.entryName);
		entryValues.type = (UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type",entryValues.type);
		entryValues.item = (ItemEntry)EditorGUILayout.ObjectField("Related item", entryValues.item, typeof(ItemEntry), false);
		entryValues.cost = EditorGUILayout.IntField("Money Cost", entryValues.cost);
		entryValues.scrap = EditorGUILayout.IntField("Scrap Cost", entryValues.scrap);

		GUILayout.Space(10);

		if (entryValues.type == UpgradeType.UPGRADE) {
			GUILayout.Label("Improvements", EditorStyles.boldLabel);
			entryValues.level = EditorGUILayout.IntField("Level", entryValues.level);
			entryValues.hit = EditorGUILayout.IntField("Hit Rate", entryValues.hit);
			entryValues.power = EditorGUILayout.IntField("Power", entryValues.power);
			GUILayout.Label("Range modification");
			GUILayout.BeginHorizontal();
			entryValues.minRange = EditorGUILayout.IntField("Min", entryValues.minRange);
			entryValues.maxRange = EditorGUILayout.IntField("Max", entryValues.maxRange);
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void SelectEntry() {
		GUI.FocusControl(null);
		if (selEntry == -1) {
			// Nothing selected
			entryValues.ResetValues();
		}
		else {
			// Something selected
			UpgradeEntry ue = (UpgradeEntry)entryLibrary.GetEntryByIndex(selEntry);
			entryValues.CopyValues(ue);
		}
	}

	void SaveSelectedEntry() {
		UpgradeEntry ce = (UpgradeEntry)entryLibrary.GetEntryByIndex(selEntry);
		ce.CopyValues(entryValues);
		Undo.RecordObject(ce, "Updated entry");
		EditorUtility.SetDirty(ce);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (entryLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		UpgradeEntry c = Editor.CreateInstance<UpgradeEntry>();
		c.name = uuid;
		c.uuid = uuid;
		c.entryName = uuid;
		c.repColor = repColor;
		string path = "Assets/LibraryData/Upgrades/" + uuid + ".asset";

		AssetDatabase.CreateAsset(c, path);
		entryLibrary.InsertEntry(c, 0);
		Undo.RecordObject(entryLibrary, "Added entry");
		EditorUtility.SetDirty(entryLibrary);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = entryLibrary.GetRepresentations("",filterStr);
		uuid = "";
		selEntry = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		UpgradeEntry c = (UpgradeEntry)entryLibrary.GetEntryByIndex(selEntry);
		string path = "Assets/LibraryData/Upgrades/" + c.uuid + ".asset";

		entryLibrary.RemoveEntryByIndex(selEntry);
		Undo.RecordObject(entryLibrary, "Deleted entry");
		EditorUtility.SetDirty(entryLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = entryLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed entry: " + c.uuid);
			selEntry = -1;
		}
	}
}
