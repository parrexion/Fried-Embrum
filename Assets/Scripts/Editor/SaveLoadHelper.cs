using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveLoadHelper : MonoBehaviour {

	[MenuItem("SaveLoad/Save")]
	private static void SaveGame() {
		string[] guids = AssetDatabase.FindAssets("SaveGameEvent");
		string result = AssetDatabase.GUIDToAssetPath(guids[0]);
		GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
		ge.Raise();
	}

	[MenuItem("SaveLoad/Load")]
	private static void LoadGame() {
		string[] guids = AssetDatabase.FindAssets("LoadGameEvent");
		string result = AssetDatabase.GUIDToAssetPath(guids[0]);
		GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
		ge.Raise();
	}

	[MenuItem("SaveLoad/Delete Savedata")]
	private static void DeleteSave() {
		if (EditorUtility.DisplayDialog("Delete savedata", "Are you sure?", "Absolutely", "Nope")) {
			FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/saveData2.xml");
			Debug.Log("Deleted savedata");
		}
	}
}
