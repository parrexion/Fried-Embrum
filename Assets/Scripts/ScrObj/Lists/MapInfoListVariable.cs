using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/MapInfoList")]
public class MapInfoListVariable : ScriptableObject {

	public List<MapInfo> values = new List<MapInfo>();
}
