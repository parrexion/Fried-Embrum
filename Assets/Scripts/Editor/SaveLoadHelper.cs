using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveLoadHelper : MonoBehaviour {

	[MenuItem("SaveLoad/Save", false, 51)]
	private static void SaveGame() {
		string[] guids = AssetDatabase.FindAssets("SaveGameEvent");
		string result = AssetDatabase.GUIDToAssetPath(guids[0]);
		GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
		ge.Raise();
	}

	[MenuItem("SaveLoad/Load", false, 52)]
	private static void LoadGame() {
		string[] guids = AssetDatabase.FindAssets("LoadGameEvent");
		string result = AssetDatabase.GUIDToAssetPath(guids[0]);
		GameEvent ge = (GameEvent)AssetDatabase.LoadAssetAtPath(result, typeof(GameEvent));
		ge.Raise();
	}

	//

	[MenuItem("SaveLoad/Delete Savedata", false, 100)]
	private static void DeleteSave() {
		if (EditorUtility.DisplayDialog("Delete savedata", "Are you sure?", "Absolutely", "Nope")) {
			FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/saveData2.xml");
			Debug.Log("Deleted savedata");
		}
	}
}
