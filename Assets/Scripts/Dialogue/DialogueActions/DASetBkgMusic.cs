using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DASetBkgMusic : DialogueAction {

	public override bool Act(DialogueScene scene, DialogueActionData data) {

		MusicEntry me = (MusicEntry)data.entries[0];
		if (me == null) {
			return false;
		}
		scene.bkgMusic.value = me.clip;
		scene.musicFocusSource.value = false;

		scene.effectStartDuration.value = 0;
		scene.effectEndDuration.value = 0;

		return true;
	}

    public override void FillData(DialogueActionData data) {
		data.type = DActionType.SET_MUSIC;
		data.autoContinue = true;
		data.useDelay = false;
        data.entries.Add(null);
    }
}
