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
	int selIndex = -1;
	string filterStr = "";

	//Creation
	string uuid;
	Color repColor = new Color(0, 0, 0, 1f);

	private bool showPlayerStuff = false;
	private bool showPlayerSpecialStuff = false;
	private bool showEnemyStuff = false;
	private bool showInteractStuff = false;
	private bool showTurnEventStuff = false;
	private bool showReinforcementStuff = false;


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

	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Map Editor", EditorStyles.boldLabel);
		if (selIndex != -1) {
			if (GUILayout.Button("Save Map")){
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
		dispRect2.width = screenWidth - 200;
		dispRect2.height = screenHeight - 50;
	}

	void DrawBackgrounds() {
		if (selectTex == null || dispTex == null)
			InitializeWindow();
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
							GUILayout.Height(selectRect.height-130));

		int oldSelected = selIndex;
		selIndex = GUILayout.SelectionGrid(selIndex, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selIndex) {
			GUI.FocusControl(null);
			SelectEntry();
		}

		EditorGUIUtility.labelWidth = 110;
		GUILayout.Label("Create new map", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Map uuid", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
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
		mapValues.mapLocation = (MapLocation)EditorGUILayout.EnumPopup("Location",mapValues.mapLocation);
		GUILayout.Label("Mission description");
		EditorStyles.textField.wordWrap = true;
		mapValues.mapDescription = EditorGUILayout.TextArea(mapValues.mapDescription, GUILayout.Width(500), GUILayout.Height(30));
		EditorStyles.textField.wordWrap = false;

		GUILayout.Space(10);
		GUILayout.Label("Map objectives", EditorStyles.boldLabel);
		EditorGUIUtility.labelWidth = 80;
		GUILayout.BeginHorizontal();
		mapValues.winCondition = (WinCondition)EditorGUILayout.EnumPopup("Win",mapValues.winCondition);
		GUILayout.Space(20);
		mapValues.loseCondition = (LoseCondition)EditorGUILayout.EnumPopup("Lose",mapValues.loseCondition);
		GUILayout.EndHorizontal();

		GUILayout.Label("Rewards", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		mapValues.reward.money = EditorGUILayout.IntField("Money", mapValues.reward.money);
		mapValues.reward.scrap = EditorGUILayout.IntField("Scrap", mapValues.reward.scrap);
		GUILayout.EndHorizontal();
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.reward.items.Count; i++) {
			GUILayout.BeginHorizontal();
			mapValues.reward.items[i] = (ItemEntry)EditorGUILayout.ObjectField("Item", mapValues.reward.items[i], typeof(ItemEntry), false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.reward.items.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
		}
		if (GUILayout.Button("+")) {
			mapValues.reward.items.Add(null);
		}

		GUILayout.Space(10);

		GUILayout.Label("Map Size", EditorStyles.boldLabel);
		EditorGUIUtility.labelWidth = 120;
		mapValues.mapSprite = (Texture2D)EditorGUILayout.ObjectField("Map Info Sprite",mapValues.mapSprite, typeof(Texture2D),false);
		GUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 80;
		mapValues.sizeX = EditorGUILayout.IntField("Size X", mapValues.sizeX);
		mapValues.sizeY = EditorGUILayout.IntField("Size Y", mapValues.sizeY);
		EditorGUIUtility.labelWidth = 120;
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("Chapter Linking", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		mapValues.mapDuration = EditorGUILayout.IntField("Map Duration", mapValues.mapDuration);
		mapValues.unlockDay = EditorGUILayout.IntField("Unlocked on day", mapValues.unlockDay);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		mapValues.skipBattlePrep = EditorGUILayout.Toggle("Skip battle preps?", mapValues.skipBattlePrep);
		mapValues.autoNextChapter = (MapEntry)EditorGUILayout.ObjectField("Auto Next Chapter", mapValues.autoNextChapter, typeof(MapEntry), false);
		GUILayout.EndHorizontal();

		GUILayout.Label("Dialogues", EditorStyles.boldLabel);
		mapValues.preDialogue = (DialogueEntry)EditorGUILayout.ObjectField("Mission dialogue",mapValues.preDialogue, typeof(DialogueEntry),false);
		mapValues.introDialogue = (DialogueEntry)EditorGUILayout.ObjectField("On mission start",mapValues.introDialogue, typeof(DialogueEntry),false);
		mapValues.endDialogue = (DialogueEntry)EditorGUILayout.ObjectField("After win",mapValues.endDialogue, typeof(DialogueEntry),false);

		GUILayout.Space(10);

		GUILayout.Label("Music", EditorStyles.boldLabel);
		mapValues.playerMusic = (MusicEntry)EditorGUILayout.ObjectField("Player music",mapValues.playerMusic, typeof(MusicEntry),false);
		mapValues.enemyMusic = (MusicEntry)EditorGUILayout.ObjectField("Enemy music",mapValues.enemyMusic, typeof(MusicEntry),false);
		mapValues.battleMusic = (MusicEntry)EditorGUILayout.ObjectField("Battle music",mapValues.battleMusic, typeof(MusicEntry),false);
		mapValues.healMusic = (MusicEntry)EditorGUILayout.ObjectField("Heal music",mapValues.healMusic, typeof(MusicEntry),false);
		
		GUILayout.Space(10);

		// Player stuff
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player spawn points", EditorStyles.boldLabel);
		if (GUILayout.Button((showPlayerStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showPlayerStuff = !showPlayerStuff;
		}
		GUILayout.EndHorizontal();
		if (showPlayerStuff)
			DrawPlayerStuff();

		GUILayout.Space(10);

		// Player selection stuff
		GUILayout.BeginHorizontal();
		GUILayout.Label("Character limitations", EditorStyles.boldLabel);
		if (GUILayout.Button((showPlayerSpecialStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showPlayerSpecialStuff = !showPlayerSpecialStuff;
		}
		GUILayout.EndHorizontal();
		if (showPlayerSpecialStuff)
			DrawCharacterLimitStuff();

		GUILayout.Space(10);

		// Enemies 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Enemy spawn points", EditorStyles.boldLabel);
		if (GUILayout.Button((showEnemyStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showEnemyStuff = !showEnemyStuff;
		}
		GUILayout.EndHorizontal();
		if (showEnemyStuff) {
			DrawEnemyStuff();
		}

		GUILayout.Space(10);

		// Interactions 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Interaction points", EditorStyles.boldLabel);
		if (GUILayout.Button((showInteractStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showInteractStuff = !showInteractStuff;
		}
		GUILayout.EndHorizontal();
		if (showInteractStuff)
			DrawInteractStuff();

		GUILayout.Space(10);

		// Turn Events 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Turn Events", EditorStyles.boldLabel);
		if (GUILayout.Button((showTurnEventStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showTurnEventStuff = !showTurnEventStuff;
		}
		GUILayout.EndHorizontal();
		if (showTurnEventStuff)
			DrawTurnEventStuff();

		GUILayout.Space(10);

		// Reinforcements 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Reinforcements", EditorStyles.boldLabel);
		if (GUILayout.Button((showReinforcementStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showReinforcementStuff = !showReinforcementStuff;
		}
		GUILayout.EndHorizontal();
		if (showReinforcementStuff) {
			DrawReinforcementStuff();
		}

		GUILayout.Space(10);

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	/// <summary>
	/// Draws the list of player spawn position and joining characters.
	/// </summary>
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
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.spawnPoints.Add(new Position());
		}
	}

	private void DrawCharacterLimitStuff() {
		GUILayout.Space(5);

		GUILayout.Label("Forced Characters");
		for (int i = 0; i < mapValues.forcedCharacters.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			mapValues.forcedCharacters[i] = (CharData)EditorGUILayout.ObjectField("Character", mapValues.forcedCharacters[i], typeof(CharData), false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.forcedCharacters.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.forcedCharacters.Add(null);
		}

		GUILayout.Space(5);

		GUILayout.Label("Locked Characters");
		for (int i = 0; i < mapValues.lockedCharacters.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			mapValues.lockedCharacters[i] = (CharData)EditorGUILayout.ObjectField("Character", mapValues.lockedCharacters[i], typeof(CharData), false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.lockedCharacters.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.lockedCharacters.Add(null);
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
			//  = (ClassType)EditorGUILayout.EnumPopup("",entryValues.advantageType[i]);
			if (GUILayout.Button("Dup", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				EnemyPosition epos = new EnemyPosition();
				epos.Copy(mapValues.enemies[i]);
				mapValues.enemies.Insert(i+1, epos);
			}
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.enemies.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			mapValues.enemies[i].charData = (CharData)EditorGUILayout.ObjectField("Character",mapValues.enemies[i].charData, typeof(CharData),false);
			mapValues.enemies[i].hasQuotes = EditorGUILayout.Toggle("Quotes", mapValues.enemies[i].hasQuotes, GUILayout.Width(80));
			GUILayout.EndHorizontal();
			
			if (mapValues.enemies[i].charData != null) {
				GUILayout.BeginHorizontal();
				mapValues.enemies[i].level = EditorGUILayout.IntField("Level", mapValues.enemies[i].level);
				mapValues.enemies[i].aggroType = (AggroType)EditorGUILayout.EnumPopup("Aggro", mapValues.enemies[i].aggroType);
				GUILayout.EndHorizontal();
			
				if (mapValues.enemies[i].aggroType == AggroType.HUNT){
					GUILayout.BeginHorizontal();
					GUILayout.Label("Hunt tile");
					mapValues.enemies[i].huntX = EditorGUILayout.IntField("", mapValues.enemies[i].huntX);
					mapValues.enemies[i].huntY = EditorGUILayout.IntField("", mapValues.enemies[i].huntY);
					GUILayout.EndHorizontal();
				}

				// Inventory
				for (int j = 0; j < mapValues.enemies[i].inventory.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					mapValues.enemies[i].inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item",mapValues.enemies[i].inventory[j].item, typeof(ItemEntry),false);
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

	private void DrawReinforcementStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.reinforcements.Count; i++) {
			ReinforcementPosition pos = mapValues.reinforcements[i];
			GUILayout.BeginHorizontal();
			GUILayout.Label("End of turn");
			pos.spawnTurn = EditorGUILayout.IntField("", pos.spawnTurn);
			if (GUILayout.Button("Dup", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				ReinforcementPosition rpos = new ReinforcementPosition();
				rpos.Copy(mapValues.reinforcements[i]);
				mapValues.reinforcements.Insert(i+1, rpos);
			}
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.reinforcements.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 40;
			GUILayout.Label("Position");
			pos.x = EditorGUILayout.IntField("X", pos.x, GUILayout.Width(90));
			pos.y = EditorGUILayout.IntField("Y", pos.y, GUILayout.Width(90));
			EditorGUIUtility.labelWidth = 70;
			pos.faction = (Faction)EditorGUILayout.EnumPopup("Faction", pos.faction);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			pos.charData = (CharData)EditorGUILayout.ObjectField("Character",pos.charData, typeof(CharData),false);
			
			if (pos.charData != null) {
				EditorGUIUtility.labelWidth = 50;
				pos.level = EditorGUILayout.IntField("Level", pos.level, GUILayout.Width(80));
				GUILayout.EndHorizontal();
			
				// Inventory
				for (int j = 0; j < pos.inventory.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					pos.inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item",pos.inventory[j].item, typeof(ItemEntry),false);
					EditorGUIUtility.labelWidth = 35;
					pos.inventory[j].droppable = EditorGUILayout.Toggle("Drop", pos.inventory[j].droppable, GUILayout.Width(50));
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						pos.inventory.RemoveAt(j);
						j--;
						continue;
					}
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 120;	
				}
				GUILayout.BeginHorizontal();
				GUILayout.Space(120);
				if (GUILayout.Button("Add Item")) {
					pos.inventory.Add(new WeaponTuple());
				}
				GUILayout.EndHorizontal();
			}
			else {
				GUILayout.EndHorizontal();
			}

			if (pos.hasQuotes) {
				// Quotes
				for (int j = 0; j < pos.quotes.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					pos.quotes[j].triggerer = (CharData)EditorGUILayout.ObjectField("Caused by",pos.quotes[j].triggerer, typeof(CharData),false);
					pos.quotes[j].quote = (DialogueEntry)EditorGUILayout.ObjectField("Quote",pos.quotes[j].quote, typeof(DialogueEntry),false);
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						pos.quotes.RemoveAt(j);
						j--;
						continue;
					}
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 120;	
				}
				GUILayout.BeginHorizontal();
				GUILayout.Space(120);
				if (GUILayout.Button("Add Quote")) {
					pos.quotes.Add(new FightQuote());
				}
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.reinforcements.Add(new ReinforcementPosition());
		}
	}

	private void DrawInteractStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.interactions.Count; i++) {
			InteractPosition pos = mapValues.interactions[i];
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			pos.x = EditorGUILayout.IntField("X", pos.x);
			pos.y = EditorGUILayout.IntField("Y", pos.y);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.interactions.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();

			pos.interactType = (InteractType)EditorGUILayout.EnumPopup("Type", pos.interactType);

			switch (pos.interactType)
			{
				case InteractType.BLOCK:
					pos.health = EditorGUILayout.IntField("Health", pos.health);
					break;
				case InteractType.VILLAGE:
					pos.dialogue = (DialogueEntry)EditorGUILayout.ObjectField("Dialogue",pos.dialogue, typeof(DialogueEntry),false);
					pos.gift = (ItemEntry)EditorGUILayout.ObjectField("Gift",pos.gift, typeof(ItemEntry),false);
					pos.ally.charData = (CharData)EditorGUILayout.ObjectField("New ally",pos.ally.charData, typeof(CharData),false);
					if (pos.ally.charData != null) {
						pos.ally.level = EditorGUILayout.IntField("Level", pos.ally.level);

						EditorGUIUtility.labelWidth = 70;
						for (int j = 0; j < pos.ally.inventory.Count; j++) {
							GUILayout.BeginHorizontal();
							pos.ally.inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item",pos.ally.inventory[j].item, typeof(ItemEntry),false);
							if (GUILayout.Button("X", GUILayout.Width(50))) {
								GUI.FocusControl(null);
								pos.ally.inventory.RemoveAt(j);
								i--;
							}
							GUILayout.EndHorizontal();
						}
						EditorGUIUtility.labelWidth = 120;
						GUILayout.BeginHorizontal();
						GUILayout.Space(120);
						if (GUILayout.Button("Add Item")) {
							pos.ally.inventory.Add(new WeaponTuple());
						}
						GUILayout.EndHorizontal();
					}
					break;
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.interactions.Add(new InteractPosition());
		}
	}

	private void DrawTurnEventStuff() {
		GUILayout.Space(5);
		for (int i = 0; i < mapValues.turnEvents.Count; i++) {
			TurnEvent pos = mapValues.turnEvents[i];
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			pos.turn = EditorGUILayout.IntField("Turn", pos.turn);
			pos.factionTurn = (Faction)EditorGUILayout.EnumPopup("Faction", pos.factionTurn);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.turnEvents.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();

			pos.type = (TurnEventType)EditorGUILayout.EnumPopup("Type", pos.type);

			switch (pos.type)
			{
				case TurnEventType.MAPCHANGE:
					GUILayout.BeginHorizontal();
					GUILayout.Label("Position");
					pos.x = EditorGUILayout.IntField("X", pos.x);
					pos.y = EditorGUILayout.IntField("Y", pos.y);
					GUILayout.EndHorizontal();
					pos.changeTerrain = (TerrainTile)EditorGUILayout.ObjectField("New Tile",pos.changeTerrain, typeof(TerrainTile),false);
					break;
				case TurnEventType.DIALOGUE:
					pos.dialogue = (DialogueEntry)EditorGUILayout.ObjectField("Dialogue",pos.dialogue, typeof(DialogueEntry),false);
					break;
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.turnEvents.Add(new TurnEvent());
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
		if (mapLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		MapEntry me = Editor.CreateInstance<MapEntry>();
		me.name = uuid;
		me.uuid = uuid;
		me.repColor = repColor;
		me.entryName = uuid;
		string path = "Assets/LibraryData/Maps/" + uuid + ".asset";

		mapLibrary.InsertEntry(me,0);
		Undo.RecordObject(mapLibrary, "Added entry");
		EditorUtility.SetDirty(mapLibrary);
		AssetDatabase.CreateAsset(me, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = mapLibrary.GetRepresentations("",filterStr);
		uuid = "";
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
