﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissionEditorWindow : GenericEntryEditorWindow {

	private string[] squadSelectionStr = new string[] { "SQUAD 1", "SQUAD 2" };
	private int[] squadSelectionInt = new int[] { 1, 2 };

	protected override string NameString => "Mission";
	protected override ScrObjLibraryEntry CreateInstance => Editor.CreateInstance<MissionEntry>();
	protected override Color BackgroundColor => new Color(0.8f, 0.3f, 0.3f, 1);


	public MissionEditorWindow(ScrObjLibraryVariable entries, MissionEntry container) {
		entryLibrary = entries;
		entryValues = container;
		LoadLibrary();
	}

	protected override void DrawContentWindow() {
		MissionEntry missionValues = (MissionEntry)entryValues;

		GUILayout.Label("Mission Info", EditorStyles.boldLabel);
		missionValues.mapLocation = (MapLocation)EditorGUILayout.EnumPopup("Location", missionValues.mapLocation);
		GUILayout.Label("Mission description");
		EditorStyles.textField.wordWrap = true;
		missionValues.mapDescription = EditorGUILayout.TextArea(missionValues.mapDescription, GUILayout.Width(500), GUILayout.Height(30));
		EditorStyles.textField.wordWrap = false;

		GUILayout.Space(10);

		GUILayout.Label("Chapter Linking", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		missionValues.duration = EditorGUILayout.IntField("Map Duration", missionValues.duration);
		missionValues.unlockDay = EditorGUILayout.IntField("Unlocked on day", missionValues.unlockDay);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		GUILayout.Label("Rewards", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		missionValues.reward.money = EditorGUILayout.IntField("Money", missionValues.reward.money);
		missionValues.reward.scrap = EditorGUILayout.IntField("Scrap", missionValues.reward.scrap);
		GUILayout.EndHorizontal();
		GUILayout.Space(5);
		for (int i = 0; i < missionValues.reward.items.Count; i++) {
			GUILayout.BeginHorizontal();
			missionValues.reward.items[i] = (ItemEntry)EditorGUILayout.ObjectField("Item", missionValues.reward.items[i], typeof(ItemEntry), false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				missionValues.reward.items.RemoveAt(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
		}
		if (GUILayout.Button("+")) {
			missionValues.reward.items.Add(null);
		}

		GUILayout.Space(20);

		GUILayout.Label("Squads", EditorStyles.boldLabel);

		for (int i = 0; i < missionValues.maps.Count; i++) {
			LibraryEditorWindow.HorizontalLine(Color.black);
			GUILayout.BeginHorizontal();
			missionValues.maps[i] = (MapEntry)EditorGUILayout.ObjectField("Map", missionValues.maps[i], typeof(MapEntry), false);
			if (GUILayout.Button("X", GUILayout.Width(50))) {
				GUI.FocusControl(null);
				missionValues.RemoveMap(i);
				i--;
				continue;
			}
			GUILayout.EndHorizontal();
			if (missionValues.maps[i] == null)
				continue;

			MapEntry map = missionValues.maps[i];
			if (map.spawnPoints2.Count == 0) {
				int size = map.spawnPoints1.Count;
				int selected = (missionValues.squads[i].squad2Size > 0) ? 2 : 1;
				GUILayout.BeginHorizontal();
				selected = EditorGUILayout.IntPopup(selected, squadSelectionStr, squadSelectionInt);
				missionValues.squads[i].squad1Size = (selected == 1) ? size : 0;
				missionValues.squads[i].squad2Size = (selected == 2) ? size : 0;
				GUILayout.Label("Squad size: " + size);
				GUILayout.EndHorizontal();
			}
			else {
				missionValues.squads[i].squad1Size = map.spawnPoints1.Count;
				missionValues.squads[i].squad2Size = map.spawnPoints2.Count;
				GUILayout.BeginHorizontal();
				GUILayout.Label("Squad 1 size: " + missionValues.squads[i].squad1Size);
				GUILayout.Label("Squad 2 size: " + missionValues.squads[i].squad2Size);
				GUILayout.EndHorizontal();
			}
		}
		LibraryEditorWindow.HorizontalLine(Color.black);
		if (GUILayout.Button("+")) {
			missionValues.AddMap();
		}
	}

}
