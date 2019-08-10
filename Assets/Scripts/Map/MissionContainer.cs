using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionProgress {

	public string uuid;
	public bool cleared;


	public MissionProgress(string missionUuid, bool cleared = false) {
		this.uuid = missionUuid;
		this.cleared = cleared;
	}
}
