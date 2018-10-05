using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PortraitEditorWindow {

	public ScrObjLibraryVariable portraitLibrary;
	public PortraitEntry portraitValues;
	public SpriteListVariable poseLibrary;
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
	public PortraitEditorWindow(ScrObjLibraryVariable entries, PortraitEntry container, SpriteListVariable poses){
		portraitLibrary = entries;
		portraitValues = container;
		poseLibrary = poses;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading character libraries...");

		portraitLibrary.GenerateDictionary();

		Debug.Log("Finished loading character libraries");

		InitializeWindow();
	}

	public void InitializeWindow() {
		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.8f, 0.5f, 0.2f));
		dispTex.Apply();

		dispOffset.right = 10;

		portraitValues.ResetValues();
		currentEntryList = portraitLibrary.GetRepresentations("","");
		filterStr = "";
	}


	public void DrawWindow() {

		GUILayout.BeginHorizontal();
		GUILayout.Label("Character Editor", EditorStyles.boldLabel);
		if (selEntry != -1) {
			if (GUILayout.Button("Save Character")){
				SaveSelectedEntry();
			}
		}
		GUILayout.EndHorizontal();

		GenerateAreas();
		DrawBackgrounds();
		DrawEntryList();
		if (selEntry != -1)
			DrawDisplayWindow();
	}

	void GenerateAreas() {

		selectRect.x = 0;
		selectRect.y = 50;
		selectRect.width = 200;
		selectRect.height = Screen.height - 50;

		dispRect.x = 200;
		dispRect.y = 50;
		dispRect.width = Screen.width - 200;
		dispRect.height = Screen.height - 50;
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
			currentEntryList = portraitLibrary.GetRepresentations("",filterStr);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(selectRect.width), 
						GUILayout.Height(selectRect.height-150));

		int oldSelected = selEntry;
		selEntry = GUILayout.SelectionGrid(selEntry, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selEntry)
			SelectEntry();

		EditorGUIUtility.labelWidth = 110;
		GUILayout.Label("Create new character", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Character Name", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		if (GUILayout.Button("Create new")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete character")) {
			DeleteEntry();
		}

		GUILayout.EndArea();
	}

	void DrawDisplayWindow() {
		GUILayout.BeginArea(dispRect);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect.width), 
							GUILayout.Height(dispRect.height-25));

		GUI.skin.textField.margin.right = 20;

		GUILayout.Label("Selected Character", EditorStyles.boldLabel);
		EditorGUILayout.SelectableLabel("UUID: " + portraitValues.uuid);
		portraitValues.repColor = EditorGUILayout.ColorField("List color", portraitValues.repColor);

		GUILayout.Space(20);

		portraitValues.entryName = EditorGUILayout.TextField("Name", portraitValues.entryName);

		if (portraitValues.poses.Length < poseLibrary.values.Length){
            System.Array.Resize(ref portraitValues.poses, poseLibrary.values.Length);
		}
		// Poses
		GUILayout.Label("Poses", EditorStyles.boldLabel);
		for (int i = 0; i < poseLibrary.values.Length; i++) {
			if (portraitValues.poses[i] == null)
				portraitValues.poses[i] = (Sprite)EditorGUILayout.ObjectField(poseLibrary.values[i].name, poseLibrary.values[i], typeof(Sprite),false);
			else
				portraitValues.poses[i] = (Sprite)EditorGUILayout.ObjectField(poseLibrary.values[i].name, portraitValues.poses[i], typeof(Sprite),false);
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void SelectEntry() {
		GUI.FocusControl(null);
		if (selEntry == -1) {
			// Nothing selected
			portraitValues.ResetValues();
		}
		else {
			// Something selected
			PortraitEntry ce = (PortraitEntry)portraitLibrary.GetEntryByIndex(selEntry);
			portraitValues.CopyValues(ce);
		}
	}

	void SaveSelectedEntry() {
		PortraitEntry ce = (PortraitEntry)portraitLibrary.GetEntryByIndex(selEntry);
		ce.CopyValues(portraitValues);
		Undo.RecordObject(ce, "Updated portrait");
		EditorUtility.SetDirty(ce);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (portraitLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		PortraitEntry c = Editor.CreateInstance<PortraitEntry>();
		c.name = uuid;
		c.uuid = uuid;
		c.entryName = uuid;
		c.repColor = repColor;
		string path = "Assets/LibraryData/Portraits/" + uuid + ".asset";

		AssetDatabase.CreateAsset(c, path);
		portraitLibrary.InsertEntry(c, 0);
		Undo.RecordObject(portraitLibrary, "Added portrait");
		EditorUtility.SetDirty(portraitLibrary);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = portraitLibrary.GetRepresentations("",filterStr);
		uuid = "";
		selEntry = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		PortraitEntry c = (PortraitEntry)portraitLibrary.GetEntryByIndex(selEntry);
		string path = "Assets/LibraryData/Portraits/" + c.uuid + ".asset";

		portraitLibrary.RemoveEntryByIndex(selEntry);
		Undo.RecordObject(portraitLibrary, "Deleted portrait");
		EditorUtility.SetDirty(portraitLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = portraitLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed portrait: " + c.uuid);
			selEntry = -1;
		}
	}
}
