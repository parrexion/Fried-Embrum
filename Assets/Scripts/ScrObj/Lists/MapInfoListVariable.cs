using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/MapInfoList")]
public class MapInfoListVariable : ScriptableObject {

	public List<MapEntry> values = new List<MapEntry>();
}
