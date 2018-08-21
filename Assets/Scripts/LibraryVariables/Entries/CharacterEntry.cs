using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Character")]
public class CharacterEntry : ScrObjLibraryEntry {

	public Sprite[] poses;


	public override void ResetValues() {
		base.ResetValues();
		poses = new Sprite[0];
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharacterEntry ce = (CharacterEntry)other;

		poses = new Sprite[ce.poses.Length];
		for (int i = 0; i < ce.poses.Length; i++) {
			poses[i] = ce.poses[i];
		}
	}
}
