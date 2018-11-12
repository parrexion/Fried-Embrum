using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportLevel {

	public string uuid;
	public int value;
}


[System.Serializable]
public class SupportTuple {
	public enum SupportLevel { NONE, C, B, A, S }
	public enum SupportSpeed { NORMAL, FAST, VERYFAST, SLOW, VERYSLOW }

	public CharData partner;
	public SupportLevel maxlevel;
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
}
