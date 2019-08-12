using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class for a list of MusicEntrys.
/// </summary>
[CreateAssetMenu(menuName="List Variables/Music Set")]
public class MusicSetEntry : ScrObjLibraryEntry {

	public MusicEntry playerTheme;
	public MusicEntry enemyTheme;
	public MusicEntry battleTheme;
	public MusicEntry healTheme;


	public override void ResetValues() {
		base.ResetValues();
		playerTheme = null;
		enemyTheme = null;
		battleTheme = null;
		healTheme = null;
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		MusicSetEntry ms = (MusicSetEntry)other;

		playerTheme = ms.playerTheme;
		enemyTheme = ms.enemyTheme;
		battleTheme = ms.battleTheme;
		healTheme = ms.healTheme;
	}
}
