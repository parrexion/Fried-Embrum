using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoints : MonoBehaviour {
    
	public Transform[] mapLocations;
	public IntVariable locationPointIndex;
	public PlayerData playerData;
	public IntVariable currentDay;
	private SpriteRenderer[] highlights = new SpriteRenderer[0];

	private void Start() {
		highlights = new SpriteRenderer[mapLocations.Length];
		for (int i = 0; i < mapLocations.Length; i++) {
			highlights[i] = mapLocations[i].GetChild(0).GetComponent<SpriteRenderer>();
		}
		//ResetMap();
		//for (int i = 0; i < playerData.missions.Count; i++) {
		//	if (playerData.missions[i].cleared || currentDay.value < playerData.missions[i].map.unlockDay)
		//		continue;
		//	SetLocation(playerData.missions[i].map.mapLocation, true);
		//}
		ChangePoint();
	}

	//public void ResetMap() {
	//	for (int i = 0; i < mapLocations.Length; i++) {
	//		mapLocations[i].gameObject.SetActive(false);
	//	}
	//}

	//public void SetLocation(MapLocation location, bool state) {
	//	int index = (int)location -1;
	//	if (index < 0)
	//		return;

	//	mapLocations[index].gameObject.SetActive(state);
	//}

	public void ChangePoint() {
		for (int i = 0; i < highlights.Length; i++) {
			highlights[i].enabled = (i == locationPointIndex.value);
		}
	}
}
