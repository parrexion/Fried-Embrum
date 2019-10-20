using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Portrait")]
public class PortraitEntry : ScrObjLibraryEntry {

	public enum Pose { NORMAL = 0, kCount = 1 }

	public Sprite small;
	public Sprite[] poses = new Sprite[0];
	public int customValue = 0;


	public override void ResetValues() {
		base.ResetValues();
		small = null;
		poses = new Sprite[0];
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		PortraitEntry ce = (PortraitEntry)other;

		small = ce.small;
		poses = new Sprite[ce.poses.Length];
		for (int i = 0; i < ce.poses.Length; i++) {
			poses[i] = ce.poses[i];
		}
	}

	public Sprite GetPose(Pose pose) {
		return poses[(int)pose];
	}
}
