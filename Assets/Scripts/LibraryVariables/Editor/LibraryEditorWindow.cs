using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LibraryEditorWindow : EditorWindow {

	private enum State { MAPS, CHARACTERS, CLASSES, ITEMS, SKILLS, BACKGROUND, PORTRAITS }

	// Header
	Rect headerRect = new Rect();
	Texture2D headerTex;
	static GUIStyle horizontalLine;

	public IntVariable currentWindow;

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
	public WeaponItem itemContainer;

	public ItemEditorWindow skillEditor;
	public ScrObjLibraryVariable skillLibrary;
	public CharacterSkill skillContainer;

	public BackgroundEditorWindow backgroundEditor;
	public ScrObjLibraryVariable backgroundLibrary;
	public BackgroundEntry backgroundContainer;

	public PortraitEditorWindow portraitEditor;
	public ScrObjLibraryVariable portraitLibrary;
	public PortraitEntry portraitContainer;
	public SpriteListVariable poseList;

	private string[] toolbarStrings = new string[] {"Maps", "Characters", "Classes", "Items", "Skills", "Background", "Portraits"};


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
		
		switch ((State)currentWindow.value)
		{
			case State.MAPS:
				mapEditor.DrawWindow();
				break;
			case State.CHARACTERS:
				characterEditor.DrawWindow();
				break;
			case State.CLASSES:
				classEditor.DrawWindow();
				break;
			case State.ITEMS:
				itemEditor.DrawWindow();
				break;
			case State.SKILLS:
				// skillEditor.DrawWindow();
				break;
			case State.BACKGROUND:
				backgroundEditor.DrawWindow();
				break;
			case State.PORTRAITS:
				portraitEditor.DrawWindow();
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
		mapEditor = new MapEditorWindow(mapLibrary, mapContainer);
		characterEditor = new CharacterEditorWindow(characterLibrary, characterContainer);
		classEditor = new ClassEditorWindow(classLibrary, classContainer);
		itemEditor = new ItemEditorWindow(itemLibrary, itemContainer);
		// skillEditor = new ItemEditorWindow(itemLibrary, skillContainer);
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
		horizontalLine.margin = new RectOffset( 0, 0, 4, 4 );
		horizontalLine.fixedHeight = 1;

		mapEditor.InitializeWindow();
		characterEditor.InitializeWindow();
		classEditor.InitializeWindow();
		itemEditor.InitializeWindow();
		backgroundEditor.InitializeWindow();
		portraitEditor.InitializeWindow();
	}

	/// <summary>
	/// Draws the header for the editor.
	/// </summary>
	void DrawHeader() {
		headerRect.x = 0;
		headerRect.y = 0;
		headerRect.width = Screen.width;
		headerRect.height = 50;
		GUI.DrawTexture(headerRect, headerTex);

		currentWindow.value = GUILayout.Toolbar(currentWindow.value, toolbarStrings);
	}

	// utility method for drawing lines
	public static void HorizontalLine ( Color color ) {
		var c = GUI.color;
		GUI.color = color;
		GUILayout.Box( GUIContent.none, horizontalLine );
		GUI.color = c;
	}
}
