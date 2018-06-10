using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SupportType {NONE, HEAL, BUFF}

[System.Serializable]
public class SupportSkill : BaseSkill {

	public SupportType supportType = SupportType.NONE;
	public float statsMultiplier;
	public int power = 5;
	public int range = 1;
	public bool variableRange = false;
	public Boost boost;


	public bool InRange(int distance) {
		return (distance == range || (distance < range && variableRange));
	}
}
