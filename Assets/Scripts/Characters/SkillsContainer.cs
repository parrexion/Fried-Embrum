using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillsContainer {

	public const int SKILL_SIZE = 5;

	public CharacterSkill[] skills;


	public SkillsContainer(ScrObjLibraryVariable iLib, CharacterSaveData saveData) {
		SetupValues(iLib, saveData);
	}

	public SkillsContainer(List<CharacterSkill> presetSkills) {
		skills = new CharacterSkill[SKILL_SIZE];
		for (int i = 0; i < presetSkills.Count; i++) {
			skills[i] = presetSkills[i];
		}
	}

	private void SetupValues(ScrObjLibraryVariable iLib, CharacterSaveData saveData) {
		skills = new CharacterSkill[SKILL_SIZE];
		for (int i = 0; i < saveData.skills.Count; i++) {
			skills[i] = (CharacterSkill) iLib.GetEntry(saveData.skills[i]);
			Debug.Log("Found skill " + skills[i].uuid);
		}
	}

	
	public bool GainSkill(CharacterSkill skill) {
		Debug.Log("Adding skill " + skill.entryName);
		for (int i = 0; i < skills.Length; i++) {
			if (skills[i] == null) {
				skills[i] = skill;
				Debug.Log("Added the skill to position " + i);
				return true;
			}
		}
		return false;
	}

	//Skill activations

	public void ActivateSkills(SkillActivation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].ActivateSkill(activation, user, enemy);
		}
	}

	public void EndSkills(SkillActivation activation, TacticsMove user, TacticsMove enemy) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].EndSkill(activation, user, enemy);
		}
	}

	public int EditValueSkills(SkillActivation activation, TacticsMove user, int value) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].EditValue(activation, value, user);
		}
		return value;
	}

	public void ForEachSkills(SkillActivation activation, TacticsMove user, CharacterListVariable list) {
		for (int i = 0; i < skills.Length; i++) {
			if (!skills[i])
				continue;
			skills[i].ActivateForEach(activation, user, list);
		}
	}


}
