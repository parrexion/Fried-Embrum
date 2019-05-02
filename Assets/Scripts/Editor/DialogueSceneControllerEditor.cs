using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueSceneController))]
public class DialogueSceneControllerEditor : Editor {

	
	public override void OnInspectorGUI() {

		if (GUILayout.Button("Show dialogue GUI")) {
			DialogueSceneController dsc = (DialogueSceneController)target;
			dsc.ActivateStuff(true);
		}
		if (GUILayout.Button("Hide dialogue GUI")) {
			DialogueSceneController dsc = (DialogueSceneController)target;
			dsc.ActivateStuff(false);
		}

		DrawDefaultInspector();
	}

}
