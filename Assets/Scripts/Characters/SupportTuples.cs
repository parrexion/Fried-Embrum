using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportValue {

	public string uuid;
	public int value;
}


[System.Serializable]
public class SupportTuple {
	public enum SupportLetter { NONE, C, B, A, S, X }
	public enum SupportSpeed { NORMAL, FAST, VERYFAST, SLOW, VERYSLOW }

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
