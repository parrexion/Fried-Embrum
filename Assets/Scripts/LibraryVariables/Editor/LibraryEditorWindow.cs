using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LibraryEditorWindow : EditorWindow {

	private enum State { MISSIONS, MAPS, CHARACTERS, CLASSES, ITEMS, UPGRADE, SKILLS, BACKGROUND, PORTRAITS }

	// Header
	Rect headerRect = new Rect();
	Texture2D headerTex;
	static GUIStyle horizontalLine;
	static GUIStyle redText;

	public IntVariable currentWindow;

	public MissionEditorWindow missionEditor;
	public ScrObjLibraryVariable missionLibrary;
	public MissionEntry missionContainer;

	public MapEditorWindow mapEditor;
	public ScrObjLibraryVariable mapLibrary;
	public MapEntry mapContainer;

	public CharacterEditorWindow characterEditor;
	public ScrObjLibraryVariable characterLibrary;
	public CharData characterContainer;

	public ClassEditorWindow classEditor;
	public ScrObjLibraryVariable classLibrary;
	public CharClass classContainer;

	public ItemEditorWindow itemEditor;
	public ScrObjLibraryVariable itemLibrary;
	public ItemEntry itemContainer;

	public UpgradeEditorWindow upgradeEditor;
	public ScrObjLibraryVariable upgradeLibrary;
	public UpgradeEntry upgradeContainer;

	public SkillEditorWindow skillEditor;
	public ScrObjLibraryVariable skillLibrary;
	public CharacterSkill skillContainer;

	public BackgroundEditorWindow backgroundEditor;
	public ScrObjLibraryVariable backgroundLibrary;
	public BackgroundEntry backgroundContainer;

	public PortraitEditorWindow portraitEditor;
	public ScrObjLibraryVariable portraitLibrary;
	public PortraitEntry portraitContainer;
	public SpriteListVariable poseList;

	private string[] toolbarStrings = new string[] { "Missions", "Maps", "Characters", "Classes", "Items", "Upgrades", "Skills", "Background", "Portraits" };


	[MenuItem("Window/LibraryEditor")]
	public static void ShowWindow() {
		GetWindow<LibraryEditorWindow>("Library Editor");
	}

	void OnEnable() {
		EditorSceneManager.sceneOpened += SceneOpenedCallback;
		LoadLibraries();
	}

	void OnDisable() {
		EditorSceneManager.sceneOpened -= SceneOpenedCallback;
	}

	/// <summary>
	/// Renders the selected window.
	/// </summary>
	void OnGUI() {
		DrawHeader();
		int width = (int)position.width;
		int height = (int)position.height;
		switch ((State)currentWindow.value) {
			case State.MISSIONS:
				missionEditor.DrawWindow(width, height);
				break;
			case State.MAPS:
				mapEditor.DrawWindow(width, height);
				break;
			case State.CHARACTERS:
				characterEditor.DrawWindow(width, height);
				break;
			case State.CLASSES:
				classEditor.DrawWindow(width, height);
				break;
			case State.ITEMS:
				itemEditor.DrawWindow(width, height);
				break;
			case State.UPGRADE:
				upgradeEditor.DrawWindow(width, height);
				break;
			case State.SKILLS:
				skillEditor.DrawWindow(width, height);
				break;
			case State.BACKGROUND:
				backgroundEditor.DrawWindow(width, height);
				break;
			case State.PORTRAITS:
				portraitEditor.DrawWindow(width, height);
				break;
		}
	}


	/// <summary>
	/// Makes sure the window stays open when switching scenes.
	/// </summary>
	/// <param name="_scene"></param>
	/// <param name="_mode"></param>
	void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode) {
		Debug.Log("SCENE LOADED");
		InitializeWindow();
	}

	/// <summary>
	/// Loads all the libraries for the editors.
	/// </summary>
	void LoadLibraries() {
		missionEditor = new MissionEditorWindow(missionLibrary, missionContainer);
		mapEditor = new MapEditorWindow(mapLibrary, mapContainer);
		characterEditor = new CharacterEditorWindow(characterLibrary, characterContainer);
		classEditor = new ClassEditorWindow(classLibrary, classContainer);
		itemEditor = new ItemEditorWindow(itemLibrary, itemContainer);
		upgradeEditor = new UpgradeEditorWindow(upgradeLibrary, upgradeContainer);
		skillEditor = new SkillEditorWindow(skillLibrary, skillContainer);
		backgroundEditor = new BackgroundEditorWindow(backgroundLibrary, backgroundContainer);
		portraitEditor = new PortraitEditorWindow(portraitLibrary, portraitContainer, poseList);

		InitializeWindow();
	}

	/// <summary>
	/// Initializes all the window specific variables.
	/// </summary>
	void InitializeWindow() {
		headerTex = new Texture2D(1, 1);
		headerTex.SetPixel(0, 0, new Color(0.5f, 0.2f, 0.8f));
		headerTex.Apply();

		// divider style
		horizontalLine = new GUIStyle();
		horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
		horizontalLine.margin = new RectOffset(0, 0, 4, 4);
		horizontalLine.fixedHeight = 1;

		missionEditor.InitializeWindow();
		mapEditor.InitializeWindow();
		characterEditor.InitializeWindow();
		classEditor.InitializeWindow();
		itemEditor.InitializeWindow();
		upgradeEditor.InitializeWindow();
		skillEditor.InitializeWindow();
		backgroundEditor.InitializeWindow();
		portraitEditor.InitializeWindow();
	}

	/// <summary>
	/// Draws the header for the editor.
	/// </summary>
	void DrawHeader() {
		if (headerTex == null) {
			InitializeWindow();
		}
		headerRect.x = 0;
		headerRect.y = 0;
		headerRect.width = Screen.width;
		headerRect.height = 50;
		GUI.DrawTexture(headerRect, headerTex);

		currentWindow.value = GUILayout.Toolbar(currentWindow.value, toolbarStrings);
	}

	// utility method for drawing lines
	public static void HorizontalLine(Color color) {
		var c = GUI.color;
		GUI.color = color;
		GUILayout.Box(GUIContent.none, horizontalLine);
		GUI.color = c;
	}

	public static GUIStyle RedField() {
		if (redText != null) {
			return redText;
		}

		redText = EditorStyles.objectField;
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, Color.red);
		redText.normal.background = texture;
		return redText;
	}
}
