using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ItemEditorWindow {

	public ScrObjLibraryVariable itemLibrary;
	public ItemEntry itemValues;
	private GUIContent[] currentEntryList;

	// Selection screen
	Rect selectRect = new Rect();
	Texture2D selectTex;
	Vector2 scrollPos;
	int selItem = -1;
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
	public ItemEditorWindow(ScrObjLibraryVariable entries, ItemEntry container){
		itemLibrary = entries;
		itemValues = container;
		LoadLibrary();
	}

	void LoadLibrary() {

		Debug.Log("Loading item equip libraries...");

		itemLibrary.GenerateDictionary();

		Debug.Log("Finished loading item equip libraries");

		InitializeWindow();
	}

	public void InitializeWindow() {
		selectTex = new Texture2D(1, 1);
		selectTex.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f));
		selectTex.Apply();

		dispTex = new Texture2D(1, 1);
		dispTex.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.1f));
		dispTex.Apply();

		dispOffset.right = 10;

		itemValues.ResetValues();
		currentEntryList = itemLibrary.GetRepresentations("","");
		filterStr = "";
	}


	public void DrawWindow(int screenWidth, int screenHeight) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Item Equip Editor", EditorStyles.boldLabel);
		if (selItem != -1) {
			if (GUILayout.Button("Save Item")){
				SaveSelectedItem();
			}
		}
		GUILayout.EndHorizontal();

		GenerateAreas(screenWidth, screenHeight);
		DrawBackgrounds();
		DrawEntryList();
		if (selItem != -1)
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
			currentEntryList = itemLibrary.GetRepresentations("",filterStr);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(selectRect.width), 
						GUILayout.Height(selectRect.height-130));

		int oldSelected = selItem;
		selItem = GUILayout.SelectionGrid(selItem, currentEntryList,1);
		EditorGUILayout.EndScrollView();

		if (oldSelected != selItem)
			SelectItem();

		EditorGUIUtility.labelWidth = 110;
		GUILayout.Label("Create new item", EditorStyles.boldLabel);
		uuid = EditorGUILayout.TextField("Item Name", uuid);
		repColor = EditorGUILayout.ColorField("Display Color", repColor);
		if (GUILayout.Button("Create new")) {
			InstansiateItem();
		}
		if (GUILayout.Button("Delete item")) {
			DeleteItem();
		}

		GUILayout.EndArea();
	}

	void DrawDisplayWindow() {
		GUILayout.BeginArea(dispRect);
		dispScrollPos = GUILayout.BeginScrollView(dispScrollPos, GUILayout.Width(dispRect.width), 
							GUILayout.Height(dispRect.height-25));

		GUI.skin.textField.margin.right = 20;

		GUILayout.Label("Selected Item", EditorStyles.boldLabel);
		EditorGUILayout.SelectableLabel("UUID: " + itemValues.uuid);

		GUILayout.Space(10);

		itemValues.entryName = EditorGUILayout.TextField("Name", itemValues.entryName);
		itemValues.itemCategory = (ItemCategory)EditorGUILayout.EnumPopup("Item Category",itemValues.itemCategory);
		itemValues.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type",itemValues.itemType);
		itemValues.description = EditorGUILayout.TextField("Description", itemValues.description);
		
		GUILayout.Space(10);

		GUILayout.Label("Basic Values", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		itemValues.cost = EditorGUILayout.IntField("Money Value", itemValues.cost);
		itemValues.maxCharge = EditorGUILayout.IntField("Max Charges", itemValues.maxCharge);
		GUILayout.EndHorizontal();
		itemValues.researchNeeded = EditorGUILayout.Toggle("Research needed", itemValues.researchNeeded);

		GUILayout.Space(10);

		if (itemValues.itemCategory != ItemCategory.CONSUME) {
			GUILayout.Label("Weapon Power", EditorStyles.boldLabel);
			itemValues.power = EditorGUILayout.IntField("Weapon Power", itemValues.power);
			itemValues.hitRate = EditorGUILayout.IntField("Hit Rate", itemValues.hitRate);
			itemValues.critRate = EditorGUILayout.IntField("Crit Rate", itemValues.critRate);
			GUILayout.BeginHorizontal();
			itemValues.range.min = EditorGUILayout.IntField("Min Range", itemValues.range.min);
			itemValues.range.max = EditorGUILayout.IntField("Max Range", itemValues.range.max);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10);

			GUILayout.Label("Requirements", EditorStyles.boldLabel);
			itemValues.skillReq = EditorGUILayout.IntField("Skill Requirement", itemValues.skillReq);
			itemValues.weight = EditorGUILayout.IntField("Item Weight", itemValues.weight);
			
			GUILayout.Space(10);

			GUILayout.Label("Advantage Types", EditorStyles.boldLabel);
			for (int i = 0; i < itemValues.advantageType.Count; i++) {
				GUILayout.BeginHorizontal();
				itemValues.advantageType[i] = (ClassType)EditorGUILayout.EnumPopup("",itemValues.advantageType[i]);
				if (GUILayout.Button("X", GUILayout.Width(50))) {
					itemValues.advantageType.RemoveAt(i);
					i--;
				}
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("+")) {
				itemValues.advantageType.Add(ClassType.NONE);
			}
		}

		GUILayout.Label("Boost", EditorStyles.boldLabel);
		itemValues.boost.hp = EditorGUILayout.IntField("Boost HP", itemValues.boost.hp);
		itemValues.boost.atk = EditorGUILayout.IntField("Boost Atk", itemValues.boost.atk);
		itemValues.boost.spd = EditorGUILayout.IntField("Boost Spd", itemValues.boost.spd);
		itemValues.boost.skl = EditorGUILayout.IntField("Boost Skl", itemValues.boost.skl);
		itemValues.boost.lck = EditorGUILayout.IntField("Boost Lck", itemValues.boost.lck);
		itemValues.boost.def = EditorGUILayout.IntField("Boost Def", itemValues.boost.def);
		itemValues.boost.res = EditorGUILayout.IntField("Boost Res", itemValues.boost.res);

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void SelectItem() {
		GUI.FocusControl(null);
		if (selItem == -1) {
			// Nothing selected
			itemValues.ResetValues();
		}
		else {
			// Something selected
			ItemEntry ce = (ItemEntry)itemLibrary.GetEntryByIndex(selItem);
			itemValues.CopyValues(ce);
		}
	}

	void SaveSelectedItem() {
		ItemEntry ce = (ItemEntry)itemLibrary.GetEntryByIndex(selItem);
		ce.CopyValues(itemValues);
		Undo.RecordObject(ce, "Updated item");
		EditorUtility.SetDirty(ce);
	}

	void InstansiateItem() {
		GUI.FocusControl(null);
		if (itemLibrary.ContainsID(uuid)) {
			Debug.Log("uuid already exists!");
			return;
		}
		ItemEntry c = Editor.CreateInstance<ItemEntry>();
		c.name = uuid;
		c.uuid = uuid;
		c.entryName = uuid;
		c.repColor = repColor;
		string path = "Assets/LibraryData/Items/" + uuid + ".asset";

		AssetDatabase.CreateAsset(c, path);
		itemLibrary.InsertEntry(c, 0);
		Undo.RecordObject(itemLibrary, "Added item");
		EditorUtility.SetDirty(itemLibrary);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = itemLibrary.GetRepresentations("",filterStr);
		uuid = "";
		selItem = 0;
		SelectItem();
	}

	void DeleteItem() {
		GUI.FocusControl(null);
		ItemEntry c = (ItemEntry)itemLibrary.GetEntryByIndex(selItem);
		string path = "Assets/LibraryData/Items/" + c.uuid + ".asset";

		itemLibrary.RemoveEntryByIndex(selItem);
		Undo.RecordObject(itemLibrary, "Deleted item");
		EditorUtility.SetDirty(itemLibrary);
		bool res = AssetDatabase.MoveAssetToTrash(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		currentEntryList = itemLibrary.GetRepresentations("",filterStr);

		if (res) {
			Debug.Log("Removed item: " + c.uuid);
			selItem = -1;
		}
	}
}
