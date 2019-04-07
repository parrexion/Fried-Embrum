using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionContainer {

	public MapEntry map;
	public bool cleared;


	public MissionContainer(MapEntry map, bool cleared = false) {
		this.map = map;
		this.cleared = cleared;
	}
}
