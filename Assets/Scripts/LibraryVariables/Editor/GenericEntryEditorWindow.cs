using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GenericEntryEditorWindow {

	abstract protected string NameString { get; }
	abstract protected ScrObjLibraryEntry CreateInstance { get; }
	abstract protected Color BackgroundColor { get; }
	virtual protected int CreateEntrySpace => 130;

	public ScrObjLibraryVariable entryLibrary;
	public ScrObjLibraryEntry entryValues;
	protected GUIContent[] currentEntryList;

	// Display screen
	protected Rect dispRect = new Rect();
	protected Rect dispRect2 = new Rect();
	protected Texture2D dispTex;
	protected Vector2 dispScrollPos;

	// Selection screen
	protected Rect selectRect = new Rect();
	protected Texture2D selectTex;
	protected Vector2 selScrollPos;
	protected int selIndex = -1;
	protected string filterStr = "";

	//Creation
	protected string uuid;
	protected Color repColor = new Color(0, 0, 0, 1f);
	private bool renameMode;
	private string renameString;
	private Color oldRepColor;


	protected void LoadLibrary() {
		//Debug.Log("Loading library...");
		entryLibrary.GenerateDictionary();
		//Debug.Log("Finished loading library");

		InitializeWindow();
	}

	public void InitializeWindow() {
		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, BackgroundColor);
		dispTex.Apply();

		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		entryValues.ResetValues();
		currentEntryList = entryLibrary.GetRepresentations("", "");
		filterStr = "";
	}

	protected void GenerateAreas(int screenWidth, int screenHeight) {
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
		dispRect2.width = screenWidth - 200;
		dispRect2.height = screenHeight - 50;
	}

	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label(NameString + " Editor", EditorStyles.boldLabel);
		if (selIndex != -1) {
			if (GUILayout.Button("Save " + NameString)) {
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

	protected void DrawBackgrounds() {
		if (selectTex == null || dispTex == null)
			InitializeWindow();
		GUI.DrawTexture(selectRect, selectTex);
		GUI.DrawTexture(dispRect, dispTex);
	}

	protected void DrawEntryList() {
		GUILayout.BeginArea(selectRect);
		GUILayout.Space(5);
		EditorGUIUtility.labelWidth = 80;

		string oldFilter = filterStr;
		filterStr = EditorGUILayout.TextField("Filter", filterStr);
		if (filterStr != oldFilter)
			currentEntryList = entryLibrary.GetRepresentations("", filterStr);

		selScrollPos = EditorGUILayout.BeginScrollView(selScrollPos, GUILayout.Width(selectRect.width),
							GUILayout.Height(selectRect.height - CreateEntrySpace));

		int oldSelected = selIndex;
		selIndex = GUILayout.SelectionGrid(selIndex, currentEntryList, 1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selIndex) {
			GUI.FocusControl(null);
			SelectEntry();
		}

		EditorGUIUtility.labelWidth = 110;
		GUILayout.Label("Create new entry", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Entry uuid", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		ExtraEntrySettings();

		if (GUILayout.Button("Create new")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete entry")) {
			DeleteEntry();
		}
		EditorGUIUtility.labelWidth = 0;

		GUILayout.EndArea();
	}

	protected virtual void ExtraEntrySettings() { }

	protected void DrawDisplayWindow() {
		EditorGUIUtility.labelWidth = 120;
		GUILayout.BeginArea(dispRect2);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect2.width),
							GUILayout.Height(dispRect.height - 10));

		GUILayout.BeginHorizontal();
		if (renameMode) {
			renameString = EditorGUILayout.TextField("New uuid", renameString);
			if (GUILayout.Button("Save", GUILayout.Width(60))) {
				if (renameString == entryValues.uuid) {
					renameMode = false;
				}
				else if (IsValidUuid(renameString)) {
					RenameEntry(renameString);
					renameMode = false;
				}
			}
			if (GUILayout.Button("Cancel", GUILayout.Width(60))) {
				renameMode = false;
			}
		}
		else {
			EditorGUILayout.SelectableLabel("Selected " + NameString + ":   " + entryValues.uuid, EditorStyles.boldLabel);
			if (GUILayout.Button("Change uuid", GUILayout.Width(100))) {
				renameString = entryValues.uuid;
				renameMode = true;
			}
		}
		GUILayout.EndHorizontal();
		entryValues.entryName = EditorGUILayout.TextField(NameString + " Name", entryValues.entryName);
		entryValues.repColor = EditorGUILayout.ColorField("List color", entryValues.repColor);
		GUILayout.Space(10);

		//This is where the magic happens ;)
		DrawContentWindow();

		GUILayout.EndScrollView();
		GUILayout.EndArea();

	}

	abstract protected void DrawContentWindow();

	protected void RefreshEntryList() {
		entryLibrary.initialized = false;
		currentEntryList = entryLibrary.GetRepresentations("", filterStr);
	}

	protected void SelectEntry() {
		// Nothing selected
		if (selIndex == -1) {
			entryValues.ResetValues();
		}
		else {
			// Something selected
			ScrObjLibraryEntry entry = entryLibrary.GetEntryByIndex(selIndex);
			entryValues.CopyValues(entry);
			oldRepColor = entryValues.repColor;
		}
	}

	protected void SaveSelectedEntry() {
		ScrObjLibraryEntry entry = entryLibrary.GetEntryByIndex(selIndex);
		entry.CopyValues(entryValues);
		Undo.RecordObject(entry, "Updated entry");
		EditorUtility.SetDirty(entry);
		if (oldRepColor != entryValues.repColor) {
			oldRepColor = entryValues.repColor;
			RefreshEntryList();
		}
	}

	protected bool IsValidUuid(string uuid) {
		if (string.IsNullOrEmpty(uuid)) {
			Debug.LogError("uuid is empty!");
			EditorUtility.DisplayDialog("Error", "uuid can not be empty!", "OK");
			return false;
		}
		if (entryLibrary.ContainsID(uuid)) {
			Debug.LogError("uuid already exists!");
			EditorUtility.DisplayDialog("Error", "That uuid already exists!", "OK");
			return false;
		}
		return true;
	}

	protected void InstansiateEntry() {
		GUI.FocusControl(null);
		if (!IsValidUuid(uuid)) {
			return;
		}
		ScrObjLibraryEntry entry = CreateInstance;
		entry.name = uuid;
		entry.uuid = uuid;
		entry.repColor = repColor;
		entry.entryName = uuid;
		string path = "Assets/LibraryData/" + NameString + "/" + NameString + "_" + uuid + ".asset";

		entryLibrary.InsertEntry(entry, 0);
		Undo.RecordObject(entryLibrary, "Added entry");
		EditorUtility.SetDirty(entryLibrary);
		AssetDatabase.CreateAsset(entry, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		uuid = "";
		selIndex = 0;
		SelectEntry();
		RefreshEntryList();
	}

	protected void RenameEntry(string newName) {
		ScrObjLibraryEntry entry = entryLibrary.GetEntryByIndex(selIndex);
		string path = "Assets/LibraryData/" + NameString + "/" + NameString + "_" + entry.uuid + ".asset";
		string newPath = NameString + "_" + newName + ".asset";

		entryValues.uuid = newName;
		SaveSelectedEntry();
		AssetDatabase.Refresh();

		string res = AssetDatabase.RenameAsset(path, newPath);
		if (!string.IsNullOrEmpty(res))
			Debug.LogError("Res:  " + res);
		SelectEntry();
		RefreshEntryList();
	}

	protected void DeleteEntry() {
		GUI.FocusControl(null);
		ScrObjLibraryEntry entry = entryLibrary.GetEntryByIndex(selIndex);
		string path = "Assets/LibraryData/" + NameString + "/" + NameString + "_" + entry.uuid + ".asset";

		entryLibrary.RemoveEntryByIndex(selIndex);
		Undo.RecordObject(entryLibrary, "Deleted entry");
		EditorUtility.SetDirty(entryLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		if (res) {
			Debug.Log("Removed entry: " + entry.uuid);
			selIndex = -1;
		}
		else {
			Debug.LogError("Failed to remove entry: " + path);
		}
		RefreshEntryList();
	}
}
