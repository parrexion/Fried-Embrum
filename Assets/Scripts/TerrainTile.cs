using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { FIELD, FOREST, MOUNTAIN, WALL }

[System.Serializable]
public class RoughnessTouple {
	public ClassType type;
	public int roughness;
}

[CreateAssetMenu]
public class TerrainTile : ScriptableObject {

	public Sprite sprite;
	public Color tint = Color.white;
	public RoughnessTouple[] canMoveTypes;
}
