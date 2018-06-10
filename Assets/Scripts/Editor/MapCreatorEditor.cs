using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor {

	Texture2D texMap = null;

	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		GUILayout.Space(10);

		MapCreator myTarget = (MapCreator)target;

		if (GUILayout.Button("Generate map links")) {
			myTarget.GenerateLinks();
			EditorUtility.SetDirty(myTarget);
			EditorSceneManager.MarkSceneDirty(myTarget.gameObject.scene);
		}

		GUILayout.Space(20);

		texMap = (Texture2D)EditorGUILayout.ObjectField(texMap, typeof(Texture2D),false);
		if (GUILayout.Button("Generate map terrain")) {
			myTarget.GenerateMap(texMap);
			EditorUtility.SetDirty(myTarget);
			EditorSceneManager.MarkSceneDirty(myTarget.gameObject.scene);
		}
	}
	
}
