using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SkillEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Skill";
	protected override ScrObjLibraryEntry CreateInstance => CreateSkill();
	protected override Color BackgroundColor => new Color(0.6f, 0.2f, 0.6f);
	protected override int CreateEntrySpace => 150;

	//Extra creation
	private SkillActivation skillActivation = SkillActivation.NONE;


	public SkillEditorWindow(ScrObjLibraryVariable entries, CharacterSkill container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void ExtraEntrySettings() {
		skillActivation = (SkillActivation)EditorGUILayout.EnumPopup("Skilltype", skillActivation);
	}

	protected override void DrawContentWindow() {
	CharacterSkill skillValues = (CharacterSkill)entryValues;
		skillValues.entryName = EditorGUILayout.TextField("Skill Name", skillValues.entryName);
		skillValues.icon = (Sprite)EditorGUILayout.ObjectField("Skill icon", skillValues.icon, typeof(Sprite), false);
		GUILayout.Label("Skill description");
		skillValues.description = EditorGUILayout.TextArea(skillValues.description, GUILayout.Height(60));

		GUILayout.Space(10);
		GUILayout.Label(skillValues.activationType.ToString(), EditorStyles.boldLabel);
		GUILayout.Space(10);

		switch (skillValues.activationType) {
			case SkillActivation.PRECOMBAT:
				GUILayout.Label("Boost during combat", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				GUILayout.Space(10);
				GUILayout.Label("Requirements", EditorStyles.boldLabel);
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Certain enemy attacks?");
				skillValues.enemyCanAttack = (EnemyCanAttack)EditorGUILayout.EnumPopup(skillValues.enemyCanAttack);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);

				GUILayout.Label("Activation range (inclusive)");
				GUILayout.BeginHorizontal();
				skillValues.range = EditorGUILayout.IntField("Min range", skillValues.range);
				skillValues.rangeMax = EditorGUILayout.IntField("Max range", skillValues.rangeMax);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);

				GUILayout.Label("Activation terrain");
				for (int i = 0; i < skillValues.terrainReq.Count; i++) {
					GUILayout.BeginHorizontal();
					skillValues.terrainReq[i] = (TerrainTile)EditorGUILayout.ObjectField("Item", skillValues.terrainReq[i], typeof(TerrainTile), false);
					if (GUILayout.Button("X", GUILayout.Width(50))) {
						GUI.FocusControl(null);
						skillValues.terrainReq.RemoveAt(i);
						i--;
						continue;
					}
					GUILayout.EndHorizontal();
				}
				if (GUILayout.Button("+")) {
					skillValues.terrainReq.Add(null);
				}
				break;


			case SkillActivation.INITCOMBAT:
				GUILayout.Label("Boost when attacking", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;

			case SkillActivation.STARTTURN:
				GUILayout.Label("Heal at start of turn", EditorStyles.boldLabel);
				skillValues.percent = EditorGUILayout.FloatField("Heal amount %", skillValues.percent);
				skillValues.range = EditorGUILayout.IntField("Heal range", skillValues.range);
				break;


			case SkillActivation.PASSIVE:
				GUILayout.Label("Increased stats", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;


			case SkillActivation.POSTCOMBAT:
				GUILayout.Label("Debuffed stats", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				break;


			case SkillActivation.REWARD:
				GUILayout.Label("Reward bonus", EditorStyles.boldLabel);
				//skillValues.chance = EditorGUILayout.FloatField("Trigger chance %", skillValues.chance);
				//skillValues.boost.hp = EditorGUILayout.IntField("Money gain", skillValues.boost.hp);
				//skillValues.boost.dmg = EditorGUILayout.IntField("Scrap gain", skillValues.boost.dmg);
				skillValues.percent = EditorGUILayout.FloatField("Exp boost %", skillValues.percent);
				break;


			case SkillActivation.COUNTER:
				GUILayout.Label("Boost when being attacked", EditorStyles.boldLabel);
				skillValues.boost.hp = EditorGUILayout.IntField("HP", skillValues.boost.hp);
				skillValues.boost.dmg = EditorGUILayout.IntField("ATK", skillValues.boost.dmg);
				skillValues.boost.mnd = EditorGUILayout.IntField("MND", skillValues.boost.mnd);
				skillValues.boost.skl = EditorGUILayout.IntField("SKL", skillValues.boost.skl);
				skillValues.boost.spd = EditorGUILayout.IntField("SPD", skillValues.boost.spd);
				skillValues.boost.def = EditorGUILayout.IntField("DEF", skillValues.boost.def);
				skillValues.boost.hit = EditorGUILayout.IntField("HIT", skillValues.boost.hit);
				skillValues.boost.crit = EditorGUILayout.IntField("CRIT", skillValues.boost.crit);
				skillValues.boost.avoid = EditorGUILayout.IntField("AVOID", skillValues.boost.avoid);
				break;
		}
	}

	private CharacterSkill CreateSkill() {
		Debug.Log("Skills::  " + skillActivation);
		CharacterSkill cs = null;
		switch (skillActivation) {
			case SkillActivation.PRECOMBAT:
				cs = Editor.CreateInstance<CombatBoost>(); break;
			case SkillActivation.INITCOMBAT:
				cs = Editor.CreateInstance<InitiateBoost>(); break;
			case SkillActivation.STARTTURN:
				cs = Editor.CreateInstance<StartTurnGain>(); break;
			case SkillActivation.PASSIVE:
				cs = Editor.CreateInstance<PermanentBoost>(); break;
			case SkillActivation.POSTCOMBAT:
				cs = Editor.CreateInstance<PostCombatDebuff>(); break;
			case SkillActivation.REWARD:
				cs = Editor.CreateInstance<ExpBoost>(); break;
			case SkillActivation.COUNTER:
				cs = Editor.CreateInstance<CounterBoost>(); break;
		}
		if (cs == null) {
			Debug.LogError("Activation type is invalid");
		}
		else {
			cs.activationType = skillActivation;
		}
		return cs;
	}

}
