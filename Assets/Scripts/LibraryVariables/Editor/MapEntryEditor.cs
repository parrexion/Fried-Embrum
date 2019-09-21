using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapEntry))]
public class MapEntryEditor : Editor {

	public override void OnInspectorGUI() {
		
		if (GUILayout.Button("Set Current Map")) {
			MapEntry map = (MapEntry)target;
			string[] guid = AssetDatabase.FindAssets("~CurrentMap~");
			if (guid.Length > 0) {
				string path = AssetDatabase.GUIDToAssetPath(guid[0]);
				ScrObjEntryReference idVariable = AssetDatabase.LoadAssetAtPath<ScrObjEntryReference>(path);
				idVariable.value = map;
				Debug.Log("Set Map ID to:  " + map.uuid);
			}
		}

		GUILayout.Space(20);

		DrawDefaultInspector();
	}
}
