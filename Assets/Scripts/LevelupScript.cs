using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelupScript : MonoBehaviour {

	public GameObject blackout;
	public GameObject levelupCongrats;
	public GameObject classChangeCongrats;
	public GameObject levelupStats;
	public GameObject levelupArrow;

	[Header("Sound")]
	public AudioQueueVariable sfxQueue;
	public UnityEvent playSfxEvent;
	public SfxEntry levelupPing;

	[Header("Text objects")]
	public Text levelText;
	public Text hpText;
	public Text dmgText;
	public Text mndText;
	public Text spdText;
	public Text sklText;
	public Text defText;

	[Header("Levelup objects")]
	public Text levelLevel;
	public Text levelHp;
	public Text levelDmg;
	public Text levelMnd;
	public Text levelSpd;
	public Text levelSkl;
	public Text levelDef;

	private int _level;
	private int _hp;
	private int _dmg;
	private int _mnd;
	private int _spd;
	private int _skl;
	private int _def;

	private bool waiting;

	
	// Update is called once per frame
	private void Update () {
		levelText.text = _level.ToString();
		hpText.text = _hp.ToString();
		dmgText.text = _dmg.ToString();
		mndText.text = _mnd.ToString();
		spdText.text = _spd.ToString();
		sklText.text = _skl.ToString();
		defText.text = _def.ToString();
	}

	public void SetupStats(StatsContainer characterStats, bool levelup) {
		if (blackout != null)
			blackout.SetActive(true);
		levelupCongrats.SetActive(levelup);
		classChangeCongrats.SetActive(!levelup);
		levelupStats.SetActive(false);
		levelupArrow.SetActive(false);

		_level = characterStats.level;
		_hp = characterStats.hp;
		_dmg = characterStats.dmg;
		_mnd = characterStats.mnd;
		_spd = characterStats.spd;
		_skl = characterStats.skl;
		_def = characterStats.def;

		levelLevel.text = "";
		levelHp.text = "";
		levelDmg.text = "";
		levelMnd.text = "";
		levelSpd.text = "";
		levelSkl.text = "";
		levelDef.text = "";
	}

	public IEnumerator RunLevelup(StatsContainer stats) {
		yield return new WaitForSeconds(2f);

		levelupCongrats.SetActive(false);
		classChangeCongrats.SetActive(false);
		levelupStats.SetActive(true);
	
		yield return new WaitForSeconds(1.5f);

		levelLevel.text = (stats.level - _level > 0) ? "+1" : "New";
		_level = stats.level;
		sfxQueue.Enqueue(levelupPing);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(0.2f);

		if (stats.hp != _hp) {
			levelHp.text = (stats.hp - _hp > 0) ? "+"+(stats.hp - _hp) : (stats.hp - _hp).ToString();
			_hp = stats.hp;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.dmg != _dmg) {
			levelDmg.text = (stats.dmg - _dmg > 0) ? "+"+(stats.dmg - _dmg) : (stats.dmg - _dmg).ToString();
			_dmg = stats.dmg;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.mnd != _mnd) {
			levelMnd.text = (stats.mnd - _mnd > 0) ? "+"+(stats.mnd - _mnd) : (stats.mnd - _mnd).ToString();
			_mnd = stats.mnd;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.spd != _spd) {
			levelSpd.text = (stats.spd - _spd > 0) ? "+"+(stats.spd - _spd) : (stats.spd - _spd).ToString();
			_spd = stats.spd;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.skl != _skl) {
			levelSkl.text = (stats.skl - _skl > 0) ? "+"+(stats.skl - _skl) : (stats.skl - _skl).ToString();
			_skl = stats.skl;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.def != _def) {
			levelDef.text = (stats.def - _def > 0) ? "+"+(stats.def - _def) : (stats.def - _def).ToString();
			_def = stats.def;
			sfxQueue.Enqueue(levelupPing);
			playSfxEvent.Invoke();
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(1f);

		levelupArrow.SetActive(true);
		waiting = true;
		while (waiting)
			yield return null;
		
		levelupArrow.SetActive(false);
		levelupStats.SetActive(false);
		if (blackout != null)
			blackout.SetActive(false);
	}

	public void Continue() {
		waiting = false;
	}
}
