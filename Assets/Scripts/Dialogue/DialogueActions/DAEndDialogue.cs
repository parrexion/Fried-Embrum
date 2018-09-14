using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
