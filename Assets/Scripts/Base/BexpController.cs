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
	public IntVariable totalScrap;
	private int awardAmount;
	private bool runningExp;

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
	public Text dmgText;
	public Text mndText;
	public Text sklText;
	public Text spdText;
	public Text defText;

	public Image[] skillIcons;
	public IconLibrary wpnIcons;
	public Transform wpnParent;
	public Transform wpnTemplate;
	private List<Transform> wpnRanks = new List<Transform>();


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
		if (runningExp) {
			levelupScript.Continue();
		}
		else if (!awardMode) {
			awardMode = true;
			awardAmount = 0;
			SetupBexpAwarding();
			SetupCharacterInfo();
			awardView.SetActive(true);
			listView.SetActive(false);
		}
		else if (awardAmount != 0) {
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
		int convertedExp = 10 * awardAmount;
		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		bonusExp.text = "Available scrap:  " + (totalScrap.value - awardAmount);
		bonusExp.color = (awardAmount > 0) ? Color.green : Color.black;
		spendExp.text = convertedExp.ToString();
		currentLevel.text = "Current level:  " + ((stats.currentExp + convertedExp >= 100) ? stats.level + 1 : stats.level);
		currentLevel.color = (stats.currentExp + convertedExp >= 100) ? Color.green : Color.black;
		currentExp.text = "Current EXP:   " + ((stats.currentExp + convertedExp) % 100);
		currentExp.color = (awardAmount > 0) ? Color.green : Color.black;
	}

	private void SetupCharacterInfo() {
		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		InventoryContainer inventory = playerData.inventory[entryList.GetPosition()];
		SkillsContainer skills = playerData.skills[entryList.GetPosition()];

		characterName.text = stats.charData.entryName;
		portrait.sprite = stats.charData.portraitSet.small;
		className.text = stats.currentClass.entryName;
		level.text = "Level: " + stats.level.ToString();
		exp.text = "EXP: " + stats.currentExp.ToString();

		hpText.text = "HP:  " + stats.hp.ToString();
		dmgText.text = "Dmg:  " + stats.dmg.ToString();
		mndText.text = "Mnd:  " + stats.mnd.ToString();
		sklText.text = "Skl:  " + stats.skl.ToString();
		spdText.text = "Spd:  " + stats.spd.ToString();
		defText.text = "Def:  " + stats.def.ToString();

		for (int i = 0; i < skillIcons.Length; i++) {
			if (skills.skills[i] == null) {
				skillIcons[i].enabled = false;
			}
			else {
				skillIcons[i].sprite = skills.skills[i].icon;
				skillIcons[i].enabled = true;
			}
		}

		for (int i = 0; i < wpnRanks.Count; i++) {
			Destroy(wpnRanks[i].gameObject);
		}
		wpnRanks.Clear();
		for (int i = 0; i < inventory.wpnSkills.Length; i++) {
			if (inventory.wpnSkills[i] == WeaponRank.NONE)
				continue;

			Transform t = Instantiate(wpnTemplate, wpnParent);
			t.GetComponentInChildren<Image>().sprite = wpnIcons.icons[i];
			t.GetComponentInChildren<Text>().text = inventory.wpnSkills[i].ToString();
			t.gameObject.SetActive(true);
			wpnRanks.Add(t);
		}
	}

	public void UpdateAwardExp(int dir) {
		if (!awardMode)
			return;
		int cap = (totalScrap.value > 0) ? Mathf.Min(11, totalScrap.value+1) : 1;
		awardAmount = OPMath.FullLoop(0, cap, awardAmount + dir);
		SetupBexpAwarding();
	}

	public IEnumerator AwardExp() {
		if (runningExp)
			yield break;

		runningExp = true;
		lockControls.value = true;
		totalScrap.value -= awardAmount;
		StatsContainer stats = playerData.stats[entryList.GetPosition()];

		expMeter.gameObject.SetActive(true);
		expMeter.currentExp = stats.currentExp;
		Debug.Log("Exp is currently: " + stats.currentExp);
		yield return new WaitForSeconds(0.5f * slowGameSpeed.value / currentGameSpeed.value);
		sfxQueue.Enqueue(levelupFill);
		playSfxEvent.Invoke();

		int convertedExp = 10 * awardAmount;
		awardAmount = 0;
		while (convertedExp > 0) {
			convertedExp--;
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
		runningExp = false;
	}
}
