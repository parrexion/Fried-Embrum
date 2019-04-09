using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BexpController : MonoBehaviour {

    public PlayerData playerData;
	public GameObject listView;
	public GameObject awardView;
	private bool awardMode;

	[Header("Entry List")]
	public Transform listParent;
	public Transform entryPrefab;
	public int visibleSize;
	private EntryList<TrainingListEntry> entryList;

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


	private void Start() {
		listView.SetActive(true);
		awardView.SetActive(false);
	}

	public void GenerateList() {
		if (entryList == null)
			entryList = new EntryList<TrainingListEntry>(visibleSize);
		entryList.ResetList();
		for (int i = 0; i < playerData.stats.Count; i++) {
			Transform t = Instantiate(entryPrefab, listParent);
			TrainingListEntry entry = entryList.CreateEntry(t);
			entry.FillData(playerData.stats[i]);
		}
		entryPrefab.gameObject.SetActive(false);

		MoveSelection(0);
	}

	public void MoveSelection(int dir) {
		if (!awardMode) {
			entryList.Move(dir);
		}
	}

	public void SelectCharacter() {
		if (!awardMode) {
			awardMode = true;
			awardExp = 0;
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
		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		bonusExp.text = "Available EXP:  " + (totalBonusExp.value - awardExp);
		bonusExp.color = (awardExp > 0) ? Color.green : Color.black;
		spendExp.text = awardExp.ToString();
		currentLevel.text = "Current level:  " + ((stats.currentExp + awardExp >= 100) ? stats.currentLevel+1 : stats.currentLevel);
		currentLevel.color = (stats.currentExp + awardExp >= 100) ? Color.green : Color.black;
		currentExp.text = "Current EXP:   " + ((stats.currentExp + awardExp) % 100);
		currentExp.color = (awardExp > 0) ? Color.green : Color.black;
	}

	private void SetupCharacterInfo() {
		StatsContainer stats = playerData.stats[entryList.GetPosition()];

		characterName.text = stats.charData.entryName;
		portrait.sprite = stats.charData.portrait;
		className.text = stats.classData.entryName;
		level.text = "Level: " + stats.currentLevel.ToString();
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

		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		awardExp = OPMath.FullLoop(0, 101, awardExp + dir);
		awardExp = Mathf.Min(awardExp, totalBonusExp.value);

		SetupBexpAwarding();
	}

	public IEnumerator AwardExp() {
		lockControls.value = true;
		totalBonusExp.value -= awardExp;
		StatsContainer stats = playerData.stats[entryList.GetPosition()];

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
				levelupScript.SetupStats(stats, true);
				Debug.Log("LEVELUP!");
				stats.GainLevel();
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

		entryList.GetEntry().FillData(stats);
		SetupBexpAwarding();
		SetupCharacterInfo();
		lockControls.value = false;
	}
}
