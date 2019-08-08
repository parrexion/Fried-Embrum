using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionContainer {

	public MissionEntry mission;
	public bool cleared;


	public MissionContainer(MissionEntry mission, bool cleared = false) {
		this.mission = mission;
		this.cleared = cleared;
	}
}
