using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClassChangeController : MonoBehaviour {

	public GameObject listView;
	public GameObject changeView;
	private bool changeMode;

	[Header("Entry List")]
    public SaveListVariable availableUnits;
	public Transform listParent;
	public Transform entryPrefab;
	private int currentListIndex;
	private int listSize;
	private List<TrainingListEntry> entryList = new List<TrainingListEntry>();

	[Header("Information box")]
	public Text characterName;
	public Image portrait;
	public Text className;
	public Text level;
	public Text exp;

	public Text hpText;
	public Text atkText;
	public Text sklText;
	public Text spdText;
	public Text lckText;
	public Text defText;
	public Text resText;
	public Text conText;
	public Text movText;


	private void OnEnable() {
		listView.SetActive(true);
		changeView.SetActive(false);

		for (int i = listParent.childCount-1; i > 1; i--) {
			GameObject.Destroy(listParent.GetChild(i).gameObject);
		}

		entryList = new List<TrainingListEntry>();
		listSize = availableUnits.stats.Count;
		for (int i = 0; i < listSize; i++) {
			CreateListEntry(availableUnits.stats[i]);
		}
		entryPrefab.gameObject.SetActive(false);

		currentListIndex = 0;
		MoveSelection(0);
	}

	private void CreateListEntry(StatsContainer stats) {
		Transform t = Instantiate(entryPrefab, listParent);

		TrainingListEntry entry = t.GetComponent<TrainingListEntry>();
		entry.FillData(stats);
		entry.SetHighlight(false);
		entry.SetDark(stats.level < 2);
		entryList.Add(entry);

		t.gameObject.SetActive(true);
	}

	public void MoveSelection(int dir) {
		if (!changeMode) {
			currentListIndex = OPMath.FullLoop(0, listSize-1, currentListIndex + dir);
			for (int i = 0; i < listSize; i++) {
				entryList[i].SetHighlight(currentListIndex == i);
			}
		}
	}

	public void SelectCharacter() {
		if (!changeMode) {
			if (entryList[currentListIndex].isDark)
				return;
			changeMode = true;
			SetupCharacterInfo();
			changeView.SetActive(true);
			listView.SetActive(false);
		}
	}

	public bool DeselectCharacter() {
		if (changeMode) {
			changeMode = false;
			listView.SetActive(true);
			changeView.SetActive(false);
			return false;
		}

		return true;
	}

	private void SetupCharacterInfo() {
		StatsContainer stats = availableUnits.stats[currentListIndex];

		characterName.text = stats.charData.entryName;
		portrait.sprite = entryList[currentListIndex].portrait.sprite;
		className.text = stats.classData.entryName;
		level.text = "Level: " + stats.level.ToString();
		exp.text = "EXP: " + stats.currentExp.ToString();

		hpText.text  = "HP:  " + stats.hp.ToString();
		atkText.text = "Atk:  " + stats.atk.ToString();
		sklText.text = "Skl:  " + stats.skl.ToString();
		spdText.text = "Spd:  " + stats.spd.ToString();
		lckText.text = "Lck:  " + stats.lck.ToString();
		defText.text = "Def:  " + stats.def.ToString();
		resText.text = "Res:  " + stats.res.ToString();
		conText.text = "Con:  " + stats.GetConstitution().ToString();
		movText.text = "Mov:  " + stats.GetMovespeed().ToString();
	}

}
