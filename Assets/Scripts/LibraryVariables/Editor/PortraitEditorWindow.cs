using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PortraitEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Portrait";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<PortraitEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.5f, 0.2f);

	public SpriteListVariable poseLibrary;


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public PortraitEditorWindow(ScrObjLibraryVariable entries, PortraitEntry container, SpriteListVariable poses) {
		entryLibrary = entries;
		entryValues = container;
		poseLibrary = poses;
		LoadLibrary();
	}


	protected override void DrawContentWindow() {
		PortraitEntry portraitValues = (PortraitEntry)entryValues;

		portraitValues.small = (Sprite)EditorGUILayout.ObjectField("Small character portrait", portraitValues.small, typeof(Sprite), false);

		GUILayout.Space(20);

		if(portraitValues.poses.Length < (int)PortraitEntry.Pose.kCount) {
			System.Array.Resize(ref portraitValues.poses, (int)PortraitEntry.Pose.kCount);
		}

		// Poses
		GUILayout.Label("Poses", EditorStyles.boldLabel);
		for(int i = 0; i < portraitValues.poses.Length; i++) {
			if(portraitValues.poses[i] == null)
				portraitValues.poses[i] = (Sprite)EditorGUILayout.ObjectField(poseLibrary.values[i].name, poseLibrary.values[i], typeof(Sprite), false);
			else
				portraitValues.poses[i] = (Sprite)EditorGUILayout.ObjectField(poseLibrary.values[i].name, portraitValues.poses[i], typeof(Sprite), false);
		}
	}

}
