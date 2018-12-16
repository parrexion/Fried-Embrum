using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BexpController : MonoBehaviour {

	public GameObject listView;
	public GameObject awardView;
	private bool awardMode;

	[Header("Entry List")]
    public SaveListVariable availableUnits;
	public Transform listParent;
	public Transform entryPrefab;
	private int currentListIndex;
	private int listSize;
	private List<TrainingListEntry> entryList = new List<TrainingListEntry>();

	[Header("Bonus EXP")]
	public Text bonusExp;
	public Text spendExp;
	public Text currentLevel;
	public Text currentExp;
	public IntVariable totalBonusExp;
	private int awardExp;

	[Header("Exp animations")]
	public UIExpMeter expMeter;
	public LevelupScript levelupScript;
	public AudioQueueVariable sfxQueue;
	public SfxEntry levelupFanfare;
	public SfxEntry levelupFill;
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
		awardView.SetActive(false);

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
		entryList.Add(entry);

		t.gameObject.SetActive(true);
	}

	public void MoveSelection(int dir) {
		if (!awardMode) {
			currentListIndex = OPMath.FullLoop(0, listSize-1, currentListIndex + dir);
			for (int i = 0; i < listSize; i++) {
				entryList[i].SetHighlight(currentListIndex == i);
			}
		}
	}

	public void SelectCharacter() {
		if (!awardMode) {
			awardMode = true;
			SetupBexpAwarding();
			SetupCharacterInfo();
			awardView.SetActive(true);
			listView.SetActive(false);
		}
		else if (awardExp != 0) {
			StartCoroutine(AwardExp());
		}
	}

	public bool DeselectCharacter() {
		if (awardMode) {
			awardMode = false;
			listView.SetActive(true);
			awardView.SetActive(false);
			return false;
		}

		return true;
	}

	private void SetupBexpAwarding() {
		StatsContainer stats = availableUnits.stats[currentListIndex];
		bonusExp.text = "Available EXP:  " + (totalBonusExp.value - awardExp);
		bonusExp.color = (awardExp > 0) ? Color.green : Color.black;
		spendExp.text = awardExp.ToString();
		currentLevel.text = "Current level:  " + ((stats.currentExp + awardExp >= 100) ? stats.level+1 : stats.level);
		currentLevel.color = (stats.currentExp + awardExp >= 100) ? Color.green : Color.black;
		currentExp.text = "Current EXP:   " + ((stats.currentExp + awardExp) % 100);
		currentExp.color = (awardExp > 0) ? Color.green : Color.black;
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

	public void UpdateAwardExp(int dir) {
		if (!awardMode)
			return;

		StatsContainer stats = availableUnits.stats[currentListIndex];
		awardExp = OPMath.FullLoop(0, 100, awardExp + dir);
		awardExp = Mathf.Min(awardExp, totalBonusExp.value);

		SetupBexpAwarding();
	}

	public IEnumerator AwardExp() {
		lockControls.value = true;
		totalBonusExp.value -= awardExp;
		StatsContainer stats = availableUnits.stats[currentListIndex];

		expMeter.gameObject.SetActive(true);
		expMeter.currentExp = stats.currentExp;
		Debug.Log("Exp is currently: " + stats.currentExp);
		yield return new WaitForSeconds(0.5f * slowGameSpeed.value / currentGameSpeed.value);
		sfxQueue.Enqueue(levelupFill);
		playSfxEvent.Invoke();
		while(awardExp > 0) {
			awardExp--;
			expMeter.currentExp++;
			if (expMeter.currentExp == 100) {
				expMeter.currentExp = 0;
				stopSfxEvent.Invoke();
				yield return new WaitForSeconds(1f * slowGameSpeed.value / currentGameSpeed.value);
				expMeter.gameObject.SetActive(false);
				levelupScript.SetupStats(stats.level,stats);
				Debug.Log("LEVELUP!");
				sfxQueue.Enqueue(levelupFanfare);
				playSfxEvent.Invoke();
				yield return StartCoroutine(levelupScript.RunLevelup(stats));
				// CharacterSkill skill = player.stats.classData.AwardSkills(player.stats.level);
				// if (skill) { TODO
				// 	player.skills.GainSkill(skill);
				// 	yield return StartCoroutine(popup.ShowPopup(skill.icon,  "gained: " + skill.entryName, popup.droppedItemFanfare));
				// }
				expMeter.gameObject.SetActive(true);
				sfxQueue.Enqueue(levelupFill);
				playSfxEvent.Invoke();
			}
			yield return null;
		}
		stopSfxEvent.Invoke();
		yield return new WaitForSeconds(0.5f * slowGameSpeed.value / currentGameSpeed.value);
		expMeter.gameObject.SetActive(false);
		stats.currentExp = expMeter.currentExp;

		entryList[currentListIndex].FillData(stats);
		SetupBexpAwarding();
		SetupCharacterInfo();
		lockControls.value = false;
	}
}
