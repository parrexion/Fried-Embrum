using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoostType { SINGLE, PASSIVE, DECREASE, ENDTURN }

[System.Serializable]
public class Boost {

	private bool active;
	public BoostType boostType;
	public int hp;
	public int atk;
	public int spd;
	public int def;
	public int res;


	public void ActivateBoost() {
		active = true;
	}
	
	public Boost InvertStats() {
		Boost temp = new Boost {
			hp = -hp,
			atk = -atk,
			spd = -spd,
			def = -def,
			res = -res
		};
		return temp;
	}

	public bool IsActive() {
		return active;
	}

	public void StartTurn() {
		Debug.Log(boostType.ToString());
		switch (boostType)
		{
		case BoostType.PASSIVE:
		case BoostType.ENDTURN:
			break;
		case BoostType.DECREASE:
			if (hp > 0) hp--;
			if (atk > 0) atk--;
			if (spd > 0) spd--;
			if (def > 0) def--;
			if (res > 0) res--;
			break;
		case BoostType.SINGLE:
			active = false;
			break;
		}
	}

	public void EndTurn() {
		if (boostType == BoostType.ENDTURN)
			active = false;
	}
}
