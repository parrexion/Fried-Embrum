using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FilterType { DEFAULT, PRELUDE, INTRO, ENDING, QUOTE, VILLAGE, DEATH, EVENT }

public class DialogueListWindow : EditorWindow {

	const int DIALOGUE_HEIGHT = 350;

	static DialogueListWindow dlw;
	static DialogueActionWindow daw;
	public DialogueHub hub;

	public ScrObjLibraryVariable backgroundLibrary;
	public ScrObjLibraryVariable characterLibrary;
	public ScrObjLibraryVariable dialogueLibrary;
	public DialogueEntry dialogueValues;

	public Texture2D dialogueBackground;
	public Texture2D frameBackground;

	public Color headerColor = new Color(0.25f,0.5f,0.75f);
	public Color charactersColor = new Color(0.35f, 0.75f, 0.35f);
	public Color charactersColor2 = new Color(0.15f, 0.55f, 0.15f);
	public Color talkingColor = new Color(0.7f, 0.7f, 0.7f);
	public Color dialogueColor = new Color(0.5f, 0.5f, 0.5f);
	public Color frameColor = new Color(0.6f, 0.4f, 0.3f);
	public Color nextColor = new Color(0.4f, 0.3f, 0.6f);
	public Color soundColor = new Color(0.8f, 0.8f, 0.2f);
	public Color effectsColor = new Color(0.15f, 0.8f, 0.8f);
	public Color moveColor = new Color(0.75f, 0.3f, 0.5f);
	public Color rightColor = new Color(0.4f, 0.6f, 0f);

	public Rect dialogueRect = new Rect();
	public Rect actionListRect = new Rect();
	public Rect stateRect = new Rect();
	public Rect actionRect = new Rect();

	//Private stuff
	FilterType filter;
	string filterEnum;
	string filterString;
	Vector2 frameScrollPos;
	Vector2 dialogueScrollPos;


	[MenuItem("Window/Dialogue Editor 2.0")]
	public static void ShowWindow() {
		dlw = GetWindow<DialogueListWindow>("Dialogue List Window");
	}

	void OnEnable() {
		dlw = GetWindow<DialogueListWindow>("Dialogue List Window");
		EditorSceneManager.sceneOpened += SceneOpenedCallback;
		InitializeWindow();
	}

	void OnDisable() {
		EditorSceneManager.sceneOpened -= SceneOpenedCallback;
	}

	private void OnDestroy() {
		dlw = null;
		if (daw != null)
			daw.Close();
	}

	/// <summary>
	/// Re-initializes the editor when a new scene is loaded.
	/// </summary>
	/// <param name="_scene"></param>
	/// <param name="_mode"></param>
	void SceneOpenedCallback( Scene _scene, OpenSceneMode _mode) {
		Debug.Log("SCENE LOADED");
		InitializeWindow();
	}

	/// <summary>
	/// Initializes the variables needed by the editor.
	/// </summary>
	void InitializeWindow() {
		if (EditorApplication.isPlaying)
			return;

		backgroundLibrary.GenerateDictionary();
		characterLibrary.GenerateDictionary();
		dialogueLibrary.GenerateDictionary();

		if (hub == null)
			hub = new DialogueHub(backgroundLibrary, characterLibrary, dialogueLibrary, dialogueValues);
		if (dlw != null && daw == null) {
			daw = GetWindow<DialogueActionWindow>("Dialogue Editor New");
			daw.InitializeWindow(hub, this);
		}
		InitTextures();
		GenerateAreas();
	}

	private void InitTextures() {
		dialogueBackground = new Texture2D(1, 1);
		dialogueBackground.SetPixel(0, 0, dialogueColor);
		dialogueBackground.Apply();

		frameBackground = new Texture2D(1, 1);
		frameBackground.SetPixel(0, 0, frameColor);
		frameBackground.Apply();
	}

	private void GenerateAreas() {
		dialogueRect.x = 0;
		dialogueRect.y = 0;
		dialogueRect.width = position.width;
		dialogueRect.height = DIALOGUE_HEIGHT;

		actionListRect.x = 0;
		actionListRect.y = DIALOGUE_HEIGHT;
		actionListRect.width = position.width;
		actionListRect.height = position.height - DIALOGUE_HEIGHT;
	}

	private void OnGUI() {
		if (hub == null)
			InitializeWindow();
		if (hub.closeTime)
			Close();
		if (dialogueBackground == null) {
			InitTextures();
			daw.InitTextures();
		}

		UpdateRects();
		DrawBackgrounds();
		DrawDialogues();
		GUILayout.Space(10);
		DrawActionList();
	}

	private void UpdateRects() {
		dialogueRect.width = position.width;
		actionListRect.width = position.width;
		actionListRect.height = position.height - DIALOGUE_HEIGHT;
	}

	void DrawBackgrounds() {
		GUI.DrawTexture(dialogueRect, dialogueBackground);
		GUI.DrawTexture(actionListRect, frameBackground);
	}

	/// <summary>
	/// Renders the frames and dialogues available and dialogue creation.
	/// </summary>
	void DrawDialogues() {
		GUILayout.BeginArea(dialogueRect);
		EditorGUIUtility.labelWidth = 60;

		GUILayout.Label("Dialogues", EditorStyles.boldLabel);

		//Filter
		GUILayout.BeginHorizontal();
		filter = (FilterType)EditorGUILayout.EnumPopup("Filter",filter, GUILayout.Width(dialogueRect.width/2 - 10));
		filterEnum = (filter == FilterType.DEFAULT) ? "" : filter.ToString();
		filterString = EditorGUILayout.TextField("Search", filterString, GUILayout.Width(dialogueRect.width/2 - 10));
		GUILayout.EndHorizontal();

		// Dialogue scroll
		dialogueScrollPos = GUILayout.BeginScrollView(dialogueScrollPos, GUILayout.Width(dialogueRect.width), 
					GUILayout.Height(dialogueRect.height-90));
		
		int newSelected = hub.selDialogue;
		GUIContent[] guic = dialogueLibrary.GetRepresentations(filterEnum, filterString);
		if (guic.Length > 0)
			newSelected = GUILayout.SelectionGrid(hub.selDialogue, guic,2);

		if (newSelected != hub.selDialogue) {
			hub.SaveSelectedDialogue();
			hub.selDialogue = newSelected;
			GUI.FocusControl(null);
			hub.SelectDialogue();
			daw.Repaint();
			// daw.Focus();
		}

		GUILayout.EndScrollView();
		GUILayout.Space(5);

		//Dialogue creation
		EditorGUIUtility.labelWidth = 150;
		hub.dialogueUuid = EditorGUILayout.TextField("Create Dialogue - Uuid", hub.dialogueUuid);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create dialogue")) {
			hub.InstansiateDialogue();
		}
		EditorGUI.BeginDisabledGroup(true);
		if (GUILayout.Button("Copy dialogue")) {

		}
		EditorGUI.EndDisabledGroup();
		if (GUILayout.Button("Delete dialogue")) {
			hub.DeleteDialogue();
		}
		EditorGUIUtility.labelWidth = 120;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawActionList() {

		EditorGUIUtility.labelWidth = 120;
		GUILayout.BeginArea(actionListRect);

		EditorGUILayout.SelectableLabel("Selected Dialogue    UUID: " + hub.dialogueValues.uuid, EditorStyles.boldLabel, GUILayout.Height(20));
		if (hub.selAction != -1) {
			hub.dialogueValues.entryName = EditorGUILayout.TextField("Dialogue name", hub.dialogueValues.entryName, GUILayout.Width(400));
			hub.dialogueValues.tag = EditorGUILayout.EnumPopup("Tag", (FilterType)System.Enum.Parse(typeof(FilterType),hub.dialogueValues.tag)).ToString();
		}
		GUILayout.Space(5);

		//SAVE
		if (hub.selAction != -1) {
			if (GUILayout.Button("SAVE")) {
				hub.dialogueValues.repColor = hub.dialogueValues.GetTagColor();
				hub.SaveSelectedDialogue();
			}
			GUILayout.Space(5);
		}

		// Frame scroll
		GUILayout.Label("Actions", EditorStyles.boldLabel);
		frameScrollPos = GUILayout.BeginScrollView(frameScrollPos, GUILayout.Width(actionListRect.width), 
					GUILayout.Height(actionListRect.height - 116));
		if (hub.selAction != -1) {
			int oldFrame = hub.selAction;
			hub.selAction = GUILayout.SelectionGrid(hub.selAction, hub.dialogueValues.GenerateActionRepresentation(), 1);
			if (oldFrame != hub.selAction) {
				daw.Repaint();
				hub.UpdateToCurrentFrame();
			}
		}

		GUILayout.EndScrollView();

		GUILayout.EndArea();
	}
}