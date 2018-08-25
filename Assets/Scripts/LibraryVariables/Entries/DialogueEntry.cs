using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Dialogue")]
public class DialogueEntry : ScrObjLibraryEntry {

	public int size { get{ return frames.Count; } }
	public List<Color> participantColors = new List<Color>();

	public DialogueEntry dialogueEntry = null;
	public BattleEntry battleEntry = null;
	public bool changePosition = false;
	public Vector2 playerPosition = new Vector2();

	public List<Frame> frames = new List<Frame>();
	public List<DialogueActionData> actions = new List<DialogueActionData>();


	public void CreateBasicActions() {
		
		actions = new List<DialogueActionData>();
		DialogueActionData data = new DialogueActionData(){type = DActionType.SET_BKG};
		DialogueAction.CreateAction(data.type).FillData(data);
		actions.Add(data);
		data = new DialogueActionData(){type = DActionType.SET_MUSIC};
		DialogueAction.CreateAction(data.type).FillData(data);
		actions.Add(data);
		data = new DialogueActionData(){type = DActionType.SET_CHARS};
		DialogueAction.CreateAction(data.type).FillData(data);
		actions.Add(data);
		data = new DialogueActionData(){type = DActionType.SET_TEXT};
		DialogueAction.CreateAction(data.type).FillData(data);
		actions.Add(data);
		data = new DialogueActionData(){type = DActionType.END_SCENE};
		DialogueAction.CreateAction(data.type).FillData(data);
		actions.Add(data);
	}

	public override void ResetValues() {
		base.ResetValues();

		participantColors = new List<Color>();
		CreateBasicActions();

		dialogueEntry = null;
		battleEntry = null;
		changePosition = false;
		playerPosition = new Vector2();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		DialogueEntry de = (DialogueEntry)other;

		actions = de.actions;
		participantColors = de.participantColors;

		dialogueEntry = de.dialogueEntry;
		battleEntry = de.battleEntry;
		changePosition = de.changePosition;
		playerPosition = de.playerPosition;
		Debug.Log("COPY");
	}

	public void InsertAction(int index, DialogueActionData da) {
		actions.Insert(index, da);
	}

	public void RemoveAction(int index) {
		actions.RemoveAt(index);
	}

	public GUIContent[] GenerateActionRepresentation() {
		GUIContent[] list = new GUIContent[actions.Count];
		GUIContent content;
		for (int i = 0; i < actions.Count; i++) {
			content = new GUIContent();
			content.text = actions[i].type.ToString();
			if (i < 3 || actions[i].type == DActionType.END_SCENE) {
				content.image = GenerateColorTexture(Color.black);
			}
			else if (!actions[i].autoContinue) {
				content.image = GenerateColorTexture(Color.blue);
			}
			else if (actions[i].useDelay) {
				content.image = GenerateColorTexture(Color.magenta);
			}
			list[i] = content;
		}
		return list;
	}
}
