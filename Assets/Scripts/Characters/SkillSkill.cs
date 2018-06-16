using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { DAMAGE, HEAL }

[System.Serializable]
public class SkillSkill : Item {

	public float power = 0.5f;
	public int maxCharge = 0;
	public SkillType type = SkillType.DAMAGE;


	public int GenerateDamage(int damage) {
		Debug.Log("Mer Damage!");
		return damage + (int)(damage * power);
	}
	
	public int GenerateHeal(int damage) {
		Debug.Log("Mer Heal!");
		return (int)(damage * power);
	}
}
