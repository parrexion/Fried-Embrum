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

	public void MergeBoost(Boost other) {
		hp = MergeValues(hp, other.hp);
		dmg = MergeValues(dmg, other.dmg);
		mnd = MergeValues(mnd, other.mnd);
		spd = MergeValues(spd, other.spd);
		skl = MergeValues(skl, other.skl);
		def = MergeValues(def, other.def);
		mov = MergeValues(mov, other.mov);

		hit = MergeValues(hit, other.hit);
		crit = MergeValues(crit, other.crit);
		avoid = MergeValues(avoid, other.avoid);
	}

	private int MergeValues(int a, int b) {
		if (Mathf.Sign(a) != Mathf.Sign(b)) {
			return a + b;
		}
		if (a > 0) {
			return Mathf.Max(a, b);
		}
		else {
			return Mathf.Min(a, b);
		}
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
				bool update = false;
				if (hp > 0) { hp--; update = true; }
				if (dmg > 0) { dmg--; update = true; }
				if (mnd > 0) { mnd--; update = true; }
				if (spd > 0) { spd--; update = true; }
				if (skl > 0) { skl--; update = true; }
				if (def > 0) { def--; update = true; }
				if (hp < 0) { hp++; update = true; }
				if (dmg < 0) { dmg++; update = true; }
				if (mnd < 0) { mnd++; update = true; }
				if (spd < 0) { spd++; update = true; }
				if (skl < 0) { skl++; update = true; }
				if (def < 0) { def++; update = true; }
				if (!update) {
					_active = false;
				}
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
