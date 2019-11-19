using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateRecycler : MonoBehaviour {

	public Transform template;

	private List<Transform> list = new List<Transform>();


	private void Start() {
		template.gameObject.SetActive(false);
	}

	public void Clear() {
		for(int i = 0; i < list.Count; i++) {
			Destroy(list[i].gameObject);
		}
		list.Clear();
	}

	public void Editor_Clear() {
		for(int i = 0; i < list.Count; i++) {
			DestroyImmediate(list[i].gameObject);
		}
		list.Clear();
	}

	public Transform CreateEntry() {
		Transform t = Instantiate(template, transform);
		list.Add(t);
		t.gameObject.SetActive(true);
		return t;
	}

	public T CreateEntry<T>() {
		Transform t = Instantiate(template, transform);
		list.Add(t);
		t.gameObject.SetActive(true);
		return t.GetComponent<T>();
	}

	public void Delete(int index) {
		Destroy(list[index].gameObject);
		list.RemoveAt(index);
	}

	public Transform GetItem(int index) {
		return list[index];
	}

	public T GetItem<T>(int index) {
		return list[index].GetComponent<T>();
	}

	public void Sort(System.Comparison<Transform> sorter) {
		list.Sort(sorter);
		for (int i = 0; i < list.Count; i++) {
			list[i].SetSiblingIndex(i);
		}
	}
}
