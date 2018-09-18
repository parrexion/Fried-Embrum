using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditorWindow {

	public ScrObjLibraryVariable mapLibrary;
	public MapEntry mapValues;
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
	string mapUuid;
	int selIndex = -1;
	string filterStr = "";
	
	private bool showPlayerStuff = false;
	private bool showEnemyStuff = false;
	private bool showInteractStuff = false;


	public MapEditorWindow(ScrObjLibraryVariable entries, MapEntry container){
		mapLibrary = entries;
		mapValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading map library...");

		mapLibrary.GenerateDictionary();

		Debug.Log("Finished loading map library");

		InitializeWindow();
	}

	public void InitializeWindow() {
		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.3f, 0.6f, 0.4f));
		dispTex.Apply();

		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		mapValues.ResetValues();
		currentEntryList = mapLibrary.GetRepresentations("","");
		filterStr = "";
	}

	public void DrawWindow() {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Map Editor", EditorStyles.boldLabel);
		if (selIndex != -1) {
			if (GUILayout.Button("Save Map")){
				SaveSelectedEntry();
			}
		}
		GUILayout.EndHorizontal();

		GenerateAreas();
		DrawBackgrounds();
		DrawEntryList();
		if (selIndex != -1)
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

		dispRect2.x = 200;
		dispRect2.y = 50;
		dispRect2.width = Screen.width - 200;
		dispRect2.height = Screen.height - 50;
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
			currentEntryList = mapLibrary.GetRepresentations("",filterStr);

		selScrollPos = EditorGUILayout.BeginScrollView(selScrollPos, GUILayout.Width(selectRect.width), 
							GUILayout.Height(selectRect.height-110));

		int oldSelected = selIndex;
		selIndex = GUILayout.SelectionGrid(selIndex, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selIndex) {
			GUI.FocusControl(null);
			SelectEntry();
		}

		mapUuid = EditorGUILayout.TextField("Map uuid", mapUuid);
		if (GUILayout.Button("Create new")) {
			InstansiateEntry();
		}
		if (GUILayout.Button("Delete Map")) {
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

		EditorGUILayout.SelectableLabel("Selected Map:   " + mapValues.uuid, EditorStyles.boldLabel);
		mapValues.entryName = EditorGUILayout.TextField("Map Name", mapValues.entryName);

		GUILayout.Space(10);
		GUILayout.Label("Win/Lose", EditorStyles.boldLabel);
		mapValues.winCondition = (WinCondition)EditorGUILayout.EnumPopup("",mapValues.winCondition);
		mapValues.loseCondition = (LoseCondition)EditorGUILayout.EnumPopup("",mapValues.loseCondition);

		GUILayout.Space(10);

		GUILayout.Label("Map Size", EditorStyles.boldLabel);
		mapValues.mapSprite = (Texture2D)EditorGUILayout.ObjectField("Map Info Sprite",mapValues.mapSprite, typeof(Texture2D),false);
		GUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 80;
		mapValues.sizeX = EditorGUILayout.IntField("Size X", mapValues.sizeX);
		mapValues.sizeY = EditorGUILayout.IntField("Size Y", mapValues.sizeY);
		EditorGUIUtility.labelWidth = 120;
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("Dialogues", EditorStyles.boldLabel);
		mapValues.preDialogue = (DialogueEntry)EditorGUILayout.ObjectField("Before battle",mapValues.preDialogue, typeof(DialogueEntry),false);
		mapValues.postDialogue = (DialogueEntry)EditorGUILayout.ObjectField("After battle",mapValues.postDialogue, typeof(DialogueEntry),false);

		GUILayout.Space(10);

		GUILayout.Label("Music", EditorStyles.boldLabel);
		mapValues.owMusic = (MusicEntry)EditorGUILayout.ObjectField("OW music",mapValues.owMusic, typeof(MusicEntry),false);
		mapValues.battleMusic = (MusicEntry)EditorGUILayout.ObjectField("Battle music",mapValues.battleMusic, typeof(MusicEntry),false);
		mapValues.healMusic = (MusicEntry)EditorGUILayout.ObjectField("Heal music",mapValues.healMusic, typeof(MusicEntry),false);

		GUILayout.Space(10);

		// Player stuff
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player spawn points", EditorStyles.boldLabel);
		if (GUILayout.Button((showPlayerStuff) ? "Hide" : "Show")) {
			showPlayerStuff = !showPlayerStuff;
		}
		GUILayout.EndHorizontal();
		if (showPlayerStuff)
			DrawPlayerStuff();

		GUILayout.Space(10);

		// Enemies 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Enemy spawn points", EditorStyles.boldLabel);
		if (GUILayout.Button((showEnemyStuff) ? "Hide" : "Show")) {
			showEnemyStuff = !showEnemyStuff;
		}
		GUILayout.EndHorizontal();
		if (showEnemyStuff)
			DrawEnemyStuff();

		GUILayout.Space(10);

		// Interactions 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Interaction points", EditorStyles.boldLabel);
		if (GUILayout.Button((showInteractStuff) ? "Hide" : "Show")) {
			showInteractStuff = !showInteractStuff;
		}
		GUILayout.EndHorizontal();
		if (showInteractStuff)
			DrawInteractStuff();

		GUILayout.Space(10);

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DrawPlayerStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.spawnPoints.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.spawnPoints[i].x = EditorGUILayout.IntField("X", mapValues.spawnPoints[i].x);
			mapValues.spawnPoints[i].y = EditorGUILayout.IntField("Y", mapValues.spawnPoints[i].y);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.spawnPoints.RemoveAt(i);
				i--;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();
			mapValues.spawnPoints[i].stats = (CharData)EditorGUILayout.ObjectField("New character",mapValues.spawnPoints[i].stats, typeof(CharData),false);
			if (mapValues.spawnPoints[i].stats != null) {
				mapValues.spawnPoints[i].level = EditorGUILayout.IntField("Level", mapValues.spawnPoints[i].level);

				EditorGUIUtility.labelWidth = 70;
				for (int j = 0; j < mapValues.spawnPoints[i].inventory.Count; j++) {
					GUILayout.BeginHorizontal();
					mapValues.spawnPoints[i].inventory[j].item = (WeaponItem)EditorGUILayout.ObjectField("Item",mapValues.spawnPoints[i].inventory[j].item, typeof(WeaponItem),false);
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						mapValues.spawnPoints[i].inventory.RemoveAt(j);
						i--;
					}
					GUILayout.EndHorizontal();
				}
				EditorGUIUtility.labelWidth = 120;
				GUILayout.BeginHorizontal();
				GUILayout.Space(120);
				if (GUILayout.Button("Add Item")) {
					mapValues.spawnPoints[i].inventory.Add(new WeaponTuple());
				}
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.spawnPoints.Add(new PlayerPosition());
		}
	}

	private void DrawEnemyStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.enemies.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.enemies[i].x = EditorGUILayout.IntField("X", mapValues.enemies[i].x);
			mapValues.enemies[i].y = EditorGUILayout.IntField("Y", mapValues.enemies[i].y);
			//  = (ClassType)EditorGUILayout.EnumPopup("",itemValues.advantageType[i]);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.enemies.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			mapValues.enemies[i].stats = (CharData)EditorGUILayout.ObjectField("Character",mapValues.enemies[i].stats, typeof(CharData),false);
			mapValues.enemies[i].hasQuotes = EditorGUILayout.Toggle("Quotes", mapValues.enemies[i].hasQuotes, GUILayout.Width(80));
			GUILayout.EndHorizontal();
			
			if (mapValues.enemies[i].stats != null) {
				GUILayout.BeginHorizontal();
				mapValues.enemies[i].level = EditorGUILayout.IntField("Level", mapValues.enemies[i].level);
				mapValues.enemies[i].aggroType = (AggroType)EditorGUILayout.EnumPopup("Aggro", mapValues.enemies[i].aggroType);
				EditorGUIUtility.labelWidth = 120;
				GUILayout.EndHorizontal();
			
				// Inventory
				for (int j = 0; j < mapValues.enemies[i].inventory.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					mapValues.enemies[i].inventory[j].item = (WeaponItem)EditorGUILayout.ObjectField("Item",mapValues.enemies[i].inventory[j].item, typeof(WeaponItem),false);
					EditorGUIUtility.labelWidth = 35;
					mapValues.enemies[i].inventory[j].droppable = EditorGUILayout.Toggle("Drop", mapValues.enemies[i].inventory[j].droppable, GUILayout.Width(50));
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						mapValues.enemies[i].inventory.RemoveAt(j);
						j--;
						continue;
					}
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 120;	
				}
				GUILayout.BeginHorizontal();
				GUILayout.Space(120);
				if (GUILayout.Button("Add Item")) {
					mapValues.enemies[i].inventory.Add(new WeaponTuple());
				}
				GUILayout.EndHorizontal();
			}

			if (mapValues.enemies[i].hasQuotes) {
				// Quotes
				for (int j = 0; j < mapValues.enemies[i].quotes.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					mapValues.enemies[i].quotes[j].triggerer = (CharData)EditorGUILayout.ObjectField("Caused by",mapValues.enemies[i].quotes[j].triggerer, typeof(CharData),false);
					mapValues.enemies[i].quotes[j].quote = (DialogueEntry)EditorGUILayout.ObjectField("Quote",mapValues.enemies[i].quotes[j].quote, typeof(DialogueEntry),false);
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						mapValues.enemies[i].quotes.RemoveAt(j);
						j--;
						continue;
					}
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 120;	
				}
				GUILayout.BeginHorizontal();
				GUILayout.Space(120);
				if (GUILayout.Button("Add Quote")) {
					mapValues.enemies[i].quotes.Add(new FightQuote());
				}
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.enemies.Add(new EnemyPosition());
		}
	}

	private void DrawInteractStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.interactions.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.interactions[i].x = EditorGUILayout.IntField("X", mapValues.interactions[i].x);
			mapValues.interactions[i].y = EditorGUILayout.IntField("Y", mapValues.interactions[i].y);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.interactions.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();

			mapValues.interactions[i].interactType = (InteractType)EditorGUILayout.EnumPopup("Type", mapValues.interactions[i].interactType);

			switch (mapValues.interactions[i].interactType)
			{
				case InteractType.BLOCK:
					mapValues.interactions[i].health = EditorGUILayout.IntField("Health", mapValues.interactions[i].health);
					break;
				case InteractType.VILLAGE:
					mapValues.interactions[i].dialogue = (DialogueEntry)EditorGUILayout.ObjectField("Dialogue",mapValues.interactions[i].dialogue, typeof(DialogueEntry),false);
					mapValues.interactions[i].gift = (WeaponItem)EditorGUILayout.ObjectField("Gift",mapValues.interactions[i].gift, typeof(WeaponItem),false);
					break;
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.interactions.Add(new InteractPosition());
		}
	}
	
	void SelectEntry() {
		// Nothing selected
		if (selIndex == -1) {
			mapValues.ResetValues();
		}
		else {
			// Something selected
			MapEntry me = (MapEntry)mapLibrary.GetEntryByIndex(selIndex);
			mapValues.CopyValues(me);
		}
	}

	void SaveSelectedEntry() {
		MapEntry me = (MapEntry)mapLibrary.GetEntryByIndex(selIndex);
		me.CopyValues(mapValues);
		Undo.RecordObject(me, "Updated entry");
		EditorUtility.SetDirty(me);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (mapLibrary.ContainsID(mapUuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		MapEntry me = Editor.CreateInstance<MapEntry>();
		me.name = mapUuid;
		me.uuid = mapUuid;
		me.entryName = mapUuid;
		string path = "Assets/LibraryData/Maps/" + mapUuid + ".asset";

		mapLibrary.InsertEntry(me,0);
		Undo.RecordObject(mapLibrary, "Added entry");
		EditorUtility.SetDirty(mapLibrary);
		AssetDatabase.CreateAsset(me, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = mapLibrary.GetRepresentations("",filterStr);
		mapUuid = "";
		selIndex = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		MapEntry me = (MapEntry)mapLibrary.GetEntryByIndex(selIndex);
		string path = "Assets/LibraryData/Maps/" + me.uuid + ".asset";

		mapLibrary.RemoveEntryByIndex(selIndex);
		Undo.RecordObject(mapLibrary, "Deleted entry");
		EditorUtility.SetDirty(mapLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = mapLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed entry: " + me.uuid);
			selIndex = -1;
		}
	}

}
