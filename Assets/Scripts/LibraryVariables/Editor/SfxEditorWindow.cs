using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SfxEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "SFX";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<SfxEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.8f, 0.2f);


	/// <summary>
	/// Constructor for the editor window.
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public SfxEditorWindow(ScrObjLibraryVariable entries, SfxEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		SfxEntry sfxValues = (SfxEntry)entryValues;
		sfxValues.entryName = EditorGUILayout.TextField("Name", sfxValues.entryName);
		sfxValues.clip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", sfxValues.clip, typeof(AudioClip), false);
	}
}
