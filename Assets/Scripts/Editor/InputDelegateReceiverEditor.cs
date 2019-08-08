using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.Events;

[CustomEditor(typeof(InputReceiverDelegate), true)]
public class InputDelegateReceiverEditor : Editor {

	public override void OnInspectorGUI() {
		InputReceiverDelegate ird = (InputReceiverDelegate)target;

		int sum = ird.menuAcceptEvent.GetPersistentEventCount() * ird.menuBackEvent.GetPersistentEventCount() *
					ird.menuMoveEvent.GetPersistentEventCount() * ird.menuFailEvent.GetPersistentEventCount();

		if (sum == 0) {
			if (GUILayout.Button("Assign Menu Events")) {
				if (ird.menuAcceptEvent.GetPersistentEventCount() == 0) {
					Undo.RecordObject(target, "Added listener");
					string[] guids = AssetDatabase.FindAssets("MenuAcceptEvent");
					string result = AssetDatabase.GUIDToAssetPath(guids[0]);
					Debug.Log(result);
					GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
					UnityEventTools.AddVoidPersistentListener(ird.menuAcceptEvent, ge.Raise);
				}
				if (ird.menuBackEvent.GetPersistentEventCount() == 0) {
					Undo.RecordObject(target, "Added listener");
					string[] guids = AssetDatabase.FindAssets("MenuBackEvent");
					string result = AssetDatabase.GUIDToAssetPath(guids[0]);
					Debug.Log(result);
					GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
					UnityEventTools.AddVoidPersistentListener(ird.menuBackEvent, ge.Raise);
				}
				if (ird.menuMoveEvent.GetPersistentEventCount() == 0) {
					Undo.RecordObject(target, "Added listener");
					string[] guids = AssetDatabase.FindAssets("MenuMoveEvent");
					string result = AssetDatabase.GUIDToAssetPath(guids[0]);
					Debug.Log(result);
					GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
					UnityEventTools.AddVoidPersistentListener(ird.menuMoveEvent, ge.Raise);
				}
				if (ird.menuFailEvent.GetPersistentEventCount() == 0) {
					Undo.RecordObject(target, "Added listener");
					string[] guids = AssetDatabase.FindAssets("MenuFailEvent");
					string result = AssetDatabase.GUIDToAssetPath(guids[0]);
					Debug.Log(result);
					GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
					UnityEventTools.AddVoidPersistentListener(ird.menuFailEvent, ge.Raise);
				}
			}
		}

		DrawDefaultInspector();
	}
}
