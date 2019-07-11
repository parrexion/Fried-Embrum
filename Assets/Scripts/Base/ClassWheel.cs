using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerClassName { NONE = -1, SCOUT, ASSAULT, TANK, PSY, MEDIC }
public enum EnemyClassName { GRUNT, SPIT, RUSH, NECROMANCER, XXX2 }

[CreateAssetMenu]
public class ClassWheel : ScriptableObject {

	public const int CLASS_COUNT = 5;
	public const int ENEMY_CLASS_COUNT = 4;
	public CharClass[] classes;
	public ScrObjLibraryVariable classLibrary;


	public CharClass GetClass(PlayerClassName className) {
		return classes[(int)className];
	}

	public WeaponRank[] GetWpnSkillFromLevel(int[] classLevels) {
		WeaponRank[] ranks = new WeaponRank[InventoryContainer.WPN_SKILLS];
		for (int i = 0; i < classLevels.Length; i++) {
			for (int level = 0; level < classLevels[i]; level++) {
				for (int wpn = 0; wpn < classes[i].weaponSkills.Count; wpn++) {
					ranks[(int)classes[i].weaponSkills[wpn]]++;
				}
			}
		}
		return ranks;
	}

	public List<CharacterSkill> GetSkillsFromLevel(int[] classLevels, CharClass startClass, int startLevel) {
		List<CharacterSkill> skills = new List<CharacterSkill>();

		bool edit = false;
		for (int i = 0; i < classLevels.Length; i++) {
			for (int level = 0; level < classLevels[i]; level++) {
				skills.Add(classes[i].skills[level]);
				//Debug.Log("Added skill:  " + classes[i].skills[level].entryName + " for class " + ((PlayerClassName)i));
				edit = true;
			}
		}

		if (!edit) {
			int levelups = 1 + (startLevel / 10);
			for (int i = 0; i < levelups; i++) {
				skills.Add(startClass.skills[i]);
			}
		}

		return skills;
	}

	public List<LevelGain> LevelupOptions(int[] classLevels) {
		List<LevelGain> gains = new List<LevelGain>();
		int nextLeft = 0, nextRight = 0;
		int adjacent = 0, current = 0;
		for (int i = 0; i < classLevels.Length; i++) {
			nextLeft = OPMath.FullLoop(0, CLASS_COUNT, i - 1);
			nextRight = OPMath.FullLoop(0, CLASS_COUNT, i + 1);
			adjacent = (classLevels[nextLeft] > 0 || classLevels[nextRight] > 0) ? 1 : 0;
			current = (classLevels[i] > 0) ? classLevels[i] + 1 : 0;

			int level = Mathf.Max(current, adjacent);
			if (level == 0)
				continue;

			LevelGain gain = new LevelGain() {
				className = classes[i].entryName,
				classIcon = classes[i].icon,
				level = level,
				weaponSkills = classes[i].weaponSkills,
				skill = classes[i].skills[level-1],
				bonusHp = classes[i].bonusHp,
				bonusDmg = classes[i].bonusDmg,
				bonusMnd = classes[i].bonusMnd,
				bonusSkl = classes[i].bonusSkl,
				bonusSpd = classes[i].bonusSpd,
				bonusDef = classes[i].bonusDef
			};
			gains.Add(gain);
		}

		return gains;
	}
}

[System.Serializable]
public class LevelGain {
	public PlayerClassName playerClassName;
	public string className;
	public Sprite classIcon;
	public int level;
	public List<WeaponType> weaponSkills = new List<WeaponType>();
	public CharacterSkill skill;
	public int bonusHp;
	public int bonusDmg;
	public int bonusMnd;
	public int bonusSkl;
	public int bonusSpd;
	public int bonusDef;
}
