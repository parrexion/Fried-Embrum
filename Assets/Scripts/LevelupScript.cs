using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelupScript : MonoBehaviour {

	public GameObject levelupCongrats;
	public GameObject levelupStats;

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

	public void SetupStats(int playerLevel, StatsContainer characterStats) {
		levelupCongrats.SetActive(true);
		levelupStats.SetActive(false);

		_level = playerLevel;
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
		levelupStats.SetActive(true);

		yield return new WaitForSeconds(1.5f);

		_level++;
		stats.level++;
		levelLevel.text = "+1";
		stats.CalculateStats();
		yield return new WaitForSeconds(0.2f);

		if (stats.hp > _hp) {
			_hp++;
			levelHp.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.atk > _atk) {
			_atk++;
			levelAtk.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.spd > _spd) {
			_spd++;
			levelSpd.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.skl > _skl) {
			_skl++;
			levelSkl.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.lck > _lck) {
			_lck++;
			levelLck.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.def > _def) {
			_def++;
			levelDef.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.res > _res) {
			_res++;
			levelRes.text = "+1";
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(1f);

		levelupStats.SetActive(false);
	}
}
