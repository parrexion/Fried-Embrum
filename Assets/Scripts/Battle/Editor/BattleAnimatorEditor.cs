using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleAnimator))]
public class BattleAnimatorEditor : Editor {

	//Debug
	private BattleAnimator.AnimationInfo debugInfo = new BattleAnimator.AnimationInfo();

	public override void OnInspectorGUI() {
		
		debugInfo.side = (AttackSide)EditorGUILayout.EnumPopup("Attack Side", debugInfo.side);
		debugInfo.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", debugInfo.weaponType);
		debugInfo.hitType = (BattleAnimator.HitType)EditorGUILayout.EnumPopup("Hit Type", debugInfo.hitType);
		debugInfo.leathal = EditorGUILayout.Toggle("Is Leathal", debugInfo.leathal);
		debugInfo.damage = EditorGUILayout.IntField("Damage", debugInfo.damage);

		if (GUILayout.Button("Play animation")) {
			BattleAnimator ba = (BattleAnimator)target;
			ba.PlayAttack(debugInfo);
		}

		GUILayout.Space(20);

		DrawDefaultInspector();
	}
}
