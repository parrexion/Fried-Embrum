using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicLibraryEditorWindow : EditorWindow {

	private enum State { MUSIC = 0, SFX = 1, SONGLIST = 2 }

	// Header
	Rect headerRect = new Rect();
	Texture2D headerTex;

	public IntVariable currentWindow;

	public MusicEditorWindow musicEditor;
	public ScrObjLibraryVariable musicLibrary;
	public MusicEntry musicContainer;

	public SfxEditorWindow sfxEditor;
	public ScrObjLibraryVariable sfxLibrary;
	public SfxEntry sfxContainer;

	public SonglistEditorWindow songlistEditor;
	public ScrObjLibraryVariable musicListLibrary;
	public MusicSetEntry musicListContainer;
	public ScrObjLibraryVariable sfxListLibrary;
	public SfxList sfxListContainer;

	private string[] toolbarStrings = new string[] { "Music", "SFX", "Songlist" };


	[MenuItem("Window/Sound Editor")]
	public static void ShowWindow() {
		GetWindow<MusicLibraryEditorWindow>("Sound Editor");
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

		switch((State)currentWindow.value) {
			case State.MUSIC:
				musicEditor.DrawWindow(width, height);
				break;
			case State.SFX:
				sfxEditor.DrawWindow(width, height);
				break;
			case State.SONGLIST:
				songlistEditor.DrawWindow(width, height);
				break;
		}
	}


	/// <summary>
	/// Makes sure the window stays open when switching scenes.
	/// </summary>
	/// <param name="_scene"></param>
	/// <param name="_mode"></param>
	void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode) {
		InitializeWindow();
	}

	/// <summary>
	/// Loads all the libraries for the editors.
	/// </summary>
	void LoadLibraries() {
		musicEditor = new MusicEditorWindow(musicLibrary, musicContainer);
		sfxEditor = new SfxEditorWindow(sfxLibrary, sfxContainer);
		songlistEditor = new SonglistEditorWindow(musicListLibrary, musicListContainer);

		InitializeWindow();
	}

	/// <summary>
	/// Initializes all the window specific variables.
	/// </summary>
	void InitializeWindow() {
		headerTex = new Texture2D(1, 1);
		headerTex.SetPixel(0, 0, new Color(0.5f, 0.2f, 0.8f));
		headerTex.Apply();

		musicEditor.InitializeWindow();
		songlistEditor.InitializeWindow();
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
}
