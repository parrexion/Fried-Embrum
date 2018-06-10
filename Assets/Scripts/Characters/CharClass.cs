using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassType { NONE, INFANTRY, ARMORED, CAVALRY, FLYING }

[CreateAssetMenu]
public class CharClass : ScriptableObject {

	public int movespeed = 2;
	public ClassType classType;
	
}
