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
	public Text defText;
	public Text resText;

	[Header("Levelup objects")]
	public GameObject levelLevel;
	public GameObject levelHp;
	public GameObject levelAtk;
	public GameObject levelSpd;
	public GameObject levelDef;
	public GameObject levelRes;
	public GameObject levelSp;

	private int _level;
	private int _hp;
	private int _atk;
	private int _spd;
	private int _def;
	private int _res;

	
	// Update is called once per frame
	private void Update () {
		levelText.text = _level.ToString();
		hpText.text = _hp.ToString();
		atkText.text = _atk.ToString();
		spdText.text = _spd.ToString();
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
		_def = characterStats.def;
		_res = characterStats.res;

		levelLevel.SetActive(false);
		levelHp.SetActive(false);
		levelAtk.SetActive(false);
		levelSpd.SetActive(false);
		levelDef.SetActive(false);
		levelRes.SetActive(false);
		levelSp.SetActive(false);
	}

	public IEnumerator RunLevelup(StatsContainer stats) {
		yield return new WaitForSeconds(2f);

		levelupCongrats.SetActive(false);
		levelupStats.SetActive(true);

		yield return new WaitForSeconds(1.5f);

		_level++;
		stats.level++;
		levelLevel.SetActive(true);
		stats.CalculateStats();
		yield return new WaitForSeconds(0.2f);

		if (stats.hp > _hp) {
			_hp++;
			levelHp.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.atk > _atk) {
			_atk++;
			levelAtk.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.spd > _spd) {
			_spd++;
			levelSpd.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.def > _def) {
			_def++;
			levelDef.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		if (stats.res > _res) {
			_res++;
			levelRes.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(1f);
	}
}
