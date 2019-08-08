using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UpgradeEditorWindow : GenericEntryEditorWindow {

	protected override string NameString => "Upgrade";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<UpgradeEntry>();
	protected override Color BackgroundColor => new Color(0.3f, 0.8f, 0.4f);


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="entries"></param>
	/// <param name="container"></param>
	public UpgradeEditorWindow(ScrObjLibraryVariable entries, UpgradeEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		UpgradeEntry upgradeEntry = (UpgradeEntry)entryValues;
		upgradeEntry.entryName = EditorGUILayout.TextField("Name", upgradeEntry.entryName);
		upgradeEntry.type = (UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type", upgradeEntry.type);
		upgradeEntry.item = (ItemEntry)EditorGUILayout.ObjectField("Related item", upgradeEntry.item, typeof(ItemEntry), false);
		upgradeEntry.rank = Mathf.Clamp(EditorGUILayout.IntField("Upgrade Rank", upgradeEntry.rank), 1, 5);

		upgradeEntry.cost = EditorGUILayout.IntField("Money Cost", upgradeEntry.cost);
		upgradeEntry.scrap = EditorGUILayout.IntField("Scrap Cost", upgradeEntry.scrap);

		GUILayout.Space(10);

		if(upgradeEntry.type == UpgradeType.UPGRADE) {
			upgradeEntry.costValue = EditorGUILayout.IntField("Value increase", upgradeEntry.costValue);
			GUILayout.Label("Improvements", EditorStyles.boldLabel);
			upgradeEntry.power = EditorGUILayout.IntField("Power", upgradeEntry.power);
			upgradeEntry.hit = EditorGUILayout.IntField("Hit Rate", upgradeEntry.hit);
			upgradeEntry.crit = EditorGUILayout.IntField("Crit Rate", upgradeEntry.crit);
			upgradeEntry.charges = EditorGUILayout.IntField("Max Charges", upgradeEntry.charges);
		}
	}
}
