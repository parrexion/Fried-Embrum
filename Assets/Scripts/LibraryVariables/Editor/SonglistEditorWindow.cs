using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SonglistEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "MusicSet";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<MusicSetEntry>();
	protected override Color BackgroundColor => new Color(0.4f, 0.6f, 0.8f);


	/// <summary>
	/// Constructor for the editor window.
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public SonglistEditorWindow(ScrObjLibraryVariable entries, MusicSetEntry container){
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		MusicSetEntry musicValues = (MusicSetEntry)entryValues;
		GUILayout.Label("Music set", EditorStyles.boldLabel);

		GUILayout.Label("Overworld Themes", EditorStyles.boldLabel);
		musicValues.playerTheme = (MusicEntry)EditorGUILayout.ObjectField("Player Theme", musicValues.playerTheme, typeof(MusicEntry), false);
		musicValues.enemyTheme = (MusicEntry)EditorGUILayout.ObjectField("Enemy Theme", musicValues.enemyTheme, typeof(MusicEntry), false);

		GUILayout.Label("Battle Themes", EditorStyles.boldLabel);
		musicValues.battleTheme = (MusicEntry)EditorGUILayout.ObjectField("Battle Theme", musicValues.battleTheme, typeof(MusicEntry), false);
		musicValues.healTheme = (MusicEntry)EditorGUILayout.ObjectField("Heal Theme", musicValues.healTheme, typeof(MusicEntry), false);
	}
	
}
