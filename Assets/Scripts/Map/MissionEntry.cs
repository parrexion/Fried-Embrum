using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Mission")]
public class MissionEntry : ScrObjLibraryEntry {

	public enum Unlocking { TIME, DEATH, RECRUITED, MISSION }

	[System.Serializable]
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
	public Unlocking unlockReq;
	public int unlockDay = 1;
	public CharData characterReq;
	public MissionEntry clearedMission;

	[Header("Reward")]
	public Reward reward = new Reward();


	public override void ResetValues() {
		base.ResetValues();
		mapLocation = MapLocation.UNKNOWN;
		mapDescription = "";

		squads = new List<SquadGroup>();
		maps = new List<MapEntry>();
		
		duration = 1;
		unlockReq = Unlocking.TIME;
		unlockDay = 1;
		characterReq = null;
		clearedMission = null;

		reward = new Reward();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
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
		unlockReq = mission.unlockReq;
		unlockDay = mission.unlockDay;
		characterReq = mission.characterReq;
		clearedMission = mission.clearedMission;

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

	public int Squad1Size() {
		int smallest = 100;
		for (int i = 0; i < squads.Count; i++) {
			if (squads[i].squad1Size > 0)
				smallest = Mathf.Min(smallest, squads[i].squad1Size);
		}
		return (smallest == 100) ? 0 : smallest;
	}

	public int Squad2Size() {
		int smallest = 100;
		for (int i = 0; i < squads.Count; i++) {
			if (squads[i].squad2Size > 0)
				smallest = Mathf.Min(smallest, squads[i].squad2Size);
		}
		return (smallest == 100) ? 0 : smallest;
	}

	public bool IsCharacterForced(CharData character) {
		for (int i = 0; i < maps.Count; i++) {
			if (maps[i].IsForced(character))
				return true;
		}
		return false;
	}

	public bool IsCharacterLocked(CharData character) {
		for (int i = 0; i < maps.Count; i++) {
			if (maps[i].IsLocked(character))
				return true;
		}
		return false;
	}
}
