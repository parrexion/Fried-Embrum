using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSave {

	public int id;
	public int statsID;
	public int level;
	public int currentExp;
	public int currentSp;
	public int weaponLevel;
	public int supportLevel;
	public int skillLevel;
	public int skillALevel;
	public int skillBLevel;
	public int skillCLevel;
	
	[Header("IV values")]
	public float iHp;
	public float iAtk;
	public float iSpd;
	public float iDef;
	public float iRes;

	[Header("EV values")]
	public float eHp;
	public float eAtk;
	public float eSpd;
	public float eDef;
	public float eRes;
	

	public CharacterSave() {
		id = -1;
		statsID = -1;
		level = -1;
	}

	public void LoadData(StatsContainer cont) {
		id = cont.id;
		statsID = cont.statsID;
		level = cont.level;
		currentExp = cont.currentExp;
		currentSp = cont.currentSp;
		weaponLevel = cont.weaponLevel;
		supportLevel = cont.supportLevel;
		skillLevel = cont.skillLevel;
		skillALevel = cont.skillALevel;
		skillBLevel = cont.skillBLevel;
		skillCLevel = cont.skillCLevel;

		iHp = cont.iHp;
		iAtk = cont.iAtk;
		iSpd = cont.iSpd;
		iDef = cont.iDef;
		iRes = cont.iRes;
		
		eHp = cont.eHp;
		eAtk = cont.eAtk;
		eSpd = cont.eSpd;
		eDef = cont.eDef;
		eRes = cont.eRes;
	}

	public void GenerateDataFromStars(int stars, CharacterStats stats) {
		level = 1;
		currentExp = 0;
		currentSp = 0;
		weaponLevel = (stats.weapons.Length > 0) ? 0 : -1;
		supportLevel = (stats.weapons.Length > 0) ? -1 : 0;
		skillLevel = -1;
		skillALevel = -1;
		skillBLevel = -1;
		skillCLevel = -1;

		GenerateIV();
		AddStarsBoost(stars, stats);
		AddIVVariance();
	}
	
	private void GenerateIV() {
		iHp = Random.Range(0.01f,0.99f);
		iAtk = Random.Range(0.01f,0.99f);
		iSpd = Random.Range(0.01f,0.99f);
		iDef = Random.Range(0.01f,0.99f);
		iRes = Random.Range(0.01f,0.99f);
	}

	private void AddStarsBoost(int stars, CharacterStats stats) {
		List<StatsTuple> list = new List<StatsTuple> {
			new StatsTuple(StatsTuple.StatsType.ATK, stats.atk),
			new StatsTuple(StatsTuple.StatsType.SPD, stats.spd),
			new StatsTuple(StatsTuple.StatsType.DEF, stats.def),
			new StatsTuple(StatsTuple.StatsType.RES, stats.res)
		};
		list.Sort();
		
		switch (stars) {
			case 1:
				break;
			case 2:
				AddValueToIV(list[0].type, 1);
				AddValueToIV(list[1].type, 1);
				break;
			case 3:
				iHp += 1;
				iAtk += 1;
				iSpd += 1;
				iDef += 1;
				iRes += 1;
				break;
			case 4:
				iHp += 1;
				AddValueToIV(list[0].type, 2);
				AddValueToIV(list[1].type, 2);
				AddValueToIV(list[2].type, 1);
				AddValueToIV(list[3].type, 1);
				break;
			case 5:
				iHp += 2;
				iAtk += 2;
				iSpd += 2;
				iDef += 2;
				iRes += 2;
			break;
		}
	}

	private void AddIVVariance() {
		List<StatsTuple> list = new List<StatsTuple> {
			new StatsTuple(StatsTuple.StatsType.HP, 0),
			new StatsTuple(StatsTuple.StatsType.ATK, 0),
			new StatsTuple(StatsTuple.StatsType.SPD, 0),
			new StatsTuple(StatsTuple.StatsType.DEF, 0),
			new StatsTuple(StatsTuple.StatsType.RES, 0)
		};
		int r1 = Random.Range(0, 5);
		int r2 = Random.Range(0, 5);
		AddValueToIV(list[r1].type, 1);
		AddValueToIV(list[r2].type, -1);
	}

	private void AddValueToIV(StatsTuple.StatsType type, int value) {
		Debug.Log("Adding " + value + " to stats " + type);
		switch (type) {
			case StatsTuple.StatsType.HP: iHp += value; break;
			case StatsTuple.StatsType.ATK: iAtk += value; break;
			case StatsTuple.StatsType.SPD: iSpd += value; break;
			case StatsTuple.StatsType.DEF: iDef += value; break;
			case StatsTuple.StatsType.RES: iRes += value; break;
		}
	}
}

public class StatsTuple : System.IComparable {
	public enum StatsType {HP, ATK, SPD, DEF, RES}

	public StatsType type;
	public int value;


	public StatsTuple(StatsType type, int value) {
		this.type = type;
		this.value = value;
	}

	public int CompareTo(object obj) {
		StatsTuple y = (StatsTuple) obj;
		
		if (y == null)
			return 1;
		
		if (value == y.value) {
			return (int) type - (int) y.type;
		}
		return value - y.value;
	}
}
