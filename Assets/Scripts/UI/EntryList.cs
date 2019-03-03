using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic list class used to show different lists with entries 
/// inheriting from the ListEntry class.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class EntryList<T> where T : ListEntry {

	public int Size { get {return entries.Count; } }

	private List<T> entries = new List<T>();
	private List<T> original = new List<T>();
	private int visibleSize;
	private int position;
	private int top;
	private int bot;


	/// <summary>
	/// Creates a new entryList with the given visible size.
	/// </summary>
	/// <param name="size"></param>
	public EntryList(int size) {
		visibleSize = size;
	}

	/// <summary>
	/// Resets the entrylist by destroying previous entries and resetting all values.
	/// </summary>
	public void ResetList() {
		for(int i = 0; i < original.Count; i++) {
			GameObject.Destroy(original[i].gameObject);
		}
		entries.Clear();
		original.Clear();
		bot = 0;
		top = 0;
		position = 0;
		UpdateEntries();
	}

	/// <summary>
	/// Creates a new entry in the list from the given transform.
	/// Sets up the basic values of the entry and returns the entryList component.
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	public T CreateEntry(Transform t) {
		t.gameObject.SetActive(true);

		T item = t.GetComponent<T>();
		item.SetHighlight(false);
		original.Add(item);
		entries.Add(item);

		top = Mathf.Min(bot + visibleSize, entries.Count);
		UpdateEntries();

		return item;
	}

	/// <summary>
	/// Removes the currently highlighted entry from the list and adjusts the highlight.
	/// </summary>
	public void RemoveEntry() {
		T entry = entries[position];
		original.Remove(entry);
		GameObject.Destroy(entries[position].gameObject);
		entries.RemoveAt(position);
		Move((position == entries.Count) ? -1 : 0);
	}

	/// <summary>
	/// Forces the highlight to move to a certain position or the next active entry.
	/// </summary>
	/// <param name="pos"></param>
	public void ForcePosition(int pos) {
		if (entries.Count == 0)
			return;
		position = Mathf.Max(0, Mathf.Clamp(pos, 0, entries.Count -1));
		UpdateEntries();
	}

	/// <summary>
	/// Moves the highlight to the next active entry in the list.
	/// dir is the amount of steps back or forward in the list.
	/// </summary>
	/// <param name="dir"></param>
	public void Move(int dir) {
		if (entries.Count == 0) {
			position = 0;
			return;
		}

		position = OPMath.FullLoop(0, entries.Count, position + dir);
		if(position <= bot)
			bot = Mathf.Max(0, position - 1);
		else if(top - 1 <= position)
			bot = Mathf.Max(0, Mathf.Min(entries.Count - visibleSize, position - visibleSize + 2));
		top = Mathf.Min(bot + visibleSize, entries.Count);

		UpdateEntries();
	}

	/// <summary>
	/// Delegate method for creating custom verify methods.
	/// Can be used to check which entries in the list should be darkened out or hidden.
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate bool EntryListFilter(T e);

	/// <summary>
	/// Filter method which takes a verify method and darkens the entries which 
	/// fulfills the criterias.
	/// </summary>
	/// <param name="filter"></param>
	public void FilterDark(EntryListFilter filter) {
		for (int i = 0; i < entries.Count; i++) {
			entries[i].SetDark(filter(entries[i]));
		}
	}

	/// <summary>
	/// Filter method which takes a verify method and darkens the entries which 
	/// fulfills the criterias.
	/// </summary>
	/// <param name="filter"></param>
	public void FilterShow(EntryListFilter filter) {
		entries.Clear();
		for (int i = 0; i < original.Count; i++) {
			original[i].gameObject.SetActive(filter(original[i]));
			if (filter(original[i]))
				entries.Add(original[i]);
		}
		Move(0);
		UpdateEntries();
	}

	/// <summary>
	/// Updates all the entries' highlights and hide the entries out of focus.
	/// </summary>
	private void UpdateEntries() {
		for(int i = 0; i < entries.Count; i++) {
			entries[i].SetHighlight(i == position);
		}
	}

	/// <summary>
	/// Get index for currently highlighted position
	/// </summary>
	/// <returns></returns>
	public int GetPosition() {
		return (entries.Count == 0) ? -1 : position;
	}

	/// <summary>
	/// Get currently highlighted entry
	/// </summary>
	/// <returns></returns>
	public T GetEntry() {
		if (entries.Count == 0)
			return null;
		return entries[position];
	}

	/// <summary>
	/// Get the entry for the given index in the list.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public T GetEntry(int index) {
		if(entries.Count <= index)
			return null;
		return entries[index];
	}
}
