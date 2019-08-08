using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Mission")]
public class MissionEntry : ScrObjLibraryEntry {

	public class SquadGroup {
		public int squadSize;
		public List<bool> available = new List<bool>();
	}

	[Header("Squads")]
	public List<SquadGroup> squadSizes = new List<SquadGroup>();

	[Header("Maps")]
	public List<MapEntry> maps = new List<MapEntry>();


	public override void ResetValues() {
		squadSizes = new List<SquadGroup>();
		maps = new List<MapEntry>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		MissionEntry mission = (MissionEntry)other;
		squadSizes = new List<SquadGroup>();
		for(int i = 0; i < mission.squadSizes.Count; i++) {
			squadSizes.Add(mission.squadSizes[i]);
		}
		maps = new List<MapEntry>();
		for(int i = 0; i < mission.maps.Count; i++) {
			maps.Add(mission.maps[i]);
		}
	}

	public void AddMap(MapEntry map) {
		maps.Add(map);
		for(int i = 0; i < squadSizes.Count; i++) {
			squadSizes[i].available.Add(false);
		}
	}

	public void RemoveMap(int index) {
		maps.RemoveAt(index);
		for(int i = 0; i < squadSizes.Count; i++) {
			squadSizes[i].available.RemoveAt(index);
		}
	}

	public void AddSquad() {
		squadSizes.Add(new SquadGroup());
	}
}
