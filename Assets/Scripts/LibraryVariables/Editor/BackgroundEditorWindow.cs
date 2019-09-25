using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BackgroundEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Background";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<BackgroundEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.5f, 0.8f);


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public BackgroundEditorWindow(ScrObjLibraryVariable entries, BackgroundEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		BackgroundEntry backgroundValues = (BackgroundEntry)entryValues;
		
		backgroundValues.sprite = (Sprite)EditorGUILayout.ObjectField("Image", backgroundValues.sprite, typeof(Sprite), false);

	}

}
