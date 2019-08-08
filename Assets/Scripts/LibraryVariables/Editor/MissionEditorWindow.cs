using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissionEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Mission";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<MissionEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.3f, 0.3f, 1);


	public MissionEditorWindow(ScrObjLibraryVariable entries, MissionEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		MissionEntry missionValues = (MissionEntry)entryValues;
		
	}

}
