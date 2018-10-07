using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DAChangePosition : DialogueAction {

	public override bool Act(DialogueScene scene, DialogueActionData data) {

		scene.effectStartDuration.value = data.values[0] * 0.001f;
		scene.effectEndDuration.value = 0;

		ScrObjLibraryEntry[] originalCharacters = new ScrObjLibraryEntry[Utility.DIALOGUE_PLAYERS_COUNT+Utility.DIALOGUE_PLAYERS_OUTSIDE_COUNT];
		int[] originalPoses = new int[Utility.DIALOGUE_PLAYERS_COUNT+Utility.DIALOGUE_PLAYERS_OUTSIDE_COUNT];
		for (int i = 0; i < scene.characters.Length; i++) {
			originalCharacters[i] = scene.characters[i].value;
			originalPoses[i] = scene.poses[i].value;
		}

		for (int i = 1; i < data.values.Count; i+=2) {
			scene.characters[data.values[i+1]].value = originalCharacters[data.values[i]];
			scene.poses[data.values[i+1]].value = originalPoses[data.values[i]];

			if (scene.characters[data.values[i]].value.IsEqual(originalCharacters[data.values[i]])) {
				scene.characters[data.values[i]].value = null;
				scene.poses[data.values[i]].value = -1;
			}

			scene.characterTransforms[data.values[i+1]].SetMoveDirection(scene.characterTransforms[data.values[i]].transform.position, data.values[i]);
		}

		return true;
	}

    public override void FillData(DialogueActionData data) {
		data.type = DActionType.MOVEMENT;
		data.autoContinue = true;
		data.useDelay = true;
        data.values.Add(1000);
    }
}
