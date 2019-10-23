using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleAnimator))]
public class BattleAnimatorEditor : Editor {

	//Debug
	private bool debugIsLeft = true;
	private WeaponType debugWeaponType;
	private BattleAnimator.HitType debugHitType;
	private bool debugIsLeathal;
	private int debugDamage = 4;


	public override void OnInspectorGUI() {
		
		debugIsLeft = EditorGUILayout.Toggle("Is Left", debugIsLeft);
		debugWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", debugWeaponType);
		debugHitType = (BattleAnimator.HitType)EditorGUILayout.EnumPopup("Hit Type", debugHitType);
		debugIsLeathal = EditorGUILayout.Toggle("Is Leathal", debugIsLeathal);
		debugDamage = EditorGUILayout.IntField("Damage", debugDamage);

		if (GUILayout.Button("Play animation")) {
			BattleAnimator ba = (BattleAnimator)target;
			ba.PlayAttack(debugIsLeft, debugWeaponType, debugHitType, debugIsLeathal, debugDamage);
		}

		GUILayout.Space(20);

		DrawDefaultInspector();
	}
}
