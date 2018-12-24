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

	[Header("Class List")]
	public Transform classParent;
	public Transform classPrefab;
	private List<ClassListEntry> classList = new List<ClassListEntry>();
	private CharClass selectedClass;
	private int nextClassIndex;

	[Header("Class Change Animation")]
	public LevelupScript levelupScript;
	public AudioQueueVariable sfxQueue;
	public SfxEntry levelupFanfare;
	public IntVariable slowGameSpeed;
	public IntVariable currentGameSpeed;
	public UnityEvent playSfxEvent;
	public UnityEvent stopSfxEvent;
	public BoolVariable lockControls;

	[Header("Information box")]
	public Text characterName;
	public Image portrait;
	public Text className;
	public Text level;
	public Text exp;

	[Header("Value texts")]
	public Text hpText;
	public Text atkText;
	public Text sklText;
	public Text spdText;
	public Text lckText;
	public Text defText;
	public Text resText;
	public Text conText;
	public Text movText;

	[Header("Diff texts")]
	public Text hpDiffText;
	public Text atkDiffText;
	public Text sklDiffText;
	public Text spdDiffText;
	public Text lckDiffText;
	public Text defDiffText;
	public Text resDiffText;
	public Text conDiffText;
	public Text movDiffText;


	private void OnEnable() {
		listView.SetActive(true);
		changeView.SetActive(false);
		CreateListEntries();
		currentListIndex = 0;
		MoveSelection(0);
	}

	private void CreateListEntries() {
		for (int i = listParent.childCount-1; i > 1; i--) {
			GameObject.Destroy(listParent.GetChild(i).gameObject);
		}

		entryList = new List<TrainingListEntry>();
		listSize = availableUnits.stats.Count;
		for (int i = 0; i < listSize; i++) {
			StatsContainer stats = availableUnits.stats[i];
			Transform t = Instantiate(entryPrefab, listParent);

			TrainingListEntry entry = t.GetComponent<TrainingListEntry>();
			entry.FillData(stats);
			entry.SetHighlight(false);
			entry.SetDark(stats.classData.nextClass.Count == 0);
			entryList.Add(entry);

			t.gameObject.SetActive(true);
		}
		entryPrefab.gameObject.SetActive(false);
	}

	public void MoveSelection(int dir) {
		if (!changeMode) {
			currentListIndex = OPMath.FullLoop(0, listSize-1, currentListIndex + dir);
			for (int i = 0; i < listSize; i++) {
				entryList[i].SetHighlight(currentListIndex == i);
			}
		}
		else {
			nextClassIndex = OPMath.FullLoop(0,selectedClass.nextClass.Count-1, nextClassIndex + dir);
			for (int i = 0; i < selectedClass.nextClass.Count; i++) {
				classList[i].SetHighlight(nextClassIndex == i);
			}
			SetupClassDiff();
		}
	}

	public void SelectCharacter() {
		if (!changeMode) {
			if (entryList[currentListIndex].isDark)
				return;
			changeMode = true;
			selectedClass = availableUnits.stats[currentListIndex].classData;
			CreateClassList();
			SetupCharacterInfo();
			SetupClassDiff();
			changeView.SetActive(true);
			listView.SetActive(false);
		}
		else {
			StartCoroutine(ChangeClass());
		}
	}

	public bool DeselectCharacter() {
		if (!changeMode) {
			return true;
		}
		else {
			changeMode = false;
			listView.SetActive(true);
			changeView.SetActive(false);
			return false;
		}
	}

	private void CreateClassList() {
		for (int i = classParent.childCount-1; i > 0; i--) {
			GameObject.Destroy(classParent.GetChild(i).gameObject);
		}

		classList = new List<ClassListEntry>();
		for (int i = 0; i < selectedClass.nextClass.Count; i++) {
			Transform t = Instantiate(classPrefab, classParent);

			ClassListEntry entry = t.GetComponent<ClassListEntry>();
			entry.FillData(selectedClass.nextClass[i]);
			entry.SetHighlight(false);
			classList.Add(entry);

			t.gameObject.SetActive(true);
		}
		MoveSelection(0);
	}

	private void SetupCharacterInfo() {
		StatsContainer stats = availableUnits.stats[currentListIndex];

		characterName.text = stats.charData.entryName;
		portrait.sprite = stats.charData.portrait;
		className.text = stats.classData.entryName;
		level.text = "Level: " + stats.currentLevel;
		exp.text = "EXP: " + stats.currentExp;

		hpText.text  = "HP:  " + stats.hp;
		atkText.text = "Atk:  " + stats.atk;
		sklText.text = "Skl:  " + stats.skl;
		spdText.text = "Spd:  " + stats.spd;
		lckText.text = "Lck:  " + stats.lck;
		defText.text = "Def:  " + stats.def;
		resText.text = "Res:  " + stats.res;
		conText.text = "Con:  " + stats.GetConstitution();
		movText.text = "Mov:  " + stats.GetMovespeed();
	}

	private void SetupClassDiff() {
		CharClass next = selectedClass.nextClass[nextClassIndex];
		hpDiffText.text  = (next.hp - selectedClass.hp).ToString();
		atkDiffText.text = (next.atk - selectedClass.atk).ToString();
		sklDiffText.text = (next.skl - selectedClass.skl).ToString();
		spdDiffText.text = (next.spd - selectedClass.spd).ToString();
		lckDiffText.text = (next.lck - selectedClass.lck).ToString();
		defDiffText.text = (next.def - selectedClass.def).ToString();
		resDiffText.text = (next.res - selectedClass.res).ToString();
		conDiffText.text = (next.con - selectedClass.con).ToString();
		movDiffText.text = (next.movespeed - selectedClass.movespeed).ToString();
	}

	private IEnumerator ChangeClass() {
		lockControls.value = true;

		yield return new WaitForSeconds(1f * slowGameSpeed.value / currentGameSpeed.value);

		StatsContainer stats = availableUnits.stats[currentListIndex]; 
		levelupScript.SetupStats(stats, false);
		Debug.Log("CLASS CHANGE!");
		stats.ChangeClass(selectedClass.nextClass[nextClassIndex]);
		sfxQueue.Enqueue(levelupFanfare);
		playSfxEvent.Invoke();
		yield return StartCoroutine(levelupScript.RunLevelup(stats));

		CreateListEntries();
		DeselectCharacter();
		lockControls.value = false;
	}
}
// class change anim och vanlig levelup