using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DAEndDialogue : DialogueAction {

	//Entry 0	BattleEntry
	//Entry 1	DialogueEntry

	/*  Values
	0	NextLocation
	1	Changed pos - bool
	2	Next area index
	3	Player xpos
	4	Player ypos
	*/

	public override bool Act(DialogueScene scene, DialogueActionData data) {
		scene.background.value = null;

		for (int i = 0; i < Utility.DIALOGUE_PLAYERS_COUNT+Utility.DIALOGUE_PLAYERS_OUTSIDE_COUNT; i++) {
			scene.characters[i].value = null;
		}

		scene.talkingIndex.value = -1;
		scene.talkingName.value = "";
		scene.inputText.value = "";
		scene.dialogueText.value = "";

		scene.flashBackground.value = null;
		scene.effectStartDuration.value = 0;
		scene.effectEndDuration.value = 0;
		return false;
	}

    public override void FillData(DialogueActionData data) {
		data.type = DActionType.END_SCENE;
		data.autoContinue = false;
		data.useDelay = false;
        data.entries.Add(null);
        data.entries.Add(null);

        data.values.Add(0);
        data.values.Add(0);
        data.values.Add(0);
        data.values.Add(0);
        data.values.Add(0);
    }
}
