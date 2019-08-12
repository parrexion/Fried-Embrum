using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MusicEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Music";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<MusicEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.8f, 0.2f);


	/// <summary>
	/// Constructor for the editor window.
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public MusicEditorWindow(ScrObjLibraryVariable entries, MusicEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		MusicEntry musicValues = (MusicEntry)entryValues;

		musicValues.entryName = EditorGUILayout.TextField("Name", musicValues.entryName);
		musicValues.clip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", musicValues.clip, typeof(AudioClip), false);
	}

}
