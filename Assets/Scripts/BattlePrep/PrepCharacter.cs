using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrepCharacter {
	
	public int index;
	public bool locked;
	public bool forced;


	public PrepCharacter(int index, bool locked = false, bool forced = false) {
		this.index = index;
		this.locked = locked;
		this.forced = forced;
	}
}
