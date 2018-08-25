using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { FIELD, FOREST, MOUNTAIN, WALL }

[System.Serializable]
public class RoughnessTuple {
	public ClassType type;
	public int roughness;
}

[CreateAssetMenu(menuName = "LibraryEntries/TerrainTile")]
public class TerrainTile : ScriptableObject {

	public Sprite sprite;
	public Color tint = Color.white;

	public int defense;
	public int avoid;

	public RoughnessTuple[] canMoveTypes;
}
