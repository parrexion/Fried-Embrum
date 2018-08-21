using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DASetText : DialogueAction {

	public override bool Act(DialogueScene scene, DialogueActionData data) {

		scene.talkingIndex.value = data.values[0];
		scene.talkingName.value = data.text[0];
		scene.inputText.value = data.text[1];
		if (data.boolValue)
			scene.dialogueText.value = "";
		if (data.autoContinue)
			scene.inputText.value += " £";
		
		scene.effectStartDuration.value = 0;
		scene.effectEndDuration.value = 0;

		return true;
	}

    public override void FillData(DialogueActionData data) {
		data.type = DActionType.SET_TEXT;
		data.autoContinue = false;
        data.text.Add(""); //Name
       	data.text.Add(""); //Text

		data.values.Add(-1); //Talking index
		data.boolValue = true; //Clear previous text
    }
}
