using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ItemEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Item";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<ItemEntry>();
	protected override Color BackgroundColor => new Color(0.7f, 0.7f, 0.1f);


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public ItemEditorWindow(ScrObjLibraryVariable entries, ItemEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		ItemEntry itemValues = (ItemEntry)entryValues;

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		itemValues.entryName = EditorGUILayout.TextField("Name", itemValues.entryName);
		itemValues.itemCategory = (ItemCategory)EditorGUILayout.EnumPopup("Item Category", itemValues.itemCategory);
		itemValues.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", itemValues.weaponType);
		itemValues.attackType = (AttackType)EditorGUILayout.EnumPopup("Attack Type", itemValues.attackType);
		itemValues.repColor = EditorGUILayout.ColorField("Rep Color", itemValues.repColor);
		GUILayout.EndVertical();
		itemValues.icon = (Sprite)EditorGUILayout.ObjectField("Icon", itemValues.icon, typeof(Sprite), false);
		GUILayout.EndHorizontal();
		itemValues.description = EditorGUILayout.TextField("Description", itemValues.description);

		GUILayout.Space(10);

		GUILayout.Label("Basic Values", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		itemValues.cost = EditorGUILayout.IntField("Money Value", itemValues.cost);
		itemValues.maxCharge = EditorGUILayout.IntField("Max Charges", itemValues.maxCharge);
		GUILayout.EndHorizontal();
		itemValues.researchNeeded = EditorGUILayout.Toggle("Research needed", itemValues.researchNeeded);

		GUILayout.Space(10);

		if(itemValues.attackType == AttackType.PHYSICAL || itemValues.attackType == AttackType.MENTAL) {
			GUILayout.Label("Weapon Stats", EditorStyles.boldLabel);
			itemValues.power = EditorGUILayout.IntField("Weapon Power", itemValues.power);
			itemValues.hitRate = EditorGUILayout.IntField("Hit Rate", itemValues.hitRate);
			itemValues.critRate = EditorGUILayout.IntField("Crit Rate", itemValues.critRate);
			GUILayout.BeginHorizontal();
			itemValues.range.min = EditorGUILayout.IntField("Min Range", itemValues.range.min);
			itemValues.range.max = EditorGUILayout.IntField("Max Range", itemValues.range.max);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.Label("Advantage Types", EditorStyles.boldLabel);
			for(int i = 0; i < itemValues.advantageType.Count; i++) {
				GUILayout.BeginHorizontal();
				itemValues.advantageType[i] = (MovementType)EditorGUILayout.EnumPopup("", itemValues.advantageType[i]);
				if(GUILayout.Button("X", GUILayout.Width(50))) {
					itemValues.advantageType.RemoveAt(i);
					i--;
				}
				GUILayout.EndHorizontal();
			}
			if(GUILayout.Button("+")) {
				itemValues.advantageType.Add(MovementType.NONE);
			}
		} else if(itemValues.attackType == AttackType.HEAL) {
			GUILayout.Label("Heal Stats", EditorStyles.boldLabel);
			itemValues.power = EditorGUILayout.IntField("Heal Power", itemValues.power);
			GUILayout.BeginHorizontal();
			itemValues.range.min = EditorGUILayout.IntField("Min Range", itemValues.range.min);
			itemValues.range.max = EditorGUILayout.IntField("Max Range", itemValues.range.max);
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(10);

		GUILayout.Label("Requirements", EditorStyles.boldLabel);
		itemValues.skillReq = (WeaponRank)EditorGUILayout.EnumPopup("Skill Requirement", itemValues.skillReq);

		GUILayout.Space(10);

		GUILayout.Label("Boost", EditorStyles.boldLabel);
		itemValues.boost.hp = EditorGUILayout.IntField("Boost HP", itemValues.boost.hp);
		itemValues.boost.dmg = EditorGUILayout.IntField("Boost Atk", itemValues.boost.dmg);
		itemValues.boost.mnd = EditorGUILayout.IntField("Boost Mnd", itemValues.boost.mnd);
		itemValues.boost.spd = EditorGUILayout.IntField("Boost Spd", itemValues.boost.spd);
		itemValues.boost.skl = EditorGUILayout.IntField("Boost Skl", itemValues.boost.skl);
		itemValues.boost.def = EditorGUILayout.IntField("Boost Def", itemValues.boost.def);
	}

}
