using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntryList<T> where T : ListEntry {

	private List<T> entries = new List<T>();
	private int size;
	private int position;
	private int top;
	private int bot;


	public EntryList(int size) {
		this.size = size;
	}

	public void ResetList() {
		for(int i = 0; i < entries.Count; i++) {
			GameObject.Destroy(entries[i].gameObject);
		}
		entries.Clear();
		bot = 0;
		top = 0;
		position = 0;
		UpdateEntries();
	}

	public T CreateEntry(Transform t) {
		t.gameObject.SetActive(true);

		T item = t.GetComponent<T>();
		item.SetHighlight(false);
		entries.Add(item);

		top = Mathf.Min(bot + size, entries.Count);
		UpdateEntries();

		return item;
	}

	public void ForcePosition(int pos) {
		position = Mathf.Clamp(pos, 0, entries.Count -1);
		UpdateEntries();
	}

	public void Move(int dir) {
		position = OPMath.FullLoop(0, entries.Count, position + dir);
		if(position <= bot)
			bot = Mathf.Max(0, position - 1);
		else if(top - 1 <= position)
			bot = Mathf.Max(0, Mathf.Min(entries.Count - size, position - size + 2));
		top = Mathf.Min(bot + size, entries.Count);

		UpdateEntries();
	}

	public delegate bool EntryListFilter(T e);
	public void FilterDark(EntryListFilter filter) {
		for (int i = 0; i < entries.Count; i++) {
			entries[i].SetDark(filter(entries[i]));
		}
	}

	private void UpdateEntries() {
		for(int i = 0; i < entries.Count; i++) {
			entries[i].SetHighlight(i == position);
			entries[i].gameObject.SetActive(entries[i].show && bot <= i && i < top);
		}
	}

	public int GetPosition() {
		return position;
	}

	public T GetEntry() {
		return entries[position];
	}

	public T GetEntry(int index) {
		if(entries.Count <= index)
			return null;
		return entries[index];
	}
}
