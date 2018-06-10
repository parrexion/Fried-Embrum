using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuScript {

	[MenuItem("Tools/Assign Material")]
	public static void AssignTileMaterial() {
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
		Material material = Resources.Load<Material>("Grid_sprite");

		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].GetComponent<Renderer>().material = material;
		}
	}

	[MenuItem("Tools/Assign Script")]
	public static void AssignScript() {
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

		for (int i = 0; i < tiles.Length; i++) {
			tiles[i].AddComponent<MapTile>();
		}
	}
}
