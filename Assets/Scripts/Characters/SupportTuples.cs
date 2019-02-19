using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportValue {

	public string uuid;
	public int value;
	public int currentLevel;
}



public enum SupportLetter { NONE = 0, C = 1, B = 2, A = 3, S = 4, X = -1 }
public enum SupportSpeed { NORMAL, FAST, VERYFAST, SLOW, VERYSLOW }

[System.Serializable]
public class SupportTuple {

	public CharData partner;
	public SupportLetter maxlevel;
	public SupportSpeed speed;

	public string GetSpeedString(){
		switch (speed)
		{
			case SupportSpeed.VERYFAST:
				return "(++)";
			case SupportSpeed.FAST:
				return "(+)";
			case SupportSpeed.NORMAL:
				return "( )";
			case SupportSpeed.SLOW:
				return "(-)";
			case SupportSpeed.VERYSLOW:
				return "(--)";
		}

		return ("(N/A)");
	}

	public SupportLetter CalculateLevel(int value) {
		if (value >= 1000) {
			return SupportLetter.S;
		}
		else if (value >= 750) {
			return SupportLetter.A;
		}
		else if (value >= 500) {
			return SupportLetter.B;
		}
		else if (value >= 250) {
			return SupportLetter.C;
		}
		else {
			return SupportLetter.X;
		}
	}
}
