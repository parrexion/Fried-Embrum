using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SkillEditorWindow {

	public ScrObjLibraryVariable skillLibrary;
	public CharacterSkill skillValues;
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
	SkillActivation skillActivation = SkillActivation.NONE;


	public SkillEditorWindow(ScrObjLibraryVariable entries, CharacterSkill container) {
		skillLibrary = entries;
		skillValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {
		Debug.Log("Loading skill library...");

		skillLibrary.GenerateDictionary();

		Debug.Log("Finished loading skill library");

		InitializeWindow();
	}

	public void InitializeWindow() {
		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.6f, 0.2f, 0.6f));
		dispTex.Apply();

		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		skillValues.ResetValues();
		currentEntryList = skillLibrary.GetRepresentations("", "");
		filterStr = "";
	}

	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Skill Editor", EditorStyles.boldLabel);
		if (selIndex != -1) {
			if (GUILayout.Button("Save Skill")) {
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
			currentEntryList = skillLibrary.GetRepresentations("", filterStr);

		selScrollPos = EditorGUILayout.BeginScrollView(selScrollPos, GUILayout.Width(selectRect.width),
							GUILayout.Height(selectRect.height - 150));

		int oldSelected = selIndex;
		selIndex = GUILayout.SelectionGrid(selIndex, currentEntryList, 1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selIndex) {
			GUI.FocusControl(null);
			SelectEntry();
		}

		GUILayout.Label("Create new skill", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Skill uuid", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		skillActivation = (SkillActivation)EditorGUILayout.EnumPopup("Skilltype", skillActivation);
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
							GUILayout.Height(dispRect.height - 45));

		EditorGUILayout.SelectableLabel("Selected Skill:   " + skillValues.uuid, EditorStyles.boldLabel);
		skillValues.entryName = EditorGUILayout.TextField("Skill Name", skillValues.entryName);
		skillValues.icon = (Sprite)EditorGUILayout.ObjectField("Skill icon", skillValues.icon, typeof(Sprite), false);
		GUILayout.Label("Skill description");
		skillValues.description = EditorGUILayout.TextArea(skillValues.description, GUILayout.Height(60));

		GUILayout.Space(10);
		GUILayout.Label(skillValues.activationType.ToString(), EditorStyles.boldLabel);
		GUILayout.Space(10);

		switch (skillValues.activationType) {
			case SkillActivation.PRECOMBAT:
				GUILayout.Label("Boost during combat", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				GUILayout.Space(10);
				GUILayout.Label("Requirements", EditorStyles.boldLabel);
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Certain enemy attacks?");
				skillValues.enemyCanAttack = (EnemyCanAttack)EditorGUILayout.EnumPopup(skillValues.enemyCanAttack);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);

				GUILayout.Label("Activation range (inclusive)");
				GUILayout.BeginHorizontal();
				skillValues.range = EditorGUILayout.IntField("Min range", skillValues.range);
				skillValues.rangeMax = EditorGUILayout.IntField("Max range", skillValues.rangeMax);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);

				GUILayout.Label("Activation terrain");
				for (int i = 0; i < skillValues.terrainReq.Count; i++) {
					GUILayout.BeginHorizontal();
					skillValues.terrainReq[i] = (TerrainTile)EditorGUILayout.ObjectField("Item", skillValues.terrainReq[i], typeof(TerrainTile), false);
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						skillValues.terrainReq.RemoveAt(i);
						i--;
						continue;
					}
					GUILayout.EndHorizontal();
				}
				if (GUILayout.Button("+")) {
					skillValues.terrainReq.Add(null);
				}
				break;


			case SkillActivation.INITCOMBAT:
				GUILayout.Label("Boost when attacking", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;

			case SkillActivation.STARTTURN:
				GUILayout.Label("Heal at start of turn", EditorStyles.boldLabel);
				skillValues.percent = EditorGUILayout.FloatField("Heal amount %", skillValues.percent);
				skillValues.range = EditorGUILayout.IntField("Heal range", skillValues.range);
				break;


			case SkillActivation.PASSIVE:
				GUILayout.Label("Increased stats", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;


			case SkillActivation.POSTCOMBAT:
				GUILayout.Label("Debuffed stats", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				break;


			case SkillActivation.REWARD:
				GUILayout.Label("Reward bonus", EditorStyles.boldLabel);
				//skillValues.chance = EditorGUILayout.FloatField("Trigger chance %", skillValues.chance);
				//skillValues.boost.hp = EditorGUILayout.IntField("Money gain", skillValues.boost.hp);
				//skillValues.boost.dmg = EditorGUILayout.IntField("Scrap gain", skillValues.boost.dmg);
				skillValues.percent = EditorGUILayout.FloatField("Exp boost %", skillValues.percent);
				break;


			case SkillActivation.COUNTER:
				GUILayout.Label("Boost when being attacked", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void SelectEntry() {
		// Nothing selected
		if (selIndex == -1) {
			skillValues.ResetValues();
		}
		else {
			// Something selected
			CharacterSkill cs = (CharacterSkill)skillLibrary.GetEntryByIndex(selIndex);
			skillValues.CopyValues(cs);
		}
	}

	void SaveSelectedEntry() {
		CharacterSkill cs = (CharacterSkill)skillLibrary.GetEntryByIndex(selIndex);
		cs.CopyValues(skillValues);
		Undo.RecordObject(cs, "Updated entry");
		EditorUtility.SetDirty(cs);
	}

	void InstansiateEntry() {
		GUI.FocusControl(null);
		if (skillLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		CharacterSkill cs = null;
		switch (skillActivation) {
			case SkillActivation.PRECOMBAT:
				cs = Editor.CreateInstance<CombatBoost>(); break;
			case SkillActivation.INITCOMBAT:
				cs = Editor.CreateInstance<InitiateBoost>(); break;
			case SkillActivation.STARTTURN:
				cs = Editor.CreateInstance<StartTurnGain>(); break;
			case SkillActivation.PASSIVE:
				cs = Editor.CreateInstance<PermanentBoost>(); break;
			case SkillActivation.POSTCOMBAT:
				cs = Editor.CreateInstance<PostCombatDebuff>(); break;
			case SkillActivation.REWARD:
				cs = Editor.CreateInstance<ExpBoost>(); break;
			case SkillActivation.COUNTER:
				cs = Editor.CreateInstance<CounterBoost>(); break;
		}
		if (cs == null) {
			Debug.LogError("Activation type is invalid");
			return;
		}
		cs.name = uuid;
		cs.uuid = uuid;
		cs.repColor = repColor;
		cs.entryName = uuid;
		cs.activationType = skillActivation;
		string path = "Assets/LibraryData/Skills/" + uuid + ".asset";

		skillLibrary.InsertEntry(cs, 0);
		Undo.RecordObject(skillLibrary, "Added entry");
		EditorUtility.SetDirty(skillLibrary);
		AssetDatabase.CreateAsset(cs, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = skillLibrary.GetRepresentations("", filterStr);
		uuid = "";
		selIndex = 0;
		SelectEntry();
	}

	void DeleteEntry() {
		GUI.FocusControl(null);
		CharacterSkill cs = (CharacterSkill)skillLibrary.GetEntryByIndex(selIndex);
		string path = "Assets/LibraryData/Skills/" + cs.uuid + ".asset";

		skillLibrary.RemoveEntryByIndex(selIndex);
		Undo.RecordObject(skillLibrary, "Deleted entry");
		EditorUtility.SetDirty(skillLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = skillLibrary.GetRepresentations("", filterStr);

		if (res) {
			Debug.Log("Removed entry: " + cs.uuid);
			selIndex = -1;
		}
	}

}
