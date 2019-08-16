using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Map";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<MapEntry>();
	protected override Color BackgroundColor => new Color(0.3f, 0.6f, 0.4f);

	private bool showPlayerStuff = false;
	private bool showPlayerSpecialStuff = false;
	private bool showEnemyStuff = false;
	private bool showInteractStuff = false;
	private bool showTurnEventStuff = false;
	private bool showReinforcementStuff = false;


	public MapEditorWindow(ScrObjLibraryVariable entries, MapEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		MapEntry mapValues = (MapEntry)entryValues;
		EditorGUIUtility.labelWidth = 120;
		mapValues.entryName = EditorGUILayout.TextField("Map Name", mapValues.entryName);

		GUILayout.Space(10);
		GUILayout.Label("Mission description");
		EditorStyles.textField.wordWrap = true;
		mapValues.mapDescription = EditorGUILayout.TextArea(mapValues.mapDescription, GUILayout.Width(500), GUILayout.Height(30));
		EditorStyles.textField.wordWrap = false;

		GUILayout.Space(10);
		GUILayout.Label("Map objectives", EditorStyles.boldLabel);
		EditorGUIUtility.labelWidth = 80;
		GUILayout.BeginHorizontal();
		mapValues.winCondition = (WinCondition)EditorGUILayout.EnumPopup("Win", mapValues.winCondition);
		GUILayout.Space(20);
		mapValues.loseCondition = (LoseCondition)EditorGUILayout.EnumPopup("Lose", mapValues.loseCondition);
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("Map Size", EditorStyles.boldLabel);
		EditorGUIUtility.labelWidth = 120;
		mapValues.mapSprite = (Texture2D)EditorGUILayout.ObjectField("Map Info Sprite", mapValues.mapSprite, typeof(Texture2D), false);
		GUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 80;
		mapValues.sizeX = EditorGUILayout.IntField("Size X", mapValues.sizeX);
		mapValues.sizeY = EditorGUILayout.IntField("Size Y", mapValues.sizeY);
		EditorGUIUtility.labelWidth = 120;
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("Dialogues", EditorStyles.boldLabel);
		mapValues.skipBattlePrep = EditorGUILayout.Toggle("Skip battle preps?", mapValues.skipBattlePrep);
		mapValues.preDialogue = (DialogueEntry)EditorGUILayout.ObjectField("Mission dialogue", mapValues.preDialogue, typeof(DialogueEntry), false);
		mapValues.introDialogue = (DialogueEntry)EditorGUILayout.ObjectField("On mission start", mapValues.introDialogue, typeof(DialogueEntry), false);
		mapValues.endDialogue = (DialogueEntry)EditorGUILayout.ObjectField("After win", mapValues.endDialogue, typeof(DialogueEntry), false);

		GUILayout.Space(10);

		GUILayout.Label("Music", EditorStyles.boldLabel);
		mapValues.playerMusic = (MusicEntry)EditorGUILayout.ObjectField("Player music", mapValues.playerMusic, typeof(MusicEntry), false);
		mapValues.enemyMusic = (MusicEntry)EditorGUILayout.ObjectField("Enemy music", mapValues.enemyMusic, typeof(MusicEntry), false);
		mapValues.battleMusic = (MusicEntry)EditorGUILayout.ObjectField("Battle music", mapValues.battleMusic, typeof(MusicEntry), false);
		mapValues.healMusic = (MusicEntry)EditorGUILayout.ObjectField("Heal music", mapValues.healMusic, typeof(MusicEntry), false);

		GUILayout.Space(10);

		// Player stuff
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player spawn points", EditorStyles.boldLabel);
		if(GUILayout.Button((showPlayerStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showPlayerStuff = !showPlayerStuff;
		}
		GUILayout.EndHorizontal();
		if(showPlayerStuff)
			DrawPlayerStuff();

		GUILayout.Space(10);

		// Player selection stuff
		GUILayout.BeginHorizontal();
		GUILayout.Label("Character limitations", EditorStyles.boldLabel);
		if(GUILayout.Button((showPlayerSpecialStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showPlayerSpecialStuff = !showPlayerSpecialStuff;
		}
		GUILayout.EndHorizontal();
		if(showPlayerSpecialStuff)
			DrawCharacterLimitStuff();

		GUILayout.Space(10);

		// Enemies 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Enemy spawn points", EditorStyles.boldLabel);
		if(GUILayout.Button((showEnemyStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showEnemyStuff = !showEnemyStuff;
		}
		GUILayout.EndHorizontal();
		if(showEnemyStuff) {
			DrawEnemyStuff();
		}

		GUILayout.Space(10);

		// Interactions 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Interaction points", EditorStyles.boldLabel);
		if(GUILayout.Button((showInteractStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showInteractStuff = !showInteractStuff;
		}
		GUILayout.EndHorizontal();
		if(showInteractStuff)
			DrawInteractStuff();

		GUILayout.Space(10);

		// Turn Events 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Turn Events", EditorStyles.boldLabel);
		if(GUILayout.Button((showTurnEventStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showTurnEventStuff = !showTurnEventStuff;
		}
		GUILayout.EndHorizontal();
		if(showTurnEventStuff)
			DrawTurnEventStuff();

		GUILayout.Space(10);

		// Reinforcements 
		GUILayout.BeginHorizontal();
		GUILayout.Label("Reinforcements", EditorStyles.boldLabel);
		if(GUILayout.Button((showReinforcementStuff) ? "Hide" : "Show", GUILayout.Width(150))) {
			showReinforcementStuff = !showReinforcementStuff;
		}
		GUILayout.EndHorizontal();
		if(showReinforcementStuff) {
			DrawReinforcementStuff();
		}
		
	}

	/// <summary>
	/// Draws the list of player spawn position and joining characters.
	/// </summary>
	private void DrawPlayerStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);
		GUILayout.Label("Squad 1");
		for (int i = 0; i < mapValues.spawnPoints1.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.spawnPoints1[i].x = EditorGUILayout.IntField("X", mapValues.spawnPoints1[i].x);
			mapValues.spawnPoints1[i].y = EditorGUILayout.IntField("Y", mapValues.spawnPoints1[i].y);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.spawnPoints1.RemoveAt(i);
				i--;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.spawnPoints1.Add(new Position());
		}

		LibraryEditorWindow.HorizontalLine(Color.black);

		GUILayout.Space(5);
		GUILayout.Label("Squad 2");
		for (int i = 0; i < mapValues.spawnPoints2.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.spawnPoints2[i].x = EditorGUILayout.IntField("X", mapValues.spawnPoints2[i].x);
			mapValues.spawnPoints2[i].y = EditorGUILayout.IntField("Y", mapValues.spawnPoints2[i].y);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.spawnPoints2.RemoveAt(i);
				i--;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if (GUILayout.Button("+")) {
			mapValues.spawnPoints2.Add(new Position());
		}
	}

	private void DrawCharacterLimitStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);

		GUILayout.Label("Forced Characters");
		for(int i = 0; i < mapValues.forcedCharacters.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			mapValues.forcedCharacters[i] = (CharData)EditorGUILayout.ObjectField("Character", mapValues.forcedCharacters[i], typeof(CharData), false);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.forcedCharacters.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.forcedCharacters.Add(null);
		}

		GUILayout.Space(5);

		GUILayout.Label("Locked Characters");
		for(int i = 0; i < mapValues.lockedCharacters.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			mapValues.lockedCharacters[i] = (CharData)EditorGUILayout.ObjectField("Character", mapValues.lockedCharacters[i], typeof(CharData), false);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.lockedCharacters.RemoveAt(i);
				i--;
			}
			GUILayout.EndHorizontal();
			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.lockedCharacters.Add(null);
		}
	}

	private void DrawEnemyStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);
		for(int i = 0; i < mapValues.enemies.Count; i++) {
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			mapValues.enemies[i].x = EditorGUILayout.IntField("X", mapValues.enemies[i].x);
			mapValues.enemies[i].y = EditorGUILayout.IntField("Y", mapValues.enemies[i].y);
			//  = (ClassType)EditorGUILayout.EnumPopup("",entryValues.advantageType[i]);
			if(GUILayout.Button("Dup", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				EnemyPosition epos = new EnemyPosition();
				epos.Copy(mapValues.enemies[i]);
				mapValues.enemies.Insert(i + 1, epos);
			}
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.enemies.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			mapValues.enemies[i].charData = (CharData)EditorGUILayout.ObjectField("Character", mapValues.enemies[i].charData, typeof(CharData), false);
			mapValues.enemies[i].hasQuotes = EditorGUILayout.Toggle("Quotes", mapValues.enemies[i].hasQuotes, GUILayout.Width(80));
			GUILayout.EndHorizontal();

			if(mapValues.enemies[i].charData != null) {
				GUILayout.BeginHorizontal();
				mapValues.enemies[i].level = EditorGUILayout.IntField("Level", mapValues.enemies[i].level);
				mapValues.enemies[i].aggroType = (AggroType)EditorGUILayout.EnumPopup("Aggro", mapValues.enemies[i].aggroType);
				GUILayout.EndHorizontal();

				if(mapValues.enemies[i].aggroType == AggroType.HUNT) {
					GUILayout.BeginHorizontal();
					GUILayout.Label("Hunt tile");
					mapValues.enemies[i].huntX = EditorGUILayout.IntField("", mapValues.enemies[i].huntX);
					mapValues.enemies[i].huntY = EditorGUILayout.IntField("", mapValues.enemies[i].huntY);
					GUILayout.EndHorizontal();
				} 
				else if(mapValues.enemies[i].aggroType == AggroType.PATROL) {
					for(int pos = 0; pos < mapValues.enemies[i].patrolPositions.Count; pos++) {
						GUILayout.BeginHorizontal();
						GUILayout.Label("Patrol tile");
						mapValues.enemies[i].patrolPositions[pos].x = EditorGUILayout.IntField("", mapValues.enemies[i].patrolPositions[pos].x);
						mapValues.enemies[i].patrolPositions[pos].y = EditorGUILayout.IntField("", mapValues.enemies[i].patrolPositions[pos].y);
						if(GUILayout.Button("X", GUILayout.Width(50))) {
							GUI.FocusControl(null);
							mapValues.enemies[i].patrolPositions.RemoveAt(i);
							i--;
							continue;
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.BeginHorizontal();
					GUILayout.Label("Add Tile");
					if (GUILayout.Button("+")) {
						mapValues.enemies[i].patrolPositions.Add(new Position());
					}
					GUILayout.EndHorizontal();
				}

				// Inventory
				for(int j = 0; j < mapValues.enemies[i].inventory.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					mapValues.enemies[i].inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item", mapValues.enemies[i].inventory[j].item, typeof(ItemEntry), false);
					EditorGUIUtility.labelWidth = 35;
					mapValues.enemies[i].inventory[j].droppable = EditorGUILayout.Toggle("Drop", mapValues.enemies[i].inventory[j].droppable, GUILayout.Width(50));
					if(GUILayout.Button("X", GUILayout.Width(50))) {
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
				if(GUILayout.Button("Add Item")) {
					mapValues.enemies[i].inventory.Add(new WeaponTuple());
				}
				GUILayout.EndHorizontal();
			}

			if(mapValues.enemies[i].hasQuotes) {
				// Quotes
				for(int j = 0; j < mapValues.enemies[i].quotes.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					mapValues.enemies[i].quotes[j].triggerer = (CharData)EditorGUILayout.ObjectField("Caused by", mapValues.enemies[i].quotes[j].triggerer, typeof(CharData), false);
					mapValues.enemies[i].quotes[j].quote = (DialogueEntry)EditorGUILayout.ObjectField("Quote", mapValues.enemies[i].quotes[j].quote, typeof(DialogueEntry), false);
					if(GUILayout.Button("X", GUILayout.Width(50))) {
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
				if(GUILayout.Button("Add Quote")) {
					mapValues.enemies[i].quotes.Add(new FightQuote());
				}
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.enemies.Add(new EnemyPosition());
		}
	}

	private void DrawReinforcementStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);
		for(int i = 0; i < mapValues.reinforcements.Count; i++) {
			ReinforcementPosition pos = mapValues.reinforcements[i];
			GUILayout.BeginHorizontal();
			GUILayout.Label("End of turn");
			pos.spawnTurn = EditorGUILayout.IntField("", pos.spawnTurn);
			if(GUILayout.Button("Dup", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				ReinforcementPosition rpos = new ReinforcementPosition();
				rpos.Copy(mapValues.reinforcements[i]);
				mapValues.reinforcements.Insert(i + 1, rpos);
			}
			if(GUILayout.Button("X", GUILayout.Width(50))) {
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
			if (pos.faction == Faction.PLAYER) {
				EditorGUIUtility.labelWidth = 40;
				pos.joiningSquad = EditorGUILayout.IntField("Squad", pos.joiningSquad, GUILayout.Width(80));
				EditorGUIUtility.labelWidth = 70;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			pos.charData = (CharData)EditorGUILayout.ObjectField("Character", pos.charData, typeof(CharData), false);

			if(pos.charData != null) {
				EditorGUIUtility.labelWidth = 50;
				pos.level = EditorGUILayout.IntField("Level", pos.level, GUILayout.Width(80));
				GUILayout.EndHorizontal();

				// Inventory
				for(int j = 0; j < pos.inventory.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					pos.inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item", pos.inventory[j].item, typeof(ItemEntry), false);
					EditorGUIUtility.labelWidth = 35;
					pos.inventory[j].droppable = EditorGUILayout.Toggle("Drop", pos.inventory[j].droppable, GUILayout.Width(50));
					if(GUILayout.Button("X", GUILayout.Width(50))) {
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
				if(GUILayout.Button("Add Item")) {
					pos.inventory.Add(new WeaponTuple());
				}
				GUILayout.EndHorizontal();
			} else {
				GUILayout.EndHorizontal();
			}

			if(pos.hasQuotes) {
				// Quotes
				for(int j = 0; j < pos.quotes.Count; j++) {
					EditorGUIUtility.labelWidth = 70;
					GUILayout.BeginHorizontal();
					pos.quotes[j].triggerer = (CharData)EditorGUILayout.ObjectField("Caused by", pos.quotes[j].triggerer, typeof(CharData), false);
					pos.quotes[j].quote = (DialogueEntry)EditorGUILayout.ObjectField("Quote", pos.quotes[j].quote, typeof(DialogueEntry), false);
					if(GUILayout.Button("X", GUILayout.Width(50))) {
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
				if(GUILayout.Button("Add Quote")) {
					pos.quotes.Add(new FightQuote());
				}
				GUILayout.EndHorizontal();
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.reinforcements.Add(new ReinforcementPosition());
		}
	}

	private void DrawInteractStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);
		for(int i = 0; i < mapValues.interactions.Count; i++) {
			InteractPosition pos = mapValues.interactions[i];
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			GUILayout.Label("Position");
			pos.x = EditorGUILayout.IntField("X", pos.x);
			pos.y = EditorGUILayout.IntField("Y", pos.y);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.interactions.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();

			pos.interactType = (InteractType)EditorGUILayout.EnumPopup("Type", pos.interactType);

			switch(pos.interactType) {
				case InteractType.BLOCK:
					pos.health = EditorGUILayout.IntField("Health", pos.health);
					break;
				case InteractType.VILLAGE:
					pos.dialogue = (DialogueEntry)EditorGUILayout.ObjectField("Dialogue", pos.dialogue, typeof(DialogueEntry), false);
					GUILayout.BeginHorizontal();
					pos.gift.money = EditorGUILayout.IntField("Money", pos.gift.money);
					pos.gift.scrap = EditorGUILayout.IntField("Scrap", pos.gift.scrap);
					GUILayout.EndHorizontal();
					if(pos.gift.items.Count < 1)
						pos.gift.items.Add(null);
					pos.gift.items[0] = (ItemEntry)EditorGUILayout.ObjectField("Item", pos.gift.items[0], typeof(ItemEntry), false);
					pos.ally.charData = (CharData)EditorGUILayout.ObjectField("New ally", pos.ally.charData, typeof(CharData), false);
					if(pos.ally.charData != null) {
						pos.ally.level = EditorGUILayout.IntField("Level", pos.ally.level);

						EditorGUIUtility.labelWidth = 70;
						for(int j = 0; j < pos.ally.inventory.Count; j++) {
							GUILayout.BeginHorizontal();
							pos.ally.inventory[j].item = (ItemEntry)EditorGUILayout.ObjectField("Item", pos.ally.inventory[j].item, typeof(ItemEntry), false);
							if(GUILayout.Button("X", GUILayout.Width(50))) {
								GUI.FocusControl(null);
								pos.ally.inventory.RemoveAt(j);
								i--;
							}
							GUILayout.EndHorizontal();
						}
						EditorGUIUtility.labelWidth = 120;
						GUILayout.BeginHorizontal();
						GUILayout.Space(120);
						if(GUILayout.Button("Add Item")) {
							pos.ally.inventory.Add(new WeaponTuple());
						}
						GUILayout.EndHorizontal();
					}
					break;
				case InteractType.DATABASE:
					GUILayout.BeginHorizontal();
					pos.gift.money = EditorGUILayout.IntField("Money", pos.gift.money);
					pos.gift.scrap = EditorGUILayout.IntField("Scrap", pos.gift.scrap);
					GUILayout.EndHorizontal();
					if(pos.gift.items.Count < 1)
						pos.gift.items.Add(null);
					pos.gift.items[0] = (ItemEntry)EditorGUILayout.ObjectField("Item", pos.gift.items[0], typeof(ItemEntry), false);
					break;
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.interactions.Add(new InteractPosition());
		}
	}

	private void DrawTurnEventStuff() {
		MapEntry mapValues = (MapEntry)entryValues;
		GUILayout.Space(5);
		for(int i = 0; i < mapValues.turnEvents.Count; i++) {
			TurnEvent pos = mapValues.turnEvents[i];
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			pos.turn = EditorGUILayout.IntField("Turn", pos.turn);
			pos.factionTurn = (Faction)EditorGUILayout.EnumPopup("Faction", pos.factionTurn);
			if(GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				mapValues.turnEvents.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUIUtility.labelWidth = 120;
			GUILayout.EndHorizontal();

			pos.type = (TurnEventType)EditorGUILayout.EnumPopup("Type", pos.type);

			switch(pos.type) {
				case TurnEventType.MAPCHANGE:
					GUILayout.BeginHorizontal();
					GUILayout.Label("Position");
					pos.x = EditorGUILayout.IntField("X", pos.x);
					pos.y = EditorGUILayout.IntField("Y", pos.y);
					GUILayout.EndHorizontal();
					pos.changeTerrain = (TerrainTile)EditorGUILayout.ObjectField("New Tile", pos.changeTerrain, typeof(TerrainTile), false);
					break;
				case TurnEventType.DIALOGUE:
					pos.dialogue = (DialogueEntry)EditorGUILayout.ObjectField("Dialogue", pos.dialogue, typeof(DialogueEntry), false);
					break;
			}

			LibraryEditorWindow.HorizontalLine(Color.black);
		}
		if(GUILayout.Button("+")) {
			mapValues.turnEvents.Add(new TurnEvent());
		}
	}
	
}
