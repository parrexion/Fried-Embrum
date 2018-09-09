using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/MapTileList")]
public class MapTileListVariable : ScriptableObject {

	public List<MapTile> values = new List<MapTile>();
}
