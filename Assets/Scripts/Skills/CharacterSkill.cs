using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillActivation { NONE = -1, PRECOMBAT, INITCOMBAT, STARTTURN, PASSIVE, POSTCOMBAT, REWARD, COUNTER }
public enum EnemyCanAttack { BOTH, NO_ATTACK, ATTACK }

public abstract class CharacterSkill : ScrObjLibraryEntry {

    public Sprite icon;
    public string description;
    public SkillActivation activationType;

    [Space(10)]
    
	[Header("Values")]
    public float chance;
    public float percent;
	public int value;
    public Boost boost;

	[Header("Requirements")]
	public bool includeSelf;
    public int range;
    public int rangeMax;
	public EnemyCanAttack enemyCanAttack = EnemyCanAttack.BOTH;
    public WeaponType weaponReq = WeaponType.NONE;
	public List<TerrainTile> terrainReq = new List<TerrainTile>();


	public override void ResetValues() {
		base.ResetValues();
		icon = null;
		description = "";
		activationType = SkillActivation.NONE;

		chance = 0;
		percent = 0;
		value = 0;
		boost = new Boost();

		includeSelf = false;
		range = 0;
		rangeMax = 0;
		enemyCanAttack = EnemyCanAttack.BOTH;
		weaponReq = WeaponType.NONE;
		terrainReq = new List<TerrainTile>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		CharacterSkill cs = (CharacterSkill)other;

		icon = cs.icon;
		description = cs.description;
		activationType = cs.activationType;

		chance = cs.chance;
		percent = cs.percent;
		value = cs.value;
		boost = new Boost();
		boost.AddBoost(cs.boost);

		includeSelf = cs.includeSelf;
		range = cs.range;
		rangeMax = cs.rangeMax;
		enemyCanAttack = cs.enemyCanAttack;
		weaponReq = cs.weaponReq;
		terrainReq = new List<TerrainTile>();
		for (int i = 0; i < cs.terrainReq.Count; i++) {
			terrainReq.Add(cs.terrainReq[i]);
		}
	}

    public virtual void UseSkill(TacticsMove user, TacticsMove enemy){ }
    public virtual void EndSkill(TacticsMove user, TacticsMove enemy){ }

    public virtual int EditValue(int value, TacticsMove user) {
        return value;
    }

    public virtual void ForEachBoost(CharacterListVariable list, TacticsMove user) { }
}
