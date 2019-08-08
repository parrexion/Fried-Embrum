using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionSaveData {

	public string id;
	public bool cleared;


	public void StoreData(MissionContainer mission) {
		id = mission.mission.uuid;
		cleared = mission.cleared;
	}
}
