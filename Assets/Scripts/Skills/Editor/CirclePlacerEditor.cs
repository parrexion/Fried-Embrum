using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CirclePlacer))]
public class CirclePlacerEditor : Editor {


	public override void OnInspectorGUI() {
		
		if (GUILayout.Button("Update placements")) {
			CirclePlacer circlePlacer = (CirclePlacer)target;
			circlePlacer.UpdatePlacements();
		}

		GUILayout.Space(10);

		DrawDefaultInspector();
	}

}
