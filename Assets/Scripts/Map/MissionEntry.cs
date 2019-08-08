using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Mission")]
public class MissionEntry : ScrObjLibraryEntry {

	public class SquadGroup {
		public int squad1Size;
		public int squad2Size;
	}
	
	[Header("Info")]
	public MapLocation mapLocation = MapLocation.UNKNOWN;
	public string mapDescription;

	[Header("Maps")]
	public List<MapEntry> maps = new List<MapEntry>();
	public List<SquadGroup> squads = new List<SquadGroup>();
	
	[Header("Mission Unlock")]
	public int duration = 1;
	public int unlockDay = 1;

	[Header("Reward")]
	public Reward reward = new Reward();


	public override void ResetValues() {
		mapLocation = MapLocation.UNKNOWN;
		mapDescription = "";

		squads = new List<SquadGroup>();
		maps = new List<MapEntry>();
		
		duration = 1;
		unlockDay = 1;

		reward = new Reward();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		MissionEntry mission = (MissionEntry)other;

		mapLocation = mission.mapLocation;
		mapDescription = mission.mapDescription;

		maps = new List<MapEntry>();
		for(int i = 0; i < mission.maps.Count; i++) {
			maps.Add(mission.maps[i]);
		}
		squads = new List<SquadGroup>();
		for(int i = 0; i < mission.squads.Count; i++) {
			squads.Add(mission.squads[i]);
		}
		
		duration = mission.duration;
		unlockDay = mission.unlockDay;

		reward = mission.reward;
	}

	public void AddMap() {
		maps.Add(null);
		squads.Add(new SquadGroup());
	}

	public void RemoveMap(int index) {
		maps.RemoveAt(index);
		squads.RemoveAt(index);
	}

}
