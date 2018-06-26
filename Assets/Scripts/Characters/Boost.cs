using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoostType { SINGLE, PASSIVE, DECREASE, ENDTURN, OTHER = 99 }

[System.Serializable]
public class Boost {

	public BoostType boostType;
	public int hp;
	public int atk;
	public int spd;
	public int skl;
	public int lck;
	public int def;
	public int res;
	
	private bool _active;


	public void ActivateBoost() {
		_active = true;
	}
	
	public Boost InvertStats() {
		Boost temp = new Boost {
			hp = -hp,
			atk = -atk,
			spd = -spd,
			skl = -skl,
			lck = -lck,
			def = -def,
			res = -res
		};
		return temp;
	}

	public bool IsActive() {
		return _active;
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
			if (skl > 0) skl--;
			if (lck > 0) lck--;
			if (def > 0) def--;
			if (res > 0) res--;
			break;
		case BoostType.SINGLE:
			_active = false;
			break;
		}
	}

	public void EndTurn() {
		if (boostType == BoostType.ENDTURN)
			_active = false;
	}
}
