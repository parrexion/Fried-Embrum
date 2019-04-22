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
	public Text atkText;
	public Text spdText;
	public Text sklText;
	public Text lckText;
	public Text defText;
	public Text resText;

	[Header("Levelup objects")]
	public Text levelLevel;
	public Text levelHp;
	public Text levelAtk;
	public Text levelSpd;
	public Text levelSkl;
	public Text levelLck;
	public Text levelDef;
	public Text levelRes;

	private int _level;
	private int _hp;
	private int _atk;
	private int _spd;
	private int _skl;
	private int _lck;
	private int _def;
	private int _res;

	private bool waiting;

	
	// Update is called once per frame
	private void Update () {
		levelText.text = _level.ToString();
		hpText.text = _hp.ToString();
		atkText.text = _atk.ToString();
		spdText.text = _spd.ToString();
		sklText.text = _skl.ToString();
		lckText.text = _lck.ToString();
		defText.text = _def.ToString();
		resText.text = _res.ToString();
	}

	public void SetupStats(StatsContainer characterStats, bool levelup) {
		if (blackout != null)
			blackout.SetActive(true);
		levelupCongrats.SetActive(levelup);
		classChangeCongrats.SetActive(!levelup);
		levelupStats.SetActive(false);
		levelupArrow.SetActive(false);

		_level = characterStats.currentLevel;
		_hp = characterStats.hp;
		_atk = characterStats.atk;
		_spd = characterStats.spd;
		_skl = characterStats.skl;
		_lck = characterStats.lck;
		_def = characterStats.def;
		_res = characterStats.res;

		levelLevel.text = "";
		levelHp.text = "";
		levelAtk.text = "";
		levelSpd.text = "";
		levelSkl.text = "";
		levelLck.text = "";
		levelDef.text = "";
		levelRes.text = "";
	}

	public IEnumerator RunLevelup(StatsContainer stats) {
		yield return new WaitForSeconds(2f);

		levelupCongrats.SetActive(false);
		classChangeCongrats.SetActive(false);
		levelupStats.SetActive(true);
	
		yield return new WaitForSeconds(1.5f);

		levelLevel.text = (stats.currentLevel - _level > 0) ? "+1" : "New";
		_level = stats.currentLevel;
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
		if (stats.atk != _atk) {
			levelAtk.text = (stats.atk - _atk > 0) ? "+"+(stats.atk - _atk) : (stats.atk - _atk).ToString();
			_atk = stats.atk;
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
		if (stats.lck != _lck) {
			levelLck.text = (stats.lck - _lck > 0) ? "+"+(stats.lck - _lck) : (stats.lck - _lck).ToString();
			_lck = stats.lck;
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
		if (stats.res != _res) {
			levelRes.text = (stats.res - _res > 0) ? "+"+(stats.res - _res) : (stats.res - _res).ToString();
			_res = stats.res;
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
