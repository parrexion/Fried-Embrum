using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoostType { SINGLE, PASSIVE, DECREASE, ENDTURN, OTHER = 99 }

[System.Serializable]
public class Boost {

	public BoostType boostType;
	public int hp;
	public int dmg;
	public int mnd;
	public int spd;
	public int skl;
	public int def;
	public int mov;

	public int hit;
	public int crit;
	public int avoid;

	private bool _active;


	public void ActivateBoost() {
		_active = true;
	}

	public Boost InvertStats() {
		Boost temp = new Boost {
			hp = -hp,
			dmg = -dmg,
			mnd = -mnd,
			spd = -spd,
			skl = -skl,
			def = -def,
			mov = -mov,

			hit = -hit,
			crit = -crit,
			avoid = -avoid
		};
		return temp;
	}

	public void AddBoost(Boost other) {
		hp += other.hp;
		dmg += other.dmg;
		mnd += other.mnd;
		spd += other.spd;
		skl += other.skl;
		def += other.def;
		mov += other.mov;

		hit += other.hit;
		crit += other.crit;
		avoid += other.avoid;
	}

	public bool IsActive() {
		return _active;
	}

	public void StartTurn() {
		Debug.Log(boostType.ToString());
		switch (boostType) {
			case BoostType.PASSIVE:
			case BoostType.ENDTURN:
				break;
			case BoostType.DECREASE:
				if (hp > 0) hp--;
				if (dmg > 0) dmg--;
				if (mnd > 0) mnd--;
				if (spd > 0) spd--;
				if (skl > 0) skl--;
				if (def > 0) def--;
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


	public override string ToString() {
		return string.Format("HP: {0}, DMG:{1}, MND:{2}, SKL:{3}, SPD:{4}, DEF:{5}, MOV:{6}, HIT:{7}, CRIT:{8}, AVOID:{9}", 
			hp, dmg, mnd, skl, spd, def, mov, hit, crit, avoid);
	}
}
