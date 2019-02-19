using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportList : MonoBehaviour {
	private enum Mode { LIST, DETAILED, PROMPT }
    public SaveListVariable playerData;
	public Transform listParent;
	public Transform entryPrefab;

	[Header("Selected Character")]
	public Text selectName;
	public Image selectIcon;

	public EntryList<SupportListEntry> supportList;
	public int maxEntries;

	[Header("Prompt")]
	public MyPrompt levelupPrompt;

	private int oldIndex;
	private Mode detailedMode;
	private int selectedIndex;


	private void Start() {
		supportList = new EntryList<SupportListEntry>(maxEntries);
	}

	public void CreateList() {
		supportList.ResetList();

		for (int i = 0; i < playerData.stats.Count; i++) {
			Transform t = Instantiate(entryPrefab, listParent);
			SupportListEntry support = supportList.CreateEntry(t);
			support.FillData(i, playerData.stats[i]);
		}
		entryPrefab.gameObject.SetActive(false);
	}

	public void MoveVertical(int dir) {
		if (detailedMode != Mode.PROMPT)
			supportList.Move(dir);
	}

	public void MoveHorizontal(int dir) {
		if (detailedMode == Mode.PROMPT)
			levelupPrompt.Move(dir);
	}

	public void SelectCharacter() {
		if(detailedMode == Mode.LIST) {
			detailedMode = Mode.DETAILED;
			selectedIndex = supportList.GetEntry().index;
			selectName.text = supportList.GetEntry().entryName.text;
			selectIcon.sprite = supportList.GetEntry().icon.sprite;
			CreateSupportEntry();
			supportList.ForcePosition(0);
		}
		else if (detailedMode == Mode.DETAILED) {
			if (supportList.GetEntry().newLevel.enabled) {
				levelupPrompt.ShowWindow("Increase support level?", true);
				detailedMode = Mode.PROMPT;
			}
		}
		else if (detailedMode == Mode.PROMPT) {
			bool res = levelupPrompt.Click(true);
			if (res) {
				//Fixa uppdaterat supportvärde
				Debug.Log("Dum dum daaah!");
			}
			detailedMode = Mode.DETAILED;
		}
	}

	public bool DeselectCharacter() {
		if (detailedMode == Mode.DETAILED) {
			detailedMode = Mode.LIST;
			supportList.FilterShow((x) => true);
			supportList.FilterDark((x) => false);
			for (int i = 0; i < playerData.stats.Count; i++) {
				supportList.GetEntry(i).SetSupportValue(null, null);
			}
			selectName.text = "";
			selectIcon.sprite = null;
			supportList.ForcePosition(oldIndex);
			return false;
		}
		else if (detailedMode == Mode.PROMPT) {
			detailedMode = Mode.DETAILED;
			levelupPrompt.Click(false);
			return false;
		}
		else
			return true;
	}

	private void CreateSupportEntry() {
		oldIndex = supportList.GetPosition();
		supportList.FilterShow((x) => x.index != selectedIndex);
		StatsContainer thisChar = playerData.stats[selectedIndex];
		for (int i = 0; i < supportList.Size(); i++) {
			CharData other = playerData.stats[supportList.GetEntry(i).index].charData;
			SupportTuple tuple = thisChar.charData.GetSupport(other);
			SupportValue value = thisChar.GetSupportValue(other);

			supportList.GetEntry(i).SetDark(tuple == null);
			supportList.GetEntry(i).SetSupportValue(tuple, value);
		}
	}

}
